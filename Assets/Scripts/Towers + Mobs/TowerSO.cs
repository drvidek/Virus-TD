using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerSO", menuName = "SOs/TowerSO")]
public class TowerSO : ScriptableObject
{
    [SerializeField] public float attackPower;
    [SerializeField] public float attackRate, attackRange, healthMax;
    [SerializeField] public EffectSO[] effects;
}
