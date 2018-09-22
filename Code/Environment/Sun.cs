using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour 
{
	public Light Sunlight;
	public string ID;
	public string DisplayName;
	public LensFlare Flare;

	
	// Update is called once per frame
	void Update () {
		
	}
}

public class SunData
{
	public RelLoc Location;
	public Vector3 Scale;
	public string ID;
	public string DisplayName;
	public float Intensity;
	public Color Color;
}
