using System.Collections.Generic;
using UnityEngine;

public class ReparentOnHit : MonoBehaviour {

    [Tooltip("The layer numbers that reparent will evaluate against")]
    public List<int> Layers = new List<int>();

    private void OnTriggerEnter(Collider other) {
        reparent(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        reparent(collision.gameObject);
    }

    void reparent(GameObject targetParent) {
        var targetLayer = targetParent.layer;

        // does the parent's layer match one specified in Layers?
        if (Layers.Contains(targetLayer)) {
            transform.SetParent(targetParent.transform, true);
        }
    }

}
