using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveSystem 
{
    private static string path= Application.dataPath + "/player.fun";
    public static void SavePlayer(string Name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        path = Application.dataPath + "/player.fun";
        FileStream stream = new FileStream(path,  FileMode.Create);
        string nameToSave = Name;
        formatter.Serialize(stream, nameToSave);
        stream.Close();
    }

    public static string LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string nameToLoad = (string)formatter.Deserialize(stream);
            stream.Close();
            return nameToLoad;
        }
        else
        {        
            return "";
        }

    }

    public static void DeletePlayer()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
