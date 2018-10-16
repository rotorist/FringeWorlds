using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingPanel : PanelBase
{
	public List<UIButton> KeyBindingButtons;
	public List<UserInputs> KeyBindingButtonIndexes;
	public GameObject SetKeyWindow;
	public UILabel KeyBindingName;
	public UIButton SaveButton;
	public UIButton RestoreButton;

	private UserInputs _pendingInput;

	public override void Initialize ()
	{
		base.Initialize();
		NGUITools.SetActive(SetKeyWindow, false);
		_pendingInput = UserInputs.None;
		SaveButton.isEnabled = true;
		RestoreButton.isEnabled = true;
	}

	public override void PerFrameUpdate ()
	{
		
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
				KeyBindingName.text = _pendingInput.ToString();
				NGUITools.SetActive(SetKeyWindow, true);
				SetKeyWindow.GetComponent<VerticalExpandWindow>().Show();
				InputEventHandler.Instance.InputState = InputState.KeyBindingEnter;
				foreach(UIButton button in KeyBindingButtons)
				{
					Collider collider = button.GetComponent<Collider>();
					collider.enabled = false;
				}
				SaveButton.isEnabled = false;
				RestoreButton.isEnabled = false;
				return;
			}
		}
	}

	public void OnKeyBindingSet(KeyInput input)
	{
		if(input.Key != KeyCode.None)
		{
			GameManager.Inst.PlayerControl.KeyBinding.Controls[_pendingInput] = input;
		}

		SetKeyWindow.GetComponent<VerticalExpandWindow>().Hide();
		_pendingInput = UserInputs.None;
		InputEventHandler.Instance.InputState = InputState.UI;
		RefreshKeyBindings();
		foreach(UIButton button in KeyBindingButtons)
		{
			Collider collider = button.GetComponent<Collider>();
			collider.enabled = true;
		}
		SaveButton.isEnabled = true;
		RestoreButton.isEnabled = true;
		return;
	}

	public void OnKeyBindingSave()
	{
		GameManager.Inst.DBManager.UserPrefDataHandler.SaveKeyBindings(GameManager.Inst.PlayerControl.KeyBinding.Controls);
		UIEventHandler.Instance.TriggerCloseKeyBindingPanel();
		Time.timeScale = 1;
		GameManager.Inst.UIManager.HUDPanel.OnUnpauseGame();
	}

	public void OnRestoreDefaultBindings()
	{
		GameManager.Inst.PlayerControl.KeyBinding.Controls = GameManager.Inst.DBManager.UserPrefDataHandler.GetKeyBindings(true);
		RefreshKeyBindings();
	}


	private void RefreshKeyBindings()
	{
		for(int i=0; i<KeyBindingButtons.Count; i++)
		{
			UILabel buttonText = KeyBindingButtons[i].GetComponent<UILabel>();
			KeyInput input = GameManager.Inst.PlayerControl.KeyBinding.Controls[KeyBindingButtonIndexes[i]];
			if(input.FnKey != KeyCode.None)
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
