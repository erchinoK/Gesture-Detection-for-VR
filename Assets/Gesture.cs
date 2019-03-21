using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a 3D hand
public class Gesture : MonoBehaviour {

    private bool show = false;      // Indicate if this model should be displayed (the gesture is being detected)
    private Vector3 position;       // Position to display
    private Quaternion rotation;    // Rotation of the model so it is always facing correctly the camera
    private Vector3 scale;          // Size of the model

    // Default area of the gesture. This value helps calculate the z value of the model (how far it is from the camera)
    // This value is hard-coded. It is calculated before hand. 
    // It is the area of the detected gesture when the real hand is at 30cm of the phone's camera
    public int defaultArea;

    // Default position of the gesture. It is the initial position which is far from the camera (hidden)
    private Vector3 defaultPosition;

	// Use this for initialization
	void Start () {

        // Save the initial position (default)
        defaultPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        // Check if the hand should be displayed (the gesture is being detected)
		if (show)
        {
            // Check if the position is not empty
            if (position != null)
            {
                // The model is displayed
                transform.position = position;

                // The rotation is changed so it is always facing correctly to the camera
                transform.localRotation = rotation;
                
            }
        } else      // The hand should not be displayed (the gesture is not being detected)
        {
            // The model returns to the initial position (hidden)
            transform.position = defaultPosition;
        }
	}

    /// <summary>
    /// Show the hand. Calculates the position and rotation
    /// </summary>
    /// <param name="x">Position X of the gesture in screen coordinates</param>
    /// <param name="y">Position Y of the gesture in screen coordinates</param>
    /// <param name="width">Width of the gesture in screen coordinates</param>
    /// <param name="height">Height of the gesture in screen coordinates</param>
    /// <param name="rotation">Rotation to apply (it is the rotation of the camera so it is always facing correctly to the camera)</param>
    public void Show(int x, int y, int width, int height, Quaternion rotation)
    {
        show = true;

        // Calculate the area of the gesture in screen coordinates
        int area = width * height;

        // The difference in ratio of the default ratio and the current ratio.
        // If the difference is smaller then the 3d hand will appear close to the camera
        // If the dfference is bigger the the 3d hand will appear far from the camera
        float ratio = (float)defaultArea / area;

        // Calculate the position in the 3d world.
        // x is a corner of the gesture so the center is calculated. Same with y
        // z is calculated according near clipping plane and the ratio
        // If the real hand is closer to the phone's camera then the ratio will be less than 1.0, thus the z will be closer to the camera
        Vector3 position3D = Camera.main.ScreenToWorldPoint(new Vector3(x - width / 2, Screen.height - y - height / 2, Camera.main.nearClipPlane + 0.5f * ratio));
        Vector3 cam = Camera.main.transform.position;
        
        // The new position and rotation are applied
        this.position = position3D;
        this.rotation = rotation;

    }

    // Hide the hand
    public void Hide()
    {
        show = false;
    }

    // Return the initial position
    public Vector3 GetDefaultPosition()
    {
        return defaultPosition;
    }

    void OnGUI()
    {
        
    }
}
