using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalExpandWindow : MonoBehaviour 
{
	public UISprite Header;
	public UISprite Footer;
	public UISprite Pane;
	public List<GameObject> Children;
	public float LowHeight;
	public float FullHeight;

	private float _paneYScale;
	private bool _isShown;
	
	// Update is called once per frame
	void Update () 
	{
		if(_isShown)
		{
			if(_paneYScale < 1)
			{
				_paneYScale += Time.unscaledDeltaTime * 7;
				Header.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y + LowHeight + (FullHeight - LowHeight) * _paneYScale, 0);
				Footer.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y - LowHeight - (FullHeight - LowHeight) * _paneYScale, 0);
				Pane.transform.localScale = new Vector3(Pane.transform.localScale.x, _paneYScale, 1);
			}
		}
		else
		{
			if(_paneYScale > 0)
			{
				_paneYScale -= Time.unscaledDeltaTime * 9;
				Header.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y + LowHeight + (FullHeight - LowHeight) * _paneYScale, 0);
				Footer.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y - LowHeight - (FullHeight - LowHeight) * _paneYScale, 0);
				Pane.transform.localScale = new Vector3(Pane.transform.localScale.x, _paneYScale, 1);
			}
			else
			{
				NGUITools.SetActive(gameObject, false);
			}
		}
	}

	public void Show()
	{
		Pane.transform.localScale = new Vector3(Pane.transform.localScale.x, 0, 1);
		Header.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y + LowHeight, 0);
		Footer.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y - LowHeight, 0);
		_isShown = true;
		_paneYScale = 0;
	}

	public void Hide()
	{
		Pane.transform.localScale = new Vector3(Pane.transform.localScale.x, 1, 1);
		Header.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y + FullHeight, 0);
		Footer.transform.localPosition = new Vector3(Header.transform.localPosition.x, Pane.transform.localPosition.y - FullHeight, 0);
		_isShown = false;
		_paneYScale = 1;
	}
}
