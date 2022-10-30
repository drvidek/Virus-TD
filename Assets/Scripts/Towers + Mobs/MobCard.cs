using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MobSO", menuName = "SOs/MobSO")]
public class MobCard : ScriptableObject
{
    [SerializeField] public float moveSpd;
    [SerializeField] public float healthMax, attackPower, attackRate, scale;
}
