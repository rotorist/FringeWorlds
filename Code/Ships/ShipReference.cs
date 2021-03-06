﻿using System.Collections;
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
	public List<Defensive> Defensives;
	public int MaxDefensives;
	public ExhaustController ExhaustController;
	public Transform FPCameraAnchor;
	public Vector3 CameraChasePoint;
	public float CameraTilt;
	public TrailRenderer VortexLeft;
	public TrailRenderer VortexRight;
	public AudioSource ShipAudio;
}
