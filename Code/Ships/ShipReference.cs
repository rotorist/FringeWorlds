using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipReference : MonoBehaviour 
{
	public string Name;
	public ShipBase ParentShip;
	public GameObject Shield;
	public float HologramScale;
	public ShipType ShipType;
	public AvoidanceDetector AvoidanceDetector;
	public List<WeaponJoint> WeaponJoints;

}
