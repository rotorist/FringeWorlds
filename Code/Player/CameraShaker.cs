using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour 
{
	public Camera MainCamera;


	private float _tempSlowDuration;
	private float _tempSlowIntensity;
	private float _tempSlowTimer;
	private float _shakeIntensity;
	private float _shakeDuration;
	private float _shakeTimer;
	private Vector3 _shakeCenter;
	private float _zoomIntensity;
	private float _zoomDuration;
	private float _zoomTimer;
	private bool _isSlowShake;
	private int _counter;

	// Update is called once per frame
	void Update()
	{
		if(_tempSlowDuration != 0 && !GameManager.Inst.PlayerControl.IsGamePaused)
		{
			HandleTempSlow();
		}
	}

	void FixedUpdate () 
	{
		

		if(_shakeDuration != 0 && !GameManager.Inst.PlayerControl.IsGamePaused)
		{
			if(_isSlowShake)
			{
				_counter ++;

				if(_counter % 2 > 0)
				{
					return;
				}
			}
			if(_shakeCenter == Vector3.zero)
			{
				HandleScreenShake();
			}
			else
			{
				HandleDirectionalShake();
			}
		}

		if(_zoomDuration != 0 && !GameManager.Inst.PlayerControl.IsGamePaused)
		{
			HandleZoomShake();
		}


	}


	public void Initialize()
	{
		
	}

	public void TriggerTempSlow(float duration, float intensity)
	{
		_tempSlowTimer = 0;
		_tempSlowIntensity = intensity;
		_tempSlowDuration = duration;
	}

	public void TriggerScreenShake(float duration, float intensity, bool isSlowShake)
	{
		if(_shakeDuration != 0)
		{
			if(intensity > _shakeIntensity)
			{
				_shakeIntensity = intensity;
				_shakeDuration = duration;
				_shakeTimer = 0;
				_isSlowShake = isSlowShake;
			}
		}
		else
		{
			_shakeIntensity = intensity;
			_shakeDuration = duration;
			_shakeTimer = 0;
			_isSlowShake = isSlowShake;
		}
	}

	public void TriggerZoomShake(float duration, float intensity)
	{
		_zoomIntensity = intensity;
		_zoomDuration = duration;
		_zoomTimer = 0;
	}

	public void TriggerDirectionalShake(float duration, float intensity, Vector3 center)
	{
		_shakeIntensity = intensity;
		_shakeDuration = duration;
		_shakeCenter = center;
		_shakeTimer = 0;
	}



	private void HandleTempSlow()
	{
		if(_tempSlowTimer < _tempSlowDuration / 2)
		{
			_tempSlowTimer += Time.deltaTime;
			Time.timeScale = Mathf.Lerp(Time.timeScale, 0, _tempSlowIntensity * Time.deltaTime);
		}
		else if(_tempSlowTimer >= _tempSlowDuration / 2 && _tempSlowTimer <= _tempSlowDuration)
		{
			_tempSlowTimer += Time.deltaTime;
			Time.timeScale = Mathf.Lerp(Time.timeScale, GameManager.Inst.MaxTimeScale, _tempSlowIntensity * Time.deltaTime);
		}
		else
		{
			Time.timeScale = GameManager.Inst.MaxTimeScale;
			_tempSlowTimer = 0;
			_tempSlowDuration = 0;
		}
	}

	private void HandleScreenShake()
	{
		if(_shakeTimer < _shakeDuration)
		{
			MainCamera.transform.localPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f) * _shakeIntensity, UnityEngine.Random.Range(-1f, 1f) * _shakeIntensity, UnityEngine.Random.Range(-1f, 1f) * _shakeIntensity);
		}
		else
		{
			_shakeDuration = 0;
			_shakeCenter = Vector3.zero;
		}

		_shakeTimer += Time.fixedDeltaTime;
	}

	private void HandleZoomShake()
	{
		if(_zoomTimer < _zoomDuration)
		{
			MainCamera.fieldOfView = MainCamera.fieldOfView - UnityEngine.Random.Range(-1f, 1f) * _zoomIntensity;
		}
		else
		{
			_zoomDuration = 0;
		}

		_zoomTimer += Time.fixedDeltaTime;
	}

	private void HandleDirectionalShake()
	{
		if(_shakeTimer < _shakeDuration)
		{
			MainCamera.transform.localPosition = new Vector3(
				_shakeCenter.x + UnityEngine.Random.Range(-1f, 1f) * _shakeIntensity, 
				_shakeCenter.y + UnityEngine.Random.Range(-1f, 1f) * _shakeIntensity, 
				UnityEngine.Random.Range(-1f, 1f) * _shakeIntensity
			);
		}
		else
		{
			_shakeDuration = 0;
			_shakeCenter = Vector3.zero;
			MainCamera.transform.localPosition = Vector3.zero;
		}

		_shakeTimer += Time.fixedDeltaTime;
	}
}
