using System;
using UnityEngine;

/// <summary>
/// This object defines the allowed connection to an existing player placed object, it associates a transform and a placable object type
/// This is typically attached to objects that are a child of a placed object, informing the game on how future pieces will connect to an existing one
/// </summary>
[Serializable]
public class PlayerPlaceObjectConnector : MonoBehaviour {

    [Tooltip("The type of object this connector is defining a transform for")]
    public PlayerBuildableType PlayerBuildableType;

}