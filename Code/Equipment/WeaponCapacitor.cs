using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCapacitor : EquipmentBase
{
	public float Capacity;
	public float Amount;
	public float RechargeRate;

	private bool IsEnabled;

	public void Initialize(InvItemData weaponCapacitorItem)
	{
		if(weaponCapacitorItem == null)
		{
			Capacity = 0;
			Amount = 0;
			RechargeRate = 0;
		}
		else
		{
			Capacity = weaponCapacitorItem.Item.GetFloatAttribute("Capacity");
			Amount = Capacity;
			RechargeRate = weaponCapacitorItem.Item.GetFloatAttribute("Recharge Rate");
			this.PowerRequired = weaponCapacitorItem.Item.GetFloatAttribute("Power Required");
		}

		IsEnabled = true;
	}

	public void Consume(float amount)
	{
		Amount -= amount;
	}

	public bool IsPowerAvailable(float consumption)
	{
		if(IsEnabled && Amount >= consumption)
		{
			return true;
		}
		else
		{
			return false;
		}
	}


	void Update()
	{
		if(Amount < Capacity)
		{
			Amount = Mathf.Clamp(Amount + RechargeRate * Time.deltaTime, 0, Capacity);
		}
		if(Amount <= Capacity * 0.02f)
		{
			IsEnabled = false;
		}
		else if(Amount > Capacity * 0.1f)
		{
			IsEnabled = true;
		}
	}


}
