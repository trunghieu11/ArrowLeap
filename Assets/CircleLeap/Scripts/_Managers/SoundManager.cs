using UnityEngine;
using System;

public class SoundManager : Singleton<SoundManager> 
{
	private const string kSoundEnabledKey = "SoundEnabled";

	private bool soundEnabled;

	private AudioSource audioSource;

	public AudioClip hitSound;
	public AudioClip crushSound;

	void Start () 
	{
		
		soundEnabled = SoundEnabled;
		audioSource = GetComponent<AudioSource>();
	}

	public bool SoundEnabled 
	{
		get 
		{
			return PlayerPrefs.GetInt(kSoundEnabledKey, 1) == 1 ? true : false;
		}
		set
		{
			soundEnabled = value;
			PlayerPrefs.SetInt(kSoundEnabledKey, value ? 1 : 0);
		}
	}

	public void EnableDisableSound(Action<bool> callback)
	{
		SoundEnabled = !soundEnabled;

		callback(soundEnabled);
	}

	public void PlayHit()
	{
		if(soundEnabled)
			audioSource.PlayOneShot(hitSound);
	}

	public void PlayCrush()
	{
		if(soundEnabled)
			audioSource.PlayOneShot(crushSound);
	}
}
