using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

public abstract class DataHandlerBase
{

	public string Path;

	public string SavePath;

	public abstract object LoadByName(string name, object [] param);

	public abstract object[] LoadAll(object [] param);


	#region Static methods

	public static Dictionary<string, object> ParseLines(string [] rawFile)
	{
		Dictionary<string, object> data = new Dictionary<string, object>();

		foreach(string s in rawFile)
		{
			//parse the file, ignoring lines starting with ';'
			if(s[0] == ';' || s[0] == '[' || s[0] == '\n')
				continue;
			string [] splitString = s.Split('=');
			if(splitString.Length < 2)
				continue;
			data.Add(splitString[0], splitString[1]);

		}

		return data;
	}

	public static Vector3 ParseVector3(string line)
	{
		string[] splitString = line.Split(',');
		Vector3 result = new Vector3();
		result.x = Convert.ToSingle(splitString[0]);
		result.y = Convert.ToSingle(splitString[1]);
		result.z = Convert.ToSingle(splitString[2]);

		return result;
	}

	#endregion

}
