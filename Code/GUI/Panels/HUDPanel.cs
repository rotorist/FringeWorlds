﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDPanel : PanelBase
{
	public UISprite Pip;
	public UISprite ShieldIndicatorFront;
	public UISprite ShieldIndicatorRear;
	public BarIndicator ShieldAmountIndicator;

	public UISprite EngineThrottleBar;
	public UISprite ThrusterBar;
	public UILabel SpeedLabel;
	public UILabel FALabel;
	public UILabel MouseFlightLabel;

	public BarIndicator TargetShield;
	public UISprite TargetShieldHolder;
	public BarIndicator TargetHull;
	public UISprite TargetHullHolder;
	public Transform HologramHolder3D;
	public GameObject TargetHologram;
	public UILabel TargetRepLabel;
	public UILabel TargetDescLabel;

	public UIButton ShipsTab;
	public UIButton BasesTab;
	public UIButton PlanetsTab;
	public UIButton ItemsTab;



	public Transform ObjEntryAnchor;

	public Dictionary<ShipBase, UISprite> UnselectedShipMarkers { get { return _unselectedShips; } }

	private Transform _selectedObject;
	private SelectedObjMarker _currentSelectMarker;

	private Dictionary<ShipBase, UISprite> _unselectedShips;
	//private Dictionary<Item, UISprite> _unselectedItems;

	private List<HUDListEntry> _allEntries;

	private SelectedHUDTab _selectedTab; 


	public override void Initialize ()
	{
		_unselectedShips = new Dictionary<ShipBase, UISprite>();
		_allEntries = new List<HUDListEntry>();
		for(int i=0; i<10; i++)
		{
			HUDListEntry entry = LoadHUDListEntry();
			entry.transform.localPosition = new Vector3(0, i * 20f, 0);
			_allEntries.Add(entry);
		}
		ClearTargetData();
	}

	public override void PerFrameUpdate ()
	{
		UpdatePipPosition();
		UpdateShieldBalance();
		UpdateSelectMarkerPosition();
		UpdateUnselectedMarkerPosition();
		UpdateCenterHUD();
		UpdateRightHUD();

	}

	public override void Show ()
	{
		base.Show();
	}

	public override void Hide ()
	{
		base.Hide();
	}



	public void OnSelectPlanetOrStation(Transform obj, string description)
	{
		_selectedObject = obj;
		if(_currentSelectMarker != null)
		{
			//remove current marker
			GameObject.Destroy(_currentSelectMarker.gameObject);
		}

		GameObject o = GameObject.Instantiate(Resources.Load("SelectedObjectMarkerNeutral")) as GameObject;
		_currentSelectMarker = o.GetComponent<SelectedObjMarker>();
		o.transform.parent = transform;
		o.transform.localScale = new Vector3(1, 1, 1);
		_currentSelectMarker.Initialize(125f, description);
	}

	public void OnSelectShip(ShipBase ship)
	{
		ClearTargetData();

		_selectedObject = ship.transform;
		if(_currentSelectMarker != null)
		{
			//remove current marker
			GameObject.Destroy(_currentSelectMarker.gameObject);
		}

		GameObject o = GameObject.Instantiate(Resources.Load("SelectedShipMarkerHostile")) as GameObject;
		_currentSelectMarker = o.GetComponent<SelectedObjMarker>();
		o.transform.parent = transform;
		o.transform.localScale = new Vector3(1, 1, 1);
		_currentSelectMarker.Initialize(125f, ship.name);

		ShipReference shipRef = ship.ShipModel.GetComponent<ShipReference>();
		TargetShieldHolder.alpha = 1;
		TargetShield.SetFillPercentage(ship.Shield.GetShieldPercentage());
		TargetHullHolder.alpha = 1;
		TargetHull.SetFillPercentage(ship.HullAmount / ship.HullCapacity);
		GameObject hologram = GameObject.Instantiate(Resources.Load(ship.ShipModelID + "Hologram")) as GameObject;
		TargetHologram = hologram;
		hologram.transform.parent = HologramHolder3D;
		hologram.transform.localPosition = Vector3.zero;
		hologram.transform.localEulerAngles = Vector3.zero;
		float scale = shipRef.HologramScale;
		hologram.transform.localScale = new Vector3(scale, scale, scale);
		TargetDescLabel.text = shipRef.Name + " - " + "New Dawn";
		TargetRepLabel.color = new Color(1, 0.3f, 0.3f);
		TargetRepLabel.text = "HOSTILE";
	}

	public void OnClearSelectedObject()
	{
		if(_currentSelectMarker != null)
		{
			//remove current marker
			GameObject.Destroy(_currentSelectMarker.gameObject);
		}
		_selectedObject = null;

		ClearTargetData();
	}

	public void OnSelectHUDTab()
	{
		ShipsTab.isEnabled = true;
		BasesTab.isEnabled = true;
		PlanetsTab.isEnabled = true;
		ItemsTab.isEnabled = true;


		if(UIButton.current.name == "ShipsTab")
		{
			_selectedTab = SelectedHUDTab.Ship;
			ShipsTab.isEnabled = false;
		}
		else if(UIButton.current.name == "BasesTab")
		{
			_selectedTab = SelectedHUDTab.Station;
			BasesTab.isEnabled = false;
		}
		else if(UIButton.current.name == "PlanetsTab")
		{
			_selectedTab = SelectedHUDTab.Planet;
			PlanetsTab.isEnabled = false;
		}
		else if(UIButton.current.name == "ItemsTab")
		{
			_selectedTab = SelectedHUDTab.Item;
			ItemsTab.isEnabled = false;
		}
	}







	private void ClearTargetData()
	{
		if(TargetHologram != null)
		{
			GameObject.Destroy(TargetHologram.gameObject);
			TargetHologram = null;
		}

		TargetShield.SetFillPercentage(0);
		TargetShield.SetFillPercentage(0);
		TargetShieldHolder.alpha = 0;
		TargetHullHolder.alpha = 0;
		TargetDescLabel.text = "";
		TargetRepLabel.text = "";
	}






	private void UpdatePipPosition()
	{
		if(GameManager.Inst.PlayerControl.TargetShip != null)
		{
			ShipBase target = GameManager.Inst.PlayerControl.TargetShip;
			ShipBase myShip = GameManager.Inst.PlayerControl.PlayerShip;
			Vector3 aimPoint = StaticUtility.FirstOrderIntercept(myShip.transform.position, myShip.RB.velocity,
				30, target.transform.position, target.RB.velocity);
			Vector3 viewPos = GameManager.Inst.UIManager.UICamera.WorldToViewportPoint(aimPoint);
			Vector3 screenPos = GameManager.Inst.UIManager.UICamera.ViewportToScreenPoint(viewPos);
			//float multiplier = 1;//(float)GameManager.Inst.UIManager.Root.manualHeight / (float)Screen.height;


			//Pip.transform.OverlayPosition(aimPoint, GameManager.Inst.MainCamera, GameManager.Inst.UIManager.UICamera);
			//Vector3 overlay = NGUIMath.WorldToLocalPoint(aimPoint, GameManager.Inst.MainCamera, GameManager.Inst.UIManager.UICamera, Pip.transform);
			Vector3 overlay = Camera.main.WorldToScreenPoint(aimPoint);
			overlay = new Vector3(overlay.x - Screen.width/2f, overlay.y - Screen.height/2f, 0) * 0.65f;
			Pip.transform.localPosition = GameManager.Inst.UIManager.GetTargetScreenPos(aimPoint);
			Pip.alpha = 0.8f;
		}
		else
		{
			Pip.alpha = 0;
		}
	}

	private void UpdateShieldBalance()
	{
		ShieldBase shieldBase = GameManager.Inst.PlayerControl.PlayerShip.Shield;
		if(shieldBase != null && shieldBase.Type == ShieldType.Fighter)
		{
			FighterShield shield = (FighterShield)shieldBase;
			ShieldIndicatorFront.alpha = Mathf.Clamp01(shield.FrontCapacity / shield.TotalCapacity);
			ShieldIndicatorRear.alpha = Mathf.Clamp01(shield.RearCapacity / shield.TotalCapacity);
			//Debug.Log(shield.FrontAmount + ", " + shield.RearAmount);

			ShieldAmountIndicator.SetFillPercentage(shield.GetShieldPercentage());
		}


	}

	private void UpdateSelectMarkerPosition()
	{
		if(_currentSelectMarker != null && _selectedObject != null)
		{
			float angle = Vector3.Angle(Camera.main.transform.forward, (_selectedObject.transform.position - Camera.main.transform.position));
			if(angle < 90)
			{
				_currentSelectMarker.transform.localPosition = GameManager.Inst.UIManager.GetTargetScreenPos(_selectedObject.transform.position);
				_currentSelectMarker.SetVisible(true);

			}
			else
			{
				_currentSelectMarker.SetVisible(false);
			}
		}
	}

	private void UpdateUnselectedMarkerPosition()
	{
		//show all ships
		ShipBase playerShip = GameManager.Inst.PlayerControl.PlayerShip;

		foreach(ShipBase ship in GameManager.Inst.NPCManager.AllShips)
		{
			

		
			bool isMarkerHidden = false;
			bool isMarkerTooFar = false;

			if(ship == GameManager.Inst.PlayerControl.TargetShip)
			{
				isMarkerHidden = true;
			}

			if(Vector3.Distance(ship.transform.position, playerShip.transform.position) > 200)
			{
				isMarkerHidden = true;
				isMarkerTooFar = true;
			}
			else
			{
				if(Vector3.Angle(playerShip.transform.forward, ship.transform.position - playerShip.transform.position) > 90)
				{
					isMarkerHidden = true;
				}
			}

			if(isMarkerHidden)
			{
				if(_unselectedShips.ContainsKey(ship))
				{
					if(isMarkerTooFar)
					{
						//destroy it
						GameObject.Destroy(_unselectedShips[ship].gameObject);
						_unselectedShips.Remove(ship);
					}
					else
					{
						_unselectedShips[ship].alpha = 0;
					}
				}

			}
			else
			{
				if(!_unselectedShips.ContainsKey(ship))
				{
					//load it
					GameObject o = GameObject.Instantiate(Resources.Load("UnselectedShipMarkerHostile")) as GameObject;
					UISprite sprite = o.GetComponent<UISprite>();
					sprite.MakePixelPerfect();
					sprite.transform.parent = transform;
					sprite.transform.localScale = new Vector3(1, 1, 1);
					sprite.width = 30;
					_unselectedShips.Add(ship, sprite);
				}

				_unselectedShips[ship].alpha = 0.8f;

				//update position
				_unselectedShips[ship].transform.localPosition = GameManager.Inst.UIManager.GetTargetScreenPos(ship.transform.position);
			}
		

		}
	}

	private void UpdateCenterHUD()
	{
		//engine throttle
		float throttle = GameManager.Inst.PlayerControl.Throttle;
		EngineThrottleBar.height = 20 + Mathf.CeilToInt(80f * throttle);
		//thruster fuel
		Thruster thruster = GameManager.Inst.PlayerControl.PlayerShip.Thruster;
		if(thruster != null)
		{
			ThrusterBar.height = 20 + Mathf.CeilToInt(80f * thruster.CurrentFuel / thruster.MaxFuel);
		}

		//ship speed
		if(GameManager.Inst.PlayerControl.PlayerShip.IsInPortal)
		{
			SpeedLabel.text = Mathf.FloorToInt(GameManager.Inst.PlayerControl.PlayerShip.InPortalSpeed * 100).ToString();
		}
		else
		{
			SpeedLabel.text = Mathf.FloorToInt(GameManager.Inst.PlayerControl.PlayerShip.RB.velocity.magnitude * 100).ToString();
		}

		//flight assist indicator
		if(GameManager.Inst.PlayerControl.IsFAKilled)
		{
			FALabel.text = "";
		}
		else
		{
			FALabel.text = "FA";
		}

		//mouse flight indicator
		if(GameManager.Inst.PlayerControl.IsMouseFlight)
		{
			MouseFlightLabel.text = "MAN";
		}
		else
		{
			MouseFlightLabel.text = "AUTO";
		}
	}

	private void UpdateRightHUD()
	{
		//if there's target ship on player control, set the right HUD hull/shield to the target ship's status
		ShipBase targetShip = GameManager.Inst.PlayerControl.TargetShip;
		if(targetShip != null)
		{
			TargetHull.SetFillPercentage(targetShip.HullAmount / targetShip.HullCapacity);
			TargetShield.SetFillPercentage(targetShip.Shield.GetShieldPercentage());
		}

		//display surrounding objects
		//first hide all of them
		foreach(HUDListEntry entry in _allEntries)
		{
			entry.SetAlpha(0);
		}
		if(_selectedTab == SelectedHUDTab.Ship)
		{
			List<ShipBase> allShips = GameManager.Inst.NPCManager.AllShips;
			//sort by distance from playership
			List<ShipBase> sorted = allShips.OrderBy(x=>Vector3.Distance(x.transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position)).ToList<ShipBase>();
			for(int i=0; i<sorted.Count; i++)
			{
				_allEntries[i].SetDistance(Vector3.Distance(sorted[i].transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position));
				_allEntries[i].SetDescription(sorted[i].ShipModel.GetComponent<ShipReference>().Name);
				_allEntries[i].SetAlpha(0.5f);
			}
		}
	}



	private HUDListEntry LoadHUDListEntry()
	{
		GameObject o = GameObject.Instantiate(Resources.Load("HUDListEntry")) as GameObject;
		HUDListEntry entry = o.GetComponent<HUDListEntry>();
		o.transform.parent = ObjEntryAnchor.transform;
		o.transform.localPosition = Vector3.zero;
		o.transform.localEulerAngles = Vector3.zero;
		o.transform.localScale = new Vector3(1, 1, 1);
		entry.SetAlpha(0);
		return entry;
	}


}

public enum SelectedHUDTab
{
	Ship,
	Station,
	Planet,
	Item,
}