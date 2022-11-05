using UnityEngine; //Connect to Unity Engine

public class BuildTower : MonoBehaviour
{
    #region Variables
    [Header("Location & Build IDs")]
    [Tooltip("Set the ID number for the player this area belongs to")]
    public ushort playerID;
    [Tooltip("Set the ID of this build location")]
    public ushort locationID;
    [Tooltip("The ID of the current tower at location. Will be set automatically as player builds his towers.")]
    public int towerID;
    [Header("References")]
    [Tooltip("Add the PlayerManager reference from the scene")]
    private PlayerManager _playerManager;
    //Bool to check if location has a tower already so we can determine if we can build here. True at start as no towers have been built
    [HideInInspector] public bool locationFree = true;
    //Tag of build location will be used to determine if location is on the path or not for towers, or is starting point for mobs
    [HideInInspector] public string locationTag;
    //Variable to store the tower that is on this location
    public TowerBase currentTower;
    #endregion
    #region Startup
    private void Start()
    {
        //If player manager is null find it in the scene
        if (_playerManager == null)
        {
            _playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        }
        //On start retrieve the tag of the build location this instance is attached to
        locationTag = gameObject.tag;
    }
    #endregion
    public void PlaceTower(int index)
    {
        //Assert throws an error if TowerCardsArr was empty
        Debug.Assert(_playerManager.TowerCardsArr[index] != null);
        Debug.Log("Set Mob for Player " + playerID + "at location " + locationID);
        //Load the prefab and assign it's tower card
        GameObject prefab = Resources.Load($"Prefabs/TowersAndMobs/Tower{(tag == "Tower" ? "Ranged" : "Blockade")}") as GameObject;
        currentTower = Instantiate(prefab, transform).GetComponent<TowerBase>();
        //We still need a way to get proper index from towers.
        int towerIndex = Random.Range(0, 3);
        currentTower.Initialise(tag == "Tower" ? _playerManager.TowerCardsArr[index] as TowerCard : Resources.Load("Cards/Towers/TowerBlock0") as TowerCard);
        //Assign tower index to this location
        towerID = towerIndex;
        
    }

    public void SetMob(int index)
    {
        //Assert to test MobCardsArr
        Debug.Assert(_playerManager.MobCardsArr[index] != null);
        Debug.Log("Set Mob for Player " + playerID + "at location " + locationID);
        //Load prefab and assign it the card
        GameObject prefab = Resources.Load("Prefabs/TowersAndMobs/Mob") as GameObject;
        Mob mob = Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<Mob>();
        mob.Initialise(_playerManager.MobCardsArr[index] as MobCard, gameObject);
    }
}