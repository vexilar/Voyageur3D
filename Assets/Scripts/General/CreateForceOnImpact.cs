using System.Collections;
using UnityEngine;

public class CreateForceOnImpact : MonoBehaviour {

    [Tooltip("The magnitude of the force created on impact")]
    public float ForceMagnitude = 250f;

    [Tooltip("The radius of the force created on impact")]
    public float ForceRadius = 10f;

    [Tooltip("The updward force multiplier")]
    public float ForceUpwards = 10f;

    [Tooltip("If true, the force will scale its magnitude based on the mass of the rigid body, creating a more consistent reaction")]
    public bool IgnoreMass = true;

    [Tooltip("The duration to wait between the impact and the creation of force, usually you need a little in order to wait for the enemy to properly ragdoll before the force")]
    public float Delay = .1f;

    bool shouldIgnore;

    private void OnTriggerEnter(Collider other) {
        onImpact(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        onImpact(collision.gameObject);
    }

    void onImpact(GameObject objectHit) {
        if (!shouldIgnore) {
            shouldIgnore = true;
            StartCoroutine(createImpactAfterDelay(objectHit));
        }
    }

    IEnumerator createImpactAfterDelay(GameObject objectHit) {
        yield return new WaitForSeconds(Delay);
        createForce(objectHit);
    }

    void createForce(GameObject objectHit) {
        var rigid = objectHit.GetComponent<Rigidbody>();

        if (rigid) {
            var force = ForceMagnitude;

            if (IgnoreMass) {
                force = force * rigid.mass;
            }

            rigid.AddExplosionForce(force, transform.position, ForceRadius, ForceUpwards, ForceMode.Impulse);
        }
    }

}
