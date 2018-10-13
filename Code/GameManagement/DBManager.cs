using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class DBManager
{
	public XMLParserBT XMLParserBT;
	public XMLParserWorld XMLParserWorld;
	public UserPrefDataHandler UserPrefDataHandler;

	public void Initialize()
	{
		XMLParserBT = new XMLParserBT();
		XMLParserWorld = new XMLParserWorld();
		UserPrefDataHandler = new UserPrefDataHandler();
	}



	public static Color ParseXmlColorNode(XmlNode colorNode)
	{
		float r, g, b;
		XmlAttributeCollection colorAttrs = colorNode.Attributes;
		r = Convert.ToSingle(colorAttrs["r"].Value.ToString());
		g = Convert.ToSingle(colorAttrs["g"].Value.ToString());
		b = Convert.ToSingle(colorAttrs["b"].Value.ToString());
		//Debug.Log(r.ToString() + g.ToString() + b.ToString());
		return new Color(r, g, b);
	}

	public static Vector3 ParseXmlVector3(XmlNode v3Node)
	{
		float x, y, z;
		XmlAttributeCollection v3Attrs = v3Node.Attributes;
		x = Convert.ToSingle(v3Attrs["x"].Value.ToString());
		y = Convert.ToSingle(v3Attrs["y"].Value.ToString());
		z = Convert.ToSingle(v3Attrs["z"].Value.ToString());
		//Debug.Log(x.ToString() + y.ToString() + z.ToString());
		return new Vector3(x, y, z);
	}
}
