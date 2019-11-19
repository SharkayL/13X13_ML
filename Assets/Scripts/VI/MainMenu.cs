using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class MainMenu : MonoBehaviour
{
    private static MainMenu _i;
    public static MainMenu i {
        get {
            if (_i == null) {
                _i = FindObjectOfType<MainMenu>();
            }
            return _i;
        }
    }

    [Header("MainMenuPanel_Big")]
    public GameObject singleTeamInfo;
    public GameObject multiTeamInfo;
    public GameObject startingPanel;

    [Header("MainMenuPanel_Small")]
    public GameObject beginggingButtonsPanel;
    public GameObject singleMultiButtonsPanel;
    public GameObject optionPanel;

    [Header("Audio")]
    public AudioMixer audioMixer;


    private void Start()
    {
        optionPanel.SetActive(false);
    }

    public void PressStart() {
        beginggingButtonsPanel.SetActive(false);
        singleMultiButtonsPanel.SetActive(true);
    }

    public void PressSingleMulti(bool single) {
        startingPanel.SetActive(false);
        if (single)
        {
            singleTeamInfo.SetActive(true);
        }
        else {
            multiTeamInfo.SetActive(true);
        }
    }

    public void PressOption(bool b)
    {
        optionPanel.SetActive(b);
    }


    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
}
