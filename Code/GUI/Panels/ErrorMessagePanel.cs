using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorMessagePanel : PanelBase
{
	public UILabel ErrorMessageLabel;

	private float _fadeTimer;

	public override void Initialize ()
	{
		base.Initialize();
	}

	public override void PerFrameUpdate ()
	{
		if(_fadeTimer < 10)
		{
			if(_fadeTimer > 0.08f && _fadeTimer < 0.16f)
			{
				ErrorMessageLabel.alpha = 0;
			}
			else if(_fadeTimer > 0.16f && _fadeTimer < 3f)
			{
				ErrorMessageLabel.alpha = 1;
			}

			if(_fadeTimer > 3)
			{
				ErrorMessageLabel.alpha = Mathf.Lerp(ErrorMessageLabel.alpha, 0, Time.deltaTime * 2);
			}

			_fadeTimer += Time.deltaTime;
		}
	}

	public override void Show ()
	{
		base.Show();
		ErrorMessageLabel.alpha = 0;
		_fadeTimer = 10;
	}

	public override void Hide ()
	{
		base.Hide();

	}

	public void DisplayMessage(string text)
	{
		ErrorMessageLabel.text = text;
		ErrorMessageLabel.alpha = 1;
		_fadeTimer = 0;
	}
}
