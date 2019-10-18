using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldSpawner : MonoBehaviour
{
    public Color stdColor;
    public float yOffset, yOffsetMark;
    public static int fieldWidth, fieldHeight, markWidth, markHeight;
    private float height, width;
    public Button tilePrefab;
    public Button markPrefab;
    public GameObject vertLine, horiLine;
    public static Button[,] field;
    public static Button[] marker;

    private int activeMarker;

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultStats();
        SpawnTiles();
        SpawnLines();
        SpawnMarks();
    }

    private void SetDefaultStats() {
        fieldWidth = 9;
        fieldHeight = 9;
        markWidth = 5;
        markHeight = 2;
        field = new Button[fieldWidth, fieldHeight];
        marker = new Button[markWidth * markHeight];
        activeMarker = 0;

        height = Camera.main.orthographicSize * 2f;
        width = height / Screen.height * Screen.width;
    }

    private void SpawnTiles() {
        // (0,0) is in the lower left corner
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                Vector3 pos = new Vector3((i - fieldWidth / 2) * width / fieldWidth, height / yOffset - (j - fieldWidth) * width / fieldWidth, 0);
                field[i, j] = Instantiate(tilePrefab, pos, Quaternion.identity, GameObject.Find("Canvas").transform);
                field[i, j].GetComponentInChildren<Text>().text = "";
                Vector2Int coordinates = new Vector2Int(i, j);
                field[i, j].onClick.AddListener(() => markField(coordinates));
            }
        }
    }

    private void SpawnLines() {
        Transform staticCanvas = GameObject.Find("StaticCanvas").transform;

        // vertical lines
        Vector3 pos = new Vector3(-width / 6, height / yOffset - (4 - fieldWidth) * width / fieldWidth);
        Instantiate(vertLine, pos, Quaternion.identity, staticCanvas);
        pos.x = width / 6;
        Instantiate(vertLine, pos, Quaternion.identity, staticCanvas);

        // horizontal lines
        pos.x = 0;
        pos.y = height / yOffset - (2.5f - fieldWidth) * width / fieldWidth;
        Instantiate(horiLine, pos, Quaternion.identity, staticCanvas);
        pos.y = height / yOffset - (5.5f - fieldWidth) * width / fieldWidth;
        Instantiate(horiLine, pos, Quaternion.identity, staticCanvas);
    }

    private void SpawnMarks() {
        // (0,0) is in the lower left corner
        for (int i = 0; i < markWidth; i++)
        {
            for (int j = 0; j < markHeight; j++)
            {
                int idx = i + j * markWidth;
                Vector3 pos = new Vector3((i - markWidth / 2) * width / markWidth, height / yOffset + yOffsetMark * height/width - j * width / markWidth, 0);
                marker[idx] = Instantiate(markPrefab, pos, Quaternion.identity, GameObject.Find("Canvas").transform);
                marker[idx].GetComponentInChildren<Text>().text = idx>0 ? idx.ToString() : "X";
                marker[idx].onClick.AddListener(() => ChangeMarker(idx));
            }
        }
    }

    public void Solve() {
        int[,] numbers = new int[9, 9];
        for(int i = 0; i<9; i++)
        {
            for(int j = 0; j<9; j++)
            {
                if (field[i, j].GetComponentInChildren<Text>().text.Equals("")) {
                    numbers[i, j] = 0;
                }
                else {
                    numbers[i, j] = int.Parse(field[i, j].GetComponentInChildren<Text>().text);
                }
            }
        }
        PrintSolution(Solver.Solve(numbers));
    }

    public void PrintSolution(int[,] solution) { 
        for(int i  = 0; i<9; i++)
        {
            for(int j = 0; j<9; j++)
            {
                field[i, j].GetComponentInChildren<Text>().text = solution[i, j].ToString();
            }
        }
    }

    public void ChangeMarker(int newMarker) {
        activeMarker = newMarker;
        for(int i = 0; i<marker.Length; i++)
        {
            marker[i].GetComponent<Image>().color = Color.red;
            marker[i].GetComponent<Image>().color = stdColor;
        }
        marker[newMarker].GetComponent<Image>().color = Color.cyan;
    }

    public void markField(Vector2Int coordinates) {
        Button toMark = field[coordinates.x, coordinates.y];
        toMark.GetComponentInChildren<Text>().text = activeMarker > 0 ? activeMarker.ToString() : "";
    }
}
