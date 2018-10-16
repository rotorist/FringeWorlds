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


	public Dictionary<UserInputs, KeyInput> GetKeyBindings(bool isDefault)
	{
		Dictionary<UserInputs, KeyInput> keyBindings = new Dictionary<UserInputs, KeyInput>();
		string filename = "";
		if(isDefault)
		{
			filename = "KeyBindingsDefault.txt";
		}
		else
		{
			filename = "KeyBindings.txt";
		}
		string [] rawFile = File.ReadAllLines(this.Path + filename);


		Dictionary<string, object> data = DataHandlerBase.ParseLines(rawFile);

		foreach(KeyValuePair<string, object> kv in data)
		{
			string value = kv.Value.ToString();
			string [] tokens = value.Split('+');
			KeyInput input = new KeyInput();
			if(tokens.Length > 1)
			{
				input.FnKey = (KeyCode)Enum.Parse(typeof(KeyCode), tokens[0]);
				input.Key = (KeyCode)Enum.Parse(typeof(KeyCode), tokens[1]);
			}
			else
			{
				input.FnKey = KeyCode.None;
				input.Key = (KeyCode)Enum.Parse(typeof(KeyCode), tokens[0]);
			}
			UserInputs key = (UserInputs)Enum.Parse(typeof(UserInputs), kv.Key);

			keyBindings.Add(key, input); 
			//Debug.Log(key + " " + keyBindings[key].IsFnSet + ", " + keyBindings[key].Key);
		}

		//check for similar inputs
		foreach(KeyValuePair<UserInputs, KeyInput> keyBinding in keyBindings)
		{
			foreach(KeyValuePair<UserInputs, KeyInput> target in keyBindings)
			{
				if(keyBinding.Value.Key == target.Value.Key && keyBinding.Value.FnKey != target.Value.FnKey)
				{
					keyBinding.Value.SimilarInput = target.Value;
				}
			}
		}

		return keyBindings;
	}

	public void SaveKeyBindings(Dictionary<UserInputs, KeyInput> keyBindings)
	{
		System.IO.File.WriteAllText(this.Path + "KeyBindings.txt", ";\r\n");

		foreach(KeyValuePair<UserInputs, KeyInput> keyBinding in keyBindings)
		{
			string text = keyBinding.Key.ToString() + "=";
			if(keyBinding.Value.FnKey != KeyCode.None)
			{
				text += keyBinding.Value.FnKey.ToString() + "+" + keyBinding.Value.Key.ToString();
			}
			else
			{
				text += keyBinding.Value.Key.ToString();
			}

			text += "\r\n";

			try
			{
				System.IO.File.AppendAllText(this.Path + "KeyBindings.txt", text);
			}
			catch(Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}




	#endregion
}

