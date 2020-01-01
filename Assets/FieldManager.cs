using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldManager : MonoBehaviour
{
    private GameObject[,] buttons;
    
    public void InitButtons()
    {
        buttons = new GameObject[9,9];
        // get all the buttons and write them into a two dimensional array
        for(int i = 0; i<9; i++)
        {
            for(int j = 0; j<9; j++)
            {
                buttons[i, j] = transform.GetChild(i * 9 + j).gameObject;
            }
        }
        SetListenersForTiles();
    }


    // writes values given in an int array to the field of buttons
    public void WriteValues(int[,] values) { 
        if(values == null || buttons == null || values.GetLength(0) != buttons.GetLength(0) || values.GetLength(1) != buttons.GetLength(1))
        {
            Debug.LogError("Dimensions error!");
            return;
        }
        for(int i = 0; i<buttons.GetLength(0); i++)
        {
            for(int j = 0; j<buttons.GetLength(1); j++)
            {
                if (values[i, j] == 0)
                {
                    buttons[i, j].transform.GetChild(0).GetComponent<Text>().text = "";
                }
                else
                {
                    buttons[i, j].transform.GetChild(0).GetComponent<Text>().text = values[i, j].ToString();
                }
            }
        }
    }

    public void SetListenersForTiles() { 
        foreach(GameObject go in buttons)
        {
            go.GetComponent<Button>().onClick.AddListener(() => AdoptThisPatternOnMainField());
        }
    }

    public void AdoptThisPatternOnMainField() {
        GameObject.Find("FieldSpawner").GetComponent<FieldSpawner>().PrintSolution(GetValues(), true);
        Camera.main.GetComponent<Animator>().Play("CameraUpHistory");
    }

    public int[,] GetValues() {
        int[,] values = new int[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (buttons[i, j].transform.GetChild(0).GetComponent<Text>().text.Equals(""))
                {
                    values[i, j] = 0;
                }
                else {
                    values[i, j] = int.Parse(buttons[i, j].transform.GetChild(0).GetComponent<Text>().text);
                }
            }
        }
        return values;
    }
}
