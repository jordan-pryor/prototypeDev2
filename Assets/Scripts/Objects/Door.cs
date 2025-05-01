using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteract
{
	[SerializeField] GameObject door;
	[SerializeField, Range(0f, 1f)] float openPercent;
	[SerializeField] GameObject doorMin;
	[SerializeField] GameObject doorMax;
	[SerializeField] float openSpeed = 1f;

	[Header("Lock Settings")]
	[SerializeField] bool isLocked = false;
	[SerializeField] string requiredKeyName = "Key"; // Custom key name

	public bool isTransition = false;
	public int sceneIndex = -1;

	Coroutine currentRoutine;

	void Start()
	{
		doorMin.SetActive(false);
		doorMax.SetActive(false);
		ApplyRotation();
	}

	void OnValidate()
	{
		ApplyRotation();
	}

	void ApplyRotation()
	{
		openPercent = Mathf.Clamp01(openPercent);
		door.transform.rotation = Quaternion.Lerp(doorMin.transform.rotation, doorMax.transform.rotation, openPercent);
	}

	IEnumerator AnimateDoor(float target)
	{
		while (!Mathf.Approximately(openPercent, target))
		{
			openPercent = Mathf.MoveTowards(openPercent, target, Time.deltaTime * openSpeed);
			ApplyRotation();
			yield return null;
		}
		openPercent = target;
		ApplyRotation();
	}

	public void Open()
	{
		if (currentRoutine != null) StopCoroutine(currentRoutine);
		currentRoutine = StartCoroutine(AnimateDoor(1f));
	}

	public void Close()
	{
		if (currentRoutine != null) StopCoroutine(currentRoutine);
		currentRoutine = StartCoroutine(AnimateDoor(0f));
	}

	public void Interact()
	{
		// Check if locked
		if (isLocked)
		{
			// Try to find key in inventory
			bool hasKey = GameManager.instance.player.GetComponent<Inventory>().Search(requiredKeyName);
			if (!hasKey)
			{
				GameManager.instance.LockPrompt(); return;
			}

			isLocked = false; // Unlock if player has the key
		}

		if (openPercent < 0.01f) Open();
		else Close();
		if (isTransition && sceneIndex > -1 && SceneManager.GetActiveScene().buildIndex != sceneIndex)
		{
			SceneManager.LoadScene(sceneIndex);

        }
	}

	public bool isDoorOpen()
	{
		return openPercent >= 1f;
	}
}

