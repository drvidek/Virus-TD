using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class PlayerManager : MonoBehaviour
{
    #region Private Variables
    private bool _ready;
    public bool Ready { get => _ready; }
    #endregion
    #region Public Variables
    [Header("UI Components")]
    [Tooltip("Add the ReadyPanel in here")]
    public GameObject readyPanel;
    #endregion
    #region Properties

    private static PlayerManager _playerManagerInstance;
    public static PlayerManager PlayerManagerInstance
    {
        //Property Read is the instance, public by default
        get => _playerManagerInstance;
        //private means only this instance of the class can access set
        private set
        {
            //set the instance to the value if the instance is null
            if (_playerManagerInstance == null)
            {
                _playerManagerInstance = value;
            }
            //if it is not null, check if the value is stored as the static instance
            else if (_playerManagerInstance != value)
            {
                //if not, throw a warning and destroy that instance

                //$ is to identify the string as containing an interpolated value
                Debug.LogWarning($"{nameof(PlayerManager)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    /// <summary>
    /// Public property representing the player's point total.
    /// </summary>
    public ushort Points { get; private set; } = MenuHandler.points;
    /// <summary>
    /// Public property to access and set the tower cards present in the 'hand' for the current game.
    /// </summary>
    [SerializeField] public TowerCard[] TowerCardsArr;// { get; private set; }
    /// <summary>
    /// Public property to access and set the mob cards present in the 'hand' for the current game.
    /// </summary>
    [SerializeField] public MobCard[] MobCardsArr;// { get; private set; }
    /// <summary>
    /// Public property to access and set the amount of resources in the current game.
    /// <para>Each array element represents a different resource type. For example:</para>
    /// <para><example><i>Index 0: Wood</i></example></para>
    /// <para><example><i>Index 1: Gold</i></example></para>
    /// </summary>
    public ushort[] ResourceCount { get; private set; } = new ushort[2] { 0, 0 };

    public List<Worker> workerList = new List<Worker>();
    #endregion

    private void Awake()
    {
        PlayerManagerInstance = this;
        //Deactivate ready panel just in case it is active
        readyPanel.SetActive(false);
        //Set the points to the value from MenuHandler at start
        //Points = MenuHandler.points;
    }

    public void SendPlayerPointsMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerID.points);
        m.AddUShort(NetworkManager.GetPlayerIDNormalised());
        m.AddUShort(Points);
        m.AddUShort(ResourceCount[0]);
        m.AddUShort(ResourceCount[1]);
        NetworkManager.NetworkManagerInstance.GameClient.Send(m);
    }

    public void ToggleReadyStatus()
    {
        if (GameManager.CurrentState != GameState.Build)
            return;

        _ready = !_ready;
        //If we are ready activate the splash screen
        readyPanel.SetActive(_ready);
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
        ResourceCount = new ushort[] { 50, 50 };
        GameManager _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ScriptableObject[,] _deck = _gameManager.Deck;

        TowerCardsArr = new TowerCard[4];
        for (int i = 0; i < 4; i++)
        {
            //int towerIndex = Random.Range(0, 7);
            //TowerCardsArr[i] = _deck[0, towerIndex] as TowerCard;
            TowerCardsArr[i] = MenuHandler.towersInHand[i];
            Debug.Log(MenuHandler.towersInHand[i].name);
        }
        MobCardsArr = new MobCard[4];
        for (int i = 0; i < 4; i++)
        {
            //int mobIndex = Random.Range(0, 7);
            //MobCardsArr[i] = _deck[1, mobIndex] as MobCard;
            Debug.Log(MenuHandler.mobsInHand[i].name);
            MobCardsArr[i] = MenuHandler.mobsInHand[i];
        }
    }
    //Allow changing of resource points from outside script from UI functions for purchasing mobs, towers and workers
    public void AdjustResources(int crypto, int ram)
    {
        ResourceCount[0] += (ushort) crypto;
        ResourceCount[1] += (ushort) ram;
    }

    public void EndPlayPhase()
    {
        SendReadyMessage(true);
    }

    public void UpdateResources(int a, int b)
    {
        ResourceCount[0] += (ushort)a;
        ResourceCount[1] += (ushort)b;
    }

    public void UpdatePoints(int score)
    {
        Points += (ushort)score;
    }


}
