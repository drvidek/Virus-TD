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
    public ushort towerID;
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
        locationTag = gameObject.tag;
    }
    #endregion

    public void PlaceTower()
    {
        Debug.Log("Set Mob for Player " + playerID + "at location " + locationID);
        /*GameObject prefab = Resources.Load($"Prefabs/TowersAndMobs/Tower{(tag == "Tower" ? "Ranged" : "Blockade")}") as GameObject;
        currentTower = Instantiate(prefab, transform).GetComponent<TowerBase>();
        int towerIndex = Random.Range(0, 3);
        currentTower.Initialise(Resources.Load($"Cards/Towers/Tower{(tag == "Tower" ? towerIndex.ToString() : "Block0")}") as TowerCard);
        towerID = towerIndex;
        */
    }

    public void SetMob()
    {
        Debug.Log("Set Mob for Player " + playerID + "at location " + locationID);
        /*GameObject prefab = Resources.Load("Prefabs/TowersAndMobs/Mob") as GameObject;
        Mob mob = Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<Mob>();
        int mobIndex = Random.Range(0, 3);
        mob.Initialise(Resources.Load("Cards/Mobs/Mob" + mobIndex.ToString()) as MobCard, gameObject);*/
    }
}