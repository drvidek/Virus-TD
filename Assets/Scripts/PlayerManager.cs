using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Private Variables
    #endregion

    #region Properties
    /// <summary>
    /// Public property representing the player's point total.
    /// </summary>
    public float Points { get; private set; } = 0;
    /// <summary>
    /// Public property to access and set the tower cards present in the 'hand' for the current game.
    /// </summary>
    public ScriptableObject[] TowerCardsArr { get; private set; }
    /// <summary>
    /// Public property to access and set the mob cards present in the 'hand' for the current game.
    /// </summary>
    public ScriptableObject[] MobCardsArr { get; private set; }
    /// <summary>
    /// Public property to access and set the amount of resources in the current game.
    /// <para>Each array element represents a different resource type. For example:</para>
    /// <para><example><i>Index 0: Wood</i></example></para>
    /// <para><example><i>Index 1: Gold</i></example></para>
    /// </summary>
    public int[] ResourceCount { get; private set; } = new int[2] {0,0};
    #endregion
}
