using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour 
{
	public Vector3 Size;//number of cells x y z
	public int CellQuantity;
	public float CellRadius;
	public float CellSize { get { return CellRadius * 2; } }
	public string AsteroidID;
	public bool IsRandomRotation;



	private List<AsteroidCell> _cells;


	public void Initialize()
	{
		_cells = new List<AsteroidCell>();

		for(int i=0; i<8; i++)
		{
			GameObject anchorObject = GameObject.Instantiate(Resources.Load("AsteroidCellAnchor")) as GameObject;
			anchorObject.transform.position = new Vector3(100000, 0, 100000);
			AsteroidCell cell = anchorObject.GetComponent<AsteroidCell>();
			cell.Initialize(this);
			cell.PopulateAsteroids();
			_cells.Add(cell);
		}
	}

	// Update is called once per frame
	public void PerFrameUpdate () 
	{
		List<Vector3> nearestCoords = FindNearestCellCoords();
		foreach(Vector3 coord in nearestCoords)
		{
			Vector3 cellPos = transform.position + coord * CellSize;
			if(!IsCoordTaken(cellPos) && Mathf.Abs(coord.x * 2) <= Size.x && Mathf.Abs(coord.y * 2) <= Size.y && Mathf.Abs(coord.z * 2) <= Size.z)
			{
				AsteroidCell cell = GetAvailableCell();
				if(cell != null)
				{
					cell.transform.position = cellPos;

				}
			}
		}

	}

	public AsteroidCell GetAvailableCell()
	{
		Vector3 playerPos = GameManager.Inst.PlayerControl.PlayerShip.transform.position;
		foreach(AsteroidCell cell in _cells)
		{
			if(Vector3.Distance(cell.transform.position, playerPos) > CellSize)
			{
				
				return cell;
			}
		}

		return null;
	}

	public List<Vector3> FindNearestCellCoords()
	{
		List<Vector3> coords = new List<Vector3>();
		//get the cell coord player is currently in
		Vector3 playerPos = GameManager.Inst.PlayerControl.PlayerShip.transform.position;
		Vector3 normalizedCoord = Vector3.zero;
		Vector3 distFromFieldCenter = playerPos - transform.position;
		normalizedCoord.x = Mathf.CeilToInt(distFromFieldCenter.x/CellSize);
		normalizedCoord.y = Mathf.CeilToInt(distFromFieldCenter.y/CellSize);
		normalizedCoord.z = Mathf.CeilToInt(distFromFieldCenter.z/CellSize);
		coords.Add(normalizedCoord);
		//get all surrounding cells that should show based on player's location in current cell
		Vector3 cellPos = transform.position + normalizedCoord * CellSize;
		Vector3 distFromCell = (playerPos - cellPos).normalized;

		if(distFromCell.x <=0 && distFromCell.z <= 0)
		{
			coords.Add(new Vector3(-1, 0, -1) + normalizedCoord);
			coords.Add(new Vector3(-1, 0, 0) + normalizedCoord);
			coords.Add(new Vector3(0, 0, -1) + normalizedCoord);
		}
		else if(distFromCell.x > 0 && distFromCell.z <= 0)
		{
			coords.Add(new Vector3(1, 0, -1) + normalizedCoord);
			coords.Add(new Vector3(1, 0, 0) + normalizedCoord);
			coords.Add(new Vector3(0, 0, -1) + normalizedCoord);
		}
		else if(distFromCell.x <= 0 && distFromCell.z > 0)
		{
			coords.Add(new Vector3(-1, 0, 1) + normalizedCoord);
			coords.Add(new Vector3(-1, 0, 0) + normalizedCoord);
			coords.Add(new Vector3(0, 0, 1) + normalizedCoord);
		}
		else if(distFromCell.x > 0 && distFromCell.z > 0)
		{
			coords.Add(new Vector3(1, 0, 1) + normalizedCoord);
			coords.Add(new Vector3(1, 0, 0) + normalizedCoord);
			coords.Add(new Vector3(0, 0, 1) + normalizedCoord);
		}

		List<Vector3> finalCoords = new List<Vector3>(coords);

		if(distFromCell.y <= 0)
		{
			foreach(Vector3 coord in coords)
			{
				finalCoords.Add(new Vector3(coord.x, coord.y - 1, coord.z));
			}
		}
		else
		{
			foreach(Vector3 coord in coords)
			{
				finalCoords.Add(new Vector3(coord.x, coord.y + 1, coord.z));
			}
		}



		return finalCoords;
	}

	private bool IsCoordTaken(Vector3 cellPos)
	{
		foreach(AsteroidCell cell in _cells)
		{
			if(Vector3.Distance(cell.transform.position, cellPos) < CellRadius)
			{
				return true;
			}
		}

		return false;
	}
}

