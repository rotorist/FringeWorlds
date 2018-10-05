using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanel : PanelBase
{
	public UISprite BlackBG;
	public UISprite WhiteBG;

	private float _fadeSpeed;
	private bool _isFadingOut;
	private bool _isWhite;

	public override void Initialize ()
	{
		base.Initialize();
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
		_isWhite = false;
	}

	public void FadeOut(float speed)
	{
		
		_fadeSpeed = speed;
		_isFadingOut = true;
		_isWhite = false;
	}

	public void WhiteFadeIn(float speed)
	{
		_fadeSpeed = speed;
		_isFadingOut = false;
		_isWhite = true;
	}

	public void WhiteFadeOut(float speed)
	{
		_fadeSpeed = speed;
		_isFadingOut = true;
		_isWhite = true;
	}

	public void SetBlackBGAlpha(float alpha)
	{
		BlackBG.alpha = alpha;
	}

	public void SetWhiteBGAlpha(float alpha)
	{
		WhiteBG.alpha = alpha;
	}


	private void UpdateFadeInFadeOut()
	{
		if(_isFadingOut)
		{
			if(_isWhite)
			{
				WhiteBG.alpha = Mathf.Clamp01(WhiteBG.alpha + Time.deltaTime * _fadeSpeed);
				if(WhiteBG.alpha >= 1)
				{
					UIEventHandler.Instance.TriggerWhiteFadeOutDone();
				}
			}
			else
			{
				BlackBG.alpha = Mathf.Clamp01(BlackBG.alpha + Time.deltaTime * _fadeSpeed);
				if(BlackBG.alpha >= 1)
				{
					UIEventHandler.Instance.TriggerFadeOutDone();
				}
			}
		}
		else
		{
			if(_isWhite)
			{
				WhiteBG.alpha = Mathf.Clamp01(WhiteBG.alpha - Time.deltaTime * _fadeSpeed);
				if(WhiteBG.alpha <= 0)
				{
					UIEventHandler.Instance.TriggerWhiteFadeInDone();
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


}
