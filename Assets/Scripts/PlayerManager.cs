using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class PlayerManager : MonoBehaviour
{
    #region Private Variables
    private bool _ready;
    #endregion

    #region Properties
    /// <summary>
    /// Public property representing the player's point total.
    /// </summary>
    public float Points { get; private set; } = 0;
    /// <summary>
    /// Public property to access and set the tower cards present in the 'hand' for the current game.
    /// </summary>
    public ScriptableObject[] TowerCardsArr { get; private set; }
    /// <summary>
    /// Public property to access and set the mob cards present in the 'hand' for the current game.
    /// </summary>
    public ScriptableObject[] MobCardsArr { get; private set; }
    /// <summary>
    /// Public property to access and set the amount of resources in the current game.
    /// <para>Each array element represents a different resource type. For example:</para>
    /// <para><example><i>Index 0: Wood</i></example></para>
    /// <para><example><i>Index 1: Gold</i></example></para>
    /// </summary>
    public int[] ResourceCount { get; private set; } = new int[2] { 0, 0 };
    #endregion

    private void SendPlayerPointsMessage(ushort playerID, ushort points, ushort resourceA, ushort resourceB)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerID.points);
        m.AddUShort(playerID);
        m.AddUShort(points);
        m.AddUShort(resourceA);
        m.AddUShort(resourceB);
        NetworkManager.NetworkManagerInstance.GameClient.Send(m);
    }

    public void ToggleReadyStatus()
    {
        _ready = !_ready;
        SendReadyMessage(_ready);
    }

    public void ResetReadyStatus()
    {
        _ready = false;
    }

    private void SendReadyMessage(bool ready)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerID.playerReady);
        m.AddBool(ready);
        NetworkManager.NetworkManagerInstance.GameClient.Send(m);
    }

    public int workerCount = 0;
    public int workerCost = 10;
    public int blockTowerCost = 10;
    //I used start to set a default value for resources so I could test purchasing features
    //Also used to populate starting hand arrays with random cards to see if UI was updating properly
    private void Start()
    {
        ResourceCount = new int[] { 10, 10 };
        GameManager _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ScriptableObject[,] _deck = _gameManager.Deck;
        TowerCardsArr = new ScriptableObject[4];
        for (int i = 0; i < 4; i++)
        {
            TowerCardsArr[i] = _deck[0, Random.Range(0, 7)];
        }
        MobCardsArr = new ScriptableObject[4];
        for (int i = 0; i < 4; i++)
        {
            MobCardsArr[i] = _deck[1, Random.Range(0, 7)];
        }
    }
    //Allow changing of resource points from outside script from UI functions for purchasing mobs, towers and workers
    public void AdjustResources(int index, int adjustAmount)
    {
        ResourceCount[index] += adjustAmount;
    }

}
