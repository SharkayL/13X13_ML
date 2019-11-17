using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    public Text input;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() => GameStarts());
    }

    void GameStarts()
    {
        //string ip = input.text.ToString();
        //MatureManager.serverIp = ip;
        MatureManager.startType = GameStart.client;
        SceneController.JumpScene(1);
    }
}
