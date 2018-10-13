using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingPanel : PanelBase
{
	public List<UIButton> KeyBindingButtons;
	public List<UserInputs> KeyBindingButtonIndexes;
	public GameObject SetKeyWindow;
	public UILabel KeyBindingName;

	private UserInputs _pendingInput;

	public override void Initialize ()
	{
		base.Initialize();
		NGUITools.SetActive(SetKeyWindow, false);
		_pendingInput = UserInputs.None;
	}

	public override void PerFrameUpdate ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			KeyInput input = new KeyInput();
			OnKeyBindingSet(input);
		}
	}

	public override void Hide ()
	{
		base.Hide();
	}

	public override void Show ()
	{
		base.Show();
		RefreshKeyBindings();

	}

	public void OnKeyBindingSelect()
	{
		UIButton currentButton = UIButton.current;
		for(int i=0; i<KeyBindingButtons.Count; i++)
		{
			if(currentButton == KeyBindingButtons[i])
			{
				_pendingInput = KeyBindingButtonIndexes[i];
				NGUITools.SetActive(SetKeyWindow, true);
				SetKeyWindow.GetComponent<VerticalExpandWindow>().Show();
			}
		}
	}

	public void OnKeyBindingSet(KeyInput input)
	{
		
		GameManager.Inst.PlayerControl.KeyBinding.Controls[_pendingInput] = input;
		SetKeyWindow.GetComponent<VerticalExpandWindow>().Hide();
		_pendingInput = UserInputs.None;

		RefreshKeyBindings();
	}



	private void RefreshKeyBindings()
	{
		for(int i=0; i<KeyBindingButtons.Count; i++)
		{
			UILabel buttonText = KeyBindingButtons[i].GetComponent<UILabel>();
			KeyInput input = GameManager.Inst.PlayerControl.KeyBinding.Controls[KeyBindingButtonIndexes[i]];
			if(input.IsFnSet)
			{
				buttonText.text = input.FnKey.ToString() + "+" + input.Key.ToString();
			}
			else
			{
				buttonText.text = input.Key.ToString();
			}
		}
	}
}
