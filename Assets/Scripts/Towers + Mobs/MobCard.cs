using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MobSO", menuName = "SOs/MobSO")]
public class MobCard : ScriptableObject
{
    public float moveSpd;
    public float healthMax, attackPower, attackRate, scale;
    public Sprite mobImage;
    public int resourceCostA, resourceCostB, pointWorth;
}
