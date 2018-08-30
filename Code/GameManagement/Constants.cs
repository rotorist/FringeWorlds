using System.Collections;
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

	public Dictionary<ShieldTech, Dictionary<DamageType, float>> ShieldDamageMultiplier;

	public void Initialize()
	{
		ShieldDamageMultiplier = new Dictionary<ShieldTech, Dictionary<DamageType, float>>()
		{
			{ ShieldTech.Gravity, new Dictionary<DamageType, float>()
				{	{ DamageType.Antimatter, 1f },
					{ DamageType.EMP, 0.7f },
					{ DamageType.Kinetic, 0.1f },
					{ DamageType.Photon, 1.5f },
					{ DamageType.Shock, 0.3f }
				}
			},
			{ ShieldTech.Magnetic, new Dictionary<DamageType, float>()
				{	{ DamageType.Antimatter, 0.75f },
					{ DamageType.EMP, 1.4f },
					{ DamageType.Kinetic, 0.25f },
					{ DamageType.Photon, 0.9f },
					{ DamageType.Shock, 0.25f }
				}
			},
			{ ShieldTech.Plasma, new Dictionary<DamageType, float>()
				{	{ DamageType.Antimatter, 1.6f },
					{ DamageType.EMP, 1.1f },
					{ DamageType.Kinetic, 0.15f },
					{ DamageType.Photon, 0.65f },
					{ DamageType.Shock, 0.2f }
				}
			}

		};

	}
}
