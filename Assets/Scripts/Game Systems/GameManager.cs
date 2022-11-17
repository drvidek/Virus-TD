using System;
using System.Collections;
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

    [SerializeField] private bool _gameResult;
    [Tooltip("This value is set at runtime in the Build phase by the server. " +
        "\nIt represents how much time (in seconds) the player has left before the phase changes automatically.")]
    [SerializeField] public float timer = 2 * 60f;

    private static GameState _currentState = GameState.PreGame;

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

    private static ushort[,] _scoreTable = new ushort[2, 3];
    private ushort _turnCurrent, _turnMax = 10;

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

    private void Start()
    {
        _currentState = GameState.PreGame;
        NextState();
    }

    void Update()
    {
        //Debug.Log($"We are in {_currentState} state.");
    }

    private void NextState()
    {
        Debug.Log("Triggered NextState");

        switch (_currentState)
        {
            case GameState.PreGame:
                StartCoroutine("StatePreGame");
                break;
            case GameState.Build:
                StartCoroutine("StateBuild");
                break;
            case GameState.Play:
                StartCoroutine("StatePlay");
                break;
            case GameState.PostGame:
                StartCoroutine("StatePostGame");
                break;
            default:
                break;
        }
    }

    IEnumerator StatePreGame()
    {
        Debug.Log($"Start {_currentState} state");
        UIManager.UIManagerInstance.SetEndPanelOnStart();
        _fogOfWar[0].SetActive(true);
        _fogOfWar[1].SetActive(true);
        while (_currentState == GameState.PreGame)
        {
            yield return null;
        }
        NextState();
    }

    IEnumerator StateBuild()
    {
        Debug.Log($"Start {_currentState} state");
        UIManager.UIManagerInstance.readyButton.interactable = true;
        _fogOfWar[0].SetActive(NetworkManager.GetPlayerIDNormalised() != 0);
        _fogOfWar[1].SetActive(NetworkManager.GetPlayerIDNormalised() != 1);
        while (_currentState == GameState.Build)
        {
            PlayerManager.PlayerManagerInstance.SendPlayerPointsMessage();
            timer = Mathf.MoveTowards(timer, 0, Time.deltaTime);
            yield return null;
        }
        NextState();
    }

    IEnumerator StatePlay()
    {
        Debug.Log($"Start {_currentState} state");
        //Deactivate ready splash screen when game starts and make ready button non-interactable
        PlayerManager.PlayerManagerInstance.readyPanel.SetActive(false);
        UIManager.UIManagerInstance.readyButton.interactable = false;
        _fogOfWar[0].SetActive(false);
        _fogOfWar[1].SetActive(false);
        while (_currentState == GameState.Play)
        {
            PlayerManager.PlayerManagerInstance.SendPlayerPointsMessage();
            if (Mob._mobCounter == 0)
            {
                PlayerManager.PlayerManagerInstance.EndPlayPhase();
            }
            yield return null;
        }
        NextState();
    }

    IEnumerator StatePostGame()
    {
        Debug.Log($"Start {_currentState} state");
        EndGame();
        while (_currentState == GameState.PostGame)
        {
            yield return null;
        }
        NextState();
    }


    private void ChangeGameState(GameState newState)
    {
        _currentState = newState;
        PlayerManager.PlayerManagerInstance.ResetReadyStatus();
    }

    private void EndGame()
    {
        int[] finalScore = new int[2] { _scoreTable[0, 0], _scoreTable[1, 0] };
        ushort myPlayerId = NetworkManager.GetPlayerIDNormalised();
        ushort otherPlayerId = myPlayerId == 0 ? (ushort)1 : (ushort)0;
        _gameResult = (finalScore[myPlayerId] > finalScore[otherPlayerId]);
        UIManager.UIManagerInstance.SetEndPanelOnEnd(_gameResult);
    }


    private void UpdateScoreDisplay()
    {
        //_uiManager.UpdateScoreDisplay(scoreTable);
    }


    [MessageHandler((ushort)ServerToClientID.stateChange)]
    private static void GetGameStateMessage(Message message)
    {
        ushort stateID = message.GetUShort();
        ushort turn = message.GetUShort();
        GameState newState = (GameState)stateID;
        GameManager.GameManagerInstance._turnCurrent = turn;
        GameManager.GameManagerInstance.ChangeGameState(newState);
    }

    [MessageHandler((ushort)ServerToClientID.points)]
    private static void ReceivePointsMessage(Message message)
    {
        for (var i = 0; i < 2; i++)
        {
            for (var ii = 0; ii < 3; ii++)
            {
                GameManager._scoreTable[i, ii] = message.GetUShort();
            }
        }
        GameManager.GameManagerInstance.UpdateScoreDisplay();
    }

    [MessageHandler((ushort)ServerToClientID.mobCount)]
    private static void SetMobCount(Message message)
    {
        Mob._mobCounter = message.GetUShort();
    }
}