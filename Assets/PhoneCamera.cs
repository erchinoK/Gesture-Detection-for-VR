using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using System.IO;

// This class controls the phone's camera and calls the detectors
public class PhoneCamera : MonoBehaviour {

    public RawImage background;             
    public AspectRatioFitter fit;
    public int requestedHeight;
    public int requestedWidth;
    public int requestedCamFPS;
    public int requestedFPS;

    public Transform cameraTransform;       // Unity's Camera
    public CharacterController character;   // Player
    public float speed;                     // Speed of the movement of the player
    
    private bool camAvailable;
    private WebCamTexture camera;
    private Texture defaultBackground;
    
    // Filenames of the detectors
    private const string cascadeFileNameOpen = "cascadeOpen.xml";
    private const string cascadeFileNamePointing = "cascadePointing.xml";
    private const string cascadeFileNameOk = "cascadeOk.xml";
    private const string cascadeFileNameNot = "cascadeNot.xml";
    private const string cascadeFileNameClose = "cascadeClose.xml";
    private const string cascadeFileNameGrabOpen = "cascadeGrabOpen.xml";
    private const string cascadeFileNameGrabClose = "cascadeGrabClose.xml";

    private List<GestureCascade> cascades;  // List of detectors

    public int nLastFrames;                 // Size of the circular buffer
    private int currentFrame = 0;           // Index of the current frame
    private CircularBuffer lastFrames;      // Buffer
    private GestureFrame currentGesture;    // Selected true positive of the current frame

    private String message1 = "";
    private String message2 = "";
    private String message3 = "";

    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = requestedFPS;
        
        Time.captureFramerate = 100;

        // Create the full path of the cascade files to send to OpenCV (the files are not created yet)
        string cascadeFilePathOpen = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNameOpen);
        string cascadeFilePathPointing = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNamePointing);
        string cascadeFilePathClose = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNameClose);
        string cascadeFilePathOk = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNameOk);
        string cascadeFilePathNot = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNameNot);
        string cascadeFilePathGrabOpen = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNameGrabOpen);
        string cascadeFilePathGrabClose = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNameGrabClose);
        //string cascadeFilePath2 = System.IO.Path.Combine(Application.persistentDataPath, cascadeFileNamePointing);

        // Extract the cascade files and copy them into new files
        StartCoroutine(ExtractFile(cascadeFileNameOpen, cascadeFilePathOpen));
        StartCoroutine(ExtractFile(cascadeFileNamePointing, cascadeFilePathPointing));
        StartCoroutine(ExtractFile(cascadeFileNameClose, cascadeFilePathClose));
        StartCoroutine(ExtractFile(cascadeFileNameOk, cascadeFilePathOk));
        StartCoroutine(ExtractFile(cascadeFileNameNot, cascadeFilePathNot));
        StartCoroutine(ExtractFile(cascadeFileNameGrabOpen, cascadeFilePathGrabOpen));
        StartCoroutine(ExtractFile(cascadeFileNameGrabClose, cascadeFilePathGrabClose));

        // Create the list of detectors
        cascades = new List<GestureCascade>();
        cascades.Add(new GestureCascade(GestureType.OPEN, cascadeFilePathOpen, 5, new Color(1.0f, 0.0f, 1.0f, 0.2f)));
        //cascades.Add(new GestureCascade(GestureType.POINTING, cascadeFilePathPointing, 5, new Color(0.0f, 1.0f, 0.0f, 0.2f)));
        //cascades.Add(new GestureCascade(GestureType.CLOSE, cascadeFilePathClose, 5, new Color(1.0f, 0.0f, 0.0f, 0.2f)));
        cascades.Add(new GestureCascade(GestureType.OK, cascadeFilePathOk, 5, new Color(0.0f, 0.0f, 0.0f, 0.2f)));
        //cascades.Add(new GestureCascade(GestureType.NOT, cascadeFilePathNot, 5, new Color(1.0f, 1.0f, 1.0f, 0.2f)));
        //cascades.Add(new GestureCascade(GestureType.GRAB_CLOSE, cascadeFilePathGrabClose, 5, new Color(0.0f, 0.5f, 0.5f, 0.2f)));
        cascades.Add(new GestureCascade(GestureType.GRAB_OPEN, cascadeFilePathGrabOpen, 5, new Color(0.5f, 0.5f, 0.0f, 0.2f)));

        lastFrames = new CircularBuffer(nLastFrames);
    }

    // Initialization
    void Start () {

        // Initialize the cascades with the files
        //cascade1 = OpenCVInterop.InitCascade("/storage/emulated/0/Android/data/com.Personal.GestureDetectionVR/files/cascade.xml");
        
        defaultBackground = background.texture;

        // Get the phone's cameras
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            // Get the front camera
            if (!devices[i].isFrontFacing)
            {
                // Initializes the front camera
                camera = new WebCamTexture(devices[i].name, requestedWidth, requestedHeight, requestedCamFPS);
            }
        }

        if (camera == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        // Starts the camera
        camera.Play();
        background.texture = camera;

        camAvailable = true;

    }

    // Update is called once per frame
    void Update () {

        message2 = "";
        message3 = "";

        if (!camAvailable)
            return;

        // Fix the aspect ratio of the camera
        float ratio = (float)camera.width / (float)camera.height;
        fit.aspectRatio = ratio;

        float scaleY = camera.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -camera.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);


        // Get the array of pixels of the current frame
        Color32[] rawImg = camera.GetPixels32();
        System.Array.Reverse(rawImg);

        currentGesture = new GestureFrame();
        
        // Search for all gestures. Call all detectors
        foreach (var cascade in cascades)
        {
            int nDetected = cascade.DetectInFrame(rawImg, camera.width, camera.height);

            // If at least one gesture is detected then it is added to the frame 
            if (nDetected > 0)
            {
                lastFrames.AddGesturesToCurrentFrame(GestureFrame.GetList(cascade.type, cascade.foundInFrame, nDetected));
            }
        }

        // Get the list of gesture types (just the names) detected
        List<GestureType> detectedGesturesTypesInCurrentFrame = lastFrames.GetGestureTypesInCurrentFrame();

        // Check how many gestures were detected
        if (detectedGesturesTypesInCurrentFrame.Count == 1)     // If only one gesture type was detected
        {
            // Select the first gesture
            currentGesture = lastFrames.GetCurrentFrame().GetGestures()[0];


        } else if (detectedGesturesTypesInCurrentFrame.Count > 1)       // If there are more than one gesture type
        {
            // Search for the most detected gesture in the previous frames
            GestureFrame prevalentPreviousFrames = lastFrames.GetPrevalentGestureInPreviousFrames();
            
            // Check if the previous frames detected anything
            if (prevalentPreviousFrames.type == GestureType.NONE)
            {
                // If there is no previous prevalent, the first one is selected (because we do not have more information to pick one)
                currentGesture = lastFrames.GetCurrentFrame().GetGestures()[0];
            } else
            {
                // Check if the prevalent gesture is among the detected ones
                if (detectedGesturesTypesInCurrentFrame.Contains(prevalentPreviousFrames.type))      // If it is among them
                {
                    // The current gesture is one of the similar gestures in the current frame
                    if (lastFrames.GetCurrentFrame().GetGestures(prevalentPreviousFrames.type).Count > 1)
                    {
                        // Should select the closest position to the previous prevalent
                    }
                    currentGesture = lastFrames.GetCurrentFrame().GetGestures(prevalentPreviousFrames.type)[0];
                }
                else    // If it is not among them
                {
                    // Any gesture is selected (first one)
                    currentGesture = lastFrames.GetCurrentFrame().GetGestures()[0];
                }
            }
        }

        // Set the true positive of the current frame
        lastFrames.SetPrevalentInCurrentFrame(currentGesture);

        // Calculate the position of the gesture according to previous ones
        //currentGesturePosition = lastFrames.GetPrevalentPositionInPreviousFrame(currentGestureType);
        //lastFrames.GetPrevalentPositionInPreviousFrame(currentGesture);


        float RotateSpeed = 700f;

        // Move to the next frame in the buffer
        lastFrames.MoveToNextPosition();

    }

    // For debugging
    // Convert from cam coordinates (176x144) to screen coordinate (1900x1600)
    public Vector4 CamCoordinatesToScreenCoordinates(int x, int y, int width, int height)
    {

        float ratio = Screen.height / camera.height;
        int leftMargin = (int)((Screen.width - (camera.width * ratio)) / 2);


        int newX = (int)((camera.width - x - width) * ratio + leftMargin);
        int newY = (int)(y * ratio);
        int newW = (int)(width * ratio);
        int newH = (int)(height * ratio);


        Vector4 xywh = new Vector4(newX, newY, newW, newH);

        return xywh;
    }

    // For debugging
    void OnGUI()
    {
        
        float ratio = Screen.height / camera.height;
        int leftMargin = (int)((Screen.width - (camera.width * ratio)) / 2);


        int newX = (int)((camera.width - currentGesture.x - currentGesture.width) * ratio + leftMargin);
        int newY = (int)(currentGesture.y * ratio);
        int newW = (int)(currentGesture.width * ratio);
        int newH = (int)(currentGesture.height * ratio);

        GUI.Box(new Rect(newX, newY, newW, newH), "", MakeStyle(new Color(0, 1f, 0, 0.4f)));

        // Loop of gestures
        foreach (var cascade in cascades)
        {

            // Loop of found gestures in the frame (per gesture)
            for (int i = 0; i < cascade.nFoundInFrame; i++)
            {
                newX = (int)((camera.width - cascade.foundInFrame[i].x - cascade.foundInFrame[i].width) * ratio + leftMargin);
                newY = (int)(cascade.foundInFrame[i].y * ratio);
                newW = (int)(cascade.foundInFrame[i].width * ratio);
                newH = (int)(cascade.foundInFrame[i].height * ratio);

                GUI.Box(new Rect(newX, newY, newW, newH), "", MakeStyle(new Color(1f, 0, 0, 0.2f)));
            }

        }

        

        GUILayout.BeginArea(new Rect(20, 20, 400, 400));
        //GUILayout.Label(
        GUILayout.Label("Screen: " + Screen.width + "x" + Screen.height);
        GUILayout.Label("Cam: " + camera.width + "x" + camera.height);
        GUILayout.Label("Selected: " + message1);
        //GUILayout.Label("Prevalent: " + message2);
        GUILayout.Label("N In Frame: " + message3);
        GUILayout.Label("Position & Size: " + newX + " " + newY + ", " + newW + " " + newH);
        GUILayout.Label(lastFrames.GetPrevalentsStrings());
        GUILayout.EndArea();


        //GUI.Box(new Rect(10, 200, 150, 20), message2);
        //GUI.Box(new Rect(10, 150, 1000, 600), message);

    }

    // Get the true positive of the current frame
    public GestureFrame GetCurrentGesture()
    {
        return currentGesture;
    }

    /** 
        Coroutine to extract a file from the compressed .apk file and copy it into a physical folder

        @fileName: name of the file to extract from the Asset section of the .apk file
        @outFullPath: full path of the new file
    */
    IEnumerator ExtractFile(String fileName, String outFullPath)
    {
        // Get the path of the file in the Asset directory inside the .apk file
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

        // Verify if it is a valid path for Android
        if (filePath.Contains("://"))
        {
            // Extract the content of the file
            var www = new WWW(filePath);
            yield return www;

            // Write the content into a separate file in the specified folder
            File.WriteAllBytes(outFullPath, www.bytes);
        }
    }

    // For debuging. Create a new style for the GUI
    private GUIStyle MakeStyle(Color color)
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.normal.background = MakeTex(2, 2, color);
        return style;
    }

    // For debuging. Create a new texture for the GUI
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }


    
}

