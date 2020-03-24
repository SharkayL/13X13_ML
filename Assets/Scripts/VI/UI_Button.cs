using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{

    public enum PanelType {
        MainMenu,
        Information,
        Setting
    }
    public enum ButtonType {
        None,
        Start,
        Option,
        Exit,
        Play,
        Apply,
        Researt,
        Exit_Ingame,
        SinglePlayer,
        MultiPlayer,
        BackToBeginning,
        BackToSingleMultiPlayer
    }
    public PanelType panelType;
    public ButtonType buttonType;

    //MainMenu
    GameObject animObj;

    //InformationPanel
    Text informationText;
    Image currentImage;
    Color informatinoTextColor;

    void Awake() {
        switch (panelType) {
            case PanelType.MainMenu:
                animObj = transform.Find("HL").gameObject;
                animObj.SetActive(false);
                break;
            case PanelType.Information:
                informationText = GetComponentInChildren<Text>();
                informatinoTextColor = informationText.color;
                currentImage = GetComponent<Image>();
                InGameUIController.i.informationButtons.Add(currentImage);
                break;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (panelType)
        {
            case PanelType.MainMenu:
                if (!animObj.activeInHierarchy)
                {
                    animObj.SetActive(true);
                }
                break;
            case PanelType.Information:
                informationText.color = Color.white;
                break;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        switch (panelType)
        {
            case PanelType.MainMenu:
                if (animObj.activeInHierarchy)
                {
                    animObj.SetActive(false);
                }
                break;
            case PanelType.Information:
                informationText.color = informatinoTextColor;

                break;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (panelType)
        {
            case PanelType.MainMenu:
                if (animObj != null)
                {
                    animObj.SetActive(false);
                }
                switch (buttonType)
                {
                    case ButtonType.Start:
                        MainMenu.i.PressStart(true);
                        break;
                    case ButtonType.BackToBeginning:
                        MainMenu.i.PressStart(false);
                        break;
                    case ButtonType.Option:
                        MainMenu.i.PressOption(true);
                        break;
                    case ButtonType.Exit:
                        SceneController.ExitGame();
                        break;
                    case ButtonType.Play:
                        MainMenu.i.transtionPanel.LoadSceneWithTransition();
                        break;
                    case ButtonType.Apply:
                        MainMenu.i.PressOption(false);
                        break;
                    case ButtonType.SinglePlayer:
                        MainMenu.i.PressSingleMulti(true,true);
                        break;
                    case ButtonType.MultiPlayer:
                        MainMenu.i.PressSingleMulti(false,true);
                        break;
                    case ButtonType.BackToSingleMultiPlayer:
                        MainMenu.i.PressSingleMulti(false, false);
                        break;
                }
                break;
            case PanelType.Information:
                InGameUIController.i.InformationButtonSelectd(currentImage);
                break;
            case PanelType.Setting:
                switch (buttonType) {
                    case ButtonType.Researt:
                        SceneController.JumpScene(1);
                        break;
                    case ButtonType.Exit_Ingame:
                        SceneController.ExitGame();
                        break;
                }
                break;
        }
    }

}
