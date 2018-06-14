using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour 
{

	public string DisplayName;
	public string ID;

	public Vector3 OriginalScale;
	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround(transform.position, transform.up, Time.deltaTime * 0.25f);

		//shrink when far from camera
		float camDist = Vector3.Distance(transform.position, Camera.main.transform.position);
		if(camDist > 1000)
		{
			transform.localScale = Vector3.Lerp(OriginalScale, OriginalScale * 0.1f, Mathf.Clamp01((camDist - 1000f) / 4000f));
		}
		else
		{
			transform.localScale = OriginalScale;
		}
	}


}

public class PlanetData
{
	public string DisplayName;
	public string ID;
	public RelLoc Location;
	public Vector3 OriginalScale;
}