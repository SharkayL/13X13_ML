using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    public InputField input;
    public List<Toggle> playerToggles;
    public Button start;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var p in playerToggles)
        {
            int id = playerToggles.IndexOf(p);
            p.GetComponent<Toggle>().onValueChanged.AddListener((value) => PlayerSelected(value, id));
        }
        start.GetComponent<Button>().onClick.AddListener(() => GameStarts());
    }

    void PlayerSelected(bool value,int id)
    {
        if(!value)
        {
            return;
        }
        MatureManager.playerID = id;
        var me = playerToggles[id];
        foreach (var toggle in playerToggles)
        {
            if(toggle != me)
            {
                toggle.isOn = false;
            }
        }
    }

    void GameStarts()
    {
        
        if (MatureManager.playerID == 0)
        {
            MatureManager.startType = GameStart.server;
        }
        else {
            string ip = input.text.ToString();
            MatureManager.serverIp = ip;
            MatureManager.startType = GameStart.client;
        }
        SceneController.JumpScene(1);
    }

}
