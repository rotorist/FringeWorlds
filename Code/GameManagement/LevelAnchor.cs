using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAnchor : MonoBehaviour 
{
	public bool IsNewGame;
	public SaveGame Save;


	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

}
