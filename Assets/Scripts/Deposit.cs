using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    [Header("Deposit")]
    public int _resources;
    [SerializeField] int _min;
    [SerializeField] int _max;
    float _reloadTimer;

    [Header("Workers")]
    [SerializeField] bool _assignmentB;
    [SerializeField] GameObject _assignmentGO;

    [Header("Misc")]
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        _reloadTimer += Time.deltaTime;
        if(_resources < 10)
        {
            if(_reloadTimer >= 2)
            {
                _resources++;
                _reloadTimer = 0;
            }
        }
    }

    void Reroll()
    {
        _resources = Random.Range(_min, _max);
    }

    public void Assignment()
    {
        _assignmentB = _assignmentB ? false : true;
        if (_assignmentB)
            _assignmentGO.SetActive(true);
        if (!_assignmentB)
            _assignmentGO.SetActive(false);
    }

    public void Add()
    {
        int _workerInv = 10;
        GameObject _worker = null;


        if (gameManager.WorkerList[0].GetComponent<Worker>()._inv <= _workerInv && gameManager.WorkerList[0].GetComponent<Worker>()._assignedDepositTransform == null && gameManager.WorkerList.Count >= 1)
        {
            _workerInv = gameManager.WorkerList[0].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[0];
        }

        if (gameManager.WorkerList[1].GetComponent<Worker>()._inv <= _workerInv && gameManager.WorkerList[1].GetComponent<Worker>()._assignedDepositTransform == null && gameManager.WorkerList.Count >= 2)
        {
            _workerInv = gameManager.WorkerList[1].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[1];
        }

        if (gameManager.WorkerList[2].GetComponent<Worker>()._inv <= _workerInv && gameManager.WorkerList[2].GetComponent<Worker>()._assignedDepositTransform == null && gameManager.WorkerList.Count >= 3)
        {
            _workerInv = gameManager.WorkerList[2].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[2];
        }

        if (gameManager.WorkerList[3].GetComponent<Worker>()._inv <= _workerInv && gameManager.WorkerList[3].GetComponent<Worker>()._assignedDepositTransform == null && gameManager.WorkerList.Count >= 4)
        {
            _workerInv = gameManager.WorkerList[3].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[3];
        }

        if (gameManager.WorkerList[4].GetComponent<Worker>()._inv <= _workerInv && gameManager.WorkerList[4].GetComponent<Worker>()._assignedDepositTransform == null && gameManager.WorkerList.Count == 5)
        {
            _workerInv = gameManager.WorkerList[4].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[4];
        }

        _worker.GetComponent<Worker>()._assignedDepositTransform = transform;
        _worker.GetComponent<Worker>()._targetDesination = transform;
    }

    public void Remove()
    {
        int _workerInv = 0;
        GameObject _worker = null;


        if (gameManager.WorkerList[0].GetComponent<Worker>()._inv >= _workerInv && gameManager.WorkerList[0].GetComponent<Worker>()._assignedDepositTransform == transform && gameManager.WorkerList.Count >= 1)
        {
            _workerInv = gameManager.WorkerList[0].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[0];
        }

        if (gameManager.WorkerList[1].GetComponent<Worker>()._inv >= _workerInv && gameManager.WorkerList[1].GetComponent<Worker>()._assignedDepositTransform == transform && gameManager.WorkerList.Count >= 2)
        {
            _workerInv = gameManager.WorkerList[1].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[1];
        }

        if (gameManager.WorkerList[2].GetComponent<Worker>()._inv >= _workerInv && gameManager.WorkerList[2].GetComponent<Worker>()._assignedDepositTransform == transform && gameManager.WorkerList.Count >= 3)
        {
            _workerInv = gameManager.WorkerList[2].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[2];
        }

        if (gameManager.WorkerList[3].GetComponent<Worker>()._inv >= _workerInv && gameManager.WorkerList[3].GetComponent<Worker>()._assignedDepositTransform == transform && gameManager.WorkerList.Count >= 4)
        {
            _workerInv = gameManager.WorkerList[3].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[3];
        }

        if (gameManager.WorkerList[4].GetComponent<Worker>()._inv >= _workerInv && gameManager.WorkerList[4].GetComponent<Worker>()._assignedDepositTransform == transform && gameManager.WorkerList.Count == 5)
        {
            _workerInv = gameManager.WorkerList[4].GetComponent<Worker>()._inv;
            _worker = gameManager.WorkerList[4];
        }

        _worker.GetComponent<Worker>()._assignedDepositTransform = null;
    }
}
