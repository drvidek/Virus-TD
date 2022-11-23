using TMPro; //Use TextMeshPro components
using UnityEngine; //Connect to Unity Engine
using UnityEngine.UI; //Allow access and modification to Unitys Canvas UI elements
using RiptideNetworking;

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("Add the PlayerManager from the scene in here")]
    [SerializeField] private PlayerManager _playerManager;
    [Tooltip("Add the GameManager from the scene in here")]
    [SerializeField] private RoundManager _gameManager;
    [Header("UI Elements")]
    [Tooltip("Add the ready button from HUD panel in here")]
    public Button readyButton;
    [Tooltip("Add the PurchasePanel object from the heirarchy so we can enable it on selection of buildable tile.")]
    public GameObject purchasePanel;
    [Tooltip("Add the four Select buttons so their appearance can be change based on what type of tower/mob building spot was selected")]
    [SerializeField] private Button[] _buttons = new Button[4];
    [Tooltip("Add the Text fields from the HUDPanel here")]
    [SerializeField] private Text[] _hudText = new Text[5];
    [Tooltip("Entry 1 should be Win panel, entry 2 should be Lose panel")]
    [SerializeField] private GameObject[] _endResultUI = new GameObject[3];
    [Tooltip("Display both player's points from the game on the end panel")]
    [SerializeField] private TextMeshProUGUI _endingPointsDisplay;
    [Tooltip("Display the player's total points on the end panel")]
    [SerializeField] private TextMeshProUGUI _totalPointsDisplay;
    //String to determine if we are building a tower or mob
    private string _buildType = null;
    //HitInfo passed from InputManager so we can use it via UI buttons outside of original function
    private RaycastHit _hitInfo;
    //Timer variable to show correct time
    private static int _timer;
    #endregion

    private static UIManager _UIManagerInstance;
    public static UIManager UIManagerInstance
    {
        //Property Read is the instance, public by default
        get => _UIManagerInstance;
        //private means only this instance of the class can access set
        private set
        {
            //set the instance to the value if the instance is null
            if (_UIManagerInstance == null)
            {
                _UIManagerInstance = value;
            }
            //if it is not null, check if the value is stored as the static instance
            else if (_UIManagerInstance != value)
            {
                //if not, throw a warning and destroy that instance

                //$ is to identify the string as containing an interpolated value
                Debug.LogWarning($"{nameof(UIManager)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }
    #region Startup
    private void Awake()
    {
        UIManagerInstance = this;
    }
    private void Start()
    {
        //If PlayerManager reference is empty find it in scene
        if (_playerManager == null)
        {
            _playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        }
    }
    private void LateUpdate()
    {
        _hudText[0].text = "Time Left: " + _timer;
        _hudText[1].text = "Points: " + _playerManager.Points;
        _hudText[2].text = "Crypto: " + _playerManager.ResourceCount[0];
        _hudText[3].text = "RAM: " + _playerManager.ResourceCount[1];
        _hudText[4].text = "Workers Busy: " + _playerManager.workerCount + "/5";
    }
    #endregion
    #region Functions
    public void SetBuildType(int index)
    {
        //If buildType check variable equals Mob we will pass the index passed from the button pressed to the SetMob function of the build location
        if (_buildType == "Mob")
        {
            _hitInfo.transform.GetComponent<BuildTower>().SetMobFromPlayerInput((ushort)index);
        }
        //Else we pass it to the PlaceTower function
        else
        {
            _hitInfo.transform.GetComponent<BuildTower>().PlaceTowerFromPlayerInput((ushort)index);
        }
    }

    public void PurchaseFromResources(int cardID)
    {
        //Retrieve Card belonging to ID and take away the cost of the card from both resources based off build type
        if (_buildType == "Mob") _playerManager.AdjustResources(-MenuHandler.mobsInHand[cardID].resourceCostA, -MenuHandler.mobsInHand[cardID].resourceCostB);
        else _playerManager.AdjustResources(-MenuHandler.towersInHand[cardID].resourceCostA, -MenuHandler.towersInHand[cardID].resourceCostB);
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
                //Adjust text on buttons to match cards in hand
                _buttons[i].transform.GetChild(0).GetComponent<Text>().text = MenuHandler.mobsInHand[i].title;
                _buttons[i].transform.GetChild(1).GetComponent<Text>().text = MenuHandler.mobsInHand[i].description;
                _buttons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Move Speed: " + MenuHandler.mobsInHand[i].moveSpd.ToString() +
                    "<br>Max Health: " + MenuHandler.mobsInHand[i].healthMax.ToString() +
                    "<br>Power: " + MenuHandler.mobsInHand[i].attackPower.ToString() +
                    "<br>Hit Rate: " + MenuHandler.mobsInHand[i].attackRate.ToString();
                _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Cost: " + MenuHandler.mobsInHand[i].resourceCostA + " Crypto, " + MenuHandler.mobsInHand[i].resourceCostB + " RAM";
                _buttons[i].GetComponent<Image>().sprite = MenuHandler.mobsInHand[i].mobImage;
                //If we have enough resources in Resource B button is interactable, else it isn't
                if (_playerManager.ResourceCount[0] >= MenuHandler.mobsInHand[i].resourceCostA && _playerManager.ResourceCount[1] >= MenuHandler.mobsInHand[i].resourceCostB) _buttons[i].interactable = true;
                else _buttons[i].interactable = false;
            }
        }
        //Else build type is tower
        else
        {
            _buildType = "Tower";
            //For each button search through our towerTypes struct array for a Card that matches the Card held in players hand
            for (int i = 0; i < _buttons.Length; i++)
            {
                //Adjust text on buttons to match cards in hand
                _buttons[i].transform.GetChild(0).GetComponent<Text>().text = MenuHandler.towersInHand[i].title;
                _buttons[i].transform.GetChild(1).GetComponent<Text>().text = MenuHandler.towersInHand[i].description;
                _buttons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Power: " + MenuHandler.towersInHand[i].attackPower.ToString() +
                    "<br>Fire Rate: " + MenuHandler.towersInHand[i].attackRate.ToString() +
                    "<br>Range: " + MenuHandler.towersInHand[i].attackRange.ToString() +
                    "<br>Damage Radius: " + MenuHandler.towersInHand[i].attackRadius.ToString();
                _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Cost: " + MenuHandler.towersInHand[i].resourceCostA + " Crypto, " + MenuHandler.towersInHand[i].resourceCostB + " RAM";
                //If we have enough resources in Resource B button is interactable, else it isn't
                if (_playerManager.ResourceCount[0] >= MenuHandler.towersInHand[i].resourceCostA && _playerManager.ResourceCount[1] >= MenuHandler.towersInHand[i].resourceCostB) _buttons[i].interactable = true;
                else _buttons[i].interactable = false;
            }
        }
    }
    public void SetEndPanelOnStart()
    {
        _endResultUI[0].SetActive(false);
        _endResultUI[1].SetActive(false);
        _endResultUI[2].SetActive(false);
    }

    public void SetEndPanelOnEnd(bool win)
    {
        _endResultUI[0].SetActive(true);
        _endResultUI[1].SetActive(win);
        _endResultUI[2].SetActive(!win);
        int myID = NetworkManager.GetPlayerIDNormalised();
        int enemyID = myID == 0 ? 1 : 0;
        _endingPointsDisplay.text = $"Your Score: {RoundManager.ScoreTable[myID,0]}\nEnemy Score: {RoundManager.ScoreTable[enemyID, 0]}";
        _totalPointsDisplay.text = $"Points in bank: {MenuHandler.pointsInBank}";
    }

    #endregion

    [MessageHandler((ushort)ServerToClientID.timer)]
    private static void GetTimerMessage(Message message)
    {
        ushort timer = message.GetUShort();
        _timer = timer;
    }
}

