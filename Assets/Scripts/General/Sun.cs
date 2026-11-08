using UnityEngine;

public class Sun : MonoBehaviour {

    [Tooltip("This sets the speed at which the sun rotates the world")]
    public float DayNightCycleSpeed = .1f;

    void Update() {
        transform.RotateAround(Vector3.zero, Vector3.right, DayNightCycleSpeed * Time.deltaTime);
    }
}
