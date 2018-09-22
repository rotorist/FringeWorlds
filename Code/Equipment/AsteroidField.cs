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
	public int AsteroidVariants;
	public bool IsRandomRotation;
	public float FogIntensity;
	public Color FogColor;
	public float FogStart;
	public float SunIntensity;
	public Color AmbientColor;
	public float Gradient { get { return _gradient; } }

	private float _gradient;
	private List<AsteroidCell> _cells;


	public void Initialize()
	{
		_cells = new List<AsteroidCell>();

		for(int i=0; i<10; i++)
		{
			GameObject anchorObject = GameObject.Instantiate(Resources.Load("AsteroidCellAnchor")) as GameObject;
			anchorObject.name = "AsteroidCell" + i;
			anchorObject.transform.position = new Vector3(100000, 100000, 100000);
			AsteroidCell cell = anchorObject.GetComponent<AsteroidCell>();
			cell.Initialize(this);
			cell.PopulateAsteroids();
			_cells.Add(cell);
		}
	}

	// Update is called once per frame
	public void PerFrameUpdate () 
	{
		Vector3 playerPos = GameManager.Inst.PlayerControl.PlayerShip.transform.position;
		if(StaticUtility.IsInArea(playerPos, transform.position, new Vector3(Size.x/2 + 0.5f, Size.y/2 + 0.5f, Size.z/2 + 0.5f) * CellSize))
		{

			List<Vector3> nearestCoords = FindNearestCellCoords();
			foreach(Vector3 coord in nearestCoords)
			{
				Vector3 cellPos = transform.position + coord * CellSize;

				if(!IsCoordTaken(cellPos) && Mathf.Abs(coord.x * 2) <= Size.x && Mathf.Abs(coord.y * 2) <= Size.y && Mathf.Abs(coord.z * 2) <= Size.z)
				{
					
					AsteroidCell cell = GetAvailableCell(playerPos);
					if(cell != null)
					{
						cell.transform.position = cellPos;

					}
				}
			}
		}
		else
		{
			
			List<Vector3> edgeCoords = FindHorizontalEdgeCoords();
			Vector3 centerCellPos = edgeCoords[0] * CellSize + transform.position;
			int cellIndex = 0;
			foreach(Vector3 coord in edgeCoords)
			{
				Vector3 cellPos = transform.position + coord * CellSize;

				//check if the coord is along the edge of the field
				if(!IsCoordTaken(cellPos) && IsCoordOnEdge(coord))
				{
					//Debug.Log(coord);
					AsteroidCell cell = _cells[cellIndex];
					if(cell != null && cellIndex <= 9)
					{
						cell.transform.position = cellPos;
						cellIndex ++;
					}
				}

			}
		}

		//check how close player is to the border to calculate a value between 0 and 1
		//to set fog intensity, fog color, and sun flare size
		_gradient = 0;


		Vector3 playerDist = playerPos - transform.position;

		if(StaticUtility.IsInArea(playerPos, transform.position, new Vector3(Size.x * CellRadius - FogStart, Size.y * CellRadius - FogStart, Size.z * CellRadius - FogStart)))
		{
			_gradient = 1;

		}
		else
		{
			if(Mathf.Abs(playerDist.x) > Size.x * CellRadius - FogStart)
			{
				_gradient = Mathf.Clamp01((Size.x * CellRadius - Mathf.Abs(playerDist.x)) / FogStart);
			}
			else if(Mathf.Abs(playerDist.y) > Size.y * CellRadius - FogStart)
			{
				_gradient = Mathf.Clamp01((Size.y * CellRadius - Mathf.Abs(playerDist.y)) / FogStart);
			}
			else if(Mathf.Abs(playerDist.z) > Size.z * CellRadius - FogStart)
			{
				_gradient = Mathf.Clamp01((Size.z * CellRadius - Mathf.Abs(playerDist.z)) / FogStart);
			}
		}



		//set fog intensity
		RenderSettings.fogDensity = Mathf.Lerp(0.0002f, FogIntensity, _gradient);
		RenderSettings.fogColor = Color.Lerp(new Color(0.01f, 0.015f, 0.02f, 1f), FogColor, _gradient);

		//set sun intensity
		foreach(Sun sun in GameManager.Inst.WorldManager.CurrentSystem.Suns)
		{
			if(sun.Flare != null)
			{
				sun.Flare.brightness = Mathf.Lerp(0.6f, 0, _gradient);
			}

			sun.Sunlight.intensity = Mathf.Lerp(1, SunIntensity, _gradient);
		}

		//set ambient
		RenderSettings.ambientLight = Color.Lerp(GameManager.Inst.WorldManager.CurrentSystem.AmbientColor, AmbientColor, _gradient);
	}

	public AsteroidCell GetAvailableCell(Vector3 pos)
	{
		
		foreach(AsteroidCell cell in _cells)
		{
			if(Vector3.Distance(cell.transform.position, pos) > CellSize * 1.1f)
			{
				//Debug.Log("selected cell " + cell.name);
				return cell;
			}
		}

		return null;
	}

	public List<Vector3> FindHorizontalEdgeCoords()
	{
		List<Vector3> coords = new List<Vector3>();
		Vector3 playerPos = GameManager.Inst.PlayerControl.PlayerShip.transform.position;
		Vector3 edgePos = Vector3.zero;
		Vector3 playerDist = playerPos - transform.position;
		//get angle between playerpos-fieldPos and fieldPos.forward
		float angle = Vector3.Angle(playerDist, transform.forward);
		float angleThreshold = Mathf.Atan(Size.z / Size.x) * Mathf.Rad2Deg;

		if(angle < angleThreshold)
		{
			float x = Mathf.FloorToInt(Mathf.Tan(angle * Mathf.Deg2Rad) * (Mathf.Abs(Size.z) / 2));
			if(playerDist.x < 0)
			{
				x *= -1;
			}
			edgePos = new Vector3(x, 0, Mathf.FloorToInt(Size.z / 2));
		}
		else if(angle > angleThreshold && angle < 180 - angleThreshold)
		{
			float z = Mathf.FloorToInt(Mathf.Tan(Mathf.Abs(angle - 90) * Mathf.Deg2Rad) * (Mathf.Abs(Size.x) / 2));
			if(playerDist.z < 0)
			{
				z *= -1;
			}
			float x = Mathf.FloorToInt(Size.x / 2);
			if(playerDist.x < 0)
			{
				x *= -1;
			}
			edgePos = new Vector3(x, 0, z);
		}
		else
		{
			float x = Mathf.FloorToInt(Mathf.Tan((180 - angle) * Mathf.Deg2Rad) * Mathf.Abs(Size.z / 2));
			if(playerDist.x < 0)
			{
				x *= -1;
			}
			edgePos = new Vector3(x, 0, Mathf.FloorToInt(Size.z / 2) * -1f);
		}


		coords.Add(edgePos);
		//Debug.Log(angle + " " + edgePos);
		//add all neighbors on x and z
		for(int i = -2; i<= 2; i++)
		{
			for(int j = -2; j < 2; j++)
			{
				if(i != 0 || j != 0)
				{
					coords.Add(edgePos + new Vector3(i, 0, j));
				}
			}
		}



		return coords;
	}

	public List<Vector3> FindNearestCellCoords()
	{
		List<Vector3> coords = new List<Vector3>();
		//get the cell coord player is currently in
		Vector3 playerPos = GameManager.Inst.PlayerControl.PlayerShip.transform.position;
		Vector3 normalizedCoord = Vector3.zero;
		Vector3 distFromFieldCenter = playerPos - transform.position;
		normalizedCoord.x = Mathf.CeilToInt(distFromFieldCenter.x/CellRadius) / 2;
		normalizedCoord.y = Mathf.CeilToInt(distFromFieldCenter.y/CellRadius) / 2;
		normalizedCoord.z = Mathf.CeilToInt(distFromFieldCenter.z/CellRadius) / 2;

		coords.Add(normalizedCoord);
		//get all surrounding cells that should show based on player's location in current cell
		Vector3 cellPos = transform.position + normalizedCoord * CellSize;
		Vector3 distFromCell = (playerPos - cellPos);

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

	private bool IsCoordOnEdge(Vector3 coord)
	{
		
		if((Mathf.Abs(coord.x * 2) + 1 == Size.x && Mathf.Abs(coord.z * 2) <= Size.z) ||
			(Mathf.Abs(coord.z * 2) + 1 == Size.z && Mathf.Abs(coord.x * 2) <= Size.x))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}

