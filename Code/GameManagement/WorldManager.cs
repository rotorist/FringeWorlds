using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager 
{
	public StarSystem CurrentSystem;
	public StationBase CurrentDockedStation;
	public Sun [] Suns;

	public void Initialize()
	{
		Suns = GameObject.FindObjectsOfType<Sun>();

		StationBase [] stations = GameObject.FindObjectsOfType<StationBase>();
		foreach(StationBase station in stations)
		{
			station.Initialize();
		}
	}

	public void PerFrameUpdate()
	{
		if(GameManager.Inst.PlayerControl.PlayerShip != null)
		{
			foreach(Sun sun in Suns)
			{
				sun.transform.LookAt(GameManager.Inst.PlayerControl.PlayerShip.transform);
			}
		}

		if(Input.GetKeyDown(KeyCode.F12))
		{
			GameManager.Inst.DBManager.XMLParserWorld.GenerateSystemXML();
		}
	}
}
