using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjMarker : MonoBehaviour 
{
	public UISprite Marker;
	public UILabel Desc;
	public UILabel DescDupe;
	public UISprite MarkerLine;
	public UISprite ShieldFill;
	public UISprite HullFill;

	private float _targetWidth;
	private float _currentWidth;
	private bool _isLabelReadyToShow;
	private float _maxAlpha;
	private float _maxLineAlpha;
	
	// Update is called once per frame
	void Update () 
	{
		if(_currentWidth > _targetWidth * 0.99f)
		{
			return;
		}

		_currentWidth = Mathf.Lerp(_currentWidth, _targetWidth, Time.deltaTime * 12);
		Marker.width = Mathf.CeilToInt(_currentWidth);
		if(Marker.width > _targetWidth * 0.9f)
		{
			_isLabelReadyToShow = true;
		}
	}

	public void Initialize(float width, string desc)
	{
		_targetWidth = width;
		_currentWidth = _targetWidth * 0.2f;
		Marker.width = Mathf.CeilToInt(width * 0.2f);
		Desc.text = desc;
		Desc.alpha = 0;
		DescDupe.text = desc;
		DescDupe.alpha = 0;

		_isLabelReadyToShow = false;
		_maxAlpha = Marker.alpha;
		_maxLineAlpha = MarkerLine.alpha;
		MarkerLine.alpha = 0;
		MarkerLine.width = Desc.width + 50;
	}

	public void SetVisible(bool isVisible)
	{
		if(isVisible)
		{
			Marker.alpha = _maxAlpha;
			if(_isLabelReadyToShow)
			{
				Desc.alpha = 1f;
				DescDupe.alpha = 1f;
				MarkerLine.alpha = _maxLineAlpha;
			}
		}
		else
		{
			Marker.alpha = 0;
			Desc.alpha = 0;
			DescDupe.alpha = 1;
			MarkerLine.alpha = 0;
		}
	}

	public void SetShieldAndHull(float shieldPercent, float hullPercent)
	{
		if(ShieldFill != null && HullFill != null)
		{
			ShieldFill.width = (int)(10 + 35f * shieldPercent);
			HullFill.width = (int)(10 + 35f * hullPercent);
		}
	}
}


