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

	void OnPress(bool isPress)
	{
		if(isPress)
		{
			OnSelectTab();
		}
	}

	public void OnSelectTab()
	{
		Color bgColor = new Color(1, 1, 1);
		Color textColor = new Color(1, 1, 1);
		if(ParentSelection.ColorScheme == UIColorScheme.Teal)
		{
			bgColor = new Color(100f/255f, 242f/255f, 242/255f, 1f);
			textColor = new Color(132f/255f, 191f/255f, 191f/255f, 1f);
		}
		else if(ParentSelection.ColorScheme == UIColorScheme.Blue)
		{
			bgColor = new Color(148f/255f, 189f/255f, 255/255f, 1f);
			textColor = new Color(158f/255f, 202f/255f, 244f/255f, 1f);
		}

		Background.color = bgColor;
		Button.hover = Background.color;
		Button.pressed = Background.color;
		Button.defaultColor = Background.color;
		Label.color = textColor;
		ParentSelection.OnTabSelect(this);
	}

	public void OnDeselectTab()
	{
		Color bgColor = new Color(1, 1, 1);
		Color textColor = new Color(1, 1, 1);
		if(ParentSelection.ColorScheme == UIColorScheme.Teal)
		{
			bgColor = new Color(75f/255f, 181f/255f, 181/255f, 1f);
			textColor = new Color(132f/255f, 191f/255f, 191f/255f, 0.75f);
		}
		else if(ParentSelection.ColorScheme == UIColorScheme.Blue)
		{
			bgColor = new Color(85f/255f, 130f/255f, 170/255f, 1f);
			textColor = new Color(158f/255f, 202f/255f, 244f/255f, 0.75f);
		}

		Background.color = bgColor;
		Button.hover = Background.color;
		Button.pressed = Background.color;
		Button.defaultColor = Background.color;
		Label.color = textColor;
	}

}

