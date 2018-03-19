using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareDockGate: DockGate
{
	public Transform Top;
	public Transform Bottom;

	private Quaternion _topCloseRot;
	private Quaternion _bottomCloseRot;

	private Vector3 _topAxis;
	private Vector3 _bottomAxis;

	void Start()
	{
		_topCloseRot = Top.rotation;
		_bottomCloseRot = Bottom.rotation;
		_topAxis = Top.right;
		_bottomAxis = Bottom.right * -1f;

		SetRedLight();
	}

	// Update is called once per frame
	void Update () 
	{
		if(IsOpen)
		{
			float topAngle = Quaternion.Angle(Top.rotation, _topCloseRot);
			if(topAngle <= 80)
			{
				Top.RotateAround(Top.position, _topAxis, 15 * Time.deltaTime);
			}
			else
			{
				IsDone = true;
			}

			float bottomAngle = Quaternion.Angle(Bottom.rotation, _bottomCloseRot);
			if(bottomAngle <= 80)
			{
				Bottom.RotateAround(Bottom.position, _bottomAxis, 15 * Time.deltaTime);
			}

		}
		else
		{
			float topAngle = Quaternion.Angle(Top.rotation, _topCloseRot);
			if(topAngle >= 2)
			{
				Top.RotateAround(Top.position, _topAxis, -15 * Time.deltaTime);
			}
			else
			{
				IsDone = true;
			}

			float bottomAngle = Quaternion.Angle(Bottom.rotation, _bottomCloseRot);
			if(bottomAngle >=2)
			{
				Bottom.RotateAround(Bottom.position, _bottomAxis, -15 * Time.deltaTime);
			}

		}
	}
}

