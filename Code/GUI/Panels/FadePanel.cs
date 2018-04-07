using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanel : PanelBase
{
	public UISprite BlackBG;

	private float _fadeSpeed;
	private bool _isFadingOut;


	public override void Initialize ()
	{
		_fadeSpeed = 0;
		_isFadingOut = false;
	}

	public override void PerFrameUpdate ()
	{
		UpdateFadeInFadeOut();
	}

	public override void Show ()
	{
		base.Show();
	}

	public override void Hide ()
	{
		base.Hide();
	}

	public void FadeIn(float speed)
	{
		_fadeSpeed = speed;
		_isFadingOut = false;
	}

	public void FadeOut(float speed)
	{
		_fadeSpeed = speed;
		_isFadingOut = true;
	}

	public void SetBGAlpha(float alpha)
	{
		BlackBG.alpha = alpha;
	}


	private void UpdateFadeInFadeOut()
	{
		if(_isFadingOut)
		{
			BlackBG.alpha = Mathf.Clamp01(BlackBG.alpha + Time.deltaTime * _fadeSpeed);
			if(BlackBG.alpha >= 1)
			{
				UIEventHandler.Instance.TriggerFadeOutDone();
			}
		}
		else
		{
			BlackBG.alpha = Mathf.Clamp01(BlackBG.alpha - Time.deltaTime * _fadeSpeed);
			if(BlackBG.alpha <= 0)
			{
				UIEventHandler.Instance.TriggerFadeInDone();
			}
		}
	}


}
