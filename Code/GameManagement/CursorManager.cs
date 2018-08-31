using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager
{
	public List<AI> OnScreenShips;


	public void Initialize()
	{
		//cursor
		Texture2D defaultCursor = Resources.Load("hw_cursor") as Texture2D;
		//Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
		Cursor.SetCursor(defaultCursor, new Vector2(defaultCursor.width/2f, defaultCursor.height/2f), CursorMode.Auto);
	}

	public void PerFrameUpdate()
	{

	}

	public GameObject SelectObject(out SelectedObjectType type)
	{
		//first see if cursor is near any onscreen objects with markers
		Vector3 mousePos = GameManager.Inst.UIManager.GetNGUIMousePosition();
		//GameObject.Find("Sphere").transform.position = mousePos;
		//Debug.Log(mousePos);
		Dictionary<ShipBase, UISprite> unselectedShipMarkers = GameManager.Inst.UIManager.HUDPanel.UnselectedShipMarkers;
		foreach(KeyValuePair<ShipBase, UISprite> marker in unselectedShipMarkers)
		{
			if(marker.Value.alpha <= 0)
			{
				continue;
			}

			Vector3 markerPos = new Vector3(marker.Value.transform.position.x, marker.Value.transform.position.y, 0);
			//marker.Value.transform.position = mousePos;
			Debug.Log(marker.Value.transform.position);
			Debug.Log(Vector3.Distance(mousePos, markerPos));
			if(Vector3.Distance(markerPos, mousePos) < 0.1f)
			{
				type = SelectedObjectType.Ship;
				Debug.Log("Selected a ship");
				return marker.Key.gameObject;
			}
		}

		//do a raycast to detect large objects
		GameObject aimedObject = null;
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//ignore small ships, pickups, shields
		int mask = ~(1<<2 | 1<<8 | 1<<9 | 1<<10 | 1<<12 | 1<<15);
		if(Physics.Raycast(ray,  out hit, 10000, mask))
		{
			Debug.Log("Hit " + hit.collider.name);
			aimedObject = hit.collider.gameObject;
			type = SelectedObjectType.Unknown;
		}
		else
		{
			type = SelectedObjectType.Unknown;
		}





		return aimedObject;
	}


	private void UpdateOnScreenObjects()
	{


	}
}
