using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the collision between the 3D hand (only the pinch gesture) and the menu
public class ColliderMenu : MonoBehaviour {

    // References to menu items
    public VRManager vrManager;
    public GameObject menuRestart;
    public GameObject menuCubeFactory;
    public GameObject menuSphereFactory;
    public GameObject menuWalk;

    // References to colliders
    private BoxCollider boxColliderGesture;
    private BoxCollider boxColliderMenuRestart;
    private BoxCollider boxColliderMenuCubeFactory;
    private BoxCollider boxColliderMenuSphereFactory;
    private BoxCollider boxColliderMenuWalk;

    // Factories (objects in the scene that create more objects)
    public ObjectFactory cubeFactory;
    public ObjectFactory sphereFactory;

    // Use this for initialization
    void Start () {

        // Initialize the box colliders of the 3D hand and the menus
        boxColliderGesture = GetComponent<BoxCollider>();
        boxColliderMenuRestart = menuRestart.GetComponent<BoxCollider>();
        boxColliderMenuCubeFactory = menuCubeFactory.GetComponent<BoxCollider>();
        boxColliderMenuSphereFactory = menuSphereFactory.GetComponent<BoxCollider>();
        boxColliderMenuWalk = menuWalk.GetComponent<BoxCollider>();
    }

	// Update is called once per frame
	void Update () {

        // Check if the menu is showing
        if (vrManager.menuManager.IsShowing())
        {

            // Check if the hand is touching this menu item
            if (boxColliderGesture.bounds.Intersects(boxColliderMenuRestart.bounds))
            {
                // Reset the game
                vrManager.Reset();

                // The menu changes color
                menuRestart.GetComponent<AlphaChanger>().ChangeAlpha(1.0f);
            }
            else      // The hand is not touching this menu
            {
                // The menu keep the default color
                menuRestart.GetComponent<AlphaChanger>().ChangeAlpha(0.5f);
            }

            // Check if the hand is touching this menu item
            if (boxColliderGesture.bounds.Intersects(boxColliderMenuCubeFactory.bounds))
            {

                // The menu changes the alpha
                menuCubeFactory.GetComponent<AlphaChanger>().ChangeAlpha(1.0f);

                // The factory starts creating objects
                cubeFactory.StartCreating();
            }
            else        // The hand is not touching this menu
            {
                
                // The menu changes the alpha
                menuCubeFactory.GetComponent<AlphaChanger>().ChangeAlpha(0.5f);

                // The factory stops
                cubeFactory.StopCreating();
            }

            // Check if the hand is touching this menu item
            if (boxColliderGesture.bounds.Intersects(boxColliderMenuSphereFactory.bounds))
            {

                // The menu changes the alpha
                menuSphereFactory.GetComponent<AlphaChanger>().ChangeAlpha(1.0f);

                // The factory starts
                sphereFactory.StartCreating();
            }
            else    // The hand is not touching this menu
            {
                // The menu changes the alpha
                menuSphereFactory.GetComponent<AlphaChanger>().ChangeAlpha(0.5f);

                // The factory stops
                sphereFactory.StopCreating();
            }


        }

        // Check if the hand is touching the button to walk
        if (boxColliderGesture.bounds.Intersects(boxColliderMenuWalk.bounds))
        {

            // The button changes colour
            menuWalk.GetComponent<AlphaChanger>().ChangeAlpha(0.8f);

            // Check if the menu is not showing
            if (!vrManager.menuManager.IsShowing())
            {
                // Move the player (walks)
                vrManager.MoveForward();
            }            
        }
        else        // The hand is not touching the button
        {

            // The button keeps the default alpha
            menuWalk.GetComponent<AlphaChanger>().ChangeAlpha(-1f);
        }
    }
    
    // For debugging
    void OnGUI()
    {

        GUILayout.BeginArea(new Rect(20, 500, 250, 120));
        //GUILayout.Label(message2);
        GUILayout.EndArea();

    }

}
