using UnityEngine;

public static class SoundExtensions {
    public static void PlayAudio(this AudioSource audioSource, AudioClip audioClip, bool loop = false, float delay = 0) {
        audioSource.Stop();
        audioSource.loop = loop;

        audioSource.clip = audioClip;
        audioSource.PlayDelayed(delay);
    }
}