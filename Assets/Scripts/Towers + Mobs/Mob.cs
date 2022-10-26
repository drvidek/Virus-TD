using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _moveDebuff, _minDist;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private float _healthCurrent, _healthMax, _attackPower, _attackRate, _attackDelay;
    [SerializeField] private int _waypointIndex;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private TowerBlockade _blockade;
    private struct Slowed
    {
        public float rate;
        public float dur;
        public TowerBase tower;
    }

    private List<Slowed> _slowList = new List<Slowed>();
    private struct Dotted { public float dmg, time; public TowerBase tower; }
    private List<Dotted> _dotList = new List<Dotted>();

    private void Start()
    {
        _moveDir = _waypoints[_waypointIndex].position - transform.position;
        _moveDir.Normalize();
    }

    private void Initialise()
    {

    }

    private void Update()
    {
        Move();
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
            _waypointIndex = 0;

        _moveDir = _waypoints[_waypointIndex].position - transform.position;
        _moveDir.Normalize();

        //else
        //{
        //    _moveDir = Vector3.zero;
        //    //EndOfLife();
        //}
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
        if (_healthCurrent <= 0)
        {
            EndOfLife();
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

    }

    private void EndOfLife()
    {
        foreach (TowerBase tower in FindObjectsOfType<TowerBase>())
        {
            tower.CheckMob(this);
        }

        Destroy(gameObject);
    }
}
