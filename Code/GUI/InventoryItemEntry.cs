using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemEntry : MonoBehaviour 
{
	public InvItemData ItemData;

	public InventoryView ParentView;
	public UILabel ItemText;
	public Transform IconAnchor;
	public UISprite HighlightBG;
	public UISprite Icon;
	public float IconSize;
	public UIButton MyButton;

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

	public void SetItemText(string text)
	{
		ItemText.text = text;
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
