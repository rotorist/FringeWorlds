using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundDockGate: DockGate
{
	public Transform TopLeft;
	public Transform TopRight;
	public Transform BottomLeft;
	public Transform BottomRight;

	private Quaternion _topLeftCloseRot;
	private Quaternion _topRightCloseRot;
	private Quaternion _bottomLeftCloseRot;
	private Quaternion _bottomRightCloseRot;

	private Vector3 _topLeftAxis;
	private Vector3 _topRightAxis;
	private Vector3 _bottomLeftAxis;
	private Vector3 _bottomRightAxis;

	void Start()
	{
		_topLeftCloseRot = TopLeft.rotation;
		_topRightCloseRot = TopRight.rotation;
		_bottomLeftCloseRot = BottomLeft.rotation;
		_bottomRightCloseRot = BottomRight.rotation;

		_topLeftAxis = TopLeft.right - TopLeft.up;
		_topRightAxis = TopRight.right + TopRight.up;
		_bottomLeftAxis = (BottomLeft.right + BottomLeft.up) * -1f;
		_bottomRightAxis = (BottomRight.right - BottomRight.up) * -1f;

		IsOpen = true;
	}

	// Update is called once per frame
	void Update () 
	{
		if(IsOpen)
		{
			float topLeftAngle = Quaternion.Angle(TopLeft.rotation, _topLeftCloseRot);
			if(topLeftAngle <= 70)
			{
				TopLeft.RotateAround(TopLeft.position, _topLeftAxis, 10 * Time.deltaTime);
			}
			else
			{
				IsDone = true;
			}

			float topRightAngle = Quaternion.Angle(TopRight.rotation, _topRightCloseRot);
			if(topRightAngle <= 70)
			{
				TopRight.RotateAround(TopRight.position, _topRightAxis, 10 * Time.deltaTime);
			}

			float bottomLeftAngle = Quaternion.Angle(BottomLeft.rotation, _bottomLeftCloseRot);
			if(bottomLeftAngle <= 70)
			{
				BottomLeft.RotateAround(BottomLeft.position, _bottomLeftAxis, 10 * Time.deltaTime);
			}

			float bottomRightAngle = Quaternion.Angle(BottomRight.rotation, _bottomRightCloseRot);
			if(bottomRightAngle <= 70)
			{
				BottomRight.RotateAround(BottomRight.position, _bottomRightAxis, 10 * Time.deltaTime);
			}
		}
		else
		{
			float topLeftAngle = Quaternion.Angle(TopLeft.rotation, _topLeftCloseRot);
			if(topLeftAngle >= 0.1f)
			{
				TopLeft.RotateAround(TopLeft.position, _topLeftAxis, -10 * Time.deltaTime);
			}
			else
			{
				IsDone = true;
			}

			float topRightAngle = Quaternion.Angle(TopRight.rotation, _topRightCloseRot);
			if(topRightAngle >= 0.1f)
			{
				TopRight.RotateAround(TopRight.position, _topRightAxis, -10 * Time.deltaTime);
			}

			float bottomLeftAngle = Quaternion.Angle(BottomLeft.rotation, _bottomLeftCloseRot);
			if(bottomLeftAngle >= 0.1f)
			{
				BottomLeft.RotateAround(BottomLeft.position, _bottomLeftAxis, -10 * Time.deltaTime);
			}

			float bottomRightAngle = Quaternion.Angle(BottomRight.rotation, _bottomRightCloseRot);
			if(bottomRightAngle >= 0.1f)
			{
				BottomRight.RotateAround(BottomRight.position, _bottomRightAxis, -10 * Time.deltaTime);
			}
		}



	}
}