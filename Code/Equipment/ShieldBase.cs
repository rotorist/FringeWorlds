﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldBase : EquipmentBase
{
	public ShieldType Type;
	public ShieldTech Tech;
	public ShieldClass Class;
	public ShipBase ParentShip;
	public float TotalCapacity;
	public MeshRenderer MyRenderer;
	public float Amount;
	public float RechargeRate;
	public float RechargeDelay;


	public virtual void Initialize(InvItemData shieldItem)
	{
		if(shieldItem == null)
		{
			//no shield installed
			TotalCapacity = 0;
		}
		else
		{
			Type = (ShieldType)Enum.Parse(typeof(ShieldType), shieldItem.Item.GetStringAttribute("Shield Type"));
			Tech = (ShieldTech)Enum.Parse(typeof(ShieldTech), shieldItem.Item.GetStringAttribute("Shield Technology"));
			Class = (ShieldClass)Enum.Parse(typeof(ShieldClass), shieldItem.Item.GetStringAttribute("Shield Class"));
			TotalCapacity = shieldItem.Item.GetFloatAttribute("Capacity");
			RechargeRate = shieldItem.Item.GetFloatAttribute("Recharge Rate");
			RechargeDelay = shieldItem.Item.GetFloatAttribute("Recharge Delay");
			this.PowerRequired = shieldItem.Item.GetFloatAttribute("Power Required");
		}

		MyRenderer = this.GetComponent<MeshRenderer>();
		Material mat = null;

		//if playership then load separate materials
		//if NPC then use preloaded materials in material manager
		//this way playership material can be faded without affecting NPC's material

		if(ParentShip == GameManager.Inst.PlayerControl.PlayerShip)
		{
			if(Tech == ShieldTech.Gravity)
			{
				mat = GameObject.Instantiate(Resources.Load("ShieldEffect1")) as Material;

			}
			else if(Tech == ShieldTech.Plasma)
			{
				mat = GameObject.Instantiate(Resources.Load("ShieldEffect2")) as Material;
			}
			else
			{
				mat = GameObject.Instantiate(Resources.Load("ShieldEffect3")) as Material;
			}
			mat.SetFloat("_Brightness", 0);

		}
		else
		{
			if(Tech == ShieldTech.Gravity)
			{
				mat = GameManager.Inst.MaterialManager.ShieldMaterial1;

			}
			else if(Tech == ShieldTech.Plasma)
			{
				mat = GameManager.Inst.MaterialManager.ShieldMaterial2;
			}
			else
			{
				mat = GameManager.Inst.MaterialManager.ShieldMaterial3;
			}


		}

		MyRenderer.materials = new Material[]{mat, mat, mat};
	}

	public virtual Damage ProcessDamage(Damage damage)
	{

		return damage;
	}

	public virtual float GetShieldPercentage()
	{
		return 1;
	}

	public virtual int GetShieldHitEffectNumber()
	{
		switch(Tech)
		{
		case ShieldTech.Gravity: 
			return 1;
		case ShieldTech.Plasma:
			return 2;
		case ShieldTech.Magnetic:
			return 3;
		}

		return 1;
	}

	public void UpdateShieldFading(float fadeAlpha)
	{
		
		MyRenderer = this.GetComponent<MeshRenderer>();
		foreach(Material mat in MyRenderer.materials)
		{
			float brightness = 3 * fadeAlpha;
			mat.SetFloat("_Brightness", brightness);
		}
	}
}

public enum ShieldType
{
	Fighter,
	BigShip,
	Capitol,
}

public enum ShieldClass
{
	Fighter = 0, //fighter
	Transporter = 1, //bigship
	Explorer = 2, //fighter
	Cruiser = 3, //bigship
	CapitolShip = 4, //capitol
}

public enum ShieldTech
{
	
	Gravity,
	Plasma,
	Magnetic,
}