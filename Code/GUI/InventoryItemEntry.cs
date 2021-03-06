﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemEntry : MonoBehaviour 
{
	public InvItemData ItemData;
	public UILabel HintText;
	public UILabel QuantityText;
	public UILabel PriceText;
	public float Price;
	public InventoryView ParentView;
	public UILabel ItemText;
	public Transform IconAnchor;
	public UISprite HighlightBG;
	public UISprite Icon;
	public float IconSize;
	public UIButton MyButton;
	public UIButton SecButton;
	public UISprite SecButtonRootSprite;
	public UISprite SecButtonIcon;
	public int InventoryIndex;



	public void OnSecButtonClick()
	{
		ParentView.OnSecButtonClick(this);
	}

	public void OnEntryClick()
	{
		ParentView.OnUserClickEntry(this);
	}

	public void OnSelect()
	{
		MyButton.isEnabled = false;

	}

	public void OnDeselect()
	{
		MyButton.isEnabled = true;

	}

	public void SetHintText(string text)
	{
		HintText.text = text;
	}

	public void SetItemText(string text)
	{
		if(text == "")
		{
			if(HintText != null)
			{
				HintText.alpha = 0.4f;
			}
		}
		else
		{
			if(HintText != null)
			{
				HintText.alpha = 0;
			}
		}

		ItemText.text = text;
	}

	public void SetItemQuantity(int quantity)
	{
		if(quantity > 1)
		{
			QuantityText.text = quantity.ToString();
		}
		else
		{
			QuantityText.text = "";
		}
	}

	public void SetItemPrice(float price)
	{
		Price = price;
		PriceText.text = "$" + Mathf.CeilToInt(price).ToString();
		PriceText.alpha = 1;
	}

	public void RefreshLoadButton()
	{
		//check if item is ammo and if it can be loaded into the selected item
		//if same ammo type then show Load button
		//if exactly same item then disable but show load button without icon
		if(SecButton != null)
		{
			if(ItemData.Item.Type == ItemType.Ammo && ParentView.RelatedItem != null)
			{
				string myAmmoType = ItemData.Item.GetStringAttribute("Ammo Type");
				string otherAmmoType = ParentView.RelatedItem.Item.GetStringAttribute("Ammo Type");
				if(ItemData.Item.ID == ParentView.RelatedItem.RelatedItemID)
				{
					SecButton.isEnabled = false;
					SecButtonRootSprite.alpha = 1;
					SecButtonIcon.alpha = 0;
				}
				else if(myAmmoType == otherAmmoType)
				{
					SecButton.isEnabled = true;
					SecButtonRootSprite.alpha = 1;
					SecButtonIcon.alpha = 1;
				}
				else
				{
					SecButton.isEnabled = false;
					SecButtonRootSprite.alpha = 0;
				}
			}
			else if(ItemData.Item.Type == ItemType.HangarItem)
			{
				if(ItemData.Item.Description == GameManager.Inst.PlayerProgress.ActiveLoadout.LoadoutID)
				{
					SecButton.isEnabled = false;
					SecButtonRootSprite.alpha = 1;
					SecButtonIcon.alpha = 0;
				}
				else
				{
					SecButton.isEnabled = true;
					SecButtonRootSprite.alpha = 1;
					SecButtonIcon.alpha = 1;
				}
			}
			else
			{
				SecButton.isEnabled = false;
				SecButtonRootSprite.alpha = 0;
			}
		}
	}

	public void SetIcon(UISprite icon)
	{
		icon = icon;
		icon.transform.parent = IconAnchor.transform;
		icon.MakePixelPerfect();
		icon.width = (int)IconSize;
		icon.transform.localPosition = Vector3.zero;
		icon.depth = 5;
	}
}
