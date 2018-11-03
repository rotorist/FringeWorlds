using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBase : EquipmentBase
{
	public ShieldType Type;
	public ShieldTech Tech;
	public ShipBase ParentShip;
	public float TotalCapacity;
	public MeshRenderer MyRenderer;
	public float Amount;
	public float RechargeRate;
	public float RechargeDelay;

	public virtual void Initialize()
	{
		
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
	F1 = 0,
	F2 = 1,
	F3 = 2,
	F4 = 3,
	F5 = 4,
	T1 = 5,
	T2 = 6,
	T3 = 7,
	C1 = 8,
	C2 = 9,
	C3 = 10,
}

public enum ShieldTech
{
	
	Gravity,
	Plasma,
	Magnetic,
}