using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeTextOnValue : MonoBehaviour
{
    public Slider slider;

    public void ChangeText() {
        if (slider.value.Equals(slider.maxValue))
        {
            this.GetComponent<Text>().text = "Instant";
            Stats.speed = 40000;
            Save_Load.SaveData();
        }
        else
        {
            this.GetComponent<Text>().text = slider.value.ToString() + "x";
            Stats.speed = (int)slider.value;
            Save_Load.SaveData();
        }
    }

}
