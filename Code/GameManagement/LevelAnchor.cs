using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAnchor : MonoBehaviour 
{
	public bool IsNewGame;
	public string SpawnSystem;
	public string ProfileName;


	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

}
