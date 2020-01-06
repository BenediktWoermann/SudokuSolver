using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldSpawner : MonoBehaviour
{
    private int frames;

    //field prefab for spawning history
    public GameObject fieldPrefab;
    private List<GameObject> historyFields;

    Solver solver;

    public bool solving;
    public float yOffset, yOffsetMark;
    public static int fieldWidth, fieldHeight, markWidth, markHeight;
    private float height, width;
    public Button tilePrefab;
    public Button markPrefab;
    public Button deleteBtn;
    public Button solveBtn;
    public GameObject vertLine, horiLine;
    public static Button[,] field;
    public static Button[] marker;

    public Slider mainColorSlider, markerColorSlider, backgroundColorSlider;
    public Image mainColorDisplay, markerColorDisplay, backgroundColorDisplay;

    private int activeMarker;

    // Variables for running the backtrackingStep Solver
    private int[,] inputField, inputBackup;
    private float starttime;

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultStats();
        SpawnTiles();
        SpawnLines();
        SpawnMarks();
        frames = 0;
    }

    private void Update()
    {
        // press f to delete history
        if (Input.GetKeyDown(KeyCode.F))
        {
            int[] amount = new int[1] { 0 };
            PlayerPrefsX.SetIntArray("amount", amount);
            Save_Load.LoadHistory();
            SpawnHistory();
            PlayerPrefs.DeleteAll();
        }

        if (frames == 0) SpawnHistory();
        frames++;
    }

    private void SetDefaultStats() {
        historyFields = new List<GameObject>();
        mainColorDisplay.color = Stats.hMainColor;
        markerColorDisplay.color = Stats.hMainColor;
        backgroundColorDisplay.color = Stats.backgroundColor;

        mainColorSlider.onValueChanged.AddListener(delegate {
            ChangeColor("main", mainColorSlider.value);
        });

        markerColorSlider.onValueChanged.AddListener(delegate {
            ChangeColor("marker", markerColorSlider.value);
        });

        backgroundColorSlider.onValueChanged.AddListener(delegate
        {
            ChangeColor("background", backgroundColorSlider.value);
        });

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

    public void ChangeColor(string identifier, float value) {
        float h, s, v;
        switch (identifier)
        {
            case "main":
                byte alpha1 = Stats.hMainColor.a;
                //print(Stats.hMainColor);
                Color.RGBToHSV(Stats.hMainColor, out h, out s, out v);
                //print(h);
                h = value;
                //print(h);
                Stats.hMainColor = Color.HSVToRGB(h, s, v);
                Stats.hMainColor.a = alpha1;
                //print(Stats.hMainColor);
                Save_Load.SaveData();
                for(int i = 0; i<field.GetLength(0); i++) { 
                    for(int j = 0; j<field.GetLength(1); j++) {
                        field[i, j].GetComponent<Image>().color = Stats.hMainColor;
                    }
                }
                solveBtn.GetComponent<Image>().color = Stats.hMainColor;
                deleteBtn.GetComponent<Image>().color = Stats.hMainColor;
                mainColorDisplay.color = Stats.hMainColor;
                break;
            case "marker":
                byte alpha2 = Stats.hMainColor.a;
                Color.RGBToHSV(Stats.hMarkerColor, out h, out s, out v);
                h = value;
                Stats.hMarkerColor = Color.HSVToRGB(h, s, v);
                Stats.hMarkerColor.a = alpha2;
                Save_Load.SaveData();
                for (int i = 0; i<marker.Length; i++) {
                    marker[i].GetComponent<Image>().color = Stats.hMarkerColor;
                }
                markerColorDisplay.color = Stats.hMarkerColor;
                break;
            case "background":
                Stats.backgroundColor = new Color32((byte)value, (byte)value, (byte)value, 255);
                Save_Load.SaveData();
                Camera.main.backgroundColor = Stats.backgroundColor;
                backgroundColorDisplay.color = Stats.backgroundColor;
                break;
        }
    }

    public void Erase() { 
        foreach(Button b in field) {
            b.GetComponentInChildren<Text>().text = "";
        }
    }

    private void SpawnTiles() {
        // (0,0) is in the lower left corner
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                Vector3 pos = new Vector3((i - fieldWidth / 2) * width / fieldWidth, -width/fieldWidth * (j-(fieldWidth-1)/2), 0);
                //Debug.Log(pos);
                field[i, j] = Instantiate(tilePrefab, pos, Quaternion.identity, GameObject.Find("MainField").transform);
                field[i, j].GetComponentInChildren<Text>().text = "";
                // Change y position of tile relative to the mainfield (parent of all tiles)
                pos = field[i, j].GetComponent<RectTransform>().localPosition;
                pos.y += field[i, j].transform.parent.GetComponent<RectTransform>().localPosition.y;
                field[i, j].GetComponent<RectTransform>().localPosition = pos;

                Vector2Int coordinates = new Vector2Int(i, j);
                field[i, j].onClick.AddListener(() => markField(coordinates));
            }
        }
    }

    private void SpawnLines() {
        Transform staticCanvas = GameObject.Find("MainField").transform;

        // vertical lines
        Vector3 pos = new Vector3(-width / 6, 0, 0);
        pos = pos + staticCanvas.transform.position;
        Instantiate(vertLine, pos, Quaternion.identity, staticCanvas);
        pos.x = width / 6 + staticCanvas.transform.position.x;
        Instantiate(vertLine, pos, Quaternion.identity, staticCanvas);

        // horizontal lines
        pos.x = staticCanvas.transform.position.x;
        pos.y = width/6 + staticCanvas.transform.position.y;
        Instantiate(horiLine, pos, Quaternion.identity, staticCanvas);
        pos.y = -width / 6 + staticCanvas.transform.position.y;
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

    private void SpawnHistory() { 
        foreach(GameObject go in historyFields)
        {
            Destroy(go);
        }
        historyFields = new List<GameObject>();
        for(int i = 0; i<Stats.history.Length; i++)
        {
            int j = Stats.history.Length - 1 - i;
            Vector3 pos = new Vector3(1.25f * Mathf.RoundToInt(Mathf.Pow(-1, j+1)), -9 - 2.5f * (j / 2), 0);
            GameObject historyField = Instantiate(fieldPrefab, pos, Quaternion.identity, GameObject.Find("CanvasOptionsAndHistory").transform);
            historyField.GetComponent<FieldManager>().InitButtons();
            historyField.GetComponent<FieldManager>().WriteValues(Stats.history[i]);
            historyFields.Add(historyField);
        }
    }

    public void SolveBtnPressed()
    {
        if (solving)
        { 
            PrintSolution(inputBackup);
            solver.StopBacktracking();
            solveBtn.GetComponentInChildren<Text>().text = "Solve Sudoku";
            solving = false;
            return;
        }
        solving = true;
        solveBtn.GetComponentInChildren<Text>().text = "Stop Solving";
        Solve();
        return;
    }

    public void Solve() {
        int[,] numbers = GetCurrentlyShownField();
        // example numbers for test purposes
        //numbers = new int[,] { {0,3,0,0,0,6,0,5,0 }, {1,6,0,3,0,0,9,0,0 }, {8,0,0,4,7,0,1,3,0 }, {0,4,7,9,0,1,6,2,3 }, {0,8,6,5,4,2,0,0,0 }, {2,0,1,7,0,0,8,0,5 }, {6,7,0,2,0,0,0,9,0 }, {4,0,0,0,9,7,0,6,8 }, {9,0,5,6,0,8,2,0,4 } };
        //numbers = new int[,] { { 8,0,4,9,0,6,2,7,1 }, { 9,6,7,8,1,2,4,0,5 }, { 2,0,0,0,3,4,6,9,8 }, { 5,8,2,1,9,0,7,4,0 }, { 6,0,0,0,4,8,1,2,3 }, { 4,1,3,2,0,7,5,8,9 }, { 7,0,5,3,8,1,9,6,4 }, { 0,9,6,4,0,5,8,1,0 }, { 1,4,8,6,7,9,3,5,0 } };
        //numbers = new int[,] { { 8,0,4,9,0,6,2,7,1 }, { 9,6,7,8,1,2,4,3,5 }, { 2,0,0,0,3,4,6,9,8 }, { 5,8,2,1,9,0,7,4,0 }, { 6,0,0,0,4,8,1,2,3 }, { 4,1,3,2,0,7,5,8,9 }, { 7,0,5,3,8,1,9,6,4 }, { 0,9,6,4,2,5,8,1,0 }, { 1,4,8,6,7,9,3,5,0 } };
        //numbers = new int[,]{ { 8, 0, 9, 7, 4, 5, 0, 1, 6 },{ 6, 7, 4, 0, 1, 2, 5, 8, 9 },{ 5, 2, 1, 6, 0, 9, 7, 3, 4 },{ 0, 8, 6, 4, 3, 7, 1, 2, 5 },{ 2, 1, 3, 9, 5, 6, 4, 0, 8 },{ 7, 4, 5, 1, 2, 8, 0, 6, 3 },{ 1, 5, 8, 2, 9, 3, 6, 4, 7 },{ 3, 6, 2, 5, 7, 4, 8, 9, 1 },{ 4, 9, 7, 8, 6, 1, 3, 5, 2 } };
        inputField = new int[9, 9];
        inputBackup = new int[9, 9];
        for(int i = 0; i<9; i++) { 
            for(int j = 0; j<9; j++) {
                inputField[i, j] = numbers[i, j];
                inputBackup[i, j] = numbers[i, j];
            }
        }
        Save_Load.SavePattern(numbers);
        Save_Load.LoadHistory();
        SpawnHistory();
        solver = new Solver(new field(numbers), 30);
        solver.Backtracking();
    }

    public void PrintSolution(int[,] solution, bool whiteFont = false) {
        for(int i  = 0; i<9; i++)
        {
            for(int j = 0; j<9; j++)
            {
                if(!whiteFont && field[i,j].GetComponentInChildren<Text>().text == "") {
                    field[i, j].GetComponentInChildren<Text>().color = Stats.hMainColor;
                }
                field[i, j].GetComponentInChildren<Text>().text = solution[i, j]==0 ? "" : solution[i, j].ToString();
            }
        }
    }

    public int[,] GetCurrentlyShownField() {
        int[,] output = new int[9, 9];
        for(int i = 0; i<9; i++) { 
            for(int j = 0; j<9; j++) {
                if (int.TryParse(field[i, j].GetComponentInChildren<Text>().text, out output[i, j])) ;
                else output[i, j] = 0;
            }
        }
        return output;
    }

    public void ChangeMarker(int newMarker) {
        activeMarker = newMarker;
        for(int i = 0; i<marker.Length; i++)
        {
            marker[i].GetComponent<Image>().color = Stats.hMarkerColor;
        }
        marker[newMarker].GetComponent<Image>().color = Color.cyan;
    }

    public void markField(Vector2Int coordinates) {
        Button toMark = field[coordinates.x, coordinates.y];
        toMark.GetComponentInChildren<Text>().text = activeMarker > 0 ? activeMarker.ToString() : "";
        toMark.GetComponentInChildren<Text>().color = Color.white;
    }
}
