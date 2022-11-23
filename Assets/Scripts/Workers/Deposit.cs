using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using TMPro;
using UnityEngine.UI;

public class Deposit : MonoBehaviour
{
    [Header("Deposit")]
    public int _resources;
    [SerializeField] int _min;
    [SerializeField] int _max;
    [SerializeField] float _timeoutTimer; //new
    [SerializeField] float _timeoutReset = 3; //new
    [SerializeField] TextMeshProUGUI _resourceDisplay;

    [Header("Workers")]
    [SerializeField] bool _assignmentB;
    [SerializeField] GameObject _assignmentGO;

    [Header("Misc")]
    PlayerManager playerManager;
    [SerializeField] Image _depositImage;

    [SerializeField] private ushort _resourceID;
    private static Deposit[,] _deposits = new Deposit[2, 9];
    [SerializeField] private ushort _playerID;

    private void SpawnResource(ushort value, string type)
    {
        _resources = (int)value;
        tag = type;
    }

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        _timeoutTimer = _timeoutReset; //new
        _deposits[_playerID, _resourceID] = this;
    }

    // Update is called once per frame
    void Update()
    {
        Timeout();
        _resourceDisplay.enabled = CheckEnable();
        _resourceDisplay.text = $"{_resources} {tag.Remove(0, 7)}";
        _depositImage.color = CheckEnable() ? Color.green : Color.white;

    }

    private bool CheckEnable()
    {
        return _resources > 0 && _playerID == NetworkManager.GetPlayerIDNormalised() && (NetworkManager.NetworkManagerInstance.GameClient.IsNotConnected || RoundManager.CurrentState != GameState.Build);
    }

    public void Assignment()
    {
        if (_playerID != NetworkManager.GetPlayerIDNormalised())
            return;

        _timeoutTimer = _timeoutReset; //new
        if (_resources != 0) //new
        {
            _assignmentB = !_assignmentB;
            _assignmentGO.SetActive(_assignmentB);
        }
        else //new
        {
            _assignmentB = false;
            _assignmentGO.SetActive(false);
        }
    }

    public void Add()
    {
        int minInv = 10;
        Worker finalWorker = null;
        _timeoutTimer = _timeoutReset; //new

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
        _timeoutTimer = _timeoutReset; //new

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

    void Timeout() //new
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

    [MessageHandler((ushort)ServerToClientID.resourceSpawn)]
    private static void ReadResourceMessageFromServer(Message message)
    {
        ushort id = message.GetUShort();
        ushort value = message.GetUShort();
        string type = message.GetString();
        if (_deposits[NetworkManager.GetPlayerIDNormalised(), id] != null)
        {
            _deposits[NetworkManager.GetPlayerIDNormalised(), id].SpawnResource(value, type);
        }
    }
}
