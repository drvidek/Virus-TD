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

public class RoundManager : MonoBehaviour
{
    #region Private Variables

    [SerializeField] private bool _gameResult;

    private static GameState _currentState = GameState.PreGame;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private FogOfWar[] _fogOfWar;

    //[Tooltip("DO NOT CHANGE THE NUMBER OF ELEMENTS IN THIS ARRAY VIA UNITY EDITOR, LOGIC LOOPS DEPEND ON IT.")]
    // Array that holds all the Tower type ScriptableObject cards
    //[SerializeField] private ScriptableObject[] _towerCards = new ScriptableObject[8];
    //[Tooltip("DO NOT CHANGE THE NUMBER OF ELEMENTS IN THIS ARRAY VIA UNITY EDITOR, LOGIC LOOPS DEPEND DEPENDS ON IT.")]
    // Array that holds all the Mob type ScriptableObject cards
    //[SerializeField] private ScriptableObject[] _mobCards = new ScriptableObject[8];
    // Array for the Deck of cards that are both mobs and towers scriptable objects.
    // The array is set to have 8 rows and 2 columns,
    // each COLUMN represents whether it is a Mob or Tower card
    // each ROW represents the individual scriptable object (card) of that type of card
    //private ScriptableObject[,] _deck = new ScriptableObject[2, 8];

    private static ushort[,] _scoreTable = new ushort[2, 3];
    public static ushort[,] ScoreTable { get => _scoreTable; }
    private static ushort _turnCurrent;

    #endregion

    #region Properties
    public static GameState CurrentState { get => _currentState; }
    #endregion

    private void Start()
    {
        _currentState = GameState.PreGame;
        NextState();
    }

    void Update()
    {
        Debug.Log($"We are in {_currentState} state on turn {_turnCurrent} and ready status is {PlayerManager.PlayerManagerInstance.Ready}");
    }

    private void NextState()
    {
        //Debug.Log("Triggered NextState");

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
        //Debug.Log($"Start {_currentState} state");
        _uiManager.SetEndPanelOnStart();
        _fogOfWar[0].SetTargetDissolve(0);
        _fogOfWar[1].SetTargetDissolve(0);
        while (_currentState == GameState.PreGame)
        {
            yield return null;
        }
        PlayerManager.PlayerManagerInstance.ResetReadyStatus();
        NextState();
    }

    IEnumerator StateBuild()
    {
        //Debug.Log($"Start {_currentState} state");
        _uiManager.readyButton.interactable = true;
        _fogOfWar[0].SetTargetDissolve(NetworkManager.GetPlayerIDNormalised() == 0 ? 1 : 0);
        _fogOfWar[1].SetTargetDissolve(NetworkManager.GetPlayerIDNormalised() == 1 ? 1 : 0);
        while (_currentState == GameState.Build)
        {
            PlayerManager.PlayerManagerInstance.SendPlayerPointsMessage();
            yield return null;
        }
        PlayerManager.PlayerManagerInstance.ResetReadyStatus();
        NextState();
    }

    IEnumerator StatePlay()
    {
        //Debug.Log($"Start {_currentState} state");
        //Deactivate ready splash screen when game starts and make ready button non-interactable
        PlayerManager.PlayerManagerInstance.readyPanel.SetActive(false);
        _uiManager.readyButton.interactable = false;
        _fogOfWar[0].SetTargetDissolve(1);
        _fogOfWar[1].SetTargetDissolve(1);
        while (_currentState == GameState.Play)
        {
            if (Mob._mobCounter == 0 && !PlayerManager.PlayerManagerInstance.Ready)
            {
                PlayerManager.PlayerManagerInstance.EndPlayPhase();
            }
            PlayerManager.PlayerManagerInstance.SendPlayerPointsMessage();
            yield return null;
        }
        PlayerManager.PlayerManagerInstance.ResetReadyStatus();
        NextState();
    }

    IEnumerator StatePostGame()
    {
        //Debug.Log($"Start {_currentState} state");
        EndGame();
        while (_currentState == GameState.PostGame)
        {
            yield return null;
        }
        NextState();
    }

    private static void ChangeGameState(GameState newState)
    {
        _currentState = newState;
    }

    public void ReturnToMenu()
    {
        NetworkManager.NetworkManagerInstance.Disconnect();
        MenuHandler.ChangeScene(0);
    }

    private void EndGame()
    {
        int[] finalScore = new int[2] { _scoreTable[0, 0], _scoreTable[1, 0] };
        ushort myPlayerId = NetworkManager.GetPlayerIDNormalised();
        ushort otherPlayerId = myPlayerId == 0 ? (ushort)1 : (ushort)0;
        _gameResult = (finalScore[myPlayerId] > finalScore[otherPlayerId]);
        //Assign points to MenuHandler version because player manager version was throwing error in save call
        MenuHandler.pointsInBank += PlayerManager.PlayerManagerInstance.Points;
        //Save the game to store current information, most importantly updated points
        BinarySave.SaveGameData(ref MenuHandler.mobsInGame, ref MenuHandler.towersInGame, MenuHandler.mobsInHand, MenuHandler.towersInHand, ref MenuHandler.pointsInBank);
        _uiManager.SetEndPanelOnEnd(_gameResult);
    }

    [MessageHandler((ushort)ServerToClientID.stateChange)]
    private static void GetGameStateMessage(Message message)
    {
        ushort stateID = message.GetUShort();
        ushort turn = message.GetUShort();
        GameState newState = (GameState)stateID;
        _turnCurrent = turn;
        ChangeGameState(newState);
    }

    [MessageHandler((ushort)ServerToClientID.points)]
    private static void ReceivePointsMessage(Message message)
    {
        for (var i = 0; i < 2; i++)
        {
            for (var ii = 0; ii < 3; ii++)
            {
                RoundManager._scoreTable[i, ii] = message.GetUShort();
            }
        }
    }

    [MessageHandler((ushort)ServerToClientID.mobCount)]
    private static void SetMobCount(Message message)
    {
        Mob._mobCounter = message.GetUShort();
    }
}