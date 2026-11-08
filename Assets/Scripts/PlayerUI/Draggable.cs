using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

/// <summary>
/// This script handles draggable operations
/// </summary>
public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

    public UnityEvent onEndDrag;
    public UnityEvent onBeginDrag;

    LayoutGroup layoutGroup;
    Image[] images;
    Canvas canvas;
    Transform parent;

    private void Start() {
        images = GetComponentsInChildren<Image>();
        layoutGroup = GetComponentInParent<LayoutGroup>();
        canvas = GetComponentInParent<Canvas>();
        parent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        transform.SetParent(canvas.transform, true);

        if (layoutGroup) {
            layoutGroup.enabled = false;
            foreach (var _image in images) {
                _image.raycastTarget = false;
            }
        }
        onBeginDrag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (layoutGroup) {
            layoutGroup.enabled = true;
        }

        foreach (var _image in images) {
            _image.raycastTarget = true;
        }

        onEndDrag?.Invoke();
        transform.SetParent(parent);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Events.invokeOnDraggableHovered();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Events.invokeOnDraggableUnHovered();
    }
}
