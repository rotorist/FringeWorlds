using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelLoc
{
	private Transform _origin;
	private Vector3 _refPos;
	private Vector3 _disposition;

	public Vector3 RealPos
	{
		get 
		{ 
			if(_origin == null)
			{
				return _refPos + _disposition;
			}
			else
			{
				return _origin.position + _disposition;
			}
		}
		set 
		{ 
			if(_origin == null)
			{
				_disposition = value - _refPos; 
			}
			else
			{
				_disposition = value - _origin.position;
			}
		}
	}


	public RelLoc(Vector3 refPos, Vector3 realPos, Transform origin)
	{
		_refPos = refPos;
		_disposition = realPos - refPos;
		_origin = origin;
	}


}
