using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustController : MonoBehaviour 
{
	public List<Transform> ExhaustHolders;
	public Vector3 FlameScale;

	private ExhaustState _state;
	private List<GameObject> _exhausts;
	private List<TrailRenderer> _exhaustTrails;
	private float _normalExhaustScaleZ;
	private GameObject _flare;

	public void setExhaustLength(float length) //length is 0 to 1
	{
		_normalExhaustScaleZ = length;
		if(_state == ExhaustState.Normal)
		{
			foreach(GameObject exhaust in _exhausts)
			{
				float zScale = exhaust.transform.localScale.z;
				zScale = FlameScale.z * 0.5f + (FlameScale.z * 0.5f) * length;
				exhaust.transform.localScale = new Vector3(FlameScale.x, FlameScale.y, zScale);
			}
		}
	}

	public ExhaustState GetExhaustState()
	{
		return _state;
	}

	public void setExhaustState(ExhaustState state)
	{
		if(state == _state)
		{
			return;
		}

		if(_exhausts == null)
		{
			_exhausts = new List<GameObject>();
			_exhaustTrails = new List<TrailRenderer>();
		}

		foreach(GameObject go in _exhausts)
		{
			GameObject.Destroy(go);
		}

		_exhausts.Clear();
		_exhaustTrails.Clear();
		_state = state;

		foreach(Transform t in ExhaustHolders)
		{
			GameObject exhaust = null;
			if(state == ExhaustState.Idle)
			{
				exhaust = GameObject.Instantiate(Resources.Load("EngineFlameIdle")) as GameObject;

			}
			else if(state == ExhaustState.Normal)
			{
				exhaust = GameObject.Instantiate(Resources.Load("EngineFlameNormal")) as GameObject;
				setExhaustLength(_normalExhaustScaleZ);

				if(_flare == null)
				{
					_flare = GameObject.Instantiate(Resources.Load("EngineFlare")) as GameObject;
					_flare.transform.parent = t;
					_flare.transform.localScale = new Vector3(1, 1, 1);
					_flare.transform.localPosition = Vector3.zero;
					_flare.transform.localEulerAngles = Vector3.zero;
				}

			}
			else if(state == ExhaustState.Thruster)
			{
				exhaust = GameObject.Instantiate(Resources.Load("EngineFlameThruster")) as GameObject;
			}
			else if(state == ExhaustState.Cruise)
			{
				exhaust = GameObject.Instantiate(Resources.Load("EngineFlameCruise")) as GameObject;
			}

			exhaust.transform.parent = t;
			exhaust.transform.localScale = FlameScale;
			exhaust.transform.localPosition = Vector3.zero;
			exhaust.transform.localEulerAngles = Vector3.zero;
			ParticleSystem particle = exhaust.GetComponent<ParticleSystem>();
			ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
			renderer.maxParticleSize *= FlameScale.x;
			_exhausts.Add(exhaust);

			TrailRenderer trail = exhaust.transform.GetComponentInChildren<TrailRenderer>();
			if(trail != null)
			{
				_exhaustTrails.Add(trail);
			}

		}
	}

	public void UpdateExhaustTrail(float speed)
	{
		if(_exhaustTrails == null)
		{
			return;
		}

		foreach(TrailRenderer trail in _exhaustTrails)
		{
			trail.startWidth = Mathf.Lerp(0, 0.3f, Mathf.Clamp01(speed / 10));
		}
	}

}

public enum ExhaustState
{
	Idle,
	Normal,
	Thruster,
	Cruise,
}