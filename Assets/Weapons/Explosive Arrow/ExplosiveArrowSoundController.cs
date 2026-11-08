using UnityEngine;

public class ExplosiveArrowSoundController : MonoBehaviour {

    public AudioClip arrowFired;
    public AudioClip arrowExplode;

    AudioSource audioSource;
    DamageSource damageSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        damageSource = GetComponent<DamageSource>();

        damageSource.onHit += onHit;

        audioSource.PlayAudio(arrowFired);
    }

    void onHit() {
        audioSource.PlayAudio(arrowExplode);
    }

}
