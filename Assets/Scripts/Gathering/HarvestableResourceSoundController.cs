using UnityEngine;

public class HarvestableResourceSoundController : MonoBehaviour {

    public AudioClip HitSound;

    AudioSource audioSource;
    HarvestableResource harvestableResource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        harvestableResource = GetComponent<HarvestableResource>();

        harvestableResource.onResourceHit += onResourceHit;
    }

    private void onResourceHit() {
        audioSource.PlayAudio(HitSound);
    }
}
