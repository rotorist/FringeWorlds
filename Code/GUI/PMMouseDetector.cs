using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMMouseDetector : MonoBehaviour 
{

	public void OnHover()
	{
		GameManager.Inst.UIManager.PowerManagementPanel.PMCursor.transform.localPosition = transform.localPosition;
		GameManager.Inst.UIManager.PowerManagementPanel.OnMouseHover(GetComponent<UIButton>());
	}
}
