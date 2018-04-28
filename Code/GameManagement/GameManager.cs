using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	#region Singleton

	public static GameManager Inst;

	#endregion

	public string SystemID;
	public string SystemName;

	public SceneType SceneType;
	public Constants Constants;
	public PlayerControl PlayerControl;
	public Camera MainCamera;
	public CameraController CameraController;

	public MaterialManager MaterialManager;
	public DBManager DBManager;
	public UIManager UIManager;
	public CursorManager CursorManager;
	public NPCManager NPCManager;
	public WorldManager WorldManager;
	public SaveGameManager SaveGameManager;

	public LevelAnchor LevelAnchor;

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
			CursorManager.PerFrameUpdate();
			NPCManager.PerFrameUpdate();
			WorldManager.PerFrameUpdate();
		}
		else if(SceneType == SceneType.SpaceTest)
		{
			PlayerControl.PerFrameUpdate();
			CursorManager.PerFrameUpdate();
			NPCManager.PerFrameUpdate();
			WorldManager.PerFrameUpdate();
		}
		else if(SceneType == SceneType.Station)
		{
			
		}

		CameraController.PerFrameUpdate();
		UIManager.PerFrameUpdate();
		InputEventHandler.Instance.PerFrameUpdate();


	}

	void FixedUpdate()
	{
		if(SceneType == SceneType.Space)
		{
			PlayerControl.FixedFrameUpdate();
			CameraController.PerFrameFixedUpdate();
		}
		else if(SceneType == SceneType.SpaceTest)
		{
			PlayerControl.FixedFrameUpdate();
			CameraController.PerFrameFixedUpdate();
		}

		if(Time.time % 5 == 0)
		{
			if(GameManager.Inst.NPCManager.AllParties.Count < 20)
			{
				NPCManager.TestSpawn();
			}
		}
	}

	void LateUpdate()
	{
		if(SceneType == SceneType.Space)
		{
			PlayerControl.LateFrameUpdate();
		}
		else if(SceneType == SceneType.SpaceTest)
		{
			PlayerControl.LateFrameUpdate();
		}
	}


	public void LoadStationScene()
	{
		UIEventHandler.Instance.OnUnloadScene();

		SceneManager.LoadScene("Station");
	}

	public void LoadSpaceScene()
	{
		UIEventHandler.Instance.OnUnloadScene();

		SceneManager.LoadScene("Space");
	}



	private void Initialize()
	{
		Inst = this;
		Constants = transform.GetComponent<Constants>();
		Constants.Initialize();
		MainCamera = Camera.main;
		CameraController.Initialize();

		DBManager = new DBManager();
		DBManager.Initialize();

		SaveGameManager = new SaveGameManager();
		LevelAnchor = GameObject.FindObjectOfType<LevelAnchor>();

		MaterialManager = new MaterialManager();
		MaterialManager.Initialize();

		if(SceneType == SceneType.Space)
		{
			


			NPCManager = new NPCManager();
			NPCManager.Initialize();

			UIManager = new UIManager();


			CursorManager = new CursorManager();
			CursorManager.Initialize();

			PlayerControl = new PlayerControl();
			PlayerControl.Initialize();

			WorldManager = new WorldManager();



			GameObject [] npcs = GameObject.FindGameObjectsWithTag("NPC");
			foreach(GameObject o in npcs)
			{
				AI ai = o.GetComponent<AI>();
				ai.Initialize();
				NPCManager.AddExistingShip(ai.MyShip);
			}

			//temporarily spawn player at a station to undock
			//StationBase station = GameObject.Find("PlanetColombiaLanding").GetComponent<StationBase>();
			//station.Undock(PlayerControl.PlayerShip);

			//check if there's anchor save
			StarSystem system = null;
			if(LevelAnchor != null && LevelAnchor.Save != null)
			{
				//load system from anchor save
				Debug.Log("Loading from anchor!");
				system = DBManager.XMLParserWorld.GenerateSystemScene(LevelAnchor.Save.SpawnSystem);
				SaveGameManager.Load(LevelAnchor.Save);

			}
			else
			{
				//this should never really happen
				//load a default scene for now
				system = DBManager.XMLParserWorld.GenerateSystemScene("washington_system");
				SaveGameManager.LoadNewGame();
			}



			WorldManager.Initialize();
			WorldManager.CurrentSystem = system;
			UIManager.Initialize();

			if(LevelAnchor != null && LevelAnchor.Save != null)
			{
				PlayerControl.SpawnPlayer();
			}


		}
		else if(SceneType == SceneType.SpaceTest)
		{
			NPCManager = new NPCManager();
			NPCManager.Initialize();

			UIManager = new UIManager();
			UIManager.Initialize();

			CursorManager = new CursorManager();
			CursorManager.Initialize();

			PlayerControl = new PlayerControl();
			PlayerControl.Initialize();

			WorldManager = new WorldManager();
			WorldManager.Initialize();

			GameObject [] npcs = GameObject.FindGameObjectsWithTag("NPC");
			foreach(GameObject o in npcs)
			{
				AI ai = o.GetComponent<AI>();
				ai.Initialize();
				NPCManager.AddExistingShip(ai.MyShip);
			}

			//temporarily spawn player at a station to undock
			//StationBase station = GameObject.Find("PlanetColombiaLanding").GetComponent<StationBase>();
			//station.Undock(PlayerControl.PlayerShip);


		}
		else if(SceneType == SceneType.Station)
		{
			UIManager = new UIManager();
			UIManager.Initialize();
		}

		for(int i=0; i<10; i++)
		{
			NPCManager.TestSpawn();
		}

	}
}

public enum SceneType
{
	Space,
	MainMenu,
	Station,
	SpaceTest,
}