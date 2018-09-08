using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveIndicator : MonoBehaviour 
{
	public UISprite Background;
	public UISprite Indicator;
	public float Ymax;
	public float Ymin;
	public float Radius;
	public float XOffset;

	public void SetValue(float value)
	{
		float originalValue = value;
		//normalize value to between 0.3 and 0.7
		value = 0.5f + (value - 0.5f) * 0.4f;
		float y = Ymin + originalValue * (Ymax - Ymin);
		float x = XOffset + (Mathf.Sqrt(1f - Mathf.Pow((0.5f - value) / 0.5f, 2)) * Radius);
		Indicator.transform.localPosition = new Vector3(x, y, 0);
	}

}
