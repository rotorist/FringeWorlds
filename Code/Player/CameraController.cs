using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour 
{
	public bool IsFirstPerson;
	public Camera CloseCamera;
	public Camera FarCamera;
	public Transform FollowTarget;


	private float _blurSpeed;
	private bool _isBlurOn;
	private BlurOptimized _cameraBlur;
	private float _xFactor;
	private float _yFactor;
	private float _tiltFactor;
	private float _prevForwardVel;
	private float _prevAcceleration;

	public void Initialize()
	{
		_cameraBlur = CloseCamera.GetComponent<BlurOptimized>();
	}

	public void PerFrameUpdate()
	{
		if(GameManager.Inst.SceneType == SceneType.Space)
		{
			
		}
		else if(GameManager.Inst.SceneType == SceneType.Station)
		{
			UpdateCameraBlur();
		}
	}

	public void PerFrameFixedUpdate()
	{
		if(GameManager.Inst.SceneType == SceneType.Space || GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			if(IsFirstPerson)
			{
				UpdateFirstPerson();
			}
			else
			{
				UpdateCameraFollow();
			}
		}
	}

	public void SetView(bool isFirstPerson)
	{
		IsFirstPerson = isFirstPerson;
	}

	public void SetCameraBlur(float speed, bool isOn)
	{
		
		_isBlurOn = isOn;
		_blurSpeed = speed;
		_cameraBlur.enabled = true;


		if(_blurSpeed >= 1000)
		{
			_cameraBlur.enabled = isOn;
			if(isOn)
			{
				_cameraBlur.blurSize = 4;
			}
			else
			{
				_cameraBlur.blurSize = 0;
			}

		}
		else
		{
			if(isOn)
			{
				_cameraBlur.blurSize = 0;
			}
			else
			{
				_cameraBlur.blurSize = 4;
			}
		}
	}

	private void UpdateFirstPerson()
	{
		ShipBase playerShip = GameManager.Inst.PlayerControl.PlayerShip;
		if(playerShip.MyReference.FPCameraAnchor != null)
		{
			transform.parent = playerShip.MyReference.FPCameraAnchor;
			transform.localEulerAngles = Vector3.zero;
			transform.localPosition = Vector3.zero;
		}
	}

	private void UpdateCameraFollow()
	{
		ShipBase playerShip = GameManager.Inst.PlayerControl.PlayerShip;
		Vector3 cameraChasePoint = playerShip.MyReference.CameraChasePoint;
		Vector3 followTarget = playerShip.transform.position - playerShip.transform.forward * cameraChasePoint.z + playerShip.transform.up * cameraChasePoint.y;
		FollowTarget.transform.position = followTarget;

		float deltaTime = Time.deltaTime;
		//if(playerShip.IsInPortal)
		{
			deltaTime = Time.fixedDeltaTime;
		}


		FollowTarget.LookAt(playerShip.transform.position + playerShip.transform.forward * 100, playerShip.transform.up);


		Vector2 mousePos = Input.mousePosition;
		Vector2 mousePosNorm = new Vector2((mousePos.x / Screen.width) - 0.5f, (mousePos.y / Screen.height) - 0.5f) * 2;

		float yawForce = GameManager.Inst.PlayerControl.YawForce;
		float pitchForce = GameManager.Inst.PlayerControl.PitchForce * -1;
		float rollForce = GameManager.Inst.PlayerControl.RollForce * -1;
		float tiltAmount = playerShip.MyReference.CameraTilt;
		_tiltFactor += Mathf.Clamp(yawForce + rollForce * 0.66f, -1 * tiltAmount, tiltAmount) * deltaTime * 3f;
		_xFactor += yawForce * deltaTime * 1f; //* 0.75f;
		_yFactor += pitchForce * deltaTime * 2.1f; // * 1.75f;


		_tiltFactor = Mathf.Lerp(_tiltFactor, 0, deltaTime * 2f);
		_xFactor = Mathf.Lerp(_xFactor, 0, deltaTime * 2f);

		if(_yFactor > 0)
		{
			_yFactor = Mathf.Lerp(_yFactor, 0, deltaTime * 5f);
		}
		else
		{
			_yFactor = Mathf.Lerp(_yFactor, 0, deltaTime * 1.5f);
		}
			
		playerShip.ShipModel.transform.localEulerAngles = new Vector3(0, 0, _tiltFactor * -20);


		Vector3 camTarget = FollowTarget.transform.position + _xFactor * FollowTarget.transform.right * 3f + _yFactor * FollowTarget.transform.up * 1.5f;
		Vector3 dist = camTarget - transform.position;


		transform.position = camTarget;

		Quaternion lookRotation = Quaternion.LookRotation(playerShip.transform.forward, playerShip.transform.up);
		transform.rotation = lookRotation;//Quaternion.Lerp(transform.rotation, lookRotation, deltaTime * 12);

		float forwardVel = Vector3.Dot(playerShip.RB.velocity, playerShip.transform.forward);
		float acceleration = (forwardVel - _prevForwardVel) / Time.fixedDeltaTime;
		_prevForwardVel = forwardVel;
		float fovTarget = Mathf.Clamp(acceleration / 3f, -0.5f, 1f);


		_prevAcceleration = acceleration;

		if(playerShip.Engine.IsCruising)
		{
			fovTarget = 1;
		}
		else if(playerShip.Engine.IsThrusting)
		{
			if(GameManager.Inst.PlayerControl.ThrusterForce >= 0)
			{
				fovTarget = 1;
			}
			else
			{
				fovTarget = -0.5f;
			}
		}



		if(!playerShip.IsInPortal)
		{
			CloseCamera.fieldOfView = Mathf.Lerp(CloseCamera.fieldOfView, 65 + 15 * fovTarget, deltaTime * 2.5f);
			FarCamera.fieldOfView = CloseCamera.fieldOfView;
		}
	}

	private void UpdateCameraBlur()
	{
		
		if(_isBlurOn)
		{
			_cameraBlur.blurSize = Mathf.Clamp(_cameraBlur.blurSize + Time.deltaTime * _blurSpeed, 0, 4f);
		}
		else
		{
			_cameraBlur.blurSize = Mathf.Clamp(_cameraBlur.blurSize - Time.deltaTime * _blurSpeed, 0, 4f);
			if(_cameraBlur.blurSize <= 0)
			{
				_cameraBlur.enabled = false;
			}
		}
	}

}
