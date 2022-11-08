using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using TMPro;


public class Deposit : MonoBehaviour
{
    [Header("Deposit")]
    public int _resources;
    [SerializeField] int _min;
    [SerializeField] int _max;
    float _reloadTimer;
    [SerializeField] TextMeshProUGUI _resourceDisplay;

    [Header("Workers")]
    [SerializeField] bool _assignmentB;
    [SerializeField] GameObject _assignmentGO;

    [Header("Misc")]
    PlayerManager playerManager;

    [SerializeField] private ushort _resourceID;
    private static Deposit[,] _deposits = new Deposit[2,9];
    [SerializeField] private ushort _playerID;

    private void SpawnResource(ushort value, string type)
    {
        tag = type;
        _resources = (int)value;
    }

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        _deposits[_playerID,_resourceID] = this;

    }

    // Update is called once per frame
    void Update()
    {
        //_reloadTimer += Time.deltaTime;
        //if(_resources < 10)
        //{
        //    if(_reloadTimer >= 2)
        //    {
        //        _resources++;
        //        _reloadTimer = 0;
        //    }
        //}
        _resourceDisplay.enabled = (tag != "Untagged" && GameManager.CurrentState != GameState.Build);
        _resourceDisplay.text = $"{_resources} {tag.Remove(0,7)}" ;

    }

    public void Assignment()
    {
        if (_playerID == NetworkManager.GetPlayerIDNormalised())
        {
            _assignmentB = !_assignmentB;
                _assignmentGO.SetActive(_assignmentB);
        }
    }

    public void Add()
    {
        int minInv = 10;
        Worker finalWorker = null;

        for (int i = 0; i < playerManager.workerList.Count; i++)
        {
            Worker currentWorker = playerManager.workerList[i];
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
        Worker finalWorker = null;

        for (int i = 0; i < playerManager.workerList.Count; i++)
        {
            Worker currentWorker = playerManager.workerList[i];
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
    
    [MessageHandler((ushort)ServerToClientID.resourceSpawn)]
    private static void ReadResourceMessageFromServer(Message message)
    {
        ushort id = message.GetUShort();
        ushort value = message.GetUShort();
        string type = message.GetString();
        if (_deposits[0,id] != null)
        {
            _deposits[0,id].SpawnResource(value, type);
        }
        if (_deposits[1, id] != null)
        {
            _deposits[1, id].SpawnResource(value, type);
        }
    }
}
