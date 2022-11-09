using System;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public enum GameState
{
    PreGame,
    Build,
    Play,
    PostGame
}

public class GameManager : MonoBehaviour
{
    #region Private Variables

    [Tooltip("This number has two values: 0 or 1.\n0 is \"Lost\", 1 is \"Won\"")]
    // This number has two values: 0 or 1. 
    // 0 is "Lost", 1 is "Won"
    [SerializeField] private ushort _gameResult = 0;
    [Tooltip("This value is set at runtime in the Build phase by the server. " +
        "\nIt represents how much time (in seconds) the player has left before the phase changes automatically.")]
    [SerializeField] public float timer = 2 * 60f;

    private static GameState _currentState = GameState.PreGame;
    [SerializeField] private ParticleSystem _fogOfWarParticleSystem;

    [SerializeField] private GameObject[] _fogOfWar;

    [Tooltip("DO NOT CHANGE THE NUMBER OF ELEMENTS IN THIS ARRAY VIA UNITY EDITOR, LOGIC LOOPS DEPEND ON IT.")]
    // Array that holds all the Tower type ScriptableObject cards
    [SerializeField] private ScriptableObject[] _towerCards = new ScriptableObject[8];
    [Tooltip("DO NOT CHANGE THE NUMBER OF ELEMENTS IN THIS ARRAY VIA UNITY EDITOR, LOGIC LOOPS DEPEND DEPENDS ON IT.")]
    // Array that holds all the Mob type ScriptableObject cards
    [SerializeField] private ScriptableObject[] _mobCards = new ScriptableObject[8];
    // Array for the Deck of cards that are both mobs and towers scriptable objects.
    // The array is set to have 8 rows and 2 columns,
    // each COLUMN represents whether it is a Mob or Tower card
    // each ROW represents the individual scriptable object (card) of that type of card
    private ScriptableObject[,] _deck = new ScriptableObject[2, 8];
    #endregion

