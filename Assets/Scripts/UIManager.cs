using System.Collections.Generic; //Allow the use of Lists
using UnityEngine; //Connect to Unity Engine
using UnityEngine.UI; //Allow access and modification to Unitys Canvas UI elements

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("Mob & Tower Types")]
    [Tooltip("Enter the list of names for available Mobs in the game.")]
    public List<string> mobTypes = new List<string>();
    [Tooltip("Enter the list of names for the available Towers in the game.")]
    public List<string> towerTypes = new List<string>();
    [Header("Players Current Hand")]
    [Tooltip("This array stores the list of Tower and Mob names the player is currently holding. Set size to number of cards player should have available to play the round with.")]
    public string[] currentHand = new string[8];
    [Header("UI Elements")]
    [Tooltip("Add the four Select buttons so their appearance can be change based on what type of tower/mob building spot was selected")]
    [SerializeField] private Button[] _buttons = new Button[4];
    #endregion
    #region Startup
    private void Start()
    {
        
    }
    #endregion
    #region Functions
    public void UpdateDisplay()
    {

    }
    public void UpdateHand()
    {

    }
    #endregion
}
