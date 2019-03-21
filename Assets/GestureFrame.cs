using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a detected gesture in a frame
public class GestureFrame {

    public GestureType type;            // The type of gesture
    public int x, y, width, height;     // The position and size of the gesture in screen coordinates (x, y)

    // Constructor to create a new instance with data
    public GestureFrame(GestureType type, CvDetectedGesture gesture)
    {
        this.type = type;
        this.x = gesture.x;
        this.y = gesture.y;
        this.width = gesture.width;
        this.height = gesture.height;
    }

    // Constructor to create an empty instance
    public GestureFrame()
    {
        type = GestureType.NONE;
    }

    // Get a list of detected gestures from the list retrieved from OpenCV
    // This function just converts the list of detected gestures retrieved from OpenCV to the format that we can use in Unity
    public static List<GestureFrame> GetList(GestureType type, CvDetectedGesture[] gestures, int n)
    {
        List<GestureFrame> list = new List<GestureFrame>();

        for (int i = 0; i < n; i++)
        {
            GestureFrame g = new GestureFrame(type, gestures[i]);
            list.Add(g);
        }

        return list;
    }
}
