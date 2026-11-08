using UnityEngine;

public static class RaycastExtensions {

    /// <summary>
    /// This processes a raycast request
    /// </summary>
    /// <param name="raycastRequest">the request</param>
    /// <returns>the result of the raycast</returns>
    public static RaycastResponse Raycast(this RaycastRequest raycastRequest) {
        var hit = Physics.Raycast(raycastRequest.Origin, raycastRequest.Direction, out var raycastHit, raycastRequest.Distance, raycastRequest.LayerMask);

        if (hit) {
            return raycastRequest.OnHit(raycastHit);
        }

        return new RaycastResponse();
    }

}
