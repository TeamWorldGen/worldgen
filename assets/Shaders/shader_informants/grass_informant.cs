using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class grass_informant : MonoBehaviour {
	private float originy;

	// Use this for initialization
	void Start () {
		originy = transform.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<Renderer>().material.SetFloat("originy", originy);

		
	}
}
