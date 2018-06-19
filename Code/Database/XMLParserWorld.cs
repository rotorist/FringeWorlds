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

	public StarSystem GenerateSystemScene(string id)
	{
		string path = Application.dataPath + "/GameData/StarSystem/" + id + ".xml";
		StarSystemData system = LoadStarSystemData(path, true);

		StarSystem starSystem = new StarSystem(system.ID, system.DisplayName);
		RenderSettings.skybox = Resources.Load<Material>(system.SkyboxName);
		RenderSettings.ambientLight = system.AmbientColor;



		foreach(SunData sunData in system.Suns)
		{
			GameObject sunObject = GameObject.Instantiate(Resources.Load(sunData.ID)) as GameObject;
			sunObject.name = sunData.ID;
			Sun sun = sunObject.GetComponent<Sun>();
			sun.transform.position = sunData.Location.RealPos;
			sun.transform.localScale = sunData.Scale;
			sun.Sunlight.color = sunData.Color;
			sun.Sunlight.intensity = sunData.Intensity;
			starSystem.Suns.Add(sun);
		}

		foreach(PlanetData planetData in system.Planets)
		{
			GameObject planetObject = GameObject.Instantiate(Resources.Load(planetData.ID)) as GameObject;
			planetObject.name = planetData.ID;
			Planet planet = planetObject.GetComponent<Planet>();
			planet.transform.position = planetData.Location.RealPos;
			planet.transform.localScale = planetData.OriginalScale;
			planet.OriginalScale = planetData.OriginalScale;
			starSystem.Planets.Add(planet);
		}

		foreach(StationData stationData in system.Stations)
		{
			GameObject stationObject = GameObject.Instantiate(Resources.Load(stationData.ID)) as GameObject;
			stationObject.name = stationData.ID;
			StationBase station = stationObject.GetComponent<StationBase>();
			station.transform.position = stationData.Location.RealPos;
			station.transform.eulerAngles = stationData.EulerAngles;
			starSystem.Stations.Add(station);
		}

		foreach(JumpGateData jumpGateData in system.JumpGates)
		{
			GameObject stationObject = GameObject.Instantiate(Resources.Load("JumpGate")) as GameObject;
			stationObject.name = jumpGateData.ID;
			JumpGate gate = stationObject.GetComponent<JumpGate>();
			gate.transform.position = jumpGateData.Location.RealPos;
			gate.transform.eulerAngles = jumpGateData.EulerAngles;
			gate.TargetSystem = jumpGateData.TargetSystem;
			gate.ExitGateID = jumpGateData.ExitGateID;
			starSystem.Stations.Add(gate);
		}

		foreach(TradelaneData tradelaneData in system.Tradelanes)
		{
			GameObject tlObject = GameObject.Instantiate(Resources.Load("Tradelane")) as GameObject;
			tlObject.name = tradelaneData.ID;
			Tradelane tl = tlObject.transform.Find("TradelaneBody").GetComponent<Tradelane>();
			tlObject.transform.position = tradelaneData.Location.RealPos;
			tlObject.transform.eulerAngles = tradelaneData.EulerAngles;
			tl.ID = tradelaneData.ID;
			tl.DisplayName = tradelaneData.DisplayName;
			tl.IsTerminalAorB = tradelaneData.IsTerminalAorB;
			tl.NeighborAID = tradelaneData.NeighborAID;
			tl.NeighborBID = tradelaneData.NeighborBID;
			tl.NeighborToA = null;
			tl.NeighborToB = null;
			starSystem.Tradelanes.Add(tl);
		}

		//now assign neighbors to tradelanes
		List<Tradelane> tradelanes = starSystem.Tradelanes;
		foreach(Tradelane tl in tradelanes)
		{
			foreach(Tradelane neighbor in tradelanes)
			{
				if(tl.NeighborAID == neighbor.ID)
				{
					tl.NeighborToA = neighbor;
				}

				if(tl.NeighborBID == neighbor.ID)
				{
					tl.NeighborToB = neighbor;
				}
			}
		}

		return starSystem;
	}

	public Dictionary<string, StarSystemData> LoadAllSystemsFromXML()
	{
		Dictionary<string, StarSystemData> allSystems = new Dictionary<string, StarSystemData>();
		DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/GameData/StarSystem/");

		FileInfo [] files = dirInfo.GetFiles("*.xml");
		foreach(FileInfo fileInfo in files)
		{
			string path = Application.dataPath + "/GameData/StarSystem/" + fileInfo.Name;
			string [] tokens = fileInfo.Name.Split('.');
			if(tokens[0] == GameManager.Inst.WorldManager.CurrentSystem.ID)
			{
				StarSystemData systemData = LoadStarSystemData(path, true);
				allSystems.Add(systemData.ID, systemData);
			}
			else
			{
				StarSystemData systemData = LoadStarSystemData(path, false);
				allSystems.Add(systemData.ID, systemData);
			}

		}

		return allSystems;
	}

	public StarSystemData LoadStarSystemData(string path, bool isScene)
	{
		//

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
		Transform origin = GameObject.Find("Origin").transform;
		if(!isScene)
		{
			origin = null;
		}

		StarSystemData systemData = new StarSystemData(systemID, systemName);
		systemData.SystemID = systemID;

		XmlNodeList systemContent = systemNode.ChildNodes;

		foreach(XmlNode node in systemContent)
		{
			if(node.Name == "systemlocation")
			{
				Vector3 location = location = DBManager.ParseXmlVector3(node.FirstChild);

				systemData.Location = new RelLoc(Vector3.zero, location, null);
			}

			if(node.Name == "ambience")
			{
				XmlNodeList content = node.ChildNodes;
				foreach(XmlNode subnode in content)
				{
					
					if(subnode.Name == "skybox")
					{
						string skyboxName = node.InnerText;
						systemData.SkyboxName = skyboxName;


					}

					if(subnode.Name == "ambientcolor")
					{
						XmlNode colorNode = subnode.FirstChild;
						systemData.AmbientColor = DBManager.ParseXmlColorNode(colorNode);


					}
				}
			}

			if(node.Name == "origin")
			{
				Vector3 originLoc = DBManager.ParseXmlVector3(node.FirstChild);
				systemData.OriginPosition = originLoc;
				if(isScene)
				{
					origin.position = originLoc;
				}
			}

			if(node.Name == "sun")
			{
				string sunID = "";
				string name = "";
				float intensity = 0;
				Vector3 scale = Vector3.zero;
				Vector3 location = Vector3.zero;
				Color color = Color.black;

				SunData sunData = new SunData();

				XmlNodeList content = node.ChildNodes;
				foreach(XmlNode subnode in content)
				{
					
					if(subnode.Name == "id")
					{
						sunID = subnode.InnerText;
					}
					if(subnode.Name == "displayname")
					{
						name = subnode.InnerText;
					}
					if(subnode.Name == "location")
					{
						location = systemData.OriginPosition + DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "scale")
					{
						scale = DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "sunlight")
					{
						XmlAttributeCollection sunlightAttr = subnode.Attributes;
						intensity = Convert.ToSingle(sunlightAttr["intensity"].Value);
						XmlNode colorNode = subnode.FirstChild;
						color = DBManager.ParseXmlColorNode(colorNode);
					}

				}

				sunData.ID = sunID;
				sunData.DisplayName = name;
				sunData.Location = new RelLoc(systemData.OriginPosition, location, origin);
				sunData.Scale = scale;
				sunData.Color = color;
				sunData.Intensity = intensity;
				systemData.Suns.Add(sunData);



			}
			if(node.Name == "planet")
			{
				string planetID = "";
				string name = "";
				Vector3 scale = Vector3.zero;
				Vector3 location = Vector3.zero;

				PlanetData planetData = new PlanetData();

				XmlNodeList content = node.ChildNodes;
				foreach(XmlNode subnode in content)
				{

					if(subnode.Name == "id")
					{
						planetID = subnode.InnerText;
					}
					if(subnode.Name == "displayname")
					{
						name = subnode.InnerText;
					}
					if(subnode.Name == "location")
					{
						location = systemData.OriginPosition + DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "scale")
					{
						scale = DBManager.ParseXmlVector3(subnode.FirstChild);
					}
				}

				planetData.ID = planetID;
				planetData.DisplayName = name;
				planetData.Location = new RelLoc(systemData.OriginPosition, location, origin);
				planetData.OriginalScale = scale;
				systemData.Planets.Add(planetData);




			}
			if(node.Name == "station")
			{
				string stationID = "";
				string name = "";
				string stationType = "";
				Vector3 location = Vector3.zero;
				Vector3 eulerAngles = Vector3.zero;
				XmlNodeList content = node.ChildNodes;
				string targetSystem = "";
				string exitGateID = "";
				List<string> neighborIDs = new List<string>();
				Vector3 spawnDisposition = Vector3.zero;

				foreach(XmlNode subnode in content)
				{

					if(subnode.Name == "id")
					{
						stationID = subnode.InnerText;
					}
					if(subnode.Name == "displayname")
					{
						name = subnode.InnerText;
					}
					if(subnode.Name == "stationtype")
					{
						stationType = subnode.InnerText;
					}
					if(subnode.Name == "location")
					{
						location = systemData.OriginPosition + DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "eulerangles")
					{
						eulerAngles = DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "targetsystem")
					{
						targetSystem = subnode.InnerText;
					}
					if(subnode.Name == "exitgateid")
					{
						exitGateID = subnode.InnerText;
					}
					if(subnode.Name == "navneighbor")
					{
						neighborIDs.Add(subnode.InnerText);
					}
					if(subnode.Name == "spawndisposition")
					{
						spawnDisposition = DBManager.ParseXmlVector3(subnode.FirstChild);
					}
				}

				if(stationType == "JumpGate")
				{
					JumpGateData jumpGateData = new JumpGateData();
					jumpGateData.ID = stationID;
					jumpGateData.DisplayName = name;
					jumpGateData.TargetSystem = targetSystem;
					jumpGateData.ExitGateID = exitGateID;
					jumpGateData.Location = new RelLoc(systemData.OriginPosition, location, origin);
					jumpGateData.EulerAngles = eulerAngles;
					jumpGateData.NeighborIDs = neighborIDs;
					jumpGateData.NavNodeType = NavNodeType.JumpGate;
					jumpGateData.SystemID = systemID;
					jumpGateData.SpawnDisposition = spawnDisposition;
					systemData.JumpGates.Add(jumpGateData);
					systemData.ChildNodes.Add(jumpGateData);
					systemData.NeighborIDs.Add(targetSystem);

				}
				else if(stationType == "Station")
				{
					StationData stationData = new StationData();
					stationData.DisplayName = name;
					stationData.ID = stationID;
					stationData.Location = new RelLoc(systemData.OriginPosition, location, origin);
					stationData.EulerAngles = eulerAngles;
					stationData.NeighborIDs = neighborIDs;
					stationData.NavNodeType = NavNodeType.Station;
					stationData.SystemID = systemID;
					systemData.Stations.Add(stationData);
					systemData.ChildNodes.Add(stationData);
				}

			}
			if(node.Name == "tradelane")
			{
				string stationID = "";
				string displayName = "";
				Vector3 location = Vector3.zero;
				Vector3 eulerAngles = Vector3.zero;
				XmlNodeList content = node.ChildNodes;
				string terminal = "";
				string neighborA = "";
				string neighborB = "";
				List<string> neighborIDs = new List<string>();
				foreach(XmlNode subnode in content)
				{

					if(subnode.Name == "id")
					{
						stationID = subnode.InnerText;
					}
					if(subnode.Name == "displayname")
					{
						displayName = subnode.InnerText;
					}
					if(subnode.Name == "location")
					{
						location = systemData.OriginPosition + DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "eulerangles")
					{
						eulerAngles = DBManager.ParseXmlVector3(subnode.FirstChild);
					}
					if(subnode.Name == "terminal")
					{
						terminal = subnode.InnerText;
					}
					if(subnode.Name == "neighbor_a")
					{
						neighborA = subnode.InnerText;
						if(neighborA != "NULL")
						{
							neighborIDs.Add(neighborA);
						}
					}
					if(subnode.Name == "neighbor_b")
					{
						neighborB = subnode.InnerText;
						if(neighborB != "NULL")
						{
							neighborIDs.Add(neighborB);
						}
					}
					if(subnode.Name == "navneighbor")
					{
						neighborIDs.Add(subnode.InnerText);
					}
				}

				TradelaneData tradelaneData = new TradelaneData();
				tradelaneData.ID = stationID;
				tradelaneData.DisplayName = displayName;
				tradelaneData.IsTerminalAorB = Convert.ToInt32(terminal);
				tradelaneData.NeighborAID = neighborA;
				tradelaneData.NeighborBID = neighborB;
				tradelaneData.Location = new RelLoc(systemData.OriginPosition, location, origin);
				tradelaneData.EulerAngles = eulerAngles;
				tradelaneData.NeighborIDs = neighborIDs;
				tradelaneData.NavNodeType = NavNodeType.Tradelane;
				tradelaneData.SystemID = systemID;
				systemData.Tradelanes.Add(tradelaneData);
				systemData.ChildNodes.Add(tradelaneData);


			}

		}





		return systemData;
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

		//system location in system map
		_xmlWriter.WriteStartElement("systemlocation");
		_xmlWriter.WriteStartElement("vector3");
		_xmlWriter.WriteAttributeString("x", "0");
		_xmlWriter.WriteAttributeString("y", "0");
		_xmlWriter.WriteAttributeString("z", "0");
		_xmlWriter.WriteEndElement();
		_xmlWriter.WriteFullEndElement();

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

		//origin
		GameObject origin = GameObject.Find("Origin");
		_xmlWriter.WriteStartElement("origin");
		_xmlWriter.WriteStartElement("vector3");
		_xmlWriter.WriteAttributeString("x", origin.transform.position.x.ToString());
		_xmlWriter.WriteAttributeString("y", origin.transform.position.y.ToString());
		_xmlWriter.WriteAttributeString("z", origin.transform.position.z.ToString());
		_xmlWriter.WriteEndElement();
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
			Vector3 disposition = sun.transform.position - origin.transform.position;
			_xmlWriter.WriteAttributeString("x", disposition.x.ToString());
			_xmlWriter.WriteAttributeString("y", disposition.y.ToString());
			_xmlWriter.WriteAttributeString("z", disposition.z.ToString());
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
			Vector3 disposition = planet.transform.position - origin.transform.position;
			_xmlWriter.WriteAttributeString("x", disposition.x.ToString());
			_xmlWriter.WriteAttributeString("y", disposition.y.ToString());
			_xmlWriter.WriteAttributeString("z", disposition.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("scale");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", planet.OriginalScale.x.ToString());
			_xmlWriter.WriteAttributeString("y", planet.OriginalScale.y.ToString());
			_xmlWriter.WriteAttributeString("z", planet.OriginalScale.z.ToString());
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

			_xmlWriter.WriteStartElement("displayname");
			_xmlWriter.WriteString(station.DisplayName);
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("stationtype");
			_xmlWriter.WriteString(station.StationType.ToString());
			_xmlWriter.WriteFullEndElement();


			_xmlWriter.WriteStartElement("location");
			_xmlWriter.WriteStartElement("vector3");
			Vector3 disposition = station.transform.position - origin.transform.position;
			_xmlWriter.WriteAttributeString("x", disposition.x.ToString());
			_xmlWriter.WriteAttributeString("y", disposition.y.ToString());
			_xmlWriter.WriteAttributeString("z", disposition.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			_xmlWriter.WriteStartElement("eulerangles");
			_xmlWriter.WriteStartElement("vector3");
			_xmlWriter.WriteAttributeString("x", station.transform.eulerAngles.x.ToString());
			_xmlWriter.WriteAttributeString("y", station.transform.eulerAngles.y.ToString());
			_xmlWriter.WriteAttributeString("z", station.transform.eulerAngles.z.ToString());
			_xmlWriter.WriteEndElement();
			_xmlWriter.WriteFullEndElement();

			foreach(StationBase neighbor in station.NeighborStations)
			{
				_xmlWriter.WriteStartElement("navneighbor");
				_xmlWriter.WriteString(neighbor.ID);
				_xmlWriter.WriteFullEndElement();
			}

			if(station.StationType == StationType.JumpGate)
			{
				JumpGate gate = (JumpGate)station;
				_xmlWriter.WriteStartElement("targetsystem");
				_xmlWriter.WriteString(gate.TargetSystem);
				_xmlWriter.WriteFullEndElement();
				_xmlWriter.WriteStartElement("exitgateid");
				_xmlWriter.WriteString(gate.ExitGateID);
				_xmlWriter.WriteFullEndElement();

				Vector3 spawnDisposition = gate.transform.forward * 15;
				_xmlWriter.WriteStartElement("spawndisposition");
				_xmlWriter.WriteStartElement("vector3");
				_xmlWriter.WriteAttributeString("x", spawnDisposition.x.ToString());
				_xmlWriter.WriteAttributeString("y", spawnDisposition.y.ToString());
				_xmlWriter.WriteAttributeString("z", spawnDisposition.z.ToString());
				_xmlWriter.WriteEndElement();
				_xmlWriter.WriteFullEndElement();
			}


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
			Vector3 disposition = tl.transform.position - origin.transform.position;
			_xmlWriter.WriteAttributeString("x", disposition.x.ToString());
			_xmlWriter.WriteAttributeString("y", disposition.y.ToString());
			_xmlWriter.WriteAttributeString("z", disposition.z.ToString());
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

			foreach(StationBase neighbor in tl.NeighborStations)
			{
				_xmlWriter.WriteStartElement("navneighbor");
				_xmlWriter.WriteString(neighbor.ID);
				_xmlWriter.WriteFullEndElement();
			}

			_xmlWriter.WriteFullEndElement();
		}




		_xmlWriter.WriteFullEndElement();
		_xmlWriter.Close();
	}



}
