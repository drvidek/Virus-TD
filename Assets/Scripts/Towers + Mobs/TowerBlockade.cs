using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlockade : TowerBase
{
    [SerializeField] private float _healthCurrent, _healthMax;
    private List<Mob> _blockedTargets = new List<Mob>();

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
        _validTargets.Clear();
        _blockedTargets.Clear();
    }

    protected override void Attack()
    {
        foreach (Mob mob in _validTargets)
        {
            mob.TakeDamage(_attackPower);
            _attackDelay = _attackRate;
            //mob.SetBlockade(this);
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
        foreach (Mob mob in _blockedTargets)
        {
            mob.SetBlockade(null);
        }
        Reset();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Mob>(out Mob m))
        {
            if (_validTargets.Contains(m))
                return;
            _validTargets.Add(m);
            _blockedTargets.Add(m);
            m.SetBlockade(this);
        }
    }

    public void AddBlockedTarget(Mob m)
    {
        _blockedTargets.Add(m);
    }

}
