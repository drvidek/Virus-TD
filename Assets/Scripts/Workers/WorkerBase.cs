using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBase : MonoBehaviour
{
    [SerializeField] private int _playerID;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private int _workerMax;

    private void Update()
    {
        if ((ushort)_playerID != NetworkManager.GetPlayerIDNormalised())
            return;

        if (RoundManager.CurrentState == GameState.Play && _playerManager.workerCount < _workerMax)
        {
            GameObject prefab = Instantiate(Resources.Load("Prefabs/Workers + Deposits/Worker") as GameObject, transform.position, Quaternion.identity);
            Worker worker = prefab.GetComponent<Worker>();
            worker.Initialise(transform);
            _playerManager.workerCount++;
        }
    }

}
