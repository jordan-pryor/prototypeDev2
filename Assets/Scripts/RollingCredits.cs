using UnityEngine;
using UnityEngine.SceneManagement;

public class RollingCredits : MonoBehaviour
{
	public RectTransform creditsText;
	public float scrollSpeed = 50f;
	public float delayBeforeScroll = 1f;
	public float endDelay = 5f;

	private float startTime;
	private float screenHeight;

	void Start()
	{
		startTime = Time.time + delayBeforeScroll;
		screenHeight = Screen.height;
	}

	void Update()
	{
		// MAKE IT SCROLL MAKE IT SCROLL
		if (Time.time >= startTime)
		{
			creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

			if (creditsText.anchoredPosition.y >= creditsText.rect.height + screenHeight)
			{
				Invoke("ExitCredits", endDelay);
			}
		}

		if (Input.anyKeyDown)
		{
			ExitCredits();
		}
	}
	// Kick 'em to the Main Menu
	void ExitCredits()
	{
		SceneManager.LoadScene("Title Screen");
	}
}