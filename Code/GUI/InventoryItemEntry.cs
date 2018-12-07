using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemEntry : MonoBehaviour 
{
	public InvItemData ItemData;
	public UILabel HintText;
	public UILabel QuantityText;
	public InventoryView ParentView;
	public UILabel ItemText;
	public Transform IconAnchor;
	public UISprite HighlightBG;
	public UISprite Icon;
	public float IconSize;
	public UIButton MyButton;
	public UIButton LoadButton;
	public UISprite LoadButtonRootSprite;
	public UISprite LoadButtonIcon;
	public int InventoryIndex;



	public void OnLoadButtonClick()
	{
		ParentView.OnLoadButtonClick(this);
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

	public void RefreshLoadButton()
	{
		//check if item is ammo and if it can be loaded into the selected item
		//if same ammo type then show Load button
		//if exactly same item then disable but show load button without icon
		if(LoadButton != null)
		{
			if(ItemData.Item.Type == ItemType.Ammo && ParentView.RelatedItem != null)
			{
				string myAmmoType = ItemData.Item.GetStringAttribute("Ammo Type");
				string otherAmmoType = ParentView.RelatedItem.Item.GetStringAttribute("Ammo Type");
				if(ItemData.Item.ID == ParentView.RelatedItem.RelatedItemID)
				{
					LoadButton.isEnabled = false;
					LoadButtonRootSprite.alpha = 1;
					LoadButtonIcon.alpha = 0;
				}
				else if(myAmmoType == otherAmmoType)
				{
					LoadButton.isEnabled = true;
					LoadButtonRootSprite.alpha = 1;
					LoadButtonIcon.alpha = 1;
				}
				else
				{
					LoadButton.isEnabled = false;
					LoadButtonRootSprite.alpha = 0;
				}
			}
			else
			{
				LoadButton.isEnabled = false;
				LoadButtonRootSprite.alpha = 0;
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
