using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Variables    
    [Header("Timers")]
    [SerializeField] float _countdown = 1;
    [SerializeField] float _invDumpTimer = 1;
    [SerializeField] float _collectionTimer = 1;

    [Header("Basics")]
    [SerializeField] float _moveSpeed = 2;
    [SerializeField] Transform _baseTransform;
    public Transform _assignedDepositTransform;
    public Transform _targetDesination;

    [Header("Inventory")]
    [SerializeField] static int _invMax = 10;
    public static int InvMax { get => _invMax; }
    public int _inv;
    [SerializeField] int _resourceA;
    [SerializeField] int _resourceB;

    [Header("Misc")]
    Rigidbody rb;
    PlayerManager playerManager;

    [Header("Dev")]
    [SerializeField] TextMeshProUGUI yesman;
    #endregion


    //// Start is called before the first frame update
    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    playerManager = FindObjectOfType<PlayerManager>();
    //    //_baseTransform = GameObject.FindGameObjectWithTag("Base").transform;
    //    _targetDesination = _baseTransform;
    //    playerManager.workerList.Add(this);
    //}

    public void Initialise(Transform transform)
    {
        _baseTransform = transform;
        _targetDesination = _baseTransform;
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.workerList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetDesination.transform.position, _moveSpeed * Time.deltaTime);
        if (GameManager.CurrentState != GameState.Play || _assignedDepositTransform == null)
            _targetDesination = _baseTransform;
        _inv = _resourceA + _resourceB;

        State();

        yesman.text = "" + _inv;
    }

    void State()
    {

        //Crypto deposit extraction
        if (Vector3.Distance(_targetDesination.transform.position, transform.position) < 1 && _targetDesination.tag == "DepositCrypto")
        {
            _collectionTimer -= Time.deltaTime;

            if (_targetDesination.GetComponent<Deposit>()._resources <= 0)
                _targetDesination = _baseTransform;

            if (_collectionTimer <= 0)
            {
                _collectionTimer = _countdown;
                _resourceA++;
                _targetDesination.GetComponent<Deposit>()._resources--;
                _targetDesination = _assignedDepositTransform;
            }
            if (_inv >= _invMax)
                _targetDesination = _baseTransform;
        }

        //NFT deposit extraction
        if (Vector3.Distance(_targetDesination.transform.position, transform.position) < 1 && _targetDesination.tag == "DepositRAM")
        {
            _collectionTimer -= Time.deltaTime;

            if (_targetDesination.GetComponent<Deposit>()._resources <= 0)
            {
                _targetDesination = _baseTransform;
            }

            if (_collectionTimer <= 0)
            {
                _collectionTimer = _countdown;
                _resourceB++;
                _targetDesination.GetComponent<Deposit>()._resources--;
                _targetDesination = _assignedDepositTransform;
            }
            if (_inv >= _invMax)
                _targetDesination = _baseTransform;
        }

        //Resource depositing
        if (Vector3.Distance(_targetDesination.transform.position, transform.position) < 1 && _targetDesination.tag == "Base")
        {
            _invDumpTimer -= Time.deltaTime;
            if (_invDumpTimer <= 0)
            {
                _invDumpTimer = _countdown;

                if (_resourceA > 0)
                {
                    _resourceA--;
                    playerManager.ResourceCount[0]++;
                }

                if (_resourceB > 0)
                {
                    _resourceB--;
                    playerManager.ResourceCount[1]++;
                }
            }
            if (_inv <= 0 && _assignedDepositTransform != null)
                _targetDesination = _assignedDepositTransform;
        }

        if (_inv >= _invMax)
            _targetDesination = _baseTransform;

    }

    //private void OnDestroy()
    //{
    //    playerManager.WorkerList.Remove(this);
    //}
}
