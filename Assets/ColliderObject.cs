using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the collision between the 3D hand and the objects. It is used to grab objects with the pinch gesture
public class ColliderObject : MonoBehaviour {

    public Timer timer;                 
    private GameObject grabbed;
    private GameObject intersected;
    public Gesture gesture;
    private VRManager vrManager;

	// Use this for initialization
	void Start () {

        // Initilize the vrManager
        vrManager = Camera.main.GetComponent<VRManager>();
	}
	
	// Update is called once per frame
	void Update () {

        // If the menu is showing do not do anything
        if (vrManager.menuManager.IsShowing())
            return;

        // Check if the hand is out of sight (the gesture is not being detected)
        if (transform.position == gesture.GetDefaultPosition())
        {
            // Drop the object
            grabbed = null;
        }

        // Check if the timer finished
        if (timer.IsRunning() && timer.IsFinished())
        {
            // The object is grabbed and the timer is stopped (hidden)
            timer.Stop();
            timer.ResetValues();
            grabbed = intersected;
        }

        // Check if it is grabbing an object
        if (grabbed != null)
        {
            // The object is placed to the right of the hand as if it is grabbing it
            // The position is a point perpendicular (Cross Operation) to the vector between the cam and the gesture and the cam's up vector.
            Vector3 cam = Camera.main.transform.position;
            Vector3 ges = this.transform.position;
            Vector3 camToGesture = new Vector3(ges.x - cam.x, ges.y - cam.y, ges.z - cam.z);
            Vector3 perpendicular = Vector3.Cross(camToGesture, Camera.main.transform.up);
            grabbed.transform.position = this.transform.position + (perpendicular.normalized * -0.6f);
        }
        
	}

    // Everytime the hand touches an object
    void OnTriggerEnter(Collider other)
    {

        // If the menu is shown do not do anything
        if (vrManager.menuManager.IsShowing())
            return;

        // Check if the object is movable (or created by the factory, which is also movable)
        if (other.tag.Equals("Movable")  || other.tag.Equals("Factory"))
        {

            // Check if it is a new object. The hand is touching it now
            if (other.gameObject != intersected)
            {
                // Save the collisioned object
                intersected = other.gameObject;

                // Check if the timer is not running and if there is no grabbed object
                if (!timer.IsRunning() && grabbed == null)
                {
                    // The timer starts running
                    timer.Run();
                }
            } else          // The hand is still touching this object
            {
                
            }
            
            
            
        } 
    }

    // Everytime the hand stops touching an object
    void OnTriggerExit(Collider other)
    {
        // If the menu is shown do not do anything
        if (vrManager.menuManager.IsShowing())
            return;

        // Check if the object is movable (or created by the factory, which is also movable)
        if (other.tag.Equals("Movable") || other.tag.Equals("Factory"))
        {

            // Check if it is not touching the object any more
            if (intersected == other.gameObject)
            {
                timer.Stop();
                timer.ResetValues();
                intersected = null;
            }

            // Check if it is dropping the object
            if (grabbed == other.gameObject)
            {
                // Check if it is not really dropping it, just moving it next to the hand to grab it
                if (grabbed != null)
                {
                    // Not necesarrily get in here
                }
                else      // It is really dropping the object
                {
                    grabbed = null;
                    intersected = null;
                }
            }
        }
    }

}
