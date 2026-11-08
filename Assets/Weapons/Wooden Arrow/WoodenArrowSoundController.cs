using UnityEngine;

public class WoodenArrowSoundController : MonoBehaviour {

    public AudioClip arrowFired;
    public AudioClip arrowStuck;

    AudioSource audioSource;
    DamageSource damageSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        damageSource = GetComponent<DamageSource>();

        damageSource.onHit += onHit;

        audioSource.PlayAudio(arrowFired);
    }

    void onHit() {
        audioSource.PlayAudio(arrowStuck);
    }

}
