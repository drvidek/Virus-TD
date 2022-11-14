using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlockade : TowerBase
{
    [SerializeField] private float _healthCurrent, _healthMax;
    private List<Mob> _blockedTargets = new List<Mob>();
    private Animator _anim;

    new void Start()
    {
        base.Start();
        _healthCurrent = _healthMax;
    }
    override public void Initialise(TowerCard towerCard, ushort playerId)
    {
        base.Initialise(towerCard,playerId);
        _healthMax = towerCard.healthMax;
        _anim = GetComponent<Animator>();
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
        //attack each mob within range of your collider
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
        _anim.SetTrigger("Hit");
        if (_healthCurrent <= 0)
        {
            EndOfLife();
        }
    }

    private void EndOfLife()
    {
        //remove the blockage for each affected mob
        foreach (Mob mob in _blockedTargets)
        {
            mob.SetBlockade(null);
            mob.RemoveRearNeighbours();
        }
        Reset();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if a mob enters your collider, try adding them to your target lists
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

    public bool CheckInMobRange(Mob m)
    {
        return _validTargets.Contains(m);
    }

}
