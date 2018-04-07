using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class XMLParserWorld
{
	private XmlDocument _currentXML; 
	private XmlWriter _xmlWriter;

	public StarSystem LoadStarSystem(string id)
	{
		//
		string path = Application.dataPath + "/GameData/StarSystem/" + id + ".xml";
		XmlDocument doc = new XmlDocument();

		string file = File.ReadAllText(path);
		try 
		{
			doc.LoadXml(file);
		}
		catch (XmlException)
		{
			return null;
		}

		XmlNode systemNode = doc.GetElementsByTagName("system")[0];
		XmlAttributeCollection systemAttrs = systemNode.Attributes;
		string systemID = systemAttrs["id"].Value.ToString();
		string systemName = systemAttrs["displayname"].Value.ToString();

		StarSystem system = new StarSystem(systemID, systemName);

		XmlNodeList systemContent = systemNode.ChildNodes;

		foreach(XmlNode node in systemContent)
		{
			if(node.Name == "ambience")
			{
				XmlNodeList content = node.ChildNodes;
				foreach(XmlNode subnode in content)
				{
					
					if(subnode.Name == "skybox")
					{
						string skyboxName = node.InnerText;
						RenderSettings.skybox = Resources.Load<Material>(skyboxName);
					}

					if(subnode.Name == "ambientcolor")
					{
						XmlNode colorNode = subnode.FirstChild;
						RenderSettings.ambientLight = DBManager.ParseXmlColorNode(colorNode);
					}
				}
			}

			if(node.Name == "sun")
			{
				string sunID = "";
				string displayName = "";
				float intensity = 0;
				Vector3 scale = Vector3.zero;
				Vector3 location = Vector3.zero;
				Color color = Color.black;

				XmlNodeList content = node.ChildNodes;
				foreach(XmlNode subnode in content)
				{
					
					if(subnode.Name == "id")
					{
						sunID = subnode.InnerText;
					}
					if(subnode.Name == "displayname")
					{
						displayName = subnode.InnerText;
					}
					if(subnode.Name == "location")
					{
						location = DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "scale")
					{
						scale = DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "sunlight")
					{

					}

				}
			}


		}

		return null;
	}

	public void GenerateSystemXML()
	{
		string id = GameManager.Inst.SystemID;
		string path = Application.dataPath + "/GameData/StarSystem/" + id + ".xml";

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.IndentChars = "    "; // note: default is two spaces
		settings.NewLineOnAttributes = false;

		_xmlWriter = XmlWriter.Create(path, settings);

		_xmlWriter.WriteStartDocument();

		_xmlWriter.WriteDocType("system", null, null, "<!ATTLIST node id ID #IMPLIED>");

		_xmlWriter.WriteStartElement("system");
		_xmlWriter.WriteAttributeString("id", id);
		_xmlWriter.WriteAttributeString("displayname", GameManager.Inst.SystemName);

		//ambience
		_xmlWriter.WriteStartElement("ambience");

		_xmlWriter.WriteStartElement("skybox");
		_xmlWriter.WriteString(RenderSettings.skybox.name);
		_xmlWriter.WriteFullEndElement();

		_xmlWriter.WriteStartElement("ambientcolor");

		_xmlWriter.WriteStartElement("color");
		_xmlWriter.WriteAttributeString("r", RenderSettings.ambientLight.r.ToString());
		_xmlWriter.WriteAttributeString("g", RenderSettings.ambientLight.g.ToString());
		_xmlWriter.WriteAttributeString("b", RenderSettings.ambientLight.b.ToString());
		_xmlWriter.WriteEndElement();

		_xmlWriter.WriteFullEndElement();

		_xmlWriter.WriteStartElement("music");
		_xmlWriter.WriteFullEndElement();

		_xmlWriter.WriteFullEndElement();

		//suns
		Sun [] suns = GameObject.FindObjectsOfType<Sun>();
		foreach(Sun sun in suns)
		{
			_xmlWriter.WriteStartElement("sun");

			_xmlWriter.WriteStartElement("id");
			_xmlWriter.WriteString(sun.ID);
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("displayname");
			_xmlWriter.WriteString(sun.DisplayName);
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("location");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", sun.transform.position.x.ToString());
			_xmlWriter.WriteAttributeString("y", sun.transform.position.y.ToString());
			_xmlWriter.WriteAttributeString("z", sun.transform.position.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("scale");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", sun.transform.localScale.x.ToString());
			_xmlWriter.WriteAttributeString("y", sun.transform.localScale.y.ToString());
			_xmlWriter.WriteAttributeString("z", sun.transform.localScale.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("sunlight");
			_xmlWriter.WriteAttributeString("intensity", sun.Sunlight.intensity.ToString());
			_xmlWriter.WriteStartElement("color");
			_xmlWriter.WriteAttributeString("r", sun.Sunlight.color.r.ToString());
			_xmlWriter.WriteAttributeString("g", sun.Sunlight.color.g.ToString());
			_xmlWriter.WriteAttributeString("b", sun.Sunlight.color.b.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteFullEndElement();
		}

		//planets
		Planet [] planets = GameObject.FindObjectsOfType<Planet>();
		foreach(Planet planet in planets)
		{
			_xmlWriter.WriteStartElement("planet");

			_xmlWriter.WriteStartElement("id");
			_xmlWriter.WriteString(planet.ID);
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("displayname");
			_xmlWriter.WriteString(planet.DisplayName);
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("location");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", planet.transform.position.x.ToString());
			_xmlWriter.WriteAttributeString("y", planet.transform.position.y.ToString());
			_xmlWriter.WriteAttributeString("z", planet.transform.position.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("scale");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", planet.transform.localScale.x.ToString());
			_xmlWriter.WriteAttributeString("y", planet.transform.localScale.y.ToString());
			_xmlWriter.WriteAttributeString("z", planet.transform.localScale.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();


			_xmlWriter.WriteFullEndElement();
		}

		//stations
		GameObject [] stations = GameObject.FindGameObjectsWithTag("Station");
		foreach(GameObject o in stations)
		{
			StationBase station = o.GetComponent<StationBase>();
			_xmlWriter.WriteStartElement("station");

			_xmlWriter.WriteStartElement("id");
			_xmlWriter.WriteString(station.ID);
			_xmlWriter.WriteFullEndElement();


			_xmlWriter.WriteStartElement("location");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", station.transform.position.x.ToString());
			_xmlWriter.WriteAttributeString("y", station.transform.position.y.ToString());
			_xmlWriter.WriteAttributeString("z", station.transform.position.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("eulerangles");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", station.transform.eulerAngles.x.ToString());
			_xmlWriter.WriteAttributeString("y", station.transform.eulerAngles.y.ToString());
			_xmlWriter.WriteAttributeString("z", station.transform.eulerAngles.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();


			_xmlWriter.WriteFullEndElement();
		}

		//tradelanes
		Tradelane [] tradelanes = GameObject.FindObjectsOfType<Tradelane>();
		foreach(Tradelane tl in tradelanes)
		{
			_xmlWriter.WriteStartElement("tradelane");

			_xmlWriter.WriteStartElement("id");
			_xmlWriter.WriteString(tl.ID);
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("displayname");
			_xmlWriter.WriteString(tl.DisplayName);
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("terminal");
			_xmlWriter.WriteString(tl.IsTerminalAorB.ToString());
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("location");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", tl.transform.position.x.ToString());
			_xmlWriter.WriteAttributeString("y", tl.transform.position.y.ToString());
			_xmlWriter.WriteAttributeString("z", tl.transform.position.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("eulerangles");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", tl.transform.eulerAngles.x.ToString());
			_xmlWriter.WriteAttributeString("y", tl.transform.eulerAngles.y.ToString());
			_xmlWriter.WriteAttributeString("z", tl.transform.eulerAngles.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("neighbor_a");
			if(tl.NeighborToA != null)
			{
				_xmlWriter.WriteString(tl.NeighborToA.ID);
			}
			else
			{
				_xmlWriter.WriteString("NULL");
			}
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("neighbor_b");
			if(tl.NeighborToB != null)
			{
				_xmlWriter.WriteString(tl.NeighborToB.ID);
			}
			else
			{
				_xmlWriter.WriteString("NULL");
			}
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteFullEndElement();
		}




		_xmlWriter.WriteFullEndElement();
		_xmlWriter.Close();
	}



}
