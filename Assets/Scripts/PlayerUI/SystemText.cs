using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible for instantiating system text instances inside of the system text component, it is attached to the system text object
/// </summary>
public class SystemText : MonoBehaviour {

    [Tooltip("The prefab that is the system notification")]
    public GameObject SystemNotificationPrefab;

    [Tooltip("The container to instantaite system notifications in")]
    public GameObject SystemNotificationContainer;

    [Tooltip("How long each new text is visible for")]
    public float TextDuration = 5f;

    private void Start() {
        SystemTextAPI.SystemText = this;
    }

    public void AddText(string text) {
        var instance = Instantiate(SystemNotificationPrefab, SystemNotificationContainer.transform);
        var textComponent = instance.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = text;
        Destroy(instance, TextDuration);
    }

}
