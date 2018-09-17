using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AsteroidCell : MonoBehaviour 
{
	public AsteroidField ParentField;
	public Transform Container;
	public List<Transform> Asteroids;



	public void PopulateAsteroids()
	{
		for(int i=0; i<ParentField.CellQuantity; i++)
		{
			GameObject o = GameObject.Instantiate(Resources.Load(ParentField.AsteroidID)) as GameObject;
			Asteroids.Add(o.transform);
			o.transform.parent = Container;
			o.transform.localPosition = new Vector3(UnityEngine.Random.Range(-1 * ParentField.CellRadius, ParentField.CellRadius), UnityEngine.Random.Range(-1 * ParentField.CellRadius, ParentField.CellRadius), 
				UnityEngine.Random.Range(-1 * ParentField.CellRadius, ParentField.CellRadius));
			if(ParentField.IsRandomRotation)
			{
				o.transform.localEulerAngles = new Vector3(UnityEngine.Random.value * 180f, UnityEngine.Random.value * 180f, UnityEngine.Random.value * 180f);
			}
			float scale = UnityEngine.Random.Range(0.5f, 1f);
			o.transform.localScale *= scale;
		}
	}

	public void Initialize(AsteroidField parentField)
	{
		ParentField = parentField;
		Asteroids = new List<Transform>();



	}

	public void PerFrameUpdate()
	{
		

	}

}
