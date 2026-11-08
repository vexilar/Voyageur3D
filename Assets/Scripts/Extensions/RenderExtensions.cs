using System.Linq;
using UnityEngine;

public static class RenderExtensions {

    public static void SetColor(this GameObject gameObject, Color color) {
        var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true).ToList();

        meshRenderers.ForEach(meshRenderer => {
            var materials = meshRenderer.materials.ToList();

            materials.ForEach(material => {
                material.SetColor("_Color", color);
            });
        });
    }

}