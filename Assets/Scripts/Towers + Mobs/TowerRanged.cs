using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRanged : TowerBase
{
    private Mob _currentTarget;
    private LineRenderer _laser;
    private SpriteRenderer _rangeUI;

    new protected void Start()
    {
        base.Start();
        _laser ??= GetComponentInChildren<LineRenderer>();
        _laser.SetPosition(0, transform.position);
        _rangeUI ??= GetComponentInChildren<SpriteRenderer>();
        _rangeUI.transform.localScale = new Vector3(_attackRange, _attackRange);
    }

    new void Update()
    {
        base.Update();
        if (_currentTarget != null)
        _laser.SetPosition(1, _currentTarget.transform.position);
    }

    protected override void Attack()
    {
        if (DetermineTarget())
        {
            _currentTarget.TakeDamage(_attackPower);
            _attackDelay = _attackRate;
            StartCoroutine("LaserEffect");
            if (_effects.Length > 0)
            {
                ApplyEffect(_currentTarget);
            }
        }
    }

    private bool DetermineTarget()
    {
        if (_currentTarget == null || !_validTargets.Contains(_currentTarget))
        {
            _currentTarget = null;
            Mob closestMob = null;
            float closestDist = float.PositiveInfinity;
            foreach (Mob mob in _validTargets)
            {
                float newDist = mob.GetDistanceToEnd();
                if (newDist < closestDist)
                {
                    closestMob = mob;
                    closestDist = newDist;
                }
            }
            _currentTarget = closestMob;
        }
        return (_currentTarget != null);
    }


    IEnumerator LaserEffect()
    {
        _laser.enabled = true;
        yield return new WaitForSeconds(0.1f);
        _laser.enabled = false;
    }
}