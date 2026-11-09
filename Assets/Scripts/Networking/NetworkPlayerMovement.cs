using Unity.Netcode;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Server-authoritative FPS movement: owner sends move/look/jump intents;
    /// server simulates CharacterController; NetworkTransform replicates pose.
    /// Camera pitch is owner-local only.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class NetworkPlayerMovement : NetworkBehaviour
    {
        [SerializeField] Transform ownerCamera;
        [SerializeField] float walkSpeed = 5f;
        [SerializeField] float runSpeed = 8f;
        [SerializeField] float jumpSpeed = 8f;
        [SerializeField] float stickToGroundForce = 10f;
        [SerializeField] float gravityMultiplier = 2f;
        [SerializeField] float mouseSensitivityX = 2f;
        [SerializeField] float mouseSensitivityY = 2f;
        [SerializeField] float minPitch = -85f;
        [SerializeField] float maxPitch = 85f;

        CharacterController characterController;
        float cameraPitch;
        float verticalVelocity;
        bool jumpQueued;
        bool wasGrounded = true;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (ownerCamera == null)
            {
                var cam = GetComponentInChildren<Camera>(true);
                if (cam != null)
                {
                    ownerCamera = cam.transform;
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            if (characterController != null)
            {
                characterController.enabled = IsServer;
            }

            if (IsOwner && ownerCamera != null)
            {
                cameraPitch = ownerCamera.localEulerAngles.x;
                if (cameraPitch > 180f)
                {
                    cameraPitch -= 360f;
                }
            }
        }

        void Update()
        {
            if (!IsSpawned || !IsOwner)
            {
                return;
            }

            // Don't fight UI (crafting / inventory).
            if (GameState.IsMouseActive)
            {
                return;
            }

            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
            ApplyOwnerCameraPitch(-mouseY);

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (Input.GetButtonDown("Jump"))
            {
                jumpQueued = true;
            }

            bool jump = jumpQueued;
            jumpQueued = false;

            float yawDelta = mouseX;
            SubmitMoveInputServerRpc(horizontal, vertical, yawDelta, jump, sprint);
        }

        void ApplyOwnerCameraPitch(float pitchDelta)
        {
            if (ownerCamera == null)
            {
                return;
            }

            cameraPitch = Mathf.Clamp(cameraPitch + pitchDelta, minPitch, maxPitch);
            ownerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        [ServerRpc]
        void SubmitMoveInputServerRpc(
            float horizontal,
            float vertical,
            float yawDelta,
            bool jump,
            bool sprint,
            ServerRpcParams rpcParams = default)
        {
            if (!IsServer)
            {
                return;
            }

            ApplyServerMove(horizontal, vertical, yawDelta, jump, sprint);
        }

        void ApplyServerMove(float horizontal, float vertical, float yawDelta, bool jump, bool sprint)
        {
            if (characterController == null || !characterController.enabled)
            {
                return;
            }

            if (Mathf.Abs(yawDelta) > 0.0001f)
            {
                transform.Rotate(0f, yawDelta, 0f, Space.Self);
            }

            Vector2 input = new Vector2(horizontal, vertical);
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            float speed = sprint ? runSpeed : walkSpeed;
            Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;
            if (desiredMove.sqrMagnitude > 0.0001f)
            {
                desiredMove.Normalize();
            }

            desiredMove *= speed;

            bool grounded = characterController.isGrounded;
            if (grounded)
            {
                verticalVelocity = -stickToGroundForce;
                if (jump)
                {
                    verticalVelocity = jumpSpeed;
                }
            }
            else
            {
                verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }

            // Landing
            if (grounded && !wasGrounded)
            {
                verticalVelocity = -stickToGroundForce;
            }

            wasGrounded = grounded;

            Vector3 move = desiredMove;
            move.y = verticalVelocity;
            characterController.Move(move * Time.deltaTime);
        }
    }
}
