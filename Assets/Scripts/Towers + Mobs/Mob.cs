using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    [SerializeField] private MobCard _myCard;
    [SerializeField] private ushort _playerId;
    public ushort PlayerID { get => _playerId; }
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _moveDebuff, _minDist;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private float _healthCurrent, _healthMax, _attackPower, _attackRate, _attackDelay;
    private int _resourceCostA, _resourceCostB, _pointWorth;
    private TowerBlockade _blockade;
    private Mob _rearNeighbour;
    private int _pathId;
    public static ushort _mobCounter;

    [SerializeField] private int _deathResourcePenalty = 2;

    [SerializeField] private int _waypointIndex;
    [SerializeField] private GameObject _waypointParent;
    [SerializeField] private Transform[] _waypoints;
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
        //if you have a card equpped, initialise it
        if (_myCard != null)
            Initialise(_myCard, _waypointParent, 0, 0);
    }

    private void ScrapePath(GameObject parent)
    {
        //get all the waypoint transforms
        Transform[] _points = parent.GetComponentsInChildren<Transform>();
        Debug.Log(_points.Length + "points");
        //set your waypoint list length to the length of that list (minus one for the parent object)
        _waypoints = new Transform[_points.Length];
        Debug.Log(_waypoints.Length + "size array");

        //loop through and add each waypoint to your waypoint list
        for (int i = 0; i < _points.Length; i++)
        {
            _waypoints[i] = _points[i];
        }
    }

    private void Start()
    {
        //max out your health
        _healthCurrent = _healthMax;
        //figure out the direction to your current waypoint
        _moveDir = GetNewDirection();
        //get the animator
        _anim = GetComponentInChildren<Animator>();
    }

    public void Initialise(MobCard mobCard, GameObject pathParent, ushort playerId, int pathId)
    {
        _playerId = playerId;
        _moveSpd = mobCard.moveSpd;
        _healthMax = mobCard.healthMax;
        _attackPower = mobCard.attackPower;
        _attackRate = mobCard.attackRate;
        transform.localScale = Vector3.one * mobCard.scale;
        if (pathParent != null)
            ScrapePath(pathParent);
        _myCard ??= mobCard;

        _pointWorth = mobCard.pointWorth;
        _resourceCostA = mobCard.resourceCostA;
        _resourceCostB = mobCard.resourceCostB;

        _pathId = pathId;
    }

    private void Update()
    {
        //if currently dying do nothing
        if (_dying)
            return;

        //if you have a blockade, attack it
        if (_blockade != null)
            ManageAttack();
        else    //otherwise move
            Move();
        //check for any Damage Over Time effects
        CalculateDot();
    }

    private void Move()
    {
        //calculate your move debuff
        _moveDebuff = CalculateSlow();
        //if you have not reached the current waypoint
        if (Vector3.Distance(transform.position, _waypoints[_waypointIndex].position) > _minDist)
        {
            //move towards it based on your speed and move debuff
            transform.position += _moveDir * _moveSpd * _moveDebuff * Time.deltaTime;
        }
        else
        {
            //snap to the waypoint position
            transform.position = _waypoints[_waypointIndex].position;
            UpdateWaypoint();
        }
    }

    private void UpdateWaypoint()
    {
        _waypointIndex++;

        if (_waypointIndex >= _waypoints.Length)
            ReachEndOfPath();
        else
            _moveDir = GetNewDirection();
    }

    private Vector3 GetNewDirection()
    {
        return (_waypoints[_waypointIndex].position - transform.position).normalized;
    }

    private void ReachEndOfPath()
    {
        if (_playerId == NetworkManager.GetPlayerIDNormalised())
            PlayerManager.PlayerManagerInstance.UpdatePoints(_pointWorth);
        StartCoroutine("EndOfLife");
    }

    private void MobDefeated()
    {
        if (_playerId == NetworkManager.GetPlayerIDNormalised())
        {
            int a = Mathf.RoundToInt(_resourceCostA / _deathResourcePenalty);
            int b = Mathf.RoundToInt(_resourceCostB / _deathResourcePenalty);
            PlayerManager.PlayerManagerInstance.UpdateResources(a, b);
        }
        StartCoroutine("EndOfLife");
    }

    private void ManageAttack()
    {
        if (!_blockade.CheckInMobRange(this))
            return;

        //count towards 0 and attack
        if (_attackDelay == 0)
        {
            Attack(_blockade);
        }
        else
        {
            _attackDelay = Mathf.MoveTowards(_attackDelay, 0, Time.deltaTime);
        }
    }

    private void Attack(TowerBlockade tb)
    {
        //apply damage to the blockade
        tb.TakeDamage(_attackPower);
        //reset your attack delay
        _attackDelay = _attackRate;
        //StartCoroutine("");
    }

    public void SetBlockade(TowerBlockade tower)
    {
        _blockade = tower;
    }

    private void SetRearNeighbour(Mob mob)
    {
        _rearNeighbour = mob;
    }

    public void RemoveRearNeighbours()
    {
        if (_rearNeighbour == null)
            return;

        Mob m = _rearNeighbour;
        _rearNeighbour = null;
        m.SetBlockade(null);
        m.RemoveRearNeighbours();
    }

    public float GetDistanceToEnd()
    {
        return Vector3.Distance(transform.position, _waypoints[_waypoints.Length - 1].position);
    }

    public void TakeDamage(float dmg)
    {
        //lose health
        _healthCurrent -= dmg;
        //play hit effect
        _anim.SetTrigger("Hit");
        //if you drop below 0 health, die
        if (_healthCurrent <= 0)
        {
            MobDefeated();
        }
    }

    public void AddSlow(float rate, float dur, TowerBase tower)
    {
        //construct the new slow effect
        Slowed newSlow;
        newSlow.rate = rate;
        newSlow.dur = dur;
        newSlow.tower = tower;

        for (int i = 0; i < _slowList.Count; i++)
        {
            //if the attacking tower already has a slow effect stored in this mob
            if (_slowList[i].tower == tower)
            {
                //refresh that effect and exit
                _slowList[i] = newSlow;
                return;
            }
        }

        //otherwise add a new entry to the slow list
        _slowList.Add(newSlow);
    }

    public void AddDot(float dmg, float dur, TowerBase tower)
    {
        //construct the new DoT effect
        Dotted newDot;
        newDot.dmg = dmg;
        newDot.time = dur;
        newDot.sec = 1;
        newDot.tower = tower;

        for (int i = 0; i < _dotList.Count; i++)
        {
            //if the attacking tower already has a DoT effect stored in this mob
            if (_dotList[i].tower == tower)
            {
                //refresh that effect and exit
                _dotList[i] = newDot;
                return;
            }
        }
        //otherwise add a new entry to the DoT list
        _dotList.Add(newDot);
    }

    private float CalculateSlow()
    {
        //initialise the debuff value
        float finalSlow = 0;
        //for each slow effect currently active
        for (int i = 0; i < _slowList.Count; i++)
        {
            //copy the effect details
            var newSlow = _slowList[i];
            //decrease the effect duration towards 0
            newSlow.dur = Mathf.MoveTowards(newSlow.dur, 0, Time.deltaTime);
            //if the duration is over 0, add the rate to the debuff value
            if (newSlow.dur > 0)
                finalSlow += _slowList[i].rate;
            else
            {
                //otherwise remove the debuff from the list and keep looping
                _slowList.RemoveAt(i);
                i--;
                continue;
            }
            //replace the entry with the new duration values
            _slowList[i] = newSlow;
        }
        //return a slow debuff no slower than 30% of your normal move spd
        return Mathf.Max(0.3f, 1f - finalSlow);
    }

    private void CalculateDot()
    {
        //for each DoT effect currently active
        for (int i = 0; i < _dotList.Count; i++)
        {
            //copy the effect details
            var newDot = _dotList[i];
            //cound down each effect's per second
            newDot.sec = Mathf.MoveTowards(newDot.sec, 0, Time.deltaTime);
            //if a second has elapsed for that effect
            if (newDot.sec <= 0)
            {
                //reduce the remaining time
                newDot.time--;
                //apply damage
                TakeDamage(newDot.dmg);
                //refresh the countdown second
                newDot.sec++;
            }
            //if the DoT has elapsed
            if (newDot.time <= 0)
            {
                //remove it from the list and keep looping
                _dotList.RemoveAt(i);
                i--;
                continue;
            }
            //replace the entry with the new duration values
            _dotList[i] = newDot;
        }
    }

    IEnumerator EndOfLife()
    {
        //start dying and wait one frame for towers to complete their attack loops
        _dying = true;
        yield return null;
        //for every tower on the map
        foreach (TowerBase tower in FindObjectsOfType<TowerBase>())
        {
            //check if it needs to remove this mob from its targets
            tower.CheckMobForRemoval(this);
        }
        RemoveRearNeighbours();
        _mobCounter--;

        //reset the mob
        Reset();
        gameObject.SetActive(false);
    }

    private void Reset()
    {
        _dying = false;
        _moveDebuff = 0;
        _healthCurrent = _healthMax;
        _blockade = null;
        _rearNeighbour = null;
        _slowList.Clear();
        _dotList.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        //if you're already blocked do nothing
        if (_blockade != null)
            return;

        //if you collide with another mob
        if (other.TryGetComponent<Mob>(out Mob m))
        {
            //if that mob is blocked
            if (m._pathId == _pathId && m._blockade != null)
            {
                //move down the neighbour chain until you find the end mob
                while (m._rearNeighbour != null)
                {
                    m = m._rearNeighbour;
                }
                //add the mob as a blockade
                SetBlockade(m._blockade);
                //add yourself to the blockade list
                m._blockade.AddBlockedTarget(this);
                m.SetRearNeighbour(this);
            }
        }
    }
}
