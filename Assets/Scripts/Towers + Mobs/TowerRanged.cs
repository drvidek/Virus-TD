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
        //update the range indicator to match the attack range
        _rangeUI.transform.localScale = new Vector3(_attackRange, _attackRange);
    }
    new protected void Start()
    {
        base.Start();
        //set the laser line up
        _laser ??= GetComponentInChildren<LineRenderer>();
        _laser.SetPosition(0, transform.position);
        //set the range indicator up
        _rangeUI ??= GetComponentInChildren<SpriteRenderer>();
        _rangeUI.transform.localScale = new Vector3(_attackRange, _attackRange);
        _delayUI = GetComponentInChildren<Image>();
    }

    new void Update()
    {
        base.Update();
        //if you have a current target
        if (_currentTarget != null)
            //update the laser end position to their position
            _laser.SetPosition(1, _currentTarget.transform.position);
        //set the UI for your attack delay
        _delayUI.fillAmount = (_attackDelay / _attackRate);
    }

    protected override void Attack()
    {
        //if you can determine a valid target
        if (DetermineTarget())
        {
            if (_attackRadius > 0.5f)   //do an AoE attack if you have a wide attack radius
            {
                AoeAttack();
            }
            else    //otherwise just attack the valid target
            {
                _currentTarget.TakeDamage(_attackPower);
                ApplyEffect(_currentTarget);
            }
            //reset the attack delay
            _attackDelay = _attackRate;
            StartCoroutine("LaserEffect");

        }
    }

    private void AoeAttack()
    {
        //get all the colliders within your attack radius from the target mob
        Collider[] hits = Physics.OverlapSphere(_currentTarget.transform.position, _attackRadius);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Mob>(out Mob m)) //if you find a mob component on the collided object
            {
                //give that mob damage and apply your effects
                m.TakeDamage(_attackPower);
                ApplyEffect(m);
            }
        }
    }

    private bool DetermineTarget()
    {
        //if you don't still have a target, or your list of targets no longer contains your previous target
        if (_currentTarget == null || !_validTargets.Contains(_currentTarget))
        {
            //clear current target to be safe
            _currentTarget = null;
            //set some initial values
            Mob closestMob = null;
            float closestDist = float.PositiveInfinity;
            //for each mob in your valid targets
            foreach (Mob mob in _validTargets)
            {
                //figure out the distance it has to the end point
                float newDist = mob.GetDistanceToEnd();
                //if that is closer than the currently closest mob
                if (newDist < closestDist)
                {
                    //update the closest mob and the distance
                    closestMob = mob;
                    closestDist = newDist;
                }
            }
            //set the current target to the mob you found nearest to the end
            _currentTarget = closestMob;
        }
        //return whether or not you found a valid target
        return (_currentTarget != null);
    }


    IEnumerator LaserEffect()
    {
        //flash the laser on then off
        _laser.enabled = true;
        yield return new WaitForSeconds(0.1f);
        _laser.enabled = false;
    }
}