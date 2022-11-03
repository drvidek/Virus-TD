using System.Collections.Generic;
using System.Collections;
using UnityEngine; //Connect to Unity Engine
using RiptideNetworking;

public class BuildTower : MonoBehaviour
{
    #region Variables
    [Header("Location & Build IDs")]
    [Tooltip("Set the ID number for the player this area belongs to")]
    public ushort playerID;
    [Tooltip("Set the ID of this build location")]
    public ushort locationID;
    [Tooltip("The ID of the current tower at location. Will be set automatically as player builds his towers.")]
    public ushort towerID;
    public static BuildTower[,] buildArray = new BuildTower[2, 100];
    private List<ushort> _mobsToSpawn = new List<ushort>();
    private float _mobSpawnRate;
    //Bool to check if location has a tower already so we can determine if we can build here. True at start as no towers have been built
    [HideInInspector] public bool locationFree = true;
    //Tag of build location will be used to determine if location is on the path or not for towers, or is starting point for mobs
    [HideInInspector] public string locationTag;
    //public TowerBase currentTower;
    #endregion
    #region Startup
    private void Start()
    {
        //On start retrieve the tag of the build location this instance is attached to
        buildArray[playerID, locationID] = this;
        locationTag = gameObject.tag;
    }
    #endregion

    #region Towers
    public void PlaceTowerFromPlayerInput(ushort playerIdCheck)
    {
        if (playerIdCheck != playerID)
            return;

        towerID = (ushort)Random.Range(0, 3);
        Debug.Log("Set Tpwer for Player " + playerID + "at location " + locationID);
        SpawnTowerFromID(towerID, playerID);
        SendTowerMessage();
    }


    public void PlaceTowerFromMessage(ushort mTowerID, ushort playerId)
    {
        SpawnTowerFromID(mTowerID, playerId);
    }

    private void SpawnTowerFromID(ushort spawningID, ushort playerId)
    {
        if (!locationFree)
            return;

        towerID = spawningID;
        GameObject prefab = Resources.Load($"Prefabs/TowersAndMobs/Tower{(tag == "Tower" ? "Ranged" : "Blockade")}") as GameObject;
        TowerBase newTower = Instantiate(prefab, transform).GetComponent<TowerBase>();
        newTower.Initialise(Resources.Load($"Cards/Towers/Tower{(tag == "Tower" ? towerID.ToString() : "Block0")}") as TowerCard, playerId);

        locationFree = false;
    }
    public void SendTowerMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerID.towerSpawn);
        m.AddUShort(playerID);
        m.AddUShort(locationID);
        m.AddUShort(towerID);

        NetworkManager.NetworkManagerInstance.GameClient.Send(m);
    }
    #endregion

    #region Mobs
    public void SetMobFromPlayerInput(ushort playerIdCheck)
    {
        if (playerIdCheck != playerID)
            return;
        Debug.Log("Set Mob for Player " + playerID + "at location " + locationID);
        ushort mobIndex = (ushort)Random.Range(0, 3);
        AddMobToList(mobIndex);
        SendMobMessage(mobIndex);
    }

    void Update()
    {
        if (_mobsToSpawn.Count > 0 && GameManager.CurrentState == GameState.Play)
        {
            ManageMobSpawnTimer();
        }

    }

    private void ManageMobSpawnTimer()
    {
        if (_mobSpawnRate == 0)
        {
            _mobSpawnRate = SpawnMobFromID(_mobsToSpawn[0], playerID);
            _mobsToSpawn.RemoveAt(0);
            return;
        }
        _mobSpawnRate = Mathf.MoveTowards(_mobSpawnRate, 0, Time.deltaTime);
    }

    //public void SpawnMobFromMessage(ushort mMobID)
    //{
    //    SpawnMobFromID(mMobID);
    //}

    private void AddMobToList(ushort mob)
    {
        _mobsToSpawn.Add(mob);
    }

    private float SpawnMobFromID(ushort mobId, ushort playerId)
    {
        GameObject prefab = Resources.Load("Prefabs/TowersAndMobs/Mob") as GameObject;
        Mob mob = Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<Mob>();
        MobCard mobCard = Resources.Load("Cards/Mobs/Mob" + mobId.ToString()) as MobCard;
        mob.Initialise(mobCard, this.gameObject, playerId);
        return mobCard.scale;
    }

    public void SendMobMessage(ushort mobId)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerID.mobSpawn);
        m.AddUShort(playerID);
        m.AddUShort(locationID);
        m.AddUShort(mobId);

        NetworkManager.NetworkManagerInstance.GameClient.Send(m);
    }
    #endregion


    [MessageHandler((ushort)ServerToClientID.towerSpawn)]
    public static void TowerMessage(Message message)
    {
        ushort playerId = message.GetUShort();
        ushort locId = message.GetUShort();
        BuildTower bt = buildArray[playerId, locId];
        if (bt != null)
            bt.PlaceTowerFromMessage(message.GetUShort(), playerId);
    }

    [MessageHandler((ushort)ServerToClientID.mobSpawn)]
    public static void MobMessage(Message message)
    {
        ushort playerId = message.GetUShort();
        ushort locId = message.GetUShort();
        BuildTower bt = buildArray[playerId, locId];
        if (bt != null)
            bt.AddMobToList(message.GetUShort());
    }
    

}