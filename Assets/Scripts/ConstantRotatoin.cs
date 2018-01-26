using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotatoin : MonoBehaviour
{
    public float RotationSpeed = 5.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var euler = transform.rotation.eulerAngles;
	    euler.y += RotationSpeed * Time.deltaTime;
	    transform.rotation = Quaternion.Euler(euler);
	}
}
