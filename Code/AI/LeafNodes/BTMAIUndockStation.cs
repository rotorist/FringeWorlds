using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMAIUndockStation : BTLeaf
{

	public override void Initialize ()
	{

	}

	public override BTResult Process ()
	{
		if(MyParty == null || MyParty.CurrentTask == null || MyParty.DockedStationID == "")
		{
			return Exit(BTResult.Fail);
		}

		MyParty.DockedStationID = "";

		foreach(ShipBase ship in MyParty.SpawnedShips)
		{
			if(ship.MyAI != null)
			{
				ship.DockedStationID = "";
				ship.MyAI.IsDocked = false;
			}
		}

		return Exit(BTResult.Success);
	}

	public override BTResult Exit (BTResult result)
	{

		return result;
	}
}
