using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is placed on the item definitions object in the scene, it defines our items all in one place
/// It supplies the Item api with the items it uses and it loads before most scripts, if you look at the project setting script execution order
/// </summary>
public class ItemDefinitions : MonoBehaviour {

    [Tooltip("These are all of our items in the game")]
    public List<Item> Items = new List<Item>();

    private void Start() {
        ItemAPI.LoadItemsFromEditor(Items);
    }

}
