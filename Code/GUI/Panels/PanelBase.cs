using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelBase : MonoBehaviour 
{
	public bool IsActive;

	public virtual void Initialize()
	{
		IsActive = true;
	}

	public virtual void PerFrameUpdate()
	{

	}

	public virtual void Show()
	{
		NGUITools.SetActive(this.gameObject, true);
		IsActive = true;
	}

	public virtual void Hide()
	{
		NGUITools.SetActive(this.gameObject, false);
		IsActive = false;
	}


}
