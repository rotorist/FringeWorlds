using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLDockingEffect : MonoBehaviour 
{

	public ParticleSystem OuterRing;
	public ParticleSystem InnerRing;
	public ParticleSystem CenterDust;
	public ParticleSystem Dust;
	public ParticleSystem ForwardRing1;
	public ParticleSystem ForwardRing2;

	public TLDockingLaser Laser1;
	public TLDockingLaser Laser2;
	public TLDockingLaser Laser3;
	public TLDockingLaser Laser4;


	public void SetStage(int stage)
	{
		//0 = nothing shows
		//1 = only outer ring
		//2 = everything
		//Debug.Log("Setting stage " + stage);
		ParticleSystem.EmissionModule outerRingEm = OuterRing.emission;
		ParticleSystem.EmissionModule innerRingEm = InnerRing.emission;
		ParticleSystem.EmissionModule centerDustEm = CenterDust.emission;
		ParticleSystem.EmissionModule dustEM = Dust.emission;
		ParticleSystem.EmissionModule forwardRing1Em = ForwardRing1.emission;
		ParticleSystem.EmissionModule forwardRing2Em  = ForwardRing2.emission;

		if(stage == 1)
		{
			outerRingEm.enabled = true;
			innerRingEm.enabled = false;
			centerDustEm.enabled = false;
			dustEM.enabled = false;
			forwardRing1Em.enabled = false;
			forwardRing2Em.enabled = false;
			Laser1.Enable();
			Laser2.Enable();
			Laser3.Enable();
			Laser4.Enable();
		}
		else if(stage == 2)
		{
			outerRingEm.enabled = true;
			innerRingEm.enabled = true;
			centerDustEm.enabled = true;
			dustEM.enabled = true;
			forwardRing1Em.enabled = true;
			forwardRing2Em.enabled = true;
			Laser1.Enable();
			Laser2.Enable();
			Laser3.Enable();
			Laser4.Enable();
		}
		else
		{
			outerRingEm.enabled = false;
			innerRingEm.enabled = false;
			centerDustEm.enabled = false;
			dustEM.enabled = false;
			forwardRing1Em.enabled = false;
			forwardRing2Em.enabled = false;
			Laser1.Disable();
			Laser2.Disable();
			Laser3.Disable();
			Laser4.Disable();
		}
	}
}
