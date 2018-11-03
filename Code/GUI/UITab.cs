using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITab : MonoBehaviour 
{
	public string Name;
	public UIButton Button;
	public UISprite Background;
	public UILabel Label;

	public UITabSelection ParentSelection;

	public void OnSelect()
	{
		Background.color = new Color(1, 1, 1);
		Button.hover = new Color(1, 1, 1);
		Button.pressed = new Color(1, 1, 1);
		Button.defaultColor = new Color(1, 1, 1);
		Label.color = new Color(0.517f, 0.75f, 0.75f);
		ParentSelection.OnTabSelect(this);
	}

	public void OnDeselect()
	{
		Background.color = new Color(0.6f, 0.6f, 0.6f);
		Button.hover = new Color(0.6f, 0.6f, 0.6f);
		Button.pressed = new Color(0.6f, 0.6f, 0.6f);
		Button.defaultColor = new Color(0.6f, 0.6f, 0.6f);
		Label.color = new Color(0.384f, 0.57f, 0.57f);
	}

}
