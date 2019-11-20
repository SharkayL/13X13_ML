using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum SoundEffects {
        FootStep,
    }

    private static GameObject audioObject;
    private static AudioSource audioSource;

    public static void PlaySound(SoundEffects sound) {
        Vector3 pos = Camera.main.transform.position;
        AudioSource.PlayClipAtPoint(GetAudioClip(sound),pos);
    }


    private static AudioClip GetAudioClip(SoundEffects sound) {
        foreach (GameAssetManager.SoundClip soundClip in GameAssetManager.i.soundClipArray)
        {
            if (soundClip.sound == sound) {
                return soundClip.audioClip;
            }
        }
        Debug.LogError("AudioClip Not Found");
        return null;
    }
}
