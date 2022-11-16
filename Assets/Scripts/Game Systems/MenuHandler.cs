using System.Collections;
using System.Collections.Generic;
using UnityEngine; //Connect to Unity Engine
using UnityEngine.UI; //Allow use of Canvas elements 
using UnityEngine.SceneManagement; //Use Scene Management to switch scenes

public class MenuHandler : MonoBehaviour
{
    #region Variables
    //Private Array of all Towers in game
    private MobCard[] _mobsInGame = new MobCard[8];
    //Private array of all mobs in game 
    private TowerCard[] _towersInGame = new TowerCard[8];
    //Public static array of mob cards in hand so PlayerManager can get current hand
    public static TowerCard[] towersInHand = new TowerCard[4]; 
    //Public static array of tower cards in hand so PlayerManager can get current hand
    public static MobCard[] mobsInHand = new MobCard[4];
    //Serialized private field to store buttons to adjust cards between mob and tower mode
    [SerializeField] private GameObject[] _buttons = new GameObject[8];
    //Serialized field for buttons in Current Hand screen
    [SerializeField] private Button[] _currentHandButtons = new Button[4];
    //Private references to the currently selected cards to change with held cards
    private TowerCard _tempTowerSelect;
    private MobCard _tempMobSelect;
    //Bool to check if we are swapping a tower, or we are swapping a mob
    private bool _swappingTower;
    #endregion
    #region Setup
    private void Awake()
    {
        //Load card lists with cards from resources
        for (int i = 0; i < 8; i++)
        {
            _mobsInGame[i] = Resources.Load<MobCard>("Cards/Mobs/Mob" + i);
            _towersInGame[i] = Resources.Load<TowerCard>("Cards/Towers/Tower" + i);
        }
        //Temporarily fill hand with cards from arrays
        mobsInHand[0] = _mobsInGame[0];
        mobsInHand[1] = _mobsInGame[1];
        mobsInHand[2] = _mobsInGame[2];
        mobsInHand[3] = _mobsInGame[3];
        towersInHand[0] = _towersInGame[0];
        towersInHand[1] = _towersInGame[1];
        towersInHand[2] = _towersInGame[2];
        towersInHand[3] = _towersInGame[3];      
    }
    #endregion
    #region Manage Display & Cards
    public void UpdateButtons(string mode)
    {
        //If mode is towers update buttons with tower images
        if (mode == "Tower")
        {
            //Find buttons tagged tower
            _buttons = GameObject.FindGameObjectsWithTag("Tower");
            //Update the image and set non-interactable if already exists in hand
            for (int i = 0; i < 8; i++)
            {
                _buttons[i].GetComponent<Image>().sprite = _towersInGame[i].towerImage;
                _buttons[i].GetComponent<Button>().interactable = true;
                foreach (TowerCard tower in towersInHand)
                {
                    if (tower == _towersInGame[i]) _buttons[i].GetComponent<Button>().interactable = false;
                }
            }
        }
        //Else update the mob buttos with mob images
        else
        {
            //Find buttons tagged Mob
            _buttons = GameObject.FindGameObjectsWithTag("Mob");
            //Update the image and set non-interactable if already exists in hand
            for (int i = 0; i < 8; i++)
            {
                _buttons[i].GetComponent<Image>().sprite = _mobsInGame[i].mobImage;
                _buttons[i].GetComponent<Button>().interactable = true;
                foreach (MobCard mob in mobsInHand)
                {
                    if (mob == _mobsInGame[i]) _buttons[i].GetComponent<Button>().interactable = false;
                }
            }
        }
    }
    public void ShowHand()
    {
        //If swapping tower update current hand window to include towers
        if (_swappingTower)
        {
            for (int i = 0; i < 4; i++)
            {
                _currentHandButtons[i].GetComponent<Image>().sprite = towersInHand[i].towerImage;
            }
        }
        //else update it to include mobs
        else
        {
            for (int i = 0; i < 4; i++)
            {
                _currentHandButtons[i].GetComponent<Image>().sprite = mobsInHand[i].mobImage;
            }
        }
    }
    public void HoldTower(int towerIndex)
    {
        //Set temporary reference to card player wants to use
        _tempTowerSelect = _towersInGame[towerIndex];
        //We are swapping a tower
        _swappingTower = true;
    }
    public void HoldMob(int mobIndex)
    {
        //Make temp holder equal object selected
        _tempMobSelect = _mobsInGame[mobIndex];
        //We are swapping a mob
        _swappingTower = false;
    }
    public void SwapCards(int swapIndex)
    {
        //If swapping a tower, swap the temp holder into the corresponding slot in tower hand 
        if (_swappingTower) towersInHand[swapIndex] = _tempTowerSelect;
        //Else swap a mob
        else mobsInHand[swapIndex] = _tempMobSelect;
    }
    #endregion
    #region Game Functions
    public void EndGame()
    {
        #if UNITY_EDITOR
UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
    #endregion
}
