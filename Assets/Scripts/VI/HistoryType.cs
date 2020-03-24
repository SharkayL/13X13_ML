using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HistoryType
{
    public enum TypeOfHistory
    {
        loseCard,
        getCard,
        getItem,
        Event_Bounty,
        Event_Substitution,
        Event_Combo,
        Event_Shackle,
        Event_HandCuffs,
        Event_Curse,
        Event_Amnesty,
        Event_BlindTrade,
        Event_BlackHole,
        Event_RubberBand,
        Item_MagnetRed,
        Item_MagnetBlue,
        Item_LongArm,
        Item_LongMug,
        Item_TeleBlade,
        Item_Dagger,
        Item_Shield,
        Item_LandWrith,
        Item_EarthPower,
        Item_TableFlip,
        Item_Oracle,
    }

    public TypeOfHistory historyIconType;
    public Sprite historySprite;
}
