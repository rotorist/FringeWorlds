using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDPanel : PanelBase
{
	public UISprite Pip;
	public UISprite PipLine;
	public UISprite ShieldIndicatorFront;
	public UISprite ShieldIndicatorRear;
	public BarIndicator ShieldAmountIndicator;
	public BarIndicator HullAmountIndicator;
	public BarIndicator CruisePrepIndicator;

	public CurveIndicator SpeedCurve;
	public CurveIndicator ThrusterCurve;
	public UILabel SpeedLabel;
	public UILabel ThrottleLabel;
	public UILabel FALabel;
	public UILabel MouseFlightLabel;

	public UILabel TargetShieldValue;
	public UILabel TargetHullValue;

	public Transform HologramHolder3D;
	public GameObject TargetHologram;
	public UILabel TargetRepLabel;
	public UILabel TargetDescLabel;

	public UIButton ShipsTab;
	public UIButton BasesTab;
	public UIButton PlanetsTab;
	public UIButton ItemsTab;


	public Transform WeaponListAnchor;
	public Transform ObjEntryAnchor;

	public Dictionary<ShipBase, UISprite> UnselectedShipMarkers { get { return _unselectedShips; } }

	private Transform _selectedObject;
	private SelectedObjMarker _currentSelectMarker;

	private Dictionary<ShipBase, UISprite> _unselectedShips;
	//private Dictionary<Item, UISprite> _unselectedItems;

	private List<HUDListEntry> _allEntries;
	private List<HUDWeaponEntry> _weaponEntries;

	private SelectedHUDTab _selectedTab; 


	public override void Initialize ()
	{
		GameEventHandler.OnShipDeath -= OnShipDeath;
		GameEventHandler.OnShipDeath += OnShipDeath;

		_unselectedShips = new Dictionary<ShipBase, UISprite>();
		_allEntries = new List<HUDListEntry>();
		_weaponEntries = new List<HUDWeaponEntry>();
		for(int i=0; i<13; i++)
		{
			HUDListEntry entry = LoadHUDListEntry();
			entry.transform.localPosition = new Vector3((Mathf.Sqrt(1f - Mathf.Pow(Mathf.Abs(6f - i) / 6f, 2)) * 9f), i * 32f, 0);
			_allEntries.Add(entry);
		}
		ClearTargetData();

		LoadWeaponEntries();
	}

	public override void PerFrameUpdate ()
	{
		UpdatePipPosition();
		UpdateShieldAmount();
		UpdateHullAmount();
		UpdateCruisePrep();
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
		_currentSelectMarker.Initialize(100f, description);
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
		_currentSelectMarker.Initialize(100f, ship.name);

		ShipReference shipRef = ship.ShipModel.GetComponent<ShipReference>();
		TargetShieldValue.text = "SHLD " + (int)ship.Shield.Amount;
		TargetHullValue.text = "HULL " + (int)ship.HullAmount;
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

	public void OnShipDeath(ShipBase ship)
	{
		if(_unselectedShips.ContainsKey(ship))
		{
			GameObject.Destroy(_unselectedShips[ship].gameObject);
			_unselectedShips.Remove(ship);
		}


	}







	private void ClearTargetData()
	{
		if(TargetHologram != null)
		{
			GameObject.Destroy(TargetHologram.gameObject);
			TargetHologram = null;
		}

		TargetShieldValue.text = "";
		TargetHullValue.text = "";
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
			Pip.alpha = 1f;

			if(_currentSelectMarker != null)
			{
				Vector3 los = _currentSelectMarker.Marker.transform.localPosition - Pip.transform.localPosition;
				Quaternion rot = Quaternion.FromToRotation(PipLine.transform.right, los);
				PipLine.transform.rotation = rot * PipLine.transform.rotation;
				PipLine.width = (int)(los.magnitude);
			}
		}
		else
		{
			Pip.alpha = 0;
		}
	}



	private void UpdateShieldAmount()
	{
		ShieldBase shieldBase = GameManager.Inst.PlayerControl.PlayerShip.Shield;
		if(shieldBase != null && shieldBase.Type == ShieldType.Fighter)
		{
			FighterShield shield = (FighterShield)shieldBase;
			ShieldAmountIndicator.SetFillPercentage(shield.GetShieldPercentage());
		}
		else if(shieldBase != null && shieldBase.Type == ShieldType.BigShip)
		{
			BigShipShield shield = (BigShipShield)shieldBase;
			ShieldAmountIndicator.SetFillPercentage(shield.GetShieldPercentage());
		}
	}

	private void UpdateHullAmount()
	{
		float totalHull = GameManager.Inst.PlayerControl.PlayerShip.HullCapacity;
		float currentHull = GameManager.Inst.PlayerControl.PlayerShip.HullAmount;
	
		HullAmountIndicator.SetFillPercentage(currentHull / totalHull);
	}

	private void UpdateCruisePrep()
	{
		float prepPercent = GameManager.Inst.PlayerControl.PlayerShip.Engine.PrepPercent;
		if(prepPercent <= 0 || prepPercent >= 0.99f)
		{
			CruisePrepIndicator.Frame.alpha = 0;
		}
		else
		{
			CruisePrepIndicator.Frame.alpha = 0.7f;
		}
		CruisePrepIndicator.SetFillPercentage(prepPercent);
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
		else if(_currentSelectMarker != null && _selectedObject == null)
		{
			_currentSelectMarker.SetVisible(false);
		}

		ShipBase targetShip = GameManager.Inst.PlayerControl.TargetShip;
		if(targetShip != null)
		{
			_currentSelectMarker.SetShieldAndHull(targetShip.Shield.GetShieldPercentage(), targetShip.HullAmount / targetShip.HullCapacity);
		}
	}

	private void UpdateUnselectedMarkerPosition()
	{
		//show all ships
		ShipBase playerShip = GameManager.Inst.PlayerControl.PlayerShip;

		//remove despawned ship or docked ship markers
		Dictionary<ShipBase, UISprite> _unselectedShipsCopy = new Dictionary<ShipBase, UISprite>(_unselectedShips);
		foreach(KeyValuePair<ShipBase, UISprite> marker in _unselectedShipsCopy)
		{
			if(!GameManager.Inst.NPCManager.AllShips.Contains(marker.Key))
			{
				_unselectedShips.Remove(marker.Key);
				GameObject.Destroy(marker.Value.gameObject);
			}

		}


		foreach(ShipBase ship in GameManager.Inst.NPCManager.AllShips)
		{
			if(ship == playerShip)
			{
				continue;
			}

			float distFromPlayer = Vector3.Distance(ship.transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position);

			bool isMarkerHidden = false;
			bool isMarkerTooFar = false;

			if(ship == GameManager.Inst.PlayerControl.TargetShip)
			{
				isMarkerHidden = true;
			}

			if(ship.DockedStationID != "")
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
					sprite.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
					sprite.width = 130;
					_unselectedShips.Add(ship, sprite);
				}

				_unselectedShips[ship].alpha = 0.8f;

				//update position
				_unselectedShips[ship].transform.localPosition = GameManager.Inst.UIManager.GetTargetScreenPos(ship.transform.position);
				//update scale
				float scale = GetUnselectedMarkerScale(distFromPlayer);
				_unselectedShips[ship].width = (int)scale;
			}
		

		}
	}

	private float GetUnselectedMarkerScale(float dist)
	{
		return 130 +  GameManager.Inst.Constants.MarkerEnlargeCurve.Evaluate((1 - Mathf.Clamp01(dist / 200))) * 200;
	}

	private void UpdateCenterHUD()
	{
		//speed
		float speed = GameManager.Inst.PlayerControl.PlayerShip.RB.velocity.magnitude / GameManager.Inst.PlayerControl.PlayerShip.Engine.CruiseSpeed;
		SpeedCurve.SetValue(speed);

		//thruster fuel
		Thruster thruster = GameManager.Inst.PlayerControl.PlayerShip.Thruster;
		if(thruster != null)
		{
			ThrusterCurve.SetValue(thruster.CurrentFuel / thruster.MaxFuel);
			//ThrusterBar.height = 20 + Mathf.CeilToInt(80f * thruster.CurrentFuel / thruster.MaxFuel);
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

		//throttle
		float throttle = GameManager.Inst.PlayerControl.Throttle;
		ThrottleLabel.text = ((int)(throttle * 100)).ToString() + "%";

		//flight assist indicator
		if(GameManager.Inst.PlayerControl.IsFAKilled)
		{
			FALabel.text = "NEWTONIAN";
		}
		else
		{
			FALabel.text = "ASSISTED";
		}

		//mouse flight indicator
		if(GameManager.Inst.PlayerControl.IsMouseFlight)
		{
			MouseFlightLabel.text = "MANUAL";
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
			TargetShieldValue.text = "SHLD " + (int)targetShip.Shield.Amount;
			TargetHullValue.text = "HULL " + (int)targetShip.HullAmount;
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
			if(allShips.Count > 0)
			{
				//sort by distance from playership
				List<ShipBase> sorted = allShips.OrderBy(x=>Vector3.Distance(x.transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position)).ToList<ShipBase>();
				sorted.Remove(GameManager.Inst.PlayerControl.PlayerShip);
				int count = 10;
				if(sorted.Count < count)
				{
					count = sorted.Count;
				}
				for(int i=0; i<count; i++)
				{
					_allEntries[i].SetDistance(Vector3.Distance(sorted[i].transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position));
					_allEntries[i].SetDescription(sorted[i].ShipModel.GetComponent<ShipReference>().Name);
					_allEntries[i].SetAlpha(0.5f);
				}
			}
		}
	}

	private void LoadWeaponEntries()
	{
		int i = 0;
		foreach(WeaponJoint joint in GameManager.Inst.PlayerControl.PlayerShip.MyReference.WeaponJoints)
		{
			GameObject o = GameObject.Instantiate(Resources.Load("HUDWeaponEntry")) as GameObject;
			HUDWeaponEntry entry = o.GetComponent<HUDWeaponEntry>();
			o.transform.parent = WeaponListAnchor;
			o.transform.localPosition = new Vector3((Mathf.Sqrt(1f - Mathf.Pow(Mathf.Abs(5f - i) / 5f, 2)) * -9f), i * 42, 0);
			o.transform.localEulerAngles = Vector3.zero;
			o.transform.localScale = new Vector3(1, 1, 1);
			_weaponEntries.Add(entry);
			int groupNumber = GameManager.Inst.PlayerControl.GetWeaponGroupNumber(joint);
			string weaponName = "";
			if(joint.MountedWeapon != null)
			{
				weaponName = joint.MountedWeapon.name;
			}
			entry.UpdateEntry(groupNumber, weaponName);

			i++;
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