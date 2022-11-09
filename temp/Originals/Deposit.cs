using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    [Header("Deposit")]
    public int _resources;
    [SerializeField] int _min;
    [SerializeField] int _max;
    [SerializeField] float _timeoutTimer; ////
    [SerializeField] float _timeoutReset = 3; ////

    [Header("Workers")]
    [SerializeField] bool _assignmentB;
    [SerializeField] GameObject _assignmentGO;

    [Header("Misc")]
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        _timeoutTimer = _timeoutReset; ////
    }

    // Update is called once per frame
    void Update()
    {
        Timeout();
    }

    public void Assignment()
    {
        _timeoutTimer = _timeoutReset; ////
        if (_resources != 0) ////
        {
            _assignmentB = !_assignmentB;
            _assignmentGO.SetActive(_assignmentB);
        }
        else ////
        {
            _assignmentB = false;
            _assignmentGO.SetActive(false);
        }
    }

    public void Add()
    {
        int minInv = 10;
        Worker2 finalWorker = null;
        _timeoutTimer = _timeoutReset; ////

        for (int i = 0; i < gameManager.WorkerList.Count; i++)
        {
            Worker2 currentWorker = gameManager.WorkerList[i];
            if (currentWorker._inv <= minInv && currentWorker._assignedDepositTransform == null)
            {
                minInv = currentWorker._inv;
                finalWorker = currentWorker;
            }
        }

        if (finalWorker == null)
            return;

        finalWorker._assignedDepositTransform = transform;
        finalWorker._targetDesination = transform;
    }

    public void Remove()
    {
        int minInv = 10;
        Worker2 finalWorker = null;
        _timeoutTimer = _timeoutReset; ////

        for (int i = 0; i < gameManager.WorkerList.Count; i++)
        {
            Worker2 currentWorker = gameManager.WorkerList[i];
            if (currentWorker._inv <= minInv && currentWorker._assignedDepositTransform == this.transform)
            {
                minInv = currentWorker._inv;
                finalWorker = currentWorker;
            }
        }

        if (finalWorker == null)
            return;

        finalWorker._assignedDepositTransform = null;
    }

    void Timeout() ////
    {
        if (_assignmentB)
        {
            _timeoutTimer -= Time.deltaTime;
            if (_timeoutTimer <= 0)
            {
                Assignment();
                _timeoutTimer = _timeoutReset;
            }
        }
    }
}
