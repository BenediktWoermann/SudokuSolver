using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldSpawner : MonoBehaviour
{
    public float yOffset;
    public static int fieldWidth, fieldHeight;
    private float height, width;
    public Button tilePrefab;
    public static Button[,] field;

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultStats();
        Spawn();
    }

    private void SetDefaultStats() {
        fieldWidth = 9;
        fieldHeight = 9;
        field = new Button[fieldWidth, fieldHeight];

        height = Camera.main.orthographicSize * 2f;
        width = height / Screen.height * Screen.width;
    }

    public void Spawn() { 
        // (0,0) is in the lower left corner
        for(int i = 0; i<fieldWidth; i++) { 
            for(int j = 0; j<fieldHeight; j++) {
                Vector3 pos = new Vector3((i-fieldWidth/2)*width/fieldWidth, yOffset + height/2f + (j-fieldWidth)*width/fieldWidth, 0);
                field[i, j] = Instantiate(tilePrefab, pos, Quaternion.identity, GameObject.Find("Canvas").transform);
            }
        }
    }
}
