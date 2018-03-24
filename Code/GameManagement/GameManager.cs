﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	#region Singleton

	public static GameManager Inst;

	#endregion

	public SceneType SceneType;
	public Constants Constants;
	public PlayerControl PlayerControl;
	public Camera MainCamera;

	public DBManager DBManager;
	public UIManager UIManager;
	public CursorManager CursorManager;
	public NPCManager NPCManager;

	// Use this for initialization
	void Start () 
	{
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = 60;
		Initialize();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(SceneType == SceneType.Space)
		{
			PlayerControl.PerFrameUpdate();
			UIManager.PerFrameUpdate();
			CursorManager.PerFrameUpdate();
			NPCManager.PerFrameUpdate();
		}
	}

	void FixedUpdate()
	{
		if(SceneType == SceneType.Space)
		{
			PlayerControl.FixedFrameUpdate();
		}
	}

	void LateUpdate()
	{
		if(SceneType == SceneType.Space)
		{
			PlayerControl.LateFrameUpdate();
		}
	}


	public void LoadStationScene()
	{
		UIEventHandler.Instance.OnUnloadScene();

		SceneManager.LoadScene("Station");
	}



	private void Initialize()
	{
		Inst = this;
		Constants = transform.GetComponent<Constants>();
		Constants.Initialize();
		MainCamera = Camera.main;

		DBManager = new DBManager();
		DBManager.Initialize();

		if(SceneType == SceneType.Space)
		{
			NPCManager = new NPCManager();
			NPCManager.Initialize();

			UIManager = new UIManager();
			UIManager.Initialize();

			CursorManager = new CursorManager();
			CursorManager.Initialize();

			PlayerControl = new PlayerControl();
			PlayerControl.Initialize();

			GameObject [] npcs = GameObject.FindGameObjectsWithTag("NPC");
			foreach(GameObject o in npcs)
			{
				AI ai = o.GetComponent<AI>();
				ai.Initialize();
				NPCManager.AddExistingShip(ai.MyShip);
			}

		}
		else if(SceneType == SceneType.Station)
		{

		}



	}
}

public enum SceneType
{
	Space,
	MainMenu,
	Station,
}