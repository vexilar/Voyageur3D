using UnityEngine;

public class PauseEditor : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.V)) {
            Debug.Break();
        }
    }
}
