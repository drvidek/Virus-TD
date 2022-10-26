using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlockade : TowerBase
{
    [SerializeField] private float _healthCurrent, _healthMax;
    
    new void Start()
    {
        base.Start();
        _healthCurrent = _healthMax;
    }
    override public void Initialise(TowerSO towerCard)
    {
        base.Initialise(towerCard);
        _healthMax = towerCard.healthMax;
    }

    override public void Reset()
    {
        base.Reset();
        _healthCurrent = _healthMax;
    }

    protected override void Attack()
    {
        foreach (Mob mob in _validTargets)
        {
            mob.TakeDamage(_attackPower);
            _attackDelay = _attackRate;
            mob.SetBlockade(this);
            //StartCoroutine("LaserEffect");
            if (_effects.Length > 0)
            {
                ApplyEffect(mob);
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        _healthCurrent -= dmg;
        if (_healthCurrent <= 0)
        {
            EndOfLife();
        }
    }

    private void EndOfLife()
    {
        foreach (Mob mob in _validTargets)
        {
            mob.SetBlockade(null);
        }
        Reset();
        gameObject.SetActive(false);
    }
}
