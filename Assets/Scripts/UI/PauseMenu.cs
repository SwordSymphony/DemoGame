using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static bool gameIsPaused = false;
	public GameObject PauseMenuUI;
	public GameObject SettingsUI;
	public static AudioManager audioManager;

	public bool gameOver;

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!gameOver)
			{
				if (gameIsPaused)
				{
					Resume();
					if (SettingsUI.activeSelf == true)
					{
						SettingsUI.SetActive(false);
					}
				}
				else 
				{
					Pause();
				}
			}
		}
	}

	public void Resume()
	{
		audioManager.PlayOneShot("UIButton");
		PauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		gameIsPaused = false;
		audioManager.Play("SagaOfTheSeaWolves", true);
	}

	void Pause()
	{
		audioManager.PlayOneShot("UIButton");
		PauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		gameIsPaused = true;
		audioManager.Pause("SagaOfTheSeaWolves");
	}

	public void ReturnToMenu()
	{
		audioManager.PlayOneShot("UIButton");
		Time.timeScale = 1f;

		SceneManager.LoadScene("MainMenu");

		audioManager.Stop("SagaOfTheSeaWolves");
		audioManager.Play("RunningWild", true);
	}

	public void Settings()
	{
		audioManager.PlayOneShot("UIButton");

	}

	public void QuitGame()
	{
		audioManager.PlayOneShot("UIButton");
		Debug.Log("quitgame");
		Application.Quit();
	}
}
