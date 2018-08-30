using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager 
{
	public Material TradelaneLightRed;
	public Material TradelaneLightGreen;
	public Material TradelaneLightDown;

	public Material ShieldMaterial1;
	public Material ShieldMaterial2;
	public Material ShieldMaterial3;

	public void Initialize()
	{
		TradelaneLightRed = GameObject.Instantiate(Resources.Load("TradelaneLightRed")) as Material;
		TradelaneLightGreen = GameObject.Instantiate(Resources.Load("TradelaneLightGreen")) as Material;
		TradelaneLightDown = GameObject.Instantiate(Resources.Load("TradelaneLightDown")) as Material;

		ShieldMaterial1 = GameObject.Instantiate(Resources.Load("ShieldEffect1")) as Material;
		ShieldMaterial2 = GameObject.Instantiate(Resources.Load("ShieldEffect2")) as Material;
		ShieldMaterial3 = GameObject.Instantiate(Resources.Load("ShieldEffect3")) as Material;
	}

}
