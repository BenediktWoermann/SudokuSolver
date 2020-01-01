using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation(int identifier) {
        switch (identifier)
        {
            case 0:
                gameObject.GetComponent<Animator>().Play("CameraUp");
                break;
            case 1:
                gameObject.GetComponent<Animator>().Play("CameraDown");
                break;
            case 2:
                gameObject.GetComponent<Animator>().Play("CameraUpHistory");
                break;
            case 3:
                gameObject.GetComponent<Animator>().Play("CameraDownHistory");
                break;
        }
    }
}
