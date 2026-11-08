using UnityEngine;

public class WoodenBowSoundController : MonoBehaviour {

    public AudioClip knockArrowSound;
    public AudioClip drawBowSound;

    PlayerBow playerBow;
    AudioSource audioSource;

    private void Start() {
        playerBow = GetComponent<PlayerBow>();
        audioSource = GetComponent<AudioSource>();

        playerBow.onPlayerKnockArrow += playNockSound;
        playerBow.onPlayerDrawBow += playDrawBowSound;
    }

    void playNockSound() {
        audioSource.PlayAudio(knockArrowSound);
    }

    void playDrawBowSound() {
        audioSource.PlayAudio(drawBowSound);
    }

}
