using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transtion : MonoBehaviour
{

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void LoadSceneWithTransition() {
        StartCoroutine(JumpScene(1));
    }

    IEnumerator JumpScene(int i)
    {
        anim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1.5f);
        SceneController.JumpScene(i);

    }
}
