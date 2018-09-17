using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDListEntry : MonoBehaviour 
{
	public UISprite SeparatorLine;
	public UILabel Description;
	public UILabel Distance;


	public void SetAlpha(float alpha)
	{
		//HorLine.alpha = alpha;
		//VerLine.alpha = alpha;
		Description.alpha = alpha;
		Distance.alpha = alpha;
		SeparatorLine.alpha = alpha;
	}

	public void SetColor(Color color)
	{
		Description.color = color;
	}

	public void SetDistance(float distance)
	{
		
		if(distance >= 1000f)
		{
			int distInt = Mathf.CeilToInt(distance/100f);
			Distance.text = (distInt/10f).ToString() + "K";
		}
		else
		{
			int distInt = Mathf.CeilToInt(distance);
			Distance.text = distInt.ToString();
		}
	}

	public void SetDescription(string desc)
	{
		Description.text = desc;
	}
}
