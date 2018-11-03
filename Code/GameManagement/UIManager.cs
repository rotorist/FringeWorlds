using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager
{
	public UIRoot Root;
	public Camera UICamera;
	public UIStateMachine UIStateMachine;


	public HUDPanel HUDPanel;
	public FadePanel FadePanel;
	public KeyBindingPanel KeyBindingPanel;
	public PowerManagementPanel PowerManagementPanel;

	public StationHUDPanel StationHUDPanel;
	public RepairPanel RepairPanel;
	public ShipInfoPanel ShipInfoPanel;

	public float UIZoom;
	public bool IsInHUDRegion;

	private List<PanelBase> _panels;

	public void Initialize()
	{
		_panels = new List<PanelBase>();

		//GameObject uiRootObj = GameObject.Instantiate(Resources.Load("UI Root")) as GameObject;

		Root = GameObject.Find("UI Root").GetComponent<UIRoot>();

		//Root.manualHeight = Screen.height;
		//Root.manualWidth = Screen.width;

		UICamera = Root.transform.Find("Camera").GetComponent<Camera>();

		if(GameManager.Inst.SceneType == SceneType.Space)
		{
			HUDPanel = UICamera.transform.Find("HUDPanel").GetComponent<HUDPanel>();
			HUDPanel.Initialize();

			_panels.Add(HUDPanel);

			KeyBindingPanel = UICamera.transform.Find("KeyBindingPanel").GetComponent<KeyBindingPanel>();
			KeyBindingPanel.Initialize();
			_panels.Add(KeyBindingPanel);

			PowerManagementPanel = UICamera.transform.Find("PowerManagement").GetComponent<PowerManagementPanel>();
			PowerManagementPanel.Initialize();
			_panels.Add(PowerManagementPanel);
		}
		else if(GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			HUDPanel = UICamera.transform.Find("HUDPanel").GetComponent<HUDPanel>();
			HUDPanel.Initialize();


			_panels.Add(HUDPanel);

		}
		else if(GameManager.Inst.SceneType == SceneType.Station)
		{
			StationHUDPanel = UICamera.transform.Find("StationHUDPanel").GetComponent<StationHUDPanel>();
			StationHUDPanel.Initialize();
			RepairPanel = UICamera.transform.Find("RepairPanel").GetComponent<RepairPanel>();
			RepairPanel.Initialize();
			ShipInfoPanel = UICamera.transform.Find("ShipInfoPanel").GetComponent<ShipInfoPanel>();
			ShipInfoPanel.Initialize();

			_panels.Add(StationHUDPanel);
			_panels.Add(RepairPanel);
			_panels.Add(ShipInfoPanel);
		}

		FadePanel = UICamera.transform.Find("FadePanel").GetComponent<FadePanel>();
		FadePanel.Initialize();
		_panels.Add(FadePanel);

		UIZoom = 1;
		
		UIStateMachine = new UIStateMachine();
		UIStateMachine.Initialize(GameManager.Inst.SceneType);
	}

	public void PerFrameUpdate()
	{
		foreach(PanelBase panel in _panels)
		{
			if(panel.IsActive)
			{
				panel.PerFrameUpdate();
			}
		}



		UpdateUIZoom();
	}


	public bool IsCursorInHUDRegion()
	{
		return false;
		/*
		Vector3 cursorLoc = Input.mousePosition; 
		Vector3 worldPos = UICamera.ScreenToWorldPoint(cursorLoc);
		Vector3 localPos = HUDPanel.transform.worldToLocalMatrix.MultiplyPoint3x4(worldPos);
		Vector3 rightHUDLoc = HUDPanel.RightHUDAnchor.localPosition;

		if(localPos.x > rightHUDLoc.x - 282 && localPos.y < rightHUDLoc.y + 188)
		{
			IsInHUDRegion = true;
			return true;
		}

		IsInHUDRegion = false;
		return false;
		*/
	}

	public int GetScreenHeight()
	{
		float ratio = (float)Root.activeHeight / Screen.height;
		return Mathf.CeilToInt(Screen.height * ratio);

	}

	public int GetScreenWidth()
	{
		float ratio = (float)Root.activeHeight / Screen.height;
		return Mathf.CeilToInt(Screen.width * ratio);

	}

	public Vector3 GetZeroCenteredMousePosition01()
	{
		Vector2 mousePos = Input.mousePosition;
		return new Vector2((mousePos.x / Screen.width) - 0.5f, (mousePos.y / Screen.height) - 0.5f) * 2;

	}

	public Vector3 GetNGUIMousePosition()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 2f;
		mousePos.x = Mathf.Clamp01(mousePos.x / Screen.width);
		mousePos.y = Mathf.Clamp01(mousePos.y / Screen.height);
		Vector3 worldPoint = GameManager.Inst.UIManager.UICamera.ViewportToWorldPoint(mousePos);
		//Debug.Log(mousePos + ", " + worldPoint);
		return new Vector3(worldPoint.x, worldPoint.y, 0);
	}

	public Vector3 GetTargetScreenPos(Vector3 target)
	{
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(target); 
		Vector3 origPoint = new Vector3(Screen.width/2f,Screen.height/2f,0); 
		Vector3 nguiPoint = screenPoint - origPoint;
		nguiPoint = new Vector3(nguiPoint.x, nguiPoint.y, 0);
		// we need the point in ideal size 
		float s = GameManager.Inst.UIManager.Root.manualHeight / (float)Screen.height; 
		return nguiPoint *= s;
	}

	public Vector2 NormalizeScreenPosition(Vector2 rawPosition)
	{
		Vector2 uiSize = GetUIWidthHeight();

		Vector2 normalized = new Vector2(uiSize.x * rawPosition.x / Screen.width - uiSize.x / 2.0f,
			uiSize.y * rawPosition.y / Screen.height - uiSize.y / 2.0f);
		return normalized;
	}

	public Vector2 GetUIWidthHeight()
	{
		float aspectRatio = 1.0f * Screen.height / Screen.width;
		int uiHeight = Root.manualHeight;
		int uiWidth = (int)(uiHeight / aspectRatio);

		return new Vector2(uiWidth, uiHeight);
	}

	public UISprite LoadUISprite(string spriteID, Transform anchor, int width, int height, int depth)
	{
		try
		{
			GameObject spriteGO = GameObject.Instantiate(Resources.Load(spriteID)) as GameObject;
			UISprite sprite = spriteGO.GetComponent<UISprite>();
			sprite.transform.parent = anchor;
			sprite.transform.localPosition = Vector3.zero;
			sprite.transform.localScale = new Vector3(1, 1, 1);
			sprite.MakePixelPerfect();
			sprite.width = width;
			sprite.height = height;
			sprite.depth = depth;

			return sprite;
		}
		catch(System.Exception e)
		{
			return null;
		}
	}


	public void HideAllPanels()
	{
		foreach(PanelBase panel in _panels)
		{
			if(panel.IsActive)
			{
				
				panel.Hide();
			}
		}
	}





	private void UpdateUIZoom()
	{
		if(Input.GetKeyDown(KeyCode.Equals))
		{
			UIZoom += 0.1f;
			//HUDPanel.UpdateScaling();
		}

		if(Input.GetKeyDown(KeyCode.Minus))
		{
			UIZoom -= 0.1f;
			if(UIZoom < 0.3f)
			{
				UIZoom = 0.3f;
			}
			//HUDPanel.UpdateScaling();
		}
	}
}
