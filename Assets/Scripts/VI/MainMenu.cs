using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject teamInfoPanel;
    public GameObject startingPanel;
    public GameObject optionPanel;

    private void Start()
    {
        optionPanel.SetActive(false);
    }

    public void PressStart() {
        startingPanel.SetActive(false);
        teamInfoPanel.SetActive(!startingPanel.activeInHierarchy);
    }

    public void PressOption(bool b)
    {
        optionPanel.SetActive(b);
    }

}
