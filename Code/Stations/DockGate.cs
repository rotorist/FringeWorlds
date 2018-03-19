using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockGate : MonoBehaviour 
{
	public bool IsOpen = false;
	public bool IsDone = true;
	public StationBase ParentStation;
	public BoxCollider DockingTrigger;
	public MeshRenderer LightRed;
	public MeshRenderer LightGreen;




	public void Open()
	{
		IsDone = false;
		IsOpen = true;
	}

	public void Close()
	{
		IsDone = false;
		IsOpen = false;
	}
	
	public void OnOpenDone()
	{

	}

	public void OnCloseDone()
	{

	}

	public void SetRedLight()
	{
		LightRed.enabled = true;
		LightGreen.enabled = false;
	}

	public void SetGreenLight()
	{
		LightRed.enabled = false;
		LightGreen.enabled = true;
	}
}


