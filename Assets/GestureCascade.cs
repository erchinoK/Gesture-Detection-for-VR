using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

// This class links to the C++ library that uses OpenCV API
internal static class OpenCVInterop
{
    // CallingConvention allows to send strings to the .dll (?)
    // IntPtr is a pointer to the native object (CascadeClassifier inside the DLL) that can be used like an instance
    //https://answers.unity.com/questions/1200157/nonstatic-extern-functions-from-dll-plugin-import.html
    [DllImport("OpencvGestureDetector", EntryPoint = "InitCascade", CallingConvention = CallingConvention.Cdecl)]
    internal unsafe static extern IntPtr InitCascade(string filePath);

    [DllImport("OpencvGestureDetector", EntryPoint = "DetectGesture")]
    internal unsafe static extern int DetectGesture(IntPtr cascade, Color32[] raw, int width, int height, CvDetectedGesture* outGestures, int maxOutGestureCount, float scaleFactor = 1.1f, int minSize = 30);
}

// Define the structure to be sequential and with the correct byte size(4 ints = 4 bytes * 4 = 16 bytes)
// Must be the same in the C++ library
[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct CvDetectedGesture
{
    public int x, y, width, height;
}

// Types of gestures. Gesture's names
public enum GestureType
{
    NONE, OK, NOT, OPEN, POINTING, CLOSE, GRAB_OPEN, GRAB_CLOSE
};

// This class represents a single detector
public class GestureCascade
{
    public GestureType type { get; set; }                       // The type of gesture this classifier can detect
    public int nFoundInFrame { get; set; }                      // The number of detected gestures in the current frame
    public CvDetectedGesture[] foundInFrame { get; set; }       // The detected gestures in the current frame
    public Color color { get; set; }                            // A colour. For debugging

    private IntPtr cascade = IntPtr.Zero;                       // A pointer to the static class that is linked to the C++ library
    private int maxPerFrame = 0;                                // Maximum number of gestures to detect in the current frame

    public GestureCascade(GestureType type, String filePath, int maxPerFrame, Color color)
    {
        // Initialize the cascades with the files. The library will read the physical file with the clasifier
        cascade = OpenCVInterop.InitCascade(filePath);

        this.type = type;
        this.maxPerFrame = maxPerFrame;
        this.color = color;
    }

    // Try to detect gestures given a frame
    public int DetectInFrame(Color32[] rawImg, int width, int height)
    {
        nFoundInFrame = 0;

        unsafe
        {
            // Create a pointer of an array of gestures to pass them as reference
            foundInFrame = new CvDetectedGesture[maxPerFrame];
            fixed (CvDetectedGesture* _foundInFramePtr = foundInFrame)
            {
                try
                {
                    // Call to the C++ library to detect gestures
                    nFoundInFrame = OpenCVInterop.DetectGesture(cascade, rawImg, width, height, _foundInFramePtr, maxPerFrame);
                }
                catch (Exception e)
                {
                    Debug.Log("Error");
                }
            }


        }

        return nFoundInFrame;
    }

}

