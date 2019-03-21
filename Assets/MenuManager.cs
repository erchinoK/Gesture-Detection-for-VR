using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class manages the menu
public class MenuManager : MonoBehaviour {

    public Transform cameraTransform;                           // Camera position and orientation
    private bool showMenu = false;                              // Indicate if the menu should be displayed
    private Vector3 menuPosHide = new Vector3(0, 10, 0);        // The relative position to the camera so the menu is displayed
    private Vector3 menuPosShow = new Vector3(0, -0.25f, 3.0f); // The relative position to the camera so the menu is hidden
    public Vector3 step;   


    // Use this for initialization
    void Start () {

        transform.localPosition = menuPosHide;

        if (step == null)
            step = new Vector3(0.0f, 100f, 0.0f);

        if (step.Equals(new Vector3()))
            step = new Vector3(0.0f, 100f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {

        // Check if the menu should be displayed or not
        if (showMenu)
        {
            MoveMenu(menuPosShow, step * -1);
        } else
        {
            MoveMenu(menuPosHide, step);                
        }
    }

    // Show the menu
    public void ShowMenu()
    {
        showMenu = true;
    }

    // Hide the menu
    public void HideMenu()
    {
        showMenu = false;
    }

    // Indicates if it is showing
    public bool IsShowing()
    {
        return showMenu;
    }

    // Indicate if it is fully shown
    public bool IsFullyShown()
    {
        return (transform.localPosition == menuPosShow);
    }

    // Indicate if it is fully hidden
    public bool IsFullyHidden()
    {
        return (transform.localPosition == menuPosHide);
    }

    // Move the menu to the front of the camera
    private bool MoveMenu(Vector3 newLocalPos, Vector3 step)
    {
        //if (!AreVeryCloseY(transform.localPosition, newLocalPos))
        //{
        //    transform.localPosition += step * Time.deltaTime;
        //    return false;
        //}
        transform.localPosition = newLocalPos;
        return true;
        
    }
    /*
    private bool AreVeryCloseY(Vector3 v1, Vector3 v2)
    {
        //if (Vector3.Distance(v1, v2) < 10)
        if (v1.y - 0.6 < v2.y && v2.y < v1.y + 0.6)
        {
            return true;
        }

        return false;
    }*/


}
