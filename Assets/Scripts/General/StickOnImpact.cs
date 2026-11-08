using UnityEngine;

public class StickOnImpact : MonoBehaviour {

    new Rigidbody rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        stickOnImpact();
    }

    private void OnCollisionEnter(Collision collision) {
        stickOnImpact();
    }

    void stickOnImpact() {
        Destroy(rigidbody);
    }

}
