using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{

	public List<ShipBase> AllShips { get { return _allShips; } }

	private List<ShipBase> _allShips;

	public void Initialize()
	{
		_allShips = new List<ShipBase>();
	}

	public void PerFrameUpdate()
	{

	}

	public void AddExistingShip(ShipBase ship)
	{
		if(!_allShips.Contains(ship))
		{
			_allShips.Add(ship);
		}
	}
}

public enum Factions
{
	Confederation,

}