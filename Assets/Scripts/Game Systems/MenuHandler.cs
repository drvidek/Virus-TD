using System.Collections;
using System.Collections.Generic;
using UnityEngine; //Connect to Unity Engine
using UnityEngine.UI; //Allow use of Canvas elements 
using UnityEngine.SceneManagement; //Use Scene Management to switch scenes
using TMPro;

public class MenuHandler : MonoBehaviour
{
    #region Variables
    //Struct and Private Array of all Towers in game and whether they are purchased or not
    struct MobsInGame
    {
        public MobCard mob;
        public bool purchased;
    }
    private MobsInGame[] _mobsInGame = new MobsInGame[8];
    //Struct and Private Array of all Mobs in game and whether they are purchased or not 
    struct TowersInGame
    {
        public TowerCard tower;
        public bool purchased;
    }
    private TowersInGame[] _towersInGame = new TowersInGame[8];
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
    //Private variable to store amount of points from save file 
    [SerializeField] private int _points;
    #endregion
    #region Setup
    private void Awake()
    {
        //Load card lists with cards from resources
        for (int i = 0; i < 8; i++)
        {
            _mobsInGame[i].mob = Resources.Load<MobCard>("Cards/Mobs/Mob" + i);
            _towersInGame[i].tower = Resources.Load<TowerCard>("Cards/Towers/Tower" + i);
            if (i < 4)
            {
                _mobsInGame[i].purchased = true;
                _towersInGame[i].purchased = true;
            }
            else
            {
                _mobsInGame[i].purchased = false;
                _towersInGame[i].purchased = false;
            }
        }
        //Temporarily fill hand with cards from arrays
        mobsInHand[0] = _mobsInGame[0].mob;
        mobsInHand[1] = _mobsInGame[1].mob;
        mobsInHand[2] = _mobsInGame[2].mob;
        mobsInHand[3] = _mobsInGame[3].mob;
        towersInHand[0] = _towersInGame[0].tower;
        towersInHand[1] = _towersInGame[1].tower;
        towersInHand[2] = _towersInGame[2].tower;
        towersInHand[3] = _towersInGame[3].tower;      
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
            //Update the image and set non-interactable if already exists in hand or costs more than you have
            for (int i = 0; i < 8; i++)
            {
                //Adjust text on towers buttons
                _buttons[i].transform.GetChild(0).GetComponent<Text>().text = _towersInGame[i].tower.title;
                _buttons[i].transform.GetChild(1).GetComponent<Text>().text = _towersInGame[i].tower.description;
                _buttons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Power: " + _towersInGame[i].tower.attackPower.ToString() +
                    "<br>Fire Rate: " + _towersInGame[i].tower.attackRate.ToString() +
                    "<br>Range: " + _towersInGame[i].tower.attackRange.ToString() +
                    "<br>Damage Radius: " + _towersInGame[i].tower.attackRadius.ToString();
                if (_towersInGame[i].purchased == true) _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Purchased";
                else _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Cost: " + _towersInGame[i].tower.pointWorth + " points";
                //_buttons[i].GetComponent<Image>().sprite = _towersInGame[i].tower.towerImage;
                //If we can afford card or it has been purchased make it selectable else we can't choose it
                if (_towersInGame[i].tower.pointWorth <= _points || _towersInGame[i].purchased) _buttons[i].GetComponent<Button>().interactable = true;
                else _buttons[i].GetComponent<Button>().interactable = false;
                //Now check if we have it in hand already and make it unselectable if it is and modify text
                foreach (TowerCard tower in towersInHand)
                {
                    if (tower == _towersInGame[i].tower)
                    {
                        _buttons[i].GetComponent<Button>().interactable = false;
                        _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "In Hand";
                    } 
                }                
            }
        }
        //Else update the mob buttons with mob images
        else
        {
            //Find buttons tagged Mob
            _buttons = GameObject.FindGameObjectsWithTag("Mob");
            //Update the image and set non-interactable if already exists in hand or costs more than you have
            for (int i = 0; i < 8; i++)
            {
                //Adjust text on mob buttons
                _buttons[i].transform.GetChild(0).GetComponent<Text>().text = _mobsInGame[i].mob.title;
                _buttons[i].transform.GetChild(1).GetComponent<Text>().text = _mobsInGame[i].mob.description;
                _buttons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Move Speed: " + _mobsInGame[i].mob.moveSpd.ToString() +
                    "<br>Max Health: " + _mobsInGame[i].mob.healthMax.ToString() +
                    "<br>Power: " + _mobsInGame[i].mob.attackPower.ToString() +
                    "<br>Hit Rate: " + _mobsInGame[i].mob.attackRate.ToString();
                if (_mobsInGame[i].purchased == true) _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Purchased";
                else _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Cost: " + _mobsInGame[i].mob.pointWorth + " points";
                //_buttons[i].GetComponent<Image>().sprite = _mobsInGame[i].mob.mobImage;
                //If we can afford card or it has been purchased make it selectable else we can't choose it
                if (_mobsInGame[i].mob.pointWorth <= _points || _mobsInGame[i].purchased) _buttons[i].GetComponent<Button>().interactable = true;
                else _buttons[i].GetComponent<Button>().interactable = false;
                //Now check if we have it in hand already and make it unselectable if it is
                foreach (MobCard mob in mobsInHand)
                {
                    if (mob == _mobsInGame[i].mob)
                    {
                        _buttons[i].GetComponent<Button>().interactable = false;
                        _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "In Hand";
                    } 
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
                //_currentHandButtons[i].GetComponent<Image>().sprite = towersInHand[i].towerImage;
                _currentHandButtons[i].transform.GetChild(0).GetComponent<Text>().text = towersInHand[i].title;
                _currentHandButtons[i].transform.GetChild(1).GetComponent<Text>().text = towersInHand[i].description;
                _currentHandButtons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Power: " + towersInHand[i].attackPower.ToString() +
                    "<br>Fire Rate: " + towersInHand[i].attackRate.ToString() +
                    "<br>Range: " + towersInHand[i].attackRange.ToString() +
                    "<br>Damage Radius: " + towersInHand[i].attackRadius.ToString();
                _currentHandButtons[i].transform.GetChild(3).GetComponent<Text>().text = "Remove?";
            }
        }
        //else update it to include mobs
        else
        {
            for (int i = 0; i < 4; i++)
            {
                //_currentHandButtons[i].GetComponent<Image>().sprite = mobsInHand[i].mobImage;
                _currentHandButtons[i].transform.GetChild(0).GetComponent<Text>().text = mobsInHand[i].title;
                _currentHandButtons[i].transform.GetChild(1).GetComponent<Text>().text = mobsInHand[i].description;
                _currentHandButtons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Move Speed: " + mobsInHand[i].moveSpd.ToString() +
                    "<br>Max Health: " + mobsInHand[i].healthMax.ToString() +
                    "<br>Power: " + mobsInHand[i].attackPower.ToString() +
                    "<br>Hit Rate: " + mobsInHand[i].attackRate.ToString();
                _currentHandButtons[i].transform.GetChild(3).GetComponent<Text>().text = "Remove?";
            }
        }
    }
    public void HoldTower(int towerIndex)
    {
        //Set temporary reference to card player wants to use
        _tempTowerSelect = _towersInGame[towerIndex].tower;
        //Take away points for card purchased and change its purchased status
        if (!_towersInGame[towerIndex].purchased) 
        { 
            _points -= _towersInGame[towerIndex].tower.pointWorth;
            _towersInGame[towerIndex].purchased = true;
        }
        //We are swapping a tower
        _swappingTower = true;
    }
    public void HoldMob(int mobIndex)
    {
        //Make temp holder equal object selected
        _tempMobSelect = _mobsInGame[mobIndex].mob;
        //Take away points for card purchased and change its purchased status
        if (!_mobsInGame[mobIndex].purchased)
        {
            _points -= _mobsInGame[mobIndex].mob.pointWorth;
            _mobsInGame[mobIndex].purchased = true;
        }
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
