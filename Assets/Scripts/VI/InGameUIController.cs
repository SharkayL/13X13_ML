using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    private static InGameUIController _i;
    public static InGameUIController i
    {
        get
        {
            if (_i == null)
            {
                _i = FindObjectOfType<InGameUIController>();
            }
            return _i;
        }
    }

    [HideInInspector]
    public List<Image> informationButtons = new List<Image>();
    public List<GameObject> informationContents = new List<GameObject>();
    public GameObject informationPanel;
    GameObject tempContent;

    private void Start()
    {
        informationPanel.SetActive(true);
    }

    public void InformationButtonSelectd(Image image)
    {
        if (image.enabled)
        {
            for (int i = 0; i < informationContents.Count; i++)
            {
                if (image.gameObject.name == informationContents[i].name)
                {
                    informationContents[i].SetActive(true);
                    tempContent = informationContents[i];
                }
            }
            image.enabled = false;
            informationContents.Remove(tempContent);
            informationContents.Insert(0, tempContent);

            informationButtons.Remove(image);
            informationButtons.Insert(0, image);
        }
        else
        {
            return;
        }
        //Close others
        for (int i = 1; i < informationButtons.Count; i++)
        {
            informationContents[i].SetActive(false);
            informationButtons[i].enabled = true;
        }
    }

    public void JumpScene(int i)
    {
        SceneController.JumpScene(i);
    }
}
