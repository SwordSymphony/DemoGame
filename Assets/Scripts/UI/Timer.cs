using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI timerText;
	public float TimePassed;

	// Update is called once per frame
	void Update()
	{
		TimePassed += Time.deltaTime;
		int minutes = Mathf.FloorToInt(TimePassed / 60);
		int seconds = Mathf.FloorToInt(TimePassed % 60);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}
}
