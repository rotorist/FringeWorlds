using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class StarMapDesinger : MonoBehaviour 
{

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F12))
		{
			StarMapNode [] systems = GameObject.FindObjectsOfType<StarMapNode>();
			foreach(StarMapNode system in systems)
			{
				string systemID = system.name;
				string path = Application.dataPath + "/GameData/StarSystem/" + systemID + ".xml";
				XmlDocument doc = new XmlDocument();
				doc.Load(path);
				XmlNode root = doc.DocumentElement;
				XmlNode systemLocNode = root.SelectSingleNode("descendant::systemlocation");
				Debug.Log("is system loc node null? " + (systemLocNode == null));
				XmlNodeList content = systemLocNode.ChildNodes;
				foreach(XmlNode node in content)
				{
					if(node.Name == "vector3")
					{
						Debug.Log("Found v3");
						((XmlElement)node).SetAttribute("x", system.transform.position.x.ToString());
						((XmlElement)node).SetAttribute("y", system.transform.position.y.ToString());
						((XmlElement)node).SetAttribute("z", system.transform.position.z.ToString());
					}
				}

				doc.Save(path);
			}

			Debug.Log("Finished editing xml");

		}
	}


}
