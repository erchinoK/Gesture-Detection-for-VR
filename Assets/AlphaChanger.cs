using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class allows changing the alpha colour (transparency) of an object
public class AlphaChanger : MonoBehaviour {

    public float alpha;             // current alpha
    private float defaultAlpha;     // default alpha
    private Color defaultColor;     // default colour

	// Use this for initialization
	void Start () {

        // Set the initial values as the defaults
        defaultAlpha = alpha;
        defaultColor = GetComponent<MeshRenderer>().material.color;

        // Set the new alpha specified from the editor
        GetComponent<MeshRenderer>().material.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultAlpha);
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    // Set a new alpha programatically
    public void ChangeAlpha(float newAlpha)
    {
        alpha = newAlpha;

        // If the new alpha is invalid then it sets the default
        if (newAlpha < 0)
            alpha = defaultAlpha;

        // Set the new alpha
        GetComponent<MeshRenderer>().material.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, alpha);
    }
}
