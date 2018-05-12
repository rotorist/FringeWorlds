using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class XMLParserBT
{
	
	private XmlDocument _currentXML; 

	public BehaviorTree LoadBehaviorTree(string treeName, AI owner)
	{
		XmlDocument xmlDoc = new XmlDocument();
		string path = Application.dataPath + "/GameData/BehaviorTree/";
		string file = File.ReadAllText(path + treeName + ".xml");
		try
		{
			xmlDoc.LoadXml(file);
		}
		catch (XmlException)
		{
			return null;
		}
		_currentXML = xmlDoc;
		XmlNodeList root = _currentXML.GetElementsByTagName("behavior");
		BehaviorTree tree = new BehaviorTree();
		tree.Name = treeName;
		tree.RootNode = LoadBTNode(root[0], owner);
		//Debug.Log("Is Root null " + (tree.RootNode == null));
		return tree;
	}

	private BTNode LoadBTNode(XmlNode currentNode, AI owner)
	{
		XmlNodeList nodeContent = currentNode.ChildNodes;

		XmlAttributeCollection nodeAttributes = currentNode.Attributes;
		BTNode node = null;
		if(currentNode.Name == "behavior")
		{
			node = LoadBTNode(nodeContent[0], owner);
		}
		else if(currentNode.Name == "composite")
		{
			BTComposite compNode = null;
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Sequence")
			{
				BTSequence seqNode = new BTSequence();
				seqNode.CompNodeType = BTCompType.Sequence;
				compNode = seqNode;
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Selector")
			{
				BTSelector selNode = new BTSelector();
				selNode.CompNodeType = BTCompType.Selector;
				compNode = selNode;
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "ParallelAnd")
			{
				BTParallelAnd parNode = new BTParallelAnd();
				parNode.CompNodeType = BTCompType.ParallelAnd;
				compNode = parNode;
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "ParallelMain")
			{
				BTParallelMain parNode = new BTParallelMain();
				parNode.CompNodeType = BTCompType.ParallelMain;
				compNode = parNode;
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Random")
			{
				BTRandom randNode = new BTRandom();
				randNode.CompNodeType = BTCompType.Random;
				compNode = randNode;
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Switch")
			{
				BTSwitch swNode = new BTSwitch();
				swNode.CompNodeType = BTCompType.Switch;
				compNode = swNode;
			}

			compNode.Children = new List<BTNode>();

			foreach(XmlNode nodeItem in nodeContent)
			{
				compNode.Children.Add(LoadBTNode(nodeItem, owner));
			}

			node = compNode;
		}
		else if(currentNode.Name == "decorator")
		{
			BTDecorator decNode = null;
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Repeat")
			{
				BTRepeat repNode = new BTRepeat();
				decNode = repNode;
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Invert")
			{
				BTInvert invNode = new BTInvert();
				decNode = invNode;
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "UntilFail")
			{
				BTUntilFail repNode = new BTUntilFail();
				decNode = repNode;
			}
			foreach(XmlNode nodeItem in nodeContent)
			{
				decNode.Child = LoadBTNode(nodeItem, owner);
			}

			node = decNode;

		}
		else if(currentNode.Name == "leaf")
		{
			XmlNodeList parameterList = currentNode.ChildNodes;
			List<string> parameters = new List<string>();
			foreach(XmlNode paramNode in parameterList)
			{
				parameters.Add(paramNode.InnerText);
			}

			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Check")
			{
				if(nodeAttributes["name"] != null)
				{
					string actionName = nodeAttributes["name"].Value;
					BTCheck checkNode = new BTCheck();
					checkNode.Action = actionName;
					checkNode.Parameters = parameters;
					checkNode.MyAI = owner;
					node = checkNode;
				}
			}
			if(nodeAttributes["type"] != null && nodeAttributes["type"].Value == "Action")
			{
				if(nodeAttributes["name"] != null)
				{
					BTLeaf leafNode = (BTLeaf)System.Activator.CreateInstance(System.Type.GetType("BT" + nodeAttributes["name"].Value));
					leafNode.Parameters = parameters;
					leafNode.MyAI = owner;
					node = leafNode;
				}
			}


		}

		return node;
	}
}
