using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    [Tooltip("The distance the player can interact with an object")]
    public float InteractDistance = 10f;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && !GameState.IsMouseActive) {
            var hit = Physics.Raycast(transform.position, transform.forward, out var raycast, InteractDistance, LayerMasks.Default | LayerMasks.PlayerBuilding | LayerMasks.WorldContainer);

            if (hit) {
                if (raycast.collider.gameObject.tag == Tags.Interactable) {
                    Events.invokeOnPlayerInteracted(raycast.collider.gameObject);
                }
            }
        }
    }

}
