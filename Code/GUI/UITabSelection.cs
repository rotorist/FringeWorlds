using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITabSelection : MonoBehaviour 
{
	public PanelBase ParentPanel;
	public List<UITab> Tabs;

	public void OnTabSelect(UITab selectedTab)
	{
		selectedTab.Background.depth = 12;
		int step = 1;
		int currentDepth = 1;
		foreach(UITab tab in Tabs)
		{
			if(tab == selectedTab)
			{
				step = -1;
				currentDepth = 10;
			}
			else
			{
				tab.Background.depth = currentDepth + step;
				currentDepth = tab.Background.depth;
				tab.OnDeselectTab();
			}
		}

		ParentPanel.OnTabSelect(selectedTab.Name);
	}

	public void ForceSelectTab(string tabName)
	{
		foreach(UITab tab in Tabs)
		{
			if(tab.Name == tabName)
			{
				tab.OnSelectTab();
			}
		}
	}
}
