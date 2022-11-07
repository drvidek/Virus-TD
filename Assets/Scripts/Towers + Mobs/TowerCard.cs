using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eType
{
    slow,
    dot
}

[System.Serializable] public struct Effect
{
    public eType type;
    [Tooltip("The strength of the effect")]
    public float value;
    [Tooltip("The duration of the effect")]
    public float range;
}

[CreateAssetMenu(fileName = "TowerSO", menuName = "SOs/TowerSO")]
public class TowerCard : ScriptableObject
{
    public float attackPower;
    public float attackRate, attackRange, attackRadius, healthMax;
    public Effect[] effects;
    public string description;
    public Sprite towerImage;
    public int resourceCost, pointCost;
}
