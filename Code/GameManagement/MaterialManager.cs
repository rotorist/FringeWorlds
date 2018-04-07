using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager 
{
	public Material TradelaneLightRed;
	public Material TradelaneLightGreen;
	public Material TradelaneLightDown;

	public void Initialize()
	{
		TradelaneLightRed = GameObject.Instantiate(Resources.Load("TradelaneLightRed")) as Material;
		TradelaneLightGreen = GameObject.Instantiate(Resources.Load("TradelaneLightGreen")) as Material;
		TradelaneLightDown = GameObject.Instantiate(Resources.Load("TradelaneLightDown")) as Material;
	}

}
