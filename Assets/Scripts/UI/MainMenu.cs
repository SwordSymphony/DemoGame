using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
	public static AudioManager audioManager;

	public void Awake()
	{
		audioManager = FindObjectOfType<AudioManager>();
	}

	public void startGame()
	{
		SceneManager.LoadScene(1);

		audioManager.Stop("RunningWild");
		audioManager.Play("SagaOfTheSeaWolves", true);
	}

	public void loadGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void exitGame()
	{
		Application.Quit();
	}
}
