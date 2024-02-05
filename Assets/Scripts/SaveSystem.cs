using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    /// <summary>
    /// Saves the current data into dat file
    /// </summary>
    public static void SaveGame(GameDataSingleton gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gamedata.dat";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(gameData);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// If a dat file exists, loads the data from dat file and returns it. Otherwise, creates new dat file then loads that file.
    /// </summary>
    public static PlayerData LoadGame()
    {       
        try
        {
            string path = Application.persistentDataPath + "/gamedata.dat";

            if (!File.Exists(path))
            {
                SaveGame(GameDataSingleton.Instance);
            }

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        catch
        {
            // If you catch error, print an error message
            Debug.Log("An error occured while loading the game!");

            return new PlayerData(GameDataSingleton.Instance);
        }
        
    }
}
