using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// This is a script to accept click events on ui elements
/// </summary>
public class ClickBehaviour : MonoBehaviour, IPointerClickHandler {

    [Tooltip("The list of events to fire when clicked")]
    public UnityEvent onClick;

    public void OnPointerClick(PointerEventData eventData) {
        onClick?.Invoke();
    }
}
