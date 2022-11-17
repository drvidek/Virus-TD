using System.Collections;
using System.Collections.Generic;
using UnityEngine; //Connect to Unity Engine
using UnityEngine.UI; //Allow use of Canvas elements 
using UnityEngine.SceneManagement; //Use Scene Management to switch scenes
using TMPro; //Use TextMeshPro components
using System.IO;

public class MenuHandler : MonoBehaviour
{
    #region Variables
    //Struct and Private Array of all Towers in game and whether they are purchased or not
    public struct MobsInGame
    {
        public MobCard mob;
        public bool purchased;
    }
    public static MobsInGame[] mobsInGame = new MobsInGame[8];
    //Struct and Private Array of all Mobs in game and whether they are purchased or not 
    public struct TowersInGame
    {
        public TowerCard tower;
        public bool purchased;
    }
    public static TowersInGame[] towersInGame = new TowersInGame[8];
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
    //Static variable to store amount of points from save file 
    public static ushort points;
    #endregion
    #region Setup
    private void Awake()
    {
        //Load card lists with cards from resources
        for (int i = 0; i < 8; i++)
        {
            mobsInGame[i].mob = Resources.Load<MobCard>("Cards/Mobs/Mob" + i);
            towersInGame[i].tower = Resources.Load<TowerCard>("Cards/Towers/Tower" + i);
        }
        //If save file exists load data from it
        if (File.Exists(BinarySave.path)) 
        {
            BinarySave.LoadPlayerData(ref mobsInGame, ref towersInGame, mobsInHand, towersInHand, points);
        }
        //else assign the default 
        else {
            for (int i = 0;  i < 4; i++)
            {
                mobsInGame[i].purchased = true;
                mobsInHand[i] = mobsInGame[i].mob;
                towersInGame[i].purchased = true;
                towersInHand[i] = towersInGame[i].tower;
            }
            for (int i = 4; i < 8; i++)
            {
                mobsInGame[i].purchased = false;
                towersInGame[i].purchased = false;
            }
        }
        Debug.Log(points);
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
                _buttons[i].transform.GetChild(0).GetComponent<Text>().text = towersInGame[i].tower.title;
                _buttons[i].transform.GetChild(1).GetComponent<Text>().text = towersInGame[i].tower.description;
                _buttons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Power: " + towersInGame[i].tower.attackPower.ToString() +
                    "<br>Fire Rate: " + towersInGame[i].tower.attackRate.ToString() +
                    "<br>Range: " + towersInGame[i].tower.attackRange.ToString() +
                    "<br>Damage Radius: " + towersInGame[i].tower.attackRadius.ToString();
                if (towersInGame[i].purchased == true) _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Purchased";
                else _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Cost: " + towersInGame[i].tower.pointWorth + " points";
                //Change BG image for button
                _buttons[i].GetComponent<Image>().sprite = towersInGame[i].tower.towerImage;
                //If we can afford card or it has been purchased make it selectable else we can't choose it
                if (towersInGame[i].tower.pointWorth <= points || towersInGame[i].purchased) _buttons[i].GetComponent<Button>().interactable = true;
                else _buttons[i].GetComponent<Button>().interactable = false;
                //Now check if we have it in hand already and make it unselectable if it is and modify text
                foreach (TowerCard tower in towersInHand)
                {
                    if (tower == towersInGame[i].tower)
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
                _buttons[i].transform.GetChild(0).GetComponent<Text>().text = mobsInGame[i].mob.title;
                _buttons[i].transform.GetChild(1).GetComponent<Text>().text = mobsInGame[i].mob.description;
                _buttons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    "Move Speed: " + mobsInGame[i].mob.moveSpd.ToString() +
                    "<br>Max Health: " + mobsInGame[i].mob.healthMax.ToString() +
                    "<br>Power: " + mobsInGame[i].mob.attackPower.ToString() +
                    "<br>Hit Rate: " + mobsInGame[i].mob.attackRate.ToString();
                if (mobsInGame[i].purchased == true) _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Purchased";
                else _buttons[i].transform.GetChild(3).GetComponent<Text>().text = "Cost: " + mobsInGame[i].mob.pointWorth + " points";
                //_buttons[i].GetComponent<Image>().sprite = _mobsInGame[i].mob.mobImage;
                //If we can afford card or it has been purchased make it selectable else we can't choose it
                if (mobsInGame[i].mob.pointWorth <= points || mobsInGame[i].purchased) _buttons[i].GetComponent<Button>().interactable = true;
                else _buttons[i].GetComponent<Button>().interactable = false;
                //Now check if we have it in hand already and make it unselectable if it is
                foreach (MobCard mob in mobsInHand)
                {
                    if (mob == mobsInGame[i].mob)
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
                _currentHandButtons[i].GetComponent<Image>().sprite = towersInHand[i].towerImage;
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
        _tempTowerSelect = towersInGame[towerIndex].tower;
        //Take away points for card purchased and change its purchased status
        if (!towersInGame[towerIndex].purchased) 
        { 
            points -= (ushort)towersInGame[towerIndex].tower.pointWorth;
            towersInGame[towerIndex].purchased = true;
        }
        //We are swapping a tower
        _swappingTower = true;
    }
    public void HoldMob(int mobIndex)
    {
        //Make temp holder equal object selected
        _tempMobSelect = mobsInGame[mobIndex].mob;
        //Take away points for card purchased and change its purchased status
        if (!mobsInGame[mobIndex].purchased)
        {
            points -= (ushort)mobsInGame[mobIndex].mob.pointWorth;
            mobsInGame[mobIndex].purchased = true;
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
        //Save data from game to save file
        BinarySave.SaveGameData(ref mobsInGame, ref towersInGame, mobsInHand, towersInHand, points);
        //If unity editor exit play mode else quit application
        #if UNITY_EDITOR
UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    public void ChangeScene(int sceneIndex)
    {
        //Change to scene matching index given
        SceneManager.LoadScene(sceneIndex);
    }
    #endregion
}
