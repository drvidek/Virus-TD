using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MobCard", menuName = "Cards/MobCard")]
public class MobCard : ScriptableObject
{
    public string title;
    public string description;
    public float moveSpd;
    public float healthMax, attackPower, attackRate, scale;
    public Sprite mobImage;
    public int resourceCostA, resourceCostB, pointWorth;

}
