using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private bool mouse_over = false;
	public static AudioManager audioManager;

	// [SerializeField] private Button Upgrade_button1;

	void Awake()
	{
		audioManager = FindObjectOfType<AudioManager>();
	}

	void Update()
	{
		if (mouse_over)
		{
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		audioManager.PlayOneShot("UIButton");
		transform.GetChild(1).gameObject.SetActive(true);
		mouse_over = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		transform.GetChild(1).gameObject.SetActive(false);
		mouse_over = false;
	}
}
