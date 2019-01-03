using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager
{
	public AudioSource Music;
	public AudioSource UI;

	private Dictionary<string, AudioClip> _audioClipPool;
	private AudioClip _nextMusicClip;
	private float _lastUISoundTime;

	public void PerFrameUpdate()
	{

		if(!Music.isPlaying)
		{
			if(_nextMusicClip != null)
			{
				Music.clip = _nextMusicClip;
				//Music.Play();
			}

		}
	}



	public AudioClip GetClip(string id)
	{
		if(_audioClipPool.ContainsKey(id))
		{
			return _audioClipPool[id];
		}
		else
		{
			//try to load the clip
			AudioClip clip = Resources.Load(id) as AudioClip;
			if(clip == null)
			{
				return null;
			}
			else
			{
				_audioClipPool.Add(id, clip);
				return clip;
			}
		}
	}



	public void PlayUISoundRateLimited(string clipName, float minDelay)
	{
		if(Time.time - _lastUISoundTime > minDelay)
		{
			UI.PlayOneShot(GetClip(clipName));
			_lastUISoundTime = Time.time;
		}
	}



	public void Initialize()
	{
		
		_audioClipPool = new Dictionary<string, AudioClip>();
		UI = GameObject.Find("AudioSourceUI").GetComponent<AudioSource>();
		Music = GameManager.Inst.MainCamera.GetComponent<AudioSource>();


	}
}
