using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerVector3
{
	public float x;
	public float y;
	public float z;

	public SerVector3(Vector3 input)
	{
		SetFromVector3(input);
	}

	public void SetFromVector3(Vector3 input)
	{
		x = input.x;
		y = input.y;
		z = input.z;
	}

	public Vector3 ConvertToVector3()
	{
		Vector3 output = new Vector3(x, y, z);
		return output;
	}
}
