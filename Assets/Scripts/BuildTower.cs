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
    public static BuildTower[,] buildArray = new BuildTower[2,100];
    //Bool to check if location has a tower already so we can determine if we can build here. True at start as no towers have been built
    [HideInInspector] public bool locationFree = true;
    //Tag of build location will be used to determine if location is on the path or not for towers, or is starting point for mobs
    [HideInInspector] public string locationTag;
    //public TowerBase currentTower;
    #endregion
    #region Startup
    private void Start()
    {
        playerID -= 1;
        //On start retrieve the tag of the build location this instance is attached to
        buildArray[playerID,locationID] = this;
        locationTag = gameObject.tag;
    }
    #endregion

    #region Towers
    public void PlaceTowerFromPlayerInput()
    {
        towerID = (ushort)Random.Range(0, 3);
        Debug.Log("Set Mob for Player " + playerID + "at location " + locationID);
        SpawnTowerFromID(towerID);
        SendTowerMessage();
    }


    public void PlaceTowerFromMessage(ushort mTowerID)
    {
        SpawnTowerFromID(mTowerID);
    }

    private void SpawnTowerFromID(ushort spawningID)
    {
        if (!locationFree)
            return;

        towerID = spawningID;
        GameObject prefab = Resources.Load($"Prefabs/TowersAndMobs/Tower{(tag == "Tower" ? "Ranged" : "Blockade")}") as GameObject;
        TowerBase newTower = Instantiate(prefab, transform).GetComponent<TowerBase>();
        newTower.Initialise(Resources.Load($"Cards/Towers/Tower{(tag == "Tower" ? towerID.ToString() : "Block0")}") as TowerCard);

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
    public void SpawnMobFromPlayerInput()
    {
        Debug.Log("Set Mob for Player " + playerID + "at location " + locationID);
        ushort mobIndex = (ushort)Random.Range(0, 3);
        SpawnMobFromID(mobIndex);
        SendMobMessage(mobIndex);
    }

    public void SpawnMobFromMessage(ushort mMobID)
    {
        SpawnMobFromID(mMobID);
    }

    private void SpawnMobFromID(ushort mobID)
    {
        GameObject prefab = Resources.Load("Prefabs/TowersAndMobs/Mob") as GameObject;
        Mob mob = Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<Mob>();
        mob.Initialise(Resources.Load("Cards/Mobs/Mob" + mobID.ToString()) as MobCard, gameObject);
    }

    public void SendMobMessage(ushort mobID)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerID.mobSpawn);
        m.AddUShort(playerID);
        m.AddUShort(locationID);
        m.AddUShort(mobID);

        NetworkManager.NetworkManagerInstance.GameClient.Send(m);
    } 
    #endregion


    [MessageHandler((ushort)ServerToClientID.towerSpawn)]
    public static void TowerMessage(Message message)
    {
        BuildTower bt = buildArray[message.GetUShort(),message.GetUShort()];
        if (bt != null)
            bt.PlaceTowerFromMessage(message.GetUShort());
    }

    [MessageHandler((ushort)ServerToClientID.mobSpawn)]
    public static void MobMessage(Message message)
    {
        BuildTower bt = buildArray[message.GetUShort(), message.GetUShort()];
        if (bt != null)
            bt.SpawnMobFromMessage(message.GetUShort());
    }
}