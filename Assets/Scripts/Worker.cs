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
    [SerializeField] int _invMax = 10;
    public int _inv;
    [SerializeField] int _crypto;
    [SerializeField] int _NFT;

    [Header("Misc")]
    Rigidbody rb;
    GameManager gameManager;

    [Header("Dev")]
    [SerializeField] TextMeshProUGUI yesman;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
        _baseTransform = GameObject.FindGameObjectWithTag("Base").transform;
        _targetDesination = _baseTransform;
        gameManager.WorkerList.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetDesination.transform.position, _moveSpeed * Time.deltaTime);
        if(gameManager.playPhase || _assignedDepositTransform == null)
            _targetDesination = _baseTransform;
        _inv = _crypto + _NFT;

        State();

        yesman.text = "" + _inv;
    }

    void State()
    {
        if (!gameManager.playPhase)
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
                    _crypto++;
                    _targetDesination.GetComponent<Deposit>()._resources--;
                    _targetDesination = _assignedDepositTransform;
                }
                if (_inv >= _invMax)
                    _targetDesination = _baseTransform;
            }

            //NFT deposit extraction
            if (Vector3.Distance(_targetDesination.transform.position, transform.position) < 1 && _targetDesination.tag == "DepositNFT")
            {
                _collectionTimer -= Time.deltaTime;

                if (_targetDesination.GetComponent<Deposit>()._resources <= 0)
                {
                    _targetDesination = _baseTransform;
                }

                if (_collectionTimer <= 0)
                {
                    _collectionTimer = _countdown;
                    _NFT++;
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

                    if (_crypto > 0)
                    {
                        _crypto--;
                        gameManager._cryptoI++;
                    }

                    if (_NFT > 0)
                    {
                        _NFT--;
                        gameManager._NFTI++;
                    }
                }
                if (_inv <= 0 && _assignedDepositTransform != null)
                    _targetDesination = _assignedDepositTransform;                
            }

            if (_inv >= _invMax)
                _targetDesination = _baseTransform;
        }
    }

    private void OnDestroy()
    {
        gameManager.WorkerList.Remove(gameObject);
    }
}
