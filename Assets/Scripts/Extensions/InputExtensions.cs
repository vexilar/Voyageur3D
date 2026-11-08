using UnityEngine;

/// <summary>
/// This class extends the input manager in unity
/// </summary>
public static class InputExtensions {

    /// <summary>
    /// Evaluates if a key was pressed that is 0-9 on the keyboard
    /// </summary>
    /// <returns>A boolean indicating if the keypress was 0-9 on the keyboard</returns>
    public static bool IsNumeric() {
        return Input.GetKeyDown(KeyCode.Alpha0) ||
                Input.GetKeyDown(KeyCode.Alpha1) ||
                Input.GetKeyDown(KeyCode.Alpha2) ||
                Input.GetKeyDown(KeyCode.Alpha3) ||
                Input.GetKeyDown(KeyCode.Alpha4) ||
                Input.GetKeyDown(KeyCode.Alpha5) ||
                Input.GetKeyDown(KeyCode.Alpha6) ||
                Input.GetKeyDown(KeyCode.Alpha7) ||
                Input.GetKeyDown(KeyCode.Alpha8) ||
                Input.GetKeyDown(KeyCode.Alpha9);
    }
}