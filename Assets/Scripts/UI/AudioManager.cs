using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
	// public static AudioManager instance;
	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	public Slider musicSlider;
	public Slider soundSlider;

	Scene currentScene;
	Scene scene;

	public float musicVolume;
	public float soundsVolume;

	void Awake()
	{
		// if (instance != null)
		// {
		// 	Destroy(gameObject);
		// }
		// else
		// {
		// 	instance = this;
		// 	DontDestroyOnLoad(gameObject);
		// }

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
		musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
		soundsVolume = PlayerPrefs.GetFloat("SoundsVolume", 0.5f);
		musicSlider.value = musicVolume;
		soundSlider.value = soundsVolume;

		scene = SceneManager.GetActiveScene();
		if (scene.name == "level_1_new")
		{
			Play("SagaOfTheSeaWolves", true);
			
			Debug.Log("play level 1");
		}
		if (scene.name == "MainMenu")
		{
			Play("RunningWild", true);
			Debug.Log("play main menu");
		}
	}

	void Update()
	{
		if (currentScene != scene)
		{
			currentScene = scene;
		}
	}

	public void SetMusicVolume(float _volume)
	{
		string sound;
		if (scene.name == "MainMenu")
		{
			sound = "RunningWild";
		}
		else
		{
			sound = "SagaOfTheSeaWolves";
		}
		
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		s.source.volume = musicVolume;

		musicVolume = _volume;
		PlayerPrefs.SetFloat("MusicVolume", _volume);
	}

	public void SetSoundVolume(float _volume)
	{
		soundsVolume = _volume;
		PlayerPrefs.SetFloat("SoundsVolume", _volume);
	}

	public void PlayOneShot(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		s.source.volume = soundsVolume;
		// s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.PlayOneShot(s.source.clip, 1);
	}

	public void Play(string sound, bool music)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		if (music)
		{
			s.source.volume = musicVolume;
		}
		else
		{
			s.source.volume = soundsVolume;
		}
		// s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

	public void Pause(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.Pause();
	}

	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.Stop();
	}

}
