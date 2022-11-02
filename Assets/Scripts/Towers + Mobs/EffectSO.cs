using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectSO", menuName = "SOs/EffectSO")]
public class EffectSO : ScriptableObject
{
    public Effect myEffect;
    public float effectValue, effectRange;
}
