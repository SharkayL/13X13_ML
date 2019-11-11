using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void JumpScene(int i) {
        SceneManager.LoadScene(i);
    }

    public static void ExitGame() {
        Application.Quit();
    }

}
