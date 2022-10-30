using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    [SerializeField] private MobSO _myCard;
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _moveDebuff, _minDist;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private float _healthCurrent, _healthMax, _attackPower, _attackRate, _attackDelay;
    [SerializeField] private int _waypointIndex;
    [SerializeField] private GameObject _waypointParent;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private TowerBlockade _blockade;
    private struct Slowed
    {
        public float rate;
        public float dur;
        public TowerBase tower;
    }

    private List<Slowed> _slowList = new List<Slowed>();
    private struct Dotted { public float dmg, time, sec; public TowerBase tower; }
    private List<Dotted> _dotList = new List<Dotted>();
    private bool _dying;
    private Animator _anim;

    private void OnValidate()
    {
        if (_waypointParent != null)
        {
            Transform[] _points = _waypointParent.GetComponentsInChildren<Transform>();
            _waypoints = new Transform[_points.Length-1];
            for (int i = 0; i < _points.Length; i++)
            {
                if (i > 0)
                {
                    _waypoints[i-1] = _points[i];
                }
            }
        }
        if (_myCard != null)
        Initialise(_myCard);
    }

    private void Start()
    {
        _healthCurrent = _healthMax;
        _moveDir = _waypoints[_waypointIndex].position - transform.position;
        _moveDir.Normalize();
        _anim = GetComponent<Animator>();
    }

    private void Initialise(MobSO mobCard)
    {
        _moveSpd = mobCard.moveSpd;
        _healthMax = mobCard.healthMax;
        _attackPower = mobCard.attackPower;
        _attackRate = mobCard.attackRate;
        transform.localScale = Vector3.one * mobCard.scale;
    }

    private void Update()
    {
        if (_dying)
            return;

        if (_blockade != null)
            ManageAttack();
        else
            Move();

        CalculateDot();
    }

    private void Move()
    {
        _moveDebuff = CalculateSlow();
        if (Vector3.Distance(transform.position, _waypoints[_waypointIndex].position) > _minDist)
        {
            transform.position += _moveDir * _moveSpd * _moveDebuff * Time.deltaTime;
        }
        else
        {
            transform.position = _waypoints[_waypointIndex].position;
            UpdateWaypoint();
        }
    }

    private void UpdateWaypoint()
    {
        _waypointIndex++;
        
        if (_waypointIndex >= _waypoints.Length)
        #region replace this code
            //currently they loop to their first waypoint
            //replace this with earning points
            _waypointIndex = 0;
        #endregion

        _moveDir = _waypoints[_waypointIndex].position - transform.position;
        _moveDir.Normalize();
    }

    private void ManageAttack()
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

    private void Attack()
    {
        _blockade.TakeDamage(_attackPower);
        _attackDelay = _attackRate;
        //StartCoroutine("LaserEffect");
    }

    public void SetBlockade(TowerBlockade tower)
    {
        _blockade = tower;
    }

    public float GetDistanceToEnd()
    {
        return Vector3.Distance(transform.position, _waypoints[_waypoints.Length - 1].position);
    }

    public void TakeDamage(float dmg)
    {
        _healthCurrent -= dmg;
        _anim.SetTrigger("Hit");
        if (_healthCurrent <= 0)
        {
            StartCoroutine("EndOfLife");
        }
    }

    public void AddSlow(float rate, float dur, TowerBase tower)
    {
        Slowed newSlow;
        newSlow.rate = rate;
        newSlow.dur = dur;
        newSlow.tower = tower;

        for (int i = 0; i < _slowList.Count; i++)
        {
            if (_slowList[i].tower == tower)
            {
                _slowList[i] = newSlow;
                return;
            }
        }
        _slowList.Add(newSlow);
    }

    public void AddDot(float dmg, float dur, TowerBase tower)
    {
        Dotted newDot;
        newDot.dmg = dmg;
        newDot.time = dur;
        newDot.sec = 1;
        newDot.tower = tower;

        for (int i = 0; i < _dotList.Count; i++)
        {
            if (_dotList[i].tower == tower)
            {
                _dotList[i] = newDot;
                return;
            }
        }
        _dotList.Add(newDot);
    }

    private float CalculateSlow()
    {
        float finalSlow = 0;
        for (int i = 0; i < _slowList.Count; i++)
        {
            var newSlow = _slowList[i];
            newSlow.dur = Mathf.MoveTowards(newSlow.dur, 0, Time.deltaTime);
            if (newSlow.dur > 0)
                finalSlow += _slowList[i].rate;
            else
            {
                _slowList.RemoveAt(i);
                i--;
                continue;
            }
            _slowList[i] = newSlow;
        }
        return 1f - finalSlow;
    }

    private void CalculateDot()
    {
        for (int i = 0; i < _dotList.Count; i++)
        {
            var newDot = _dotList[i];
            newDot.sec = Mathf.MoveTowards(newDot.sec, 0, Time.deltaTime);
            if (newDot.sec <= 0)
            {
                newDot.time--;
                TakeDamage(newDot.dmg);
                newDot.sec++;
            }
            if (newDot.time<= 0)
            {
                _dotList.RemoveAt(i);
                i--;
                continue;
            }
            _dotList[i] = newDot;
        }
    }

    IEnumerator EndOfLife()
    {
        _dying = true;
        yield return null;
        foreach (TowerBase tower in FindObjectsOfType<TowerBase>())
        {
            tower.CheckMob(this);
        }
        Reset();
        gameObject.SetActive(false);
    }

    private void Reset()
    {
        _dying = false;
        _moveDebuff = 0;
        _healthCurrent = _healthMax;
        _blockade = null;
        _slowList.Clear();
        _dotList.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Mob>(out Mob m))
        {
            if (m._blockade != null)
            {
                SetBlockade(m._blockade);
                _blockade.AddBlockedTarget(this);
            }
        }
    }
}
