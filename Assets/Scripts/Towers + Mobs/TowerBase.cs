using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect
{
    slow,
    dot
}

public class TowerBase : MonoBehaviour
{
    [SerializeField] protected TowerSO _myCard;
    [SerializeField] protected float _attackPower;
    [SerializeField] protected float _attackRate, _attackRange, _attackDelay, _attackRadius;
    [SerializeField] protected EffectSO[] _effects;
    [SerializeField] private SphereCollider _myCollider;
    [SerializeField] protected List<Mob> _validTargets = new List<Mob>();

    protected void Start()
    {
        _myCollider = GetComponent<SphereCollider>();
        _myCollider.radius = _attackRange;
    }

    protected void OnValidate()
    {
        if (_myCard != null)
        Initialise(_myCard);
    }

    virtual public void Initialise(TowerSO towerCard)
    {
        _attackPower = towerCard.attackPower;
        _attackRate = towerCard.attackRate;
        _attackRange = towerCard.attackRange;
        _attackRadius = towerCard.attackRadius;
        _effects = towerCard.effects;
        _myCollider = GetComponent<SphereCollider>();
        _myCollider.radius = _attackRange;
    }

    virtual public void Reset()
    {
        _validTargets.Clear();
        _attackDelay = 0;
    }

    protected void Update()
    {
        if (_attackDelay == 0)
        {
            Attack();
        }
        else
        {
            _attackDelay = Mathf.MoveTowards(_attackDelay, 0, Time.deltaTime);
        }
    }

    virtual protected void Attack()
    {
        //each tower has its own attack method
        //this has to be here for Update to reference it though
    }


    protected void ApplyEffect(Mob mob, bool aoe = true)
    {
        foreach (EffectSO effect in _effects)
        {
            switch (effect.myEffect)
            {
                case Effect.slow:
                    mob.AddSlow(effect.effectValue, effect.effectRange, this);
                    break;
                case Effect.dot:
                    mob.AddDot(effect.effectValue, effect.effectRange, this);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Mob>(out Mob m))
        {
            if (!_validTargets.Contains(m))
                _validTargets.Add(m);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Mob>(out Mob m))
        {
            if (_validTargets.Contains(m))
                _validTargets.Remove(m);
        }
    }

    public void CheckMob(Mob m)
    {
        if (_validTargets.Contains(m))
            _validTargets.Remove(m);
    }
}
