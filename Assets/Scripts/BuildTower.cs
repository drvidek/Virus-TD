using UnityEngine; //Connect to Unity Engine

public class BuildTower : MonoBehaviour
{
    #region Variables
    //Bool to check if location has a tower already so we can determine if we can build here. True at start as no towers have been built
    [HideInInspector] public bool locationFree = true;
    //Tag of build location will be used to determine if location is on the path or not for towers, or is starting point for mobs
    [HideInInspector] public string locationTag;
    //public TowerBase currentTower;
    #endregion
    #region Startup
    private void Start()
    {
        locationTag = gameObject.tag;
    }
    #endregion
}
