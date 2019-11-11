using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Button : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    public enum ButtonType {
        Start,
        Option,
        Exit,
        Play,
        Apply
    }

    public ButtonType type;
    GameObject animObj;

    void Awake() {
        animObj = transform.Find("HL").gameObject;
        animObj.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!animObj.activeInHierarchy) {
            animObj.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (animObj.activeInHierarchy)
        {
            animObj.SetActive(false);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        animObj.SetActive(false);
        switch (type) {
            case ButtonType.Start:
                MainMenu.i.PressStart();
                break;
            case ButtonType.Option:
                MainMenu.i.PressOption(true);
                break;
            case ButtonType.Exit:
                SceneController.ExitGame();
                break;
            case ButtonType.Play:
                SceneController.JumpScene(1);
                break;
            case ButtonType.Apply:
                MainMenu.i.PressOption(false);
                break;

        }
    }

}
