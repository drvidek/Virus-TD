using UnityEngine; //Connect to Unity Engine
using UnityEngine.UI; //Allow access and modification to Unitys Canvas UI elements

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("Add the PlayerManager from the scene in here")]
    [SerializeField] private PlayerManager _playerManager;
    //Create a struct to store various input data relating to our mobs including Card, Image and costs
    [System.Serializable]
    public struct MobTypes
    {
        public ScriptableObject mob;
        public Sprite mobImage;
        public int mobResourceCost;
        public int mobPointCost;
    }
    [Header("Mob & Tower Types")]
    [Tooltip("Enter the data including names, images and costs for available Mobs in the game.")]
    public MobTypes[] mobTypes = new MobTypes[8];
    //Create a struct to store various input data relating to our towers including Card, Image and costs
    [System.Serializable]
    public struct TowerTypes
    {
        public ScriptableObject tower;
        public Sprite towerImage;
        public int towerResourceCost;
        public int towerPointCost;
    }
    [Tooltip("Enter the data including names, images and costs for the available Towers in the game.")]
    public TowerTypes[] towerTypes = new TowerTypes[8];
    [Header("UI Elements")]
    [Tooltip("Add the PurchasePanel object from the heirarchy so we can enable it on selection of buildable tile.")]
    public GameObject purchasePanel;
    [Tooltip("Add the four Select buttons so their appearance can be change based on what type of tower/mob building spot was selected")]
    [SerializeField] private Button[] _buttons = new Button[4];
    [Tooltip("Add the HireWorker Button so we can control it's interactions")]
    [SerializeField] private Button _workerButton;
    [Tooltip("Add the GUI Skin from the UI folder here")]
    [SerializeField] private GUISkin _guiSkin;
    //String to determine if we are building a tower or mob
    private string _buildType = null;
    //HitInfo passed from InputManager so we can use it via UI buttons outside of original function
    private RaycastHit _hitInfo;
    #endregion
    #region Startup
    private void Start()
    {
        //If PlayerManager reference is empty find it in scene
        if (_playerManager == null)
        {
            _playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        }
        //Update worker purchase button to include cost of workers, can be moved to other function if cost will change
        _workerButton.GetComponentInChildren<Text>().text = "Hire Worker: " + _playerManager.workerCost + " Resources";
    }
    #endregion
    #region Functions
    public void SetBuildType(int index)
    {
        //If buildType check variable equals Mob we will pass the index passed from the button pressed to the SetMob function of the build location
        if (_buildType == "Mob")
        {
            _hitInfo.transform.GetComponent<BuildTower>().SetMobFromPlayerInput(NetworkManager.GetPlayerIDNormalised(), (ushort)index);
        }
        //Else we pass it to the PlaceTower function
        else
        {
            _hitInfo.transform.GetComponent<BuildTower>().PlaceTowerFromPlayerInput(NetworkManager.GetPlayerIDNormalised(), (ushort)index);
        }
    }
    public void HireWorkers()
    {
        //When hire worker button is pushed worker count is incremented
        _playerManager.workerCount++;
        //Adjust resources to take away cost of worker from Resource B
        _playerManager.AdjustResources(1, -_playerManager.workerCost);
        //If we still have enough resources button is interactable, else it isn't
        if (_playerManager.ResourceCount[1] >= _playerManager.workerCost)
        {
            _workerButton.interactable = true;
        }
        else
        {
            _workerButton.interactable = false;
        }
    }
    public void PurchaseFromResources(Text cost)
    {
        //If we have placed a mob take away the cost from the button from Resource B, else take it away from resource A because we built a tower
        if (_buildType == "Mob")
        {
            _playerManager.AdjustResources(1, -int.Parse(cost.text));
        }
        else
        {
            _playerManager.AdjustResources(0, -int.Parse(cost.text));
        }
    }
    public void UpdateDisplay(RaycastHit hitInfo)
    {
        //Make our local _hitinfo equal the value passed from the InputManager. This is so SetBuildType will be able to access the function from the correct location
        _hitInfo = hitInfo;
        //If tag is start build type is Mob
        if (hitInfo.transform.tag == "Start")
        {
            _buildType = "Mob";
            //For each button search through our mobTypes struct array for a Card that matches the Card held in players hand
            for (int i = 0; i < _buttons.Length; i++)
            {
                for (int n = 0; n < 8; n++)
                {
                    if (_playerManager.MobCardsArr[i] == mobTypes[n].mob)
                    {
                        //Assign Image that corresponds to Card in hand to the button and adjust it's text to display cost
                        _buttons[i].GetComponent<Image>().sprite = mobTypes[n].mobImage;
                        _buttons[i].GetComponentInChildren<Text>().text = mobTypes[n].mobResourceCost.ToString();
                        //If we have enough resources in Resource B button is interactable, else it isn't
                        if (_playerManager.ResourceCount[1] >= mobTypes[n].mobResourceCost)
                        {
                            _buttons[i].interactable = true;
                        }
                        else
                        {
                            _buttons[i].interactable = false;
                        }
                        //Break operation once we've assigned a Card as we don't need to keep looking for it
                        break;
                    }
                }
            }
        }
        //Else build type is tower
        else
        {
            _buildType = "Tower";
            //For each button search through our towerTypes struct array for a Card that matches the Card held in players hand
            for (int i = 0; i < _buttons.Length; i++)
            {
                for (int n = 0; n < 8; n++)
                {
                    if (_playerManager.TowerCardsArr[i] == towerTypes[n].tower)
                    {
                        //Assign Image that corresponds to Card in hand to the button and adjust it's text to display cost
                        _buttons[i].GetComponent<Image>().sprite = towerTypes[n].towerImage;
                        _buttons[i].GetComponentInChildren<Text>().text = towerTypes[n].towerResourceCost.ToString();
                        //If we have enough resources in Resource B button is interactable, else it isn't
                        if (_playerManager.ResourceCount[0] >= towerTypes[n].towerResourceCost)
                        {
                            _buttons[i].interactable = true;
                        }
                        else
                        {
                            _buttons[i].interactable = false;
                        }
                        //Break operation once we've assigned a Card as we don't need to keep looking for it
                        break;
                    }
                }
            }
        }
    }
    public void UpdateHand()
    {
        //Will be implemented once main menu scene is ready to implement
    }
    #endregion
    #region
    private void OnGUI()
    {
        //Apply the GUISkin to our IMGUI elements
        GUI.skin = _guiSkin;
        //Create a box to display the points, resources and worker count
        GUI.Box(new Rect(Screen.width / 6, (Screen.height / 18) * 17, Screen.width / 6, Screen.height / 18), "<b>Poinst: " + _playerManager.Points + "</b>");
        GUI.Box(new Rect((Screen.width / 6) * 2, (Screen.height / 18) * 17, Screen.width / 6, Screen.height / 18), "<b>Resources A: " + _playerManager.ResourceCount[0] + "</b>");
        GUI.Box(new Rect((Screen.width / 6) * 3, (Screen.height / 18) * 17, Screen.width / 6, Screen.height / 18), "<b>Resources B: " + _playerManager.ResourceCount[1] + "</b>");
        GUI.Box(new Rect((Screen.width / 6) * 4, (Screen.height / 18) * 17, Screen.width / 6, Screen.height / 18), "<b>Workers: " + _playerManager.workerCount + "</b>");
    }
    #endregion
}
