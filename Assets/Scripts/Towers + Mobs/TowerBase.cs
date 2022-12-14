using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TowerBase : MonoBehaviour
{
    [SerializeField] protected TowerCard _myCard;
    [SerializeField] protected ushort _playerId;
    [SerializeField] protected float _attackPower;
    [SerializeField] protected float _attackRate, _attackRange, _attackRadius;
    protected float _attackDelay;
    [SerializeField] protected Effect[] _effects;
    [SerializeField] private SphereCollider _myCollider;
    [SerializeField] protected List<Mob> _validTargets = new List<Mob>();

    protected void Start()
    {
        //Use the attack range to set the collider radius
        _myCollider = GetComponent<SphereCollider>();
        _myCollider.radius = _attackRange;
    }

    protected void OnValidate()
    {
        //if you have a card equpped, initialise it
        if (_myCard != null)
        Initialise(_myCard, 1);
    }

    virtual public void Initialise(TowerCard towerCard, ushort playerId)
    {
        _playerId = playerId;
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
        //count down to 0 and try an attack
        if (_attackDelay == 0 && _attackRate > 0)
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


    protected void ApplyEffect(Mob mob)
    {
        foreach (Effect effect in _effects)
        {
            switch (effect.type)
            {
                case eType.slow:
                    mob.AddSlow(effect.value, effect.range, this);
                    break;
                case eType.dot:
                    mob.AddDot(effect.value, effect.range, this);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if a mob enters your attack range, add it to your list of possible targets
        if (other.TryGetComponent<Mob>(out Mob m))
        {
            if (!_validTargets.Contains(m))
                _validTargets.Add(m);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if a mob exits your attack range, remove it from your list of possible targets
        if (other.TryGetComponent<Mob>(out Mob m))
        {
            if (_validTargets.Contains(m))
                _validTargets.Remove(m);
        }
    }

    public void CheckMobForRemoval(Mob m)
    {
        //check if a mob is in your list, and if so, remove it
        if (_validTargets.Contains(m))
            _validTargets.Remove(m);
    }
}
