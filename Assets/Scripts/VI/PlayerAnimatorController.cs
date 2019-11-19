using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAnimatorController
{
    public static void UpdateAnimation(PlayerState currentPlayer, bool isGhost, bool isTurn)
    {
        currentPlayer.playerOG.GetComponent<Animator>().SetBool("isGhost", isGhost);
        currentPlayer.playerOG.GetComponent<Animator>().SetBool("isTurn", isTurn);
    }
}
