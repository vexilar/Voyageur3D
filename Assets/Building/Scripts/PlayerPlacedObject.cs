using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script is placed on a placed object, and keeps track of the connectors that are children of the placed object
/// It's used for when the player raycasts to place an object, and hits a placed object, needing to know the snap points for 
/// the next piece to snap to this one
/// </summary>
public class PlayerPlacedObject : MonoBehaviour {

    [Tooltip("The type of buildable object this is")]
    public PlayerBuildableType PlayerBuildableType;

    [NonSerialized]
    public List<PlayerPlaceObjectConnector> Connectors = new List<PlayerPlaceObjectConnector>();

    private void Start() {
        Connectors = GetComponentsInChildren<PlayerPlaceObjectConnector>(true).ToList();
    }
}
