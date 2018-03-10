using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLDockingLaser : MonoBehaviour 
{
	public LineRenderer Laser;
	public ParticleSystem LaserRoot;
	public ParticleSystem LaserParticle;
	public ParticleSystem LaserHit;

	public void Enable()
	{
		Laser.enabled = true;
		ParticleSystem.EmissionModule laserRootEm = LaserRoot.emission;
		ParticleSystem.EmissionModule laserParticleEm = LaserParticle.emission;
		ParticleSystem.EmissionModule laserHitEm = LaserHit.emission;

		laserRootEm.enabled = true;
		laserParticleEm.enabled = true;
		laserHitEm.enabled = true;
	}

	public void Disable()
	{
		Laser.enabled = false;
		ParticleSystem.EmissionModule laserRootEm = LaserRoot.emission;
		ParticleSystem.EmissionModule laserParticleEm = LaserParticle.emission;
		ParticleSystem.EmissionModule laserHitEm = LaserHit.emission;

		laserRootEm.enabled = false;
		laserParticleEm.enabled = false;
		laserHitEm.enabled = false;
	}

}
