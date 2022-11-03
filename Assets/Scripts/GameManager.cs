using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Variables

    [Tooltip("This number has two values: 0 or 1.\n0 is \"Lost\", 1 is \"Won\"")]
    // This number has two values: 0 or 1. 
    // 0 is "Lost", 1 is "Won"
    [SerializeField] private ushort _gameResult = 0;
    [Tooltip("This value is set at runtime in the Build phase by the server. " +
        "\nIt represents how much time (in seconds) the player has left before the phase changes automatically.")]
    [SerializeField] private float _timer = 2*60f;
    private enum GameState
    {
        PreGame,
        Build,
        PostGame
    };
    private GameState _currentState = GameState.PreGame;
    [SerializeField] private ParticleSystem _fogOfWarParticleSystem;

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
    private ScriptableObject[,] _deck = new ScriptableObject[2,8];
    #endregion

    #region Properties
    /// <summary>
    /// Public property for the fog of war Particle System that overlays the opposing side of the battlefield.
    /// </summary>
    public ParticleSystem FogOfWarParticleSystem 
    { 
        get{ return _fogOfWarParticleSystem; }
        set{ _fogOfWarParticleSystem = value; } 
    }
    /// <summary>
    /// Public property for the array of both Card ScriptableObject types (Towers and Mobs.)
    /// </summary>
    public ScriptableObject[,] Deck { get { return _deck; } set { _deck = value; } } 
    #endregion


    void Awake()
    {
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
                            (ScriptableObject)Resources.Load("/Cards/Towers/Tower" + cardIndex);
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
                            (ScriptableObject)Resources.Load("/Cards/Mobs/Mob" + cardIndex);
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
                    Debug.Log("We are in PreGame state.");
                    break; 
                }
            case GameState.Build:
                {
                    break;
                }
            case GameState.PostGame:
                {
                    break;
                }
            default:
                {
                    Debug.LogError("Something went wrong with the GameState in Update().");
                    break;
                }
        }
    }

    private void ChangeGameState()
    {

    }

    private void OnValidate()
    {
        Mathf.Clamp(_gameResult, 0, 1);
    }
}