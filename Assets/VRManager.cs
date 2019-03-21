using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the gameplay accordingly to the detected gesture in the current frame
public class VRManager : MonoBehaviour {
    
    public MenuManager menuManager;
    public Timer timerOpen;
    public Timer timerOk;
    public PhoneCamera phoneCamera;

    public CharacterController character;

    public GameObject menuWalk;

    public Gesture gestureOpen;
    public Gesture gestureOk;
    public Gesture gestureNot;
    public Gesture gesturePointing;
    public Gesture gestureClose;
    public Gesture gestureGrabOpen;
    public Gesture gestureGrabClose;

    public GameObject scenario;

    public float speed;
    public float rotationSpeed;


    private Camera camera;

    // Use this for initialization
    void Start () {

        camera = Camera.main;

        // Initialize all objects in the scene
        InitMovables();



    }
    string message = "";

    // Update is called once per frame
    void Update () {

        // Get the detected gesture of the current frame
        var currentGesture = phoneCamera.GetCurrentGesture();

        // Hide all the 3D hands
        HideAllGestures();

        // Check if a gesture was detected
        if (currentGesture.type != GestureType.NONE)
        {
            // For debugging. Convert the position of the gesture from cam coordinates (176x144) to screen coordinates (1900x1600)
            Vector4 xywhScreenCoord = phoneCamera.CamCoordinatesToScreenCoordinates(currentGesture.x, currentGesture.y, currentGesture.width, currentGesture.height);
        
            // Check the type of gesture to trigger an action
            switch (currentGesture.type)
            {
                case GestureType.POINTING:

                    gesturePointing.Show((int)xywhScreenCoord.x, (int)xywhScreenCoord.y, (int)xywhScreenCoord.z, (int)xywhScreenCoord.w, Camera.main.transform.rotation);

                    // Moves the player
                    MoveForward();
                    break;
                case GestureType.CLOSE:

                    gestureClose.Show((int)xywhScreenCoord.x, (int)xywhScreenCoord.y, (int)xywhScreenCoord.z, (int)xywhScreenCoord.w, Camera.main.transform.rotation);

                    // Move the player
                    MoveBackward();
                    break;
                case GestureType.OPEN:

                    gestureOpen.Show((int)xywhScreenCoord.x, (int)xywhScreenCoord.y, (int)xywhScreenCoord.z, (int)xywhScreenCoord.w, Camera.main.transform.rotation);

                    // Check if the menu is not open and the timer is not running
                    if (!timerOpen.IsRunning() && !menuManager.IsShowing())
                    {
                        // Start the timer
                        timerOpen.Run();
                    }
                    else
                    {
                        if (timerOpen.IsFinished())
                        {
                            // Show the menu when the timer finishes
                            menuWalk.SetActive(false);
                            menuManager.ShowMenu();
                            timerOpen.Stop();
                        }
                    }

                    break;
                case GestureType.OK:

                    // Some gestures are positioned with an offset that I do not understand. (x / 2 is a hard code).
                    gestureOk.Show((int)xywhScreenCoord.x, (int)xywhScreenCoord.y, (int)xywhScreenCoord.z, (int)xywhScreenCoord.w, Camera.main.transform.rotation);
                    
                    // Check if the menu is shown and the timer is not running
                    if (!timerOk.IsRunning() && menuManager.IsFullyShown())
                    {
                        // Starts the timer
                        timerOk.Run();
                    }
                    else
                    {
                        if (timerOk.IsFinished())
                        {
                            // Hides the menu when the timer finishes
                            menuWalk.SetActive(true);
                            menuManager.HideMenu();
                            timerOk.Stop();
                        }
                    }

                    break;
                case GestureType.NOT:

                    // Some gestures are positioned with an offset that I do not understand. (x * 2 is a hard code).
                    gestureNot.Show((int)xywhScreenCoord.x * 2, (int)xywhScreenCoord.y, (int)xywhScreenCoord.z, (int)xywhScreenCoord.w, Camera.main.transform.rotation);

                    //Rotate(rotationSpeed);

                    break;
                case GestureType.GRAB_OPEN:

                    gestureGrabOpen.Show((int)xywhScreenCoord.x, (int)xywhScreenCoord.y, (int)xywhScreenCoord.z, (int)xywhScreenCoord.w, Camera.main.transform.rotation);
                                        
                    break;
                case GestureType.GRAB_CLOSE:

                    gestureGrabClose.Show((int)xywhScreenCoord.x, (int)xywhScreenCoord.y, (int)xywhScreenCoord.z, (int)xywhScreenCoord.w, Camera.main.transform.rotation);
                    
                    break;
                default: break;
            }

            
        }

        // Stops the timer if the gesture is not being detected
        if (currentGesture.type != GestureType.OPEN)
        {
            if (timerOpen.IsRunning())
            {
                timerOpen.Stop();
                timerOpen.ResetValues();
            }
        }

        // Stops the timer if the gesture is not being detected
        if (currentGesture.type != GestureType.OK)
        {
            if (timerOk.IsRunning())
            {
                timerOk.Stop();
                timerOk.ResetValues();
            }
        }
    }

    // Hide the 3D hands
    public void HideAllGestures()
    {
        gestureOpen.Hide();
        gestureOk.Hide();
        gestureNot.Hide();
        gesturePointing.Hide();
        gestureClose.Hide();
        gestureGrabOpen.Hide();
        gestureGrabClose.Hide();
    }

    // Initialize the objects in the scene
    public void InitMovables()
    {
        // Get the movable objects and add the Default component
        var movables = GameObject.FindGameObjectsWithTag("Movable");
        foreach (var obj in movables)
        {
            obj.AddComponent<Default>();
        }
        
    }

    // Reset the game. Put the objects in the initial position and remove the created ones
    public void Reset()
    {
        var movables = GameObject.FindGameObjectsWithTag("Movable");
        foreach (var obj in movables)
        {
            var defaultValues = obj.GetComponent<Default>();
            obj.transform.position = defaultValues.position;
            obj.transform.rotation = defaultValues.rotation;
        }

        var defaultCam = camera.GetComponent<Default>();
        camera.transform.position = defaultCam.position;
        camera.transform.rotation = defaultCam.rotation;

        
        var factory = GameObject.FindGameObjectsWithTag("Factory");
        foreach (var obj in factory)
        {
            Destroy(obj);
        }

        var cubeFactory = GameObject.Find("CubeFactory").GetComponent<ObjectFactory>();
        cubeFactory.Reset();

        var sphereFactory = GameObject.Find("SphereFactory").GetComponent<ObjectFactory>();
        sphereFactory.Reset();
    }

    // Change the alpha of the gestures
    public void ChangeGesturesAlphas(float alpha)
    {
        gestureOpen.transform.GetChild(0).GetComponent<AlphaChanger>().ChangeAlpha(alpha);
        gestureOk.transform.GetChild(0).GetComponent<AlphaChanger>().ChangeAlpha(alpha);
        gestureNot.transform.GetChild(0).GetComponent<AlphaChanger>().ChangeAlpha(alpha);
        gesturePointing.transform.GetChild(0).GetComponent<AlphaChanger>().ChangeAlpha(alpha);
        gestureClose.transform.GetChild(0).GetComponent<AlphaChanger>().ChangeAlpha(alpha);
        gestureGrabOpen.transform.GetChild(0).GetComponent<AlphaChanger>().ChangeAlpha(alpha);
        gestureGrabClose.transform.GetChild(0).GetComponent<AlphaChanger>().ChangeAlpha(alpha);
    }

    // Move the player to the front
    public void MoveForward()
    {
        Vector3 forward = camera.transform.TransformDirection(Vector3.forward);
        character.SimpleMove(forward * speed * Time.deltaTime);
    }

    // Move the player to the back
    public void MoveBackward()
    {
        Vector3 backward = camera.transform.TransformDirection(Vector3.back);
        character.SimpleMove(backward * speed * Time.deltaTime);
    }

    // Rotate the player
    public void Rotate(float speed)
    {        
        camera.transform.Rotate(Vector3.up * speed * Time.deltaTime);
        character.transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
    
    // For debugging
    void OnGUI()
    {

        GestureFrame currentGesture = phoneCamera.GetCurrentGesture();
        Vector4 xywhScreenCoord = phoneCamera.CamCoordinatesToScreenCoordinates(currentGesture.x, currentGesture.y, currentGesture.width, currentGesture.height);

        GUILayout.BeginArea(new Rect(500, 20, 250, 120));
        GUILayout.Label(message);
        GUILayout.EndArea();



    }
}
