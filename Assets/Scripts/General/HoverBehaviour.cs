using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public UnityEvent onHover;
    public UnityEvent onExitHover;

    public void OnPointerEnter(PointerEventData eventData) {
        onHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        onExitHover?.Invoke();
    }
}