    #region Properties
    private static GameManager _gameManagerInstance;
    public static GameManager GameManagerInstance
    {
        //Property Read is the instance, public by default
        get => _gameManagerInstance;
        //private means only this instance of the class can access set
        private set
        {
            //set the instance to the value if the instance is null
            if (_gameManagerInstance == null)
            {
                _gameManagerInstance = value;
            }
            //if it is not null, check if the value is stored as the static instance
            else if (_gameManagerInstance != value)
            {
                //if not, throw a warning and destroy that instance

                //$ is to identify the string as containing an interpolated value
                Debug.LogWarning($"{nameof(GameManager)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    /// <summary>
    /// Public property for the fog of war Particle System that overlays the opposing side of the battlefield.
    /// </summary>
    public ParticleSystem FogOfWarParticleSystem
    {
        get { return _fogOfWarParticleSystem; }
        set { _fogOfWarParticleSystem = value; }
    }
    /// <summary>
    /// Public property for the array of both Card ScriptableObject types (Towers and Mobs.)
    /// </summary>
    public ScriptableObject[,] Deck { get { return _deck; } set { _deck = value; } }

    public static GameState CurrentState { get => _currentState; }
    #endregion


    void Awake()
    {
        GameManagerInstance = this;
        
        // loop through the 2D card ScriptableObject array COLUMNS to determine the card type (Mob or Tower).
        for (int cardTypeIndex = 0; cardTypeIndex < 2; ++cardTypeIndex)
        {
            // if the card type index is 0, which is the tower cards type index
            if (cardTypeIndex == 0)
            {
                // loop through the 2D card ScriptableObject array ROWS to determine the individual cards.
                for (int cardIndex = 0; cardIndex < 8; ++cardIndex)
                {
                    try
                    {
                        // Assign the current Card Object in the 2D array to the Tower Card ScriptableObject
                        // loaded from the Resources folder in the Unity directory.
                        _deck[cardTypeIndex, cardIndex] =
                            Resources.Load<ScriptableObject>("Cards/Towers/Tower" + cardIndex);
                        // Setting the element in the Tower Cards array to the same SO the Deck array was passed.
                        _towerCards[cardIndex] = _deck[cardTypeIndex, cardIndex];
                        /*
                        // assign the current Card Object in the 2D array
                        // to the Card Object at the same index in the tower cards array.
                        _deck[cardTypeIndex, cardIndex] = _towerCards[cardIndex];
                        */
                        Debug.Log($"Added card Scriptable Object with index: {cardIndex} to Tower element.");
                    }
                    catch (NullReferenceException nullRefE)
                    {
                        /*Debug.LogError(
                            $"There's a null reference in the SO card arrays when assigning the singlular arrays to the 2D array." +
                            $"\n{nullRefE}");*/
                        Debug.LogError(
                            $"There's a null reference in the SO card arrays when loading and assigning " +
                            $"the Tower ScriptableObject from the Resources folder to the 2D array." +
                            $"\n{nullRefE}");
                        throw;
                    }
                    catch (IndexOutOfRangeException indexRangeE)
                    {
                        /*Debug.LogError(
                            $"The index in one of the arrays in the SO card arrays when assigning the " +
                            $"singlular arrays to the 2D array is out of range. Maybe a logic error? " +
                            $"\n{indexRangeE}");*/
                        Debug.LogError(
                            $"The index in one of the arrays in the SO card arrays when loading and " +
                            $"assigning the Tower ScriptableObject from the Resources folder to the 2D " +
                            $"array is out of range. Maybe a logic error?" +
                            $"\n{indexRangeE}");
                        throw;
                    }
                    catch (Exception e)
                    {
                        /*Debug.LogError(
                            $"There is an uncaught exception in the Try/Catch block when assigning the singlular " +
                            $"arrays to the 2D array. \n{e}");*/
                        Debug.LogError(
                            $"There is an uncaught exception in the Try/Catch block when loading and " +
                            $"assigning the Tower ScriptableObject from the Resources folder to the 2D array. " +
                            $"\n{e}");
                        throw;
                    }

                }
            }
            // otherwise if the card type index is 0, which is the mob cards type index
            if (cardTypeIndex == 1)
            {
                // loop through the 2D card ScriptableObject array ROWS to determine the individual cards.
                for (int cardIndex = 0; cardIndex < 8; ++cardIndex)
                {
                    try
                    {
                        // Assign the current Card Object in the 2D array to the Mob Card ScriptableObject
                        // loaded from the Resources folder in the Unity directory.
                        _deck[cardTypeIndex, cardIndex] =
                            (ScriptableObject)Resources.Load("Cards/Mobs/Mob" + cardIndex);
                        // Setting the element in the Mob Cards array to the same SO the Deck array was passed.
                        _mobCards[cardIndex] = _deck[cardTypeIndex, cardIndex];
                        /* 
                        // Assign the current Card Object in the 2D array
                        // to the Card Object at the same index in the mob cards array
                        _deck[cardTypeIndex, cardIndex] = _mobCards[cardIndex];
                        */
                        Debug.Log($"Added card Scriptable Object with index: {cardIndex} to Mob element.");
                    }
                    catch (NullReferenceException nullRefE)
                    {
                        /*Debug.LogError(
                            $"There's a null reference in the SO card arrays when assigning the singlular arrays to the 2D array." +
                            $"\n{nullRefE}");*/
                        Debug.LogError(
                            $"There's a null reference in the SO card arrays when loading and assigning " +
                            $"the Mob ScriptableObject from the Resources folder to the 2D array." +
                            $"\n{nullRefE}");
                        throw;
                    }
                    catch (IndexOutOfRangeException indexRangeE)
                    {
                        /*Debug.LogError(
                            $"The index in one of the arrays in the SO card arrays when assigning the " +
                            $"singlular arrays to the 2D array is out of range. Maybe a logic error? " +
                            $"\n{indexRangeE}");*/
                        Debug.LogError(
                            $"The index in one of the arrays in the SO card arrays when loading and " +
                            $"assigning the Mob ScriptableObject from the Resources folder to the 2D " +
                            $"array is out of range. Maybe a logic error?" +
                            $"\n{indexRangeE}");
                        throw;
                    }
                    catch (Exception e)
                    {
                        /*Debug.LogError(
                            $"There is an uncaught exception in the Try/Catch block when assigning the singlular " +
                            $"arrays to the 2D array. \n{e}");*/
                        Debug.LogError(
                            $"There is an uncaught exception in the Try/Catch block when loading and " +
                            $"assigning the Mob ScriptableObject from the Resources folder to the 2D array. " +
                            $"\n{e}");
                        throw;
                    }

                }
            }
        }
        Debug.Log("Reached the end of Awake().");
    }

    void Update()
    {
        switch (_currentState)
        {
            case GameState.PreGame:
                {
                    Debug.Log($"We are in {_currentState} state.");
                    break;
                }
            case GameState.Build:
                {
                    _fogOfWar[0].SetActive(NetworkManager.GetPlayerIDNormalised() != 0);
                    _fogOfWar[1].SetActive(NetworkManager.GetPlayerIDNormalised() != 1);

                        Debug.Log($"We are in {_currentState} state.");
                    break;
                }
            case GameState.Play:
                _fogOfWar[0].SetActive(false);
                _fogOfWar[1].SetActive(false);
                Debug.Log($"We are in {_currentState} state.");
                break;
            case GameState.PostGame:
                {
                    Debug.Log($"We are in {_currentState} state.");
                    break;
                }
            default:
                {
                    Debug.LogError("Something went wrong with the GameState in Update().");
                    break;
                }
        }
    }

    private void ChangeGameState(GameState newState)
    {
        GameManager._currentState = newState;
        GetComponent<PlayerManager>().ResetReadyStatus();
    }

    private void OnValidate()
    {
        Mathf.Clamp(_gameResult, 0, 1);
    }

    [MessageHandler((ushort)ServerToClientID.stateChange)]
    private static void GetGameStateMessage(Message message)
    {
        ushort stateID = message.GetUShort();
        GameState newState = (GameState)stateID;
        GameManager.GameManagerInstance.ChangeGameState(newState);
    }

}