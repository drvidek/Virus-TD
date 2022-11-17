using UnityEngine; //Require for Unity Connection
using System;
//Allow for class to be serialized for 
[Serializable]
public class SaveData
{
    #region Variables
    //int arrays to store card information and purchase status
    public int[] purchasedMobBool;
    public int[] handMobIndex;
    public int[] handTowerIndex;
    public int[] purchasedTowerBool;
    //ushort to store the score
    public ushort playerScore;
    #endregion
    #region Save
    public SaveData(ref MenuHandler.MobsInGame[] mobs, ref MenuHandler.TowersInGame[] towers, MobCard[] mobsInHand, TowerCard[] towersInHand, ushort score)
    {
        //Initialize arrays
        purchasedMobBool = new int[mobs.Length];
        purchasedTowerBool = new int[towers.Length];
        handMobIndex = new int[mobsInHand.Length];
        handTowerIndex = new int[towersInHand.Length];
        //store ints to iterate through hand indexes when we find matches
        int mobIndex = 0;
        int towerIndex = 0;
        //For each card in game add the bool converted to int to the array, and if card exists in hand add index to hand array
        for (int i = 0; i < 8; i++) 
        {
            purchasedMobBool[i] = Convert.ToInt32(mobs[i].purchased);
            foreach (MobCard mob in mobsInHand)
            {
                if (mob == mobs[i].mob)
                {
                    handMobIndex[mobIndex] = i;
                    mobIndex++;
                }
            }
            purchasedTowerBool[i] = Convert.ToInt32(towers[i].purchased);
            foreach (TowerCard tower in towersInHand)
            {
                if (tower == towers[i].tower)
                {
                    handTowerIndex[towerIndex] = i;
                    towerIndex++;
                }
            }
        }
        //Store the points in the proper index
        playerScore = score;
    }
    #endregion
    #region Load
    public void LoadPlayerData(ref MenuHandler.MobsInGame[] mobs, ref MenuHandler.TowersInGame[] towers, MobCard[] mobsInHand, TowerCard[] towersInHand, ushort score)
    {
        //Convert int back to boolean and store it in arrays
        for (int i = 0; i < 8; i++)
        {
            mobs[i].purchased = Convert.ToBoolean(purchasedMobBool[i]);
            towers[i].purchased = Convert.ToBoolean(purchasedTowerBool[i]);
        } 
        //Adjust cards in hand to saved values
        for (int i = 0; i < 4; i++)
        {
            mobsInHand[i] = mobs[handMobIndex[i]].mob;
            towersInHand[i] = towers[handTowerIndex[i]].tower;
        }
        score = playerScore;
    }
    #endregion
}
