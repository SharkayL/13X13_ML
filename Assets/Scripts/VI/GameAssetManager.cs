using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAssetManager : MonoBehaviour
{
    private static GameAssetManager _i;
    public static GameAssetManager i
    {
        get
        {
            if (_i == null)
            {
                _i = FindObjectOfType<GameAssetManager>();
            }
            return _i;
        }
    }

    public SoundClip[] soundClipArray;

    [System.Serializable]
    public class SoundClip
    {
        public SoundManager.SoundEffects sound;
        public AudioClip audioClip;
    }
}
