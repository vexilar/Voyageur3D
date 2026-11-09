using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is attached to a player building object preview
/// The reason it is called preview is that it shows the player what they are about to build and where, so they are essentially previewing what they are going to build
/// The way it works is that we associate a real placable object to the preview, so when the player commits by left clicking, it instantiates the real placed object, which has colliders and 
/// other scripts on it this one doesnt need
/// </summary>
public class PlayerPlacableObjectPreview : MonoBehaviour {

    public event Action<PlayerPlacableObjectPreview> onPreviewSelected;

    public event Action onPlayerPlacedObject;

    [Tooltip("The object that is created when the player chooses to place based on this preview")]
    public GameObject PlacableObject;

    [Tooltip("The type of buildable object this preview represents")]
    public PlayerBuildableType PlayerBuildableType;

    [Tooltip("If true, this object can only be snapped to connect with existing pieces of player building")]
    public bool SnapOnlyPlacement;

    [Tooltip("This defines which buildable objects this preview must be placed against, leave empty for no restriction. Note: If you define connections inside the object placer, they will override this validation")]
    public List<PlayerBuildableType> MustBePlacedAgainst = new List<PlayerBuildableType>();

    [Tooltip("The color to assign the mesh materials when the preview placement is valid")]
    public Color ValidPreviewColor = new Color(.6f, .6f, .6f, .6f);

    [Tooltip("The color to assign the mesh materials when the preview placement is invalid")]
    public Color InvalidPreviewColor = new Color(1, .4f, .4f, .5f);

    /// <summary>
    /// This indicated whether the preview is connected to another piece
    /// </summary>
    [NonSerialized]
    public bool ConnectorFound;

    /// <summary>
    /// This indicates the existing player built object the preview is up on/against
    /// </summary>
    [NonSerialized]
    public PlayerPlacedObject PlayerPlacedObjectedAgainst;

    bool isValid;

    private void OnEnable() {
        onPreviewSelected?.Invoke(this);
    }

    private void Update() {
        validate();

        if (Input.GetMouseButtonDown(0) && !GameState.IsMouseActive) {
            if (isValid) {
                placeObject();
            }
        }
    }

    void validate() {
        isValid = true;

        if (SnapOnlyPlacement && !ConnectorFound) {
            isValid = false;
        }

        var previewMustBePlacedAgainstValidObject = MustBePlacedAgainst.Count > 0;

        if (previewMustBePlacedAgainstValidObject) {
            var wasntPlacedAgainst = PlayerPlacedObjectedAgainst != null && MustBePlacedAgainst.Contains(PlayerPlacedObjectedAgainst.PlayerBuildableType);

            if (!wasntPlacedAgainst) {
                isValid = false;
            }
        }

        if (isValid) {
            renderValidPlacement();
        } else {
            renderInvalidPlacement();
        }
    }

    void renderValidPlacement() {
        gameObject.SetColor(ValidPreviewColor);
    }

    void renderInvalidPlacement() {
        gameObject.SetColor(InvalidPreviewColor);
    }

    void placeObject() {
        onPlayerPlacedObject?.Invoke();

        var itemIdentifier = GetComponent<ItemIdentifier>() ?? GetComponentInParent<ItemIdentifier>();
        var networkBuilding = FindOwnerNetworkBuilding();
        if (networkBuilding != null && itemIdentifier != null) {
            networkBuilding.RequestPlace(
                itemIdentifier.ItemSubType,
                transform.position,
                transform.rotation,
                transform.lossyScale);
            return;
        }

        // Single-player / offline fallback
        var placedObject = Instantiate(PlacableObject, transform.position, transform.rotation);
        placedObject.transform.localScale = transform.lossyScale;
    }

    Voyageur.Networking.NetworkPlayerBuilding FindOwnerNetworkBuilding() {
        if (Unity.Netcode.NetworkManager.Singleton == null ||
            !Unity.Netcode.NetworkManager.Singleton.IsListening) {
            return null;
        }

        var buildings = FindObjectsByType<Voyageur.Networking.NetworkPlayerBuilding>(FindObjectsInactive.Exclude);
        for (int i = 0; i < buildings.Length; i++) {
            if (buildings[i] != null && buildings[i].IsOwner) {
                return buildings[i];
            }
        }

        return null;
    }

}
