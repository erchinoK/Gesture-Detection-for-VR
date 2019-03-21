using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this object creates more objects
public class ObjectFactory : MonoBehaviour {
        
    private bool isCreating = false;        // Indicates if the factory is on
    public GameObject template;             // The type of object to create
    public GameObject parent;               // The parent of the new objects
    private float time = 0.0f;              
    public float timeLimit;
    public int max;                         // Maximum number of objects to create
    private int n = 0;                      // Number of objects created

	// Use this for initialization
	void Start () {
		if (max == 0)
        {
            max = 10;
        }
	}
	
	// Update is called once per frame
	void Update () {

        // Check if the factory is On
		if (isCreating)
        {
            // Check if the factory can create more objects
            if (n < max)
            {
                // Check if the template and parent are not empty. The factory creates a new object when every time the timer is 0
                if (template != null && parent != null && time == 0)
                {
                    // Create a new object
                    GameObject newObject = Object.Instantiate(template, transform.position, transform.rotation, parent.transform);
                    newObject.tag = "Factory";
                    n++;
                }
                // The timer is increased and initialized to zero every timeLimit. This allows to create a new object every few seconds
                time += Time.deltaTime;
                if (time > timeLimit) time = 0;
            }
        }
	}

    // Turn On the factory
    public void StartCreating()
    {
        isCreating = true;
    }

    // Turn Off the factory
    public void StopCreating()
    {
        isCreating = false;
        time = 0;
    }

    public void Reset()
    {
        n = 0;
    }


}
