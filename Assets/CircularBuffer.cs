using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This class stores the state of the last n frames
public class CircularBuffer
{

    public int nFrames;             // number of frames to store
    private Frame[] frames;         // array of frames
    public int currentFrame = 0;    // index of the current frame

    // Constructor
    public CircularBuffer(int n)
    {
        // Initializes variables
        this.nFrames = n;
        frames = new Frame[n];
        for (int i = 0; i < n; i++)
        {
            frames[i] = new Frame();
        }
    }

    // Get current frame
    public Frame GetCurrentFrame()
    {
        return frames[currentFrame];
    }

    // Add list of detected gesture to the current frame
    public void AddGesturesToCurrentFrame(List<GestureFrame> gestures)
    {
        frames[currentFrame].AddGestures(gestures);
    }

    // Set the true positive of the current frame
    public void SetPrevalentInCurrentFrame(GestureFrame prevalent)
    {
        frames[currentFrame].SetPrevalentGesture(prevalent);
    }

    // Get the list of gestures (just names) that were detected in the current frame
    public List<GestureType> GetGestureTypesInCurrentFrame()
    {
        return frames[currentFrame].GetGestureTypes();
    }

    // Get the most detected gesture in the previous frames. The most detected gesture is then nominated as the true positive of
    // the current frame because there is a high probability that it is still the same gesture
    public GestureFrame GetPrevalentGestureInPreviousFrames()
    {

        GestureFrame prevalent = new GestureFrame();

        // If the buffer has 1 frame then there are no previous frames
        if (nFrames == 1)
            return prevalent;

        // Create a counter for all the gestures in the format (name, n)
        Dictionary<GestureType, int> counter = new Dictionary<GestureType, int>();

        // Loop through all the previous frames
        int prev = GetPreviousFrame();
        while (prev != currentFrame)
        {
            // Check if this previous frame has a true positive
            if (frames[prev].isPrevalentCalculated && frames[prev].prevalent.type != GestureType.NONE)
            {
                // Check if the counter does not have the true positive of this previous frame
                if (!counter.ContainsKey(frames[prev].prevalent.type))
                {
                    // If the counter does not exist then it is created and initialized with 1
                    counter.Add(frames[prev].prevalent.type, 1);
                }
                else
                {
                    // If the counter exists the it is increased
                    counter[frames[prev].prevalent.type] += 1;
                }
            }

            // Get the next previous frame
            prev--;
            if (prev < 0) prev = nFrames - 1;
        }

        // Check if the counter is empty. This means that the previous frames did not detect anything. Returns an empty object
        if (counter.Count == 0)
        {
            return prevalent;
        }

        // Get the most detected gesture in all the previous frames (the counter that has more number)
        var max = counter.OrderByDescending(entry => entry.Value).FirstOrDefault();

        if (!max.Equals(new KeyValuePair<GestureType, int>())) // Check if it found anything
        {
            // At this point max.Key has the most detected gesture among all the previous frames
            prevalent.type = max.Key;

            // Get the position of the most detected gesture. This is necessary because the previous steps only found the NAME of the gesture, 
            // not the position
            GetPrevalentPositionInPreviousFrame(prevalent);
        }

        return prevalent;
    }

    // Get the position of a gesture in the first previous frame that has it
    private void GetPrevalentPositionInPreviousFrame(GestureFrame prevalent)
    {
        // Loop through previous frames
        int prev = GetPreviousFrame();
        while (prev != currentFrame)
        {
            // Check if this previous frame has a true positive
            if (frames[prev].isPrevalentCalculated)
            {
                // Check if this true positive is the same than the one we are looking for
                if (frames[prev].prevalent.type == prevalent.type)
                {
                    // The gesture is updated. The position in the only value overwritten, the rest is the same
                    prevalent = frames[prev].prevalent;
                    return;
                }
            }
            prev--;
            if (prev < 0) prev = nFrames - 1;
        }
    }

    // Get string with the true positive of each frame ordered sequentially. The one wrapped in *** is the current frame
    // It is used for debugging
    public string GetPrevalentsStrings()
    {
        string str = "";

        for (int i = 0; i < nFrames; i++)
        {
            if (i == currentFrame)
            {
                str += "***" + frames[i].prevalent.type + "*** ";
            }
            else
            {
                str += frames[i].prevalent.type + " ";
            }
        }

        return str;

    }

    // Move the pointer to the next frame
    public void MoveToNextPosition()
    {
        currentFrame = (currentFrame + 1) % nFrames;
        frames[currentFrame].Clear();
    }

    // Get the index of the previous frame
    public int GetPreviousFrame()
    {
        int prevFrame = currentFrame - 1;

        if (prevFrame < 0)
        {
            prevFrame = nFrames - 1;
        }

        return prevFrame;
    }
}
