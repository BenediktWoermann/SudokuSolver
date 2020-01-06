using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Linq;

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

    public static void SavePattern(int[,] values) {
        DeleteDuplicants(values);
        if (values.GetLength(0) != 9 || values.GetLength(1) != 9)
        {
            Debug.LogError("Dimension Error!");
            return;
        }

        // copy value field to simple array in order to save it
        int[] toSave = new int[81];
        for(int i = 0; i<9; i++)
        {
            for(int j = 0; j<9; j++)
            {
                toSave[i * 9 + j] = values[i, j]; 
            }
        }

        // amount array has the information, how many fields are already stored
        int[] amount = PlayerPrefsX.GetIntArray("amount");
        int id = 0;
        if (amount.Length == 1) id = amount[0];

        // if there are already 6 elements saved, delete the oldest 
        if(id >= 6)
        {
            for(int i = 0; i<5; i++)
            {
                PlayerPrefsX.SetIntArray("Pattern" + i, PlayerPrefsX.GetIntArray("Pattern" + (i + 1).ToString()));
            }
            id = 5;
        }
        string name = "Pattern" + id.ToString();
        PlayerPrefsX.SetIntArray(name, toSave);
        amount = new int[1];
        amount[0] = id + 1;
        PlayerPrefsX.SetIntArray("amount", amount);
    }

    public static void DeleteDuplicants(int[,] values) { 
        // search for index of duplicant
        int index = -1;
        for(int i = 0; i<Stats.history.Length; i++)
        {
            if (values.Cast<int>().SequenceEqual(Stats.history[i].Cast<int>())) index = i;
        }

        // overwrite duplicant and shift all following patterns to a smaller index
        if (index == -1) return;
        for(int i = index; i<Stats.history.Length; i++)
        {
            PlayerPrefsX.SetIntArray("Pattern" + i.ToString(), PlayerPrefsX.GetIntArray("Pattern" + (i + 1).ToString()));
        }

        // Adjust "amount" to show the correct amount of saved patterns. Last two patterns are equal, but the last one doesnt get overwritten.
        int[] amount = PlayerPrefsX.GetIntArray("amount");
        if (amount.Length == 0)
        {
            Debug.LogError("Problem with dimensions of amount array!");
            return;
        }
        amount[0]--;
        PlayerPrefsX.SetIntArray("amount", amount);
    }

    public static void LoadHistory() {
        int[] amountArray = PlayerPrefsX.GetIntArray("amount");
        if(amountArray.Length != 1 && amountArray.Length != 0)
        {
            Debug.LogError("Error with dimensions of amount array! Required length: 1 or 0  Found length: "+amountArray.Length);
            return;
        }
        int amount;
        if (amountArray.Length == 1)
        {
            amount = amountArray[0];
        }
        else
        {
            amount = 0;
        }
        Stats.history = new int[amount][,];
        for(int i = 0; i<amount; i++)
        {
            Stats.history[i] = new int[9, 9];
            string name = "Pattern" + i.ToString();
            int[] loadedArray = PlayerPrefsX.GetIntArray(name);
            for(int j = 0; j<loadedArray.Length; j++)
            {
                Stats.history[i][j / 9, j % 9] = loadedArray[j];
            }
        }
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
    public int speed;

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
        speed = Stats.speed;
        //Debug.Log(Stats.backgroundColor.r);
    }
}
