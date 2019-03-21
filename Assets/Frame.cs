using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class saves the state of a frame: the list of detected gestures and the selected true positive from the list
public class Frame {

    // Stores the number of each gestures in the frame
    // e.g: (POINTING, 2) , (CLOSE, 1)
    private Dictionary<GestureType, int> gesturesCounters;
    
    // The selected true positive
    public GestureFrame prevalent;

    // Indicate if a true positive has been selected
    public bool isPrevalentCalculated = false;

    // The list of detected gesture in this frame
    private List<GestureFrame> gestures;
        
    // Constructor
    public Frame()
    {
        gestures = new List<GestureFrame>();
        gesturesCounters = new Dictionary<GestureType, int>();
        prevalent = new GestureFrame();
    }

    // Get the list of detected gestures
    public List<GestureFrame> GetGestures()
    {
        return gestures;
    }

    // Get the list of detected gestures of a specific type. Remember that a frame might detect many gestures of the same type but with
    // different positions and sizes (many false positives)
    public List<GestureFrame> GetGestures(GestureType type)
    {
        List<GestureFrame> _gestures = new List<GestureFrame>();

        foreach (var gesture in gestures)
        {
            if (gesture.type == type)
            {
                _gestures.Add(gesture);
            }
        }
        return _gestures;
    }

    // Add a gesture to the list of detectred
    public void AddGesture(GestureFrame gesture)
    {
        gestures.Add(gesture);

        // Update the counter of gestures. Increases the gesture counter by 1
        UpdateCounters(gesture.type, 1);

    }

    // Add a list of gestures to the list of detected
    public void AddGestures(List<GestureFrame> gestures)
    {
        if (gestures == null)
        {
            return;
        }

        if (gestures.Count > 0)
        {
            this.gestures.AddRange(gestures);

            // Update the counters. Increases the counter
            UpdateCounters(gestures[0].type, gestures.Count);

        }

    }

    // Get the amount of detected gestures of a certain type
    public int GetGestureCounter(GestureType type)
    {
        if (!gesturesCounters.ContainsKey(type))
        {
            return 0;
        }

        return gesturesCounters[type];
    }

    // Get a list with the names of the detected gestures (non-repetitive).
    public List<GestureType> GetGestureTypes()
    {
        List<GestureType> types = new List<GestureType>();

        foreach (var counter in gesturesCounters)
        {
            if (counter.Key != GestureType.NONE)
                types.Add(counter.Key);
        }

        return types;
    }

    // Update the counter of a gesture. Everytime a gesture is added then the corresponding counter increases
    private void UpdateCounters(GestureType type, int n)
    {
        if (gesturesCounters.ContainsKey(type))
        {
            gesturesCounters[type] += n;
        } else
        {
            gesturesCounters[type] = n;
        }
    }

    // Select a true positive
    public void SetPrevalentGesture(GestureFrame gesture)
    {        
        prevalent = gesture;

        isPrevalentCalculated = true;
    }

    // Remove the information of the frame. Remember that it is a circular buffer. It recycles frames
    public void Clear()
    {
        gestures.Clear();
        gesturesCounters.Clear();
        prevalent = new GestureFrame();
        isPrevalentCalculated = false;
    }

        
}
