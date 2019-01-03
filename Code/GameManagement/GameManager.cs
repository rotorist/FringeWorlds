using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	#region Singleton

	public static GameManager Inst;

	#endregion

	public string SystemID; //used for creating system in test space
	public string SystemName;//used for creating system in test space

	public SceneType SceneType;
	public Constants Constants;
	public PlayerControl PlayerControl;
	public PlayerProgress PlayerProgress;
	public Camera MainCamera;
	public CameraController CameraController;
	public CameraShaker CameraShaker;

	public MaterialManager MaterialManager;
	public DBManager DBManager;
	public UIManager UIManager;
	public CursorManager CursorManager;
	public NPCManager NPCManager;
	public WorldManager WorldManager;
	public SaveGameManager SaveGameManager;
	public SoundManager SoundManager;
	public ItemManager ItemManager;
	public EconomyManager EconomyManager;

	public ShipBase ShipForTesting;

	public LevelAnchor LevelAnchor;

	public int MaxTimeScale = 1;

	private float _secondsTimer;

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
			UIManager.PerFrameUpdate();
			CameraController.PerFrameUpdate();
		}
		else if(SceneType == SceneType.SpaceTest)
		{
			PlayerControl.PerFrameUpdate();
			//CursorManager.PerFrameUpdate();
			//NPCManager.PerFrameUpdate();
			//WorldManager.PerFrameUpdate();
		}
		else if(SceneType == SceneType.Station)
		{
			UIManager.PerFrameUpdate();
			CameraController.PerFrameUpdate();
		}



		InputEventHandler.Instance.PerFrameUpdate();


	}

	void FixedUpdate()
	{
		if(SceneType == SceneType.Space)
		{
			PlayerControl.FixedFrameUpdate();
			CameraController.PerFrameFixedUpdate();

			/*
			if(Time.time % 5 == 0)
			{
				if(GameManager.Inst.NPCManager.AllParties.Count < 6)
				{
					NPCManager.TestSpawn();
				}
			}
			*/
		}
		else if(SceneType == SceneType.SpaceTest)
		{
			PlayerControl.FixedFrameUpdate();
			//CameraController.PerFrameFixedUpdate();
		}

		_secondsTimer += Time.fixedDeltaTime;
		if(_secondsTimer >= 1)
		{
			PerSecondUpdate();
			_secondsTimer = 0;
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

	private void PerSecondUpdate()
	{
		if(SceneType == SceneType.Space || SceneType == SceneType.Station)
		{
			WorldManager.PerSecondUpdate();
			NPCManager.PerSecondUpdate();
		}
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

		if(LevelAnchor == null)
		{
			GameObject o = GameObject.Instantiate(Resources.Load("LevelAnchor")) as GameObject;
			LevelAnchor = o.GetComponent<LevelAnchor>();
		}

		MaterialManager = new MaterialManager();
		MaterialManager.Initialize();

		SoundManager = new SoundManager();
		SoundManager.Initialize();

		ItemManager = new ItemManager();
		ItemManager.Initialize();

		EconomyManager = new EconomyManager();

		if(SceneType == SceneType.Space)
		{
			Debug.Log("Loading space scene");


			NPCManager = new NPCManager();
			NPCManager.Initialize();

			UIManager = new UIManager();


			CursorManager = new CursorManager();
			CursorManager.Initialize();

			PlayerProgress = new PlayerProgress();
			PlayerProgress.Initialize();

			PlayerControl = new PlayerControl();
			PlayerControl.Initialize();

			WorldManager = new WorldManager();
			WorldManager.PreInitialize();



			//temporarily spawn player at a station to undock
			//StationBase station = GameObject.Find("PlanetColombiaLanding").GetComponent<StationBase>();
			//station.Undock(PlayerControl.PlayerShip);

			//check if there's anchor save
			StarSystem system = null;
			if(LevelAnchor != null && !LevelAnchor.IsNewGame)
			{
				//load system from anchor save
				Debug.Log("Loading from anchor!");
				system = DBManager.XMLParserWorld.GenerateSystemScene(LevelAnchor.SpawnSystem);
				SaveGameManager.GetSave(LevelAnchor.ProfileName);


			}
			else if(LevelAnchor.IsNewGame)
			{
				//load a new game
				SaveGameManager.LoadNewGameInSpace();
				system = DBManager.XMLParserWorld.GenerateSystemScene(GameManager.Inst.PlayerProgress.SpawnSystemID);

			}


			WorldManager.CurrentSystem = system;
			WorldManager.InitializeSpace();

			EconomyManager.Initialize();

			if(LevelAnchor != null && !LevelAnchor.IsNewGame)
			{
				SaveGameManager.LoadSaveInSpace();
			}

			Debug.Log("About to create the player");
			PlayerControl.CreatePlayerParty();
			PlayerControl.LoadPlayerShip();

			UIManager.Initialize();

			if(LevelAnchor != null && !LevelAnchor.IsNewGame)
			{
				PlayerControl.SpawnPlayer();

			}

			LevelAnchor.IsNewGame = false;



		}
		else if(SceneType == SceneType.SpaceTest)
		{
			NPCManager = new NPCManager();
			NPCManager.Initialize();

			UIManager = new UIManager();
			//UIManager.Initialize();

			CursorManager = new CursorManager();
			CursorManager.Initialize();

			PlayerControl = new PlayerControl();
			PlayerControl.Initialize();

			WorldManager = new WorldManager();

			//WorldManager.Initialize();

			/*
			GameObject [] npcs = GameObject.FindGameObjectsWithTag("NPC");
			foreach(GameObject o in npcs)
			{
				AI ai = o.GetComponent<AI>();
				ai.Initialize();
				NPCManager.AddExistingShip(ai.MyShip);
			}
			*/
			//temporarily spawn player at a station to undock
			//StationBase station = GameObject.Find("PlanetColombiaLanding").GetComponent<StationBase>();
			//station.Undock(PlayerControl.PlayerShip);


		}
		else if(SceneType == SceneType.Station)
		{
			UIManager = new UIManager();
			UIManager.Initialize();

			PlayerProgress = new PlayerProgress();
			PlayerProgress.Initialize();

			WorldManager = new WorldManager();
			WorldManager.PreInitialize();
			WorldManager.InitializeDocked();

			EconomyManager.Initialize();

			NPCManager = new NPCManager();
			NPCManager.InitializeDocked();

			if(LevelAnchor != null && !LevelAnchor.IsNewGame)
			{
				SaveGameManager.GetSave(LevelAnchor.ProfileName);
				SaveGameManager.LoadSaveInStation();
			}
			else if(LevelAnchor.IsNewGame)
			{
				SaveGameManager.LoadNewGameInStation();
			}

			LevelAnchor.IsNewGame = false;
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