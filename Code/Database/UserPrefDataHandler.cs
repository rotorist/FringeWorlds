using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

public class UserPrefDataHandler : DataHandlerBase
{

	#region Public fields



	#endregion


	#region Private fields


	#endregion


	#region Constructor
	public UserPrefDataHandler()
	{
		Initialize();
	}

	#endregion


	#region Private methods
	private void Initialize()
	{
		this.Path = Application.dataPath + "/GameData/Preference/";
	}

	#endregion

	#region Public methods
	public override object LoadByName (string name, object[] param)
	{
		return null;
	}

	public override object[] LoadAll (object [] param)
	{
		return null;
	}


	public Dictionary<UserInputs, KeyInput> GetKeyBindings()
	{
		Dictionary<UserInputs, KeyInput> keyBindings = new Dictionary<UserInputs, KeyInput>();
		string [] rawFile = File.ReadAllLines(this.Path + "KeyBindings.txt");

		Dictionary<string, object> data = DataHandlerBase.ParseLines(rawFile);

		foreach(KeyValuePair<string, object> kv in data)
		{
			string value = kv.Value.ToString();
			string [] tokens = value.Split('+');
			KeyInput input = new KeyInput();
			if(tokens.Length > 1)
			{
				input.IsFnSet = true;
				input.FnKey = (KeyCode)Enum.Parse(typeof(KeyCode), tokens[0]);
				input.Key = (KeyCode)Enum.Parse(typeof(KeyCode), tokens[1]);
			}
			else
			{
				input.IsFnSet = false;
				input.Key = (KeyCode)Enum.Parse(typeof(KeyCode), tokens[0]);
			}
			UserInputs key = (UserInputs)Enum.Parse(typeof(UserInputs), kv.Key);
			keyBindings.Add(key, input); 
			//Debug.Log(key + " " + keyBindings[key].IsFnSet + ", " + keyBindings[key].Key);
		}

		return keyBindings;
	}






	#endregion
}

