using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class is a circular timer. It is used with the gestures
public class Timer : MonoBehaviour {

    public GameObject gesture;
    public float timeAmount;
    private Image circle;
    private float time = 0;
    private bool isRunning = false;
    private bool isFinished = false;

	// Use this for initialization
	void Start () {
        circle = this.GetComponent<Image>();
        circle.fillAmount = 0;

        if (timeAmount == 0)
            timeAmount = 1;
	}

    // Update is called once per frame
    void Update() {

        transform.rotation = gesture.transform.rotation;

        if (isRunning)
        {
            if (time < timeAmount)
            {
                time += Time.deltaTime;
                circle.fillAmount = time / timeAmount; 
            }
            else
            {
                isFinished = true;

                ResetValues();
            }
        }
	}

    public void Run()
    {
        isRunning = true;
        isFinished = false;
    }

    public void Stop()
    {
        isRunning = false;
        isFinished = false;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsFinished()
    {
        return isFinished;
    }

    public void ResetValues()
    {
        time = 0;
        circle.fillAmount = 0;
    }
}
