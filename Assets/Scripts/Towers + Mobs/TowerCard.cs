using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eType
{
    slow,
    dot
}

public struct Effect
{
    public eType type;
    public float value;
    public float range;
}

[CreateAssetMenu(fileName = "TowerSO", menuName = "SOs/TowerSO")]
public class TowerCard : ScriptableObject
{
    public float attackPower;
    public float attackRate, attackRange, attackRadius, healthMax;
    [SerializeField] public Effect[] effects;
    public string description;
    public Sprite towerImage;
    public int resourceCost, pointCost;
}
