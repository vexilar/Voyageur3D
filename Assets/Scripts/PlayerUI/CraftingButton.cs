using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingButton : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        Events.invokeOnPlayerClickedToggleCraftingButton();
    }
}
