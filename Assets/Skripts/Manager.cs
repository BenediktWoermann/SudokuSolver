using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public Slider sliderMainColor, sliderMarkerColor, sliderBackgroundColor, sliderSpeed;

    // Start is called before the first frame update
    void Start()
    {
        LoadColor();
        LoadHistory();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadHistory() {
        Save_Load.LoadHistory();
    }

    private void LoadColor() {
        DataType read = Save_Load.LoadData();

        Stats.speed = 10;
        if(read != null)
        {
            Stats.speed = read.speed;
            sliderSpeed.value = Mathf.Min(sliderSpeed.maxValue, Stats.speed);
        }

        Stats.hMainColor = new Color32(84, 231, 136, 189);
        if (read != null)
        {
            Color col1 = new Color(read.rMainColor, read.gMainColor, read.bMainColor, read.aMainColor);
            Color.RGBToHSV(col1, out float h, out float s, out float v);
            sliderMainColor.value = h;
            Color.RGBToHSV(Stats.hMainColor, out float mainH, out float mainS, out float mainV);
            Stats.hMainColor = Color.HSVToRGB(h, mainS, mainV);
        }

        Stats.hMarkerColor = new Color32(255, 183, 0, 1);
        if (read != null)
        {
            Color col2 = new Color(read.rMarkerColor, read.gMarkerColor, read.bMarkerColor, read.aMarkerColor);
            Color.RGBToHSV(col2, out float h, out float s, out float v);
            sliderMarkerColor.value = h;
            Color.RGBToHSV(Stats.hMarkerColor, out float markerH, out float markerS, out float markerV);
            Stats.hMarkerColor = Color.HSVToRGB(h, markerS, markerV);
        }

        Stats.backgroundColor = new Color32(30, 30, 30, 255);
        if(read != null) 
        {
            //print(read.backgroundGreyValue);
            Stats.backgroundColor = new Color32(read.backgroundGreyValue, read.backgroundGreyValue, read.backgroundGreyValue, 255);
            sliderBackgroundColor.value = read.backgroundGreyValue;
        }

    }

}
