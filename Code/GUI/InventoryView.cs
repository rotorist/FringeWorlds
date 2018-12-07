using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour 
{
	public UISprite BackdropSprite;
	public float TopY;
	public float LeftX;
	public float EntryYSize;

	public InvItemData RelatedItem;

	public PanelBase SelectedItemHandler;

	public List<InventoryItemEntry> ItemEntries;

	private List<InvItemData> _inventory;


	public virtual void Initialize(List<InvItemData> inventory)
	{
		Debug.Log("initializing inventory view");
		_inventory = inventory;
		if(ItemEntries == null)
		{
			ItemEntries = new List<InventoryItemEntry>();
		}
		Refresh();
	}

	public virtual void Refresh()
	{
		if(_inventory == null)
		{
			return;
		}

		//destroy all existing entries
		foreach(InventoryItemEntry entry in ItemEntries)
		{
			GameObject.Destroy(entry.gameObject);
		}
		ItemEntries.Clear();

		//add entries
		int i = 0;
		foreach(InvItemData itemData in _inventory)
		{
			
			GameObject entryGO = GameObject.Instantiate(Resources.Load("InventoryItemEntry")) as GameObject;
			entryGO.transform.parent = BackdropSprite.transform;
			entryGO.transform.localScale = new Vector3(1, 1, 1);
			entryGO.transform.localEulerAngles = Vector3.zero;
			entryGO.transform.localPosition = new Vector3(LeftX, TopY - EntryYSize * i, 0);

			InventoryItemEntry entry = entryGO.GetComponent<InventoryItemEntry>();
			entry.ParentView = this;
			entry.OnDeselect();
			entry.ItemData = itemData;
			entry.SetItemText(itemData.Item.DisplayName);
			ItemEntries.Add(entry);

			i ++;
		}
	}

	public void RefreshLoadButtons()
	{
		foreach(InventoryItemEntry entry in ItemEntries)
		{
			entry.RefreshLoadButton();
		}
	}

	public void OnLoadButtonClick(InventoryItemEntry clickedEntry)
	{

	}

	public void OnUserClickEntry(InventoryItemEntry clickedEntry)
	{
		SelectedItemHandler.OnItemSelect(clickedEntry, this);

		clickedEntry.OnSelect();

	}

	public void DeselectAll()
	{
		foreach(InventoryItemEntry entry in ItemEntries)
		{
			entry.OnDeselect();
		}
	}

}
