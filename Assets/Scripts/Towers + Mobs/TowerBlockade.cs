using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlockade : TowerBase
{
    private float _healthCurrent, _healthMax;

    override public void Initialise(TowerSO towerCard)
    {
        base.Initialise(towerCard);
        _healthMax = towerCard.healthMax;
    }

    override public void RoundStart()
    {
        base.RoundStart();
        _healthCurrent = _healthMax;
    }

    protected override void Attack()
    {
        foreach (Mob mob in _validTargets)
        {
            mob.TakeDamage(_damage);
            _attackDelay = _attackRate;
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

    }
}
