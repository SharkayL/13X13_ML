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

    Camera mainCamera;
    [Header("CameraForwardPosition")]
    public Vector3 originalPosition;
    public Vector3 middlePosition;
    public Vector3 endPosition;
    public float cameraMoveSpeed = 3f;
    bool isMovingToMiddle;
    bool isMovingToEnd;

    [Header("MainMenuPanel_Small")]
    public GameObject beginggingButtonsPanel;
    public GameObject singleMultiButtonsPanel;
    public GameObject optionPanel;

    public GameObject singleTeamInfo;
    public GameObject multiTeamInfo;

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Transition")]
    public Transtion transtionPanel;

    private void Start()
    {
        mainCamera = Camera.main;
        optionPanel.SetActive(false);
        singleTeamInfo.SetActive(false);
        multiTeamInfo.SetActive(false);
    }

    public void PressStart(bool forward) {
        isMovingToMiddle = forward;
        beginggingButtonsPanel.SetActive(!forward);
        singleMultiButtonsPanel.SetActive(forward);
    }

    public void PressSingleMulti(bool single, bool forward) {
        isMovingToEnd = forward;
        singleTeamInfo.SetActive((single && forward));
        multiTeamInfo.SetActive((!single && forward));
        singleMultiButtonsPanel.SetActive(!forward);
    }

    public void PressOption(bool b)
    {
        optionPanel.SetActive(b);
    }


    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    private void Update()
    {
        if (isMovingToMiddle && !isMovingToEnd)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, middlePosition, cameraMoveSpeed * Time.deltaTime);
        }
        else if (!isMovingToMiddle && !isMovingToEnd)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, originalPosition, cameraMoveSpeed * Time.deltaTime);
        } else if (isMovingToEnd && isMovingToMiddle) {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, endPosition, cameraMoveSpeed * Time.deltaTime);
        }
    }
}
