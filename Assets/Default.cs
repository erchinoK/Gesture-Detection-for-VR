using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class saves the initial position and rotation of an object in the scene
public class Default : MonoBehaviour {

    public Vector3 position;
    public Quaternion rotation;

	// Use this for initialization
	void Start () {
        position = transform.position;
        rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
