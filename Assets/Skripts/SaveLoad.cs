using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Save_Load
{
    public static void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/color.txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        DataType data = new DataType();
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static DataType LoadData()
    {
        string path = Application.persistentDataPath + "/color.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            DataType data = (DataType)formatter.Deserialize(stream);
            //return (DataType)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("File " + path + " not found!");
            return null;
        }
    }
}

[System.Serializable]
public class DataType
{
    public byte aMainColor, rMainColor, gMainColor, bMainColor, aMarkerColor, rMarkerColor, gMarkerColor, bMarkerColor, backgroundGreyValue;

    public DataType() {
        aMainColor = Stats.hMainColor.a;
        rMainColor = Stats.hMainColor.r;
        gMainColor = Stats.hMainColor.g;
        bMainColor = Stats.hMainColor.b;
        aMarkerColor = Stats.hMarkerColor.a;
        rMarkerColor = Stats.hMarkerColor.r;
        gMarkerColor = Stats.hMarkerColor.g;
        bMarkerColor = Stats.hMarkerColor.b;
        backgroundGreyValue = Stats.backgroundColor.r;
        //Debug.Log(Stats.backgroundColor.r);
    }
}

[System.Serializable]
public class History {
    public int[][,] fieldlist;
    public int[][,] inputlist;
} 
