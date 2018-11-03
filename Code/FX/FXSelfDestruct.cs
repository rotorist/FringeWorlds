using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSelfDestruct : MonoBehaviour 
{
	public bool IsTTLEnabled;
	public float TTL;

	private float _timer;

	// Update is called once per frame
	void Update () 
	{
		if(IsTTLEnabled)
		{
			if(_timer > TTL)
			{
				GameObject.Destroy(this.gameObject);
			}
			_timer += Time.deltaTime;
		}


	}
}
