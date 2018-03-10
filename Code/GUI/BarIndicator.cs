using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarIndicator : MonoBehaviour 
{
	public UISprite Sprite;
	public bool IsInstant;
	public int MaxSize;
	public int MinSize;

	private float _targetSize;
	private float _currentSize;

	void Start()
	{
		_currentSize = MaxSize * 1f;
		_targetSize = _currentSize;
	}

	// Update is called once per frame
	void Update () 
	{
		if(!IsInstant)
		{
			if(_currentSize < _targetSize)
			{
				_currentSize = Mathf.Clamp(_currentSize + Time.deltaTime * 10, 0, _targetSize);
			}
			else
			{
				_currentSize = Mathf.Clamp(_currentSize - Time.deltaTime * 10, 0, _targetSize);
			}
			//Debug.Log(_currentSize);
			Sprite.width = Mathf.FloorToInt(_currentSize);
			if(_currentSize < MinSize)
			{
				Sprite.alpha = 0;
			}
			else
			{
				Sprite.alpha = 1;
			}
		}
	}


	public void SetFillPercentage(float percentage)
	{
		_targetSize = 1f * MaxSize * percentage;

		if(IsInstant)
		{
			float width = _targetSize;
			Sprite.width = Mathf.FloorToInt(width);
			if(width < MinSize)
			{
				Sprite.alpha = 0;
			}
			else
			{
				Sprite.alpha = 1;
			}
		}
	}


}
