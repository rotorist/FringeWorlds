﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour 
{

	public AnimationCurve MouseYawCurve;
	public AnimationCurve MousePitchCurve;
	public AnimationCurve RollCurve;
	public AnimationCurve ShieldProtectionCurve;
	public AnimationCurve PlayerShieldProtectionCurve;
	public AnimationCurve CameraFollowCurve;
	public AnimationCurve MarkerEnlargeCurve;

	public Dictionary<ShieldTech, Dictionary<DamageType, float>> ShieldDamageMultiplier;

	public int MaxTraderParties;

	public void Initialize()
	{
		ShieldDamageMultiplier = new Dictionary<ShieldTech, Dictionary<DamageType, float>>()
		{
			{ ShieldTech.Gravity, new Dictionary<DamageType, float>()
				{	{ DamageType.Antimatter, 1f },
					{ DamageType.EMP, 0.7f },
					{ DamageType.Kinetic, 0.6f },
					{ DamageType.Photon, 1.5f },
					{ DamageType.Shock, 0.3f },
					{ DamageType.Ion, 1f },
				}
			},
			{ ShieldTech.Magnetic, new Dictionary<DamageType, float>()
				{	{ DamageType.Antimatter, 0.75f },
					{ DamageType.EMP, 1.4f },
					{ DamageType.Kinetic, 0.45f },
					{ DamageType.Photon, 0.9f },
					{ DamageType.Shock, 0.25f },
					{ DamageType.Ion, 1.2f },
				}
			},
			{ ShieldTech.Plasma, new Dictionary<DamageType, float>()
				{	{ DamageType.Antimatter, 1.6f },
					{ DamageType.EMP, 1.1f },
					{ DamageType.Kinetic, 0.9f },
					{ DamageType.Photon, 0.65f },
					{ DamageType.Shock, 0.2f },
					{ DamageType.Ion, 0.75f },
				}
			}

		};

	}
}
