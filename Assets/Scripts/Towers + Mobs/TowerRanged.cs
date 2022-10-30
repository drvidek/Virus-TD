using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerRanged : TowerBase
{
    private Mob _currentTarget;
    private LineRenderer _laser;
    private SpriteRenderer _rangeUI;
    private Image _delayUI;

    new private void OnValidate()
    {
        base.OnValidate();
        _rangeUI = GetComponentInChildren<SpriteRenderer>();
        _delayUI = GetComponentInChildren<Image>();
        _rangeUI.transform.localScale = new Vector3(_attackRange, _attackRange);
    }
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
        _delayUI.fillAmount = (_attackDelay / _attackRate);
    }

    protected override void Attack()
    {
        if (DetermineTarget())
        {
            if (_attackRadius > 0.5f)
            {
                AoeAttack();
            }
            else
            {
                _currentTarget.TakeDamage(_attackPower);
                ApplyEffect(_currentTarget);
            }
            _attackDelay = _attackRate;
            StartCoroutine("LaserEffect");

        }
    }

    private void AoeAttack()
    {
        Collider[] hits = Physics.OverlapSphere(_currentTarget.transform.position, _attackRadius);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Mob>(out Mob m))
            {
                m.TakeDamage(_attackPower);
                ApplyEffect(m);
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