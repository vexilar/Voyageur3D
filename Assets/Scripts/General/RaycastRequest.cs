using System;
using UnityEngine;

public class RaycastRequest {
    public Vector3 Origin;
    public Vector3 Direction;
    public float Distance;
    public int LayerMask;
    public Func<RaycastHit, RaycastResponse> OnHit;
}

public class RaycastResponse {
    public bool DidHit;
    public RaycastHit HitInformation;
}