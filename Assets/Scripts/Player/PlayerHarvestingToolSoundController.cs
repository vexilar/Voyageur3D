using UnityEngine;

public class PlayerHarvestingToolSoundController : MonoBehaviour {

    [Tooltip("The sound to play when the axe missed")]
    public AudioClip SwingSound;

    AudioSource audioSource;
    PlayerHarvestingTool playerHarvestingTool;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        playerHarvestingTool = GetComponent<PlayerHarvestingTool>();

        playerHarvestingTool.onSwung += onSwung;
    }

    private void onSwung() {
        audioSource.PlayAudio(SwingSound);
    }

}
