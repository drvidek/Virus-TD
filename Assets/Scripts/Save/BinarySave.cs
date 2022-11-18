using UnityEngine; //Required for Unity connection
using System.IO; //Use FileStream to read from and save to files
using System.Runtime.Serialization.Formatters.Binary; //Allows use of the BinaryFormatter

public class BinarySave
{
    //Static reference to the save file
    public static string path = Path.Combine(Application.streamingAssetsPath, "SaveFile.bin");

    //Save function requires Deck information, whether they are purchased, current hand information and points balance
    public static void SaveGameData(ref MenuHandler.MobsInGame[] mobs, ref MenuHandler.TowersInGame[] towers, MobCard[] mobsInHand, TowerCard[] towersInHand, ref ushort score)
    {
        Debug.Log(score);
        //Retrieve SaveData information so we can save it to file
        SaveData data = new SaveData(ref mobs, ref towers, mobsInHand, towersInHand, ref score);
        //Create a new BinaryFormatter so we can convert SaveData information
        BinaryFormatter formatter = new BinaryFormatter();
        //Open a new FileStream to stream the data to the file
        FileStream stream = new FileStream(path, FileMode.Create);
        //Use formatter to convert stream to binary
        formatter.Serialize(stream, data);
        //Close FileStream
        stream.Close();
    }
    //Load function  requires Player transforms, states and a save index to be fed to it when calling so it will change the values. Interacts with SaveData file.
    public static SaveData LoadPlayerData(ref MenuHandler.MobsInGame[] mobs, ref MenuHandler.TowersInGame[] towers, MobCard[] mobsInHand, TowerCard[] towersInHand, ref ushort score)
    {
        //If the file exists at that path
        if (File.Exists(path))
        {
            //Create a new BinaryFormatter so we can convert back from binary
            BinaryFormatter formatter = new BinaryFormatter();
            //OOpen a new FileStream to read from file
            FileStream stream = new FileStream(path, FileMode.Open);
            //New SaveData reference to store our deserialized data into for loading
            SaveData data = (SaveData)formatter.Deserialize(stream);
            //Close the stream
            stream.Close();
            //Call the LoadPlayerData function in SaveData to store correct values in the arrays and variables so it can be passed to MenuHandler
            data.LoadPlayerData(ref mobs, ref towers, mobsInHand, towersInHand, ref score);
            Debug.Log(score);
            //Return the data
            return data;            
        }
        //If the specified file doesn't exist return nothing
        else
        {
            return null;
        }
    }
}
