using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// singleton manager class responsible for managing all of the background
/// nodes with-in a DEVN scene
/// </summary>
public class BackgroundManager : MonoBehaviour
{
	// singleton
	private static BackgroundManager m_instance;
	
	// scene manager ref
	private SceneManager m_sceneManager;

	// references to the background elements in the scene
    [Header("Background Images")]
	[SerializeField] private Image m_imageBackground;
	[SerializeField] private Image m_colourBackground;

	#region getters

	public static BackgroundManager GetInstance() { return m_instance; }

	#endregion

	/// <summary>
	/// constructor
	/// </summary>
	void Awake ()
	{
		m_instance = this; // initialise singleton

		// cache scene manager
		m_sceneManager = GetComponent<SceneManager>();
	}

	/// <summary>
	/// set background function which determines whether to fade a background
	/// in or out, and performs the relevant coroutine
	/// </summary>
	public void SetBackground()
	{
		BackgroundNode currentNode = m_sceneManager.GetCurrentNode() as BackgroundNode;
		m_colourBackground.color = currentNode.GetFadeColour(); // set fade colour

		if (m_imageBackground.sprite == null)
		{
			// set background
			m_imageBackground.sprite = currentNode.GetBackground();

			// perform fade in
			StartCoroutine(FadeIn(m_imageBackground, currentNode.GetFadeTime())); 
		}
		else
		{
			bool isExit = currentNode.GetToggleSelection() == 1;

			// perform fade out
			StartCoroutine(FadeOut(m_imageBackground, currentNode.GetFadeTime(), isExit)); 
		}
	}

	/// <summary>
	/// helper function which performs a fade-in on an image over a 
	/// certain amount of time
	/// </summary>
	/// <param name="image">the image to fade in</param>
	/// <param name="fadeInTime">the amount of time to fade in</param>
	/// <returns>coroutine IEnumerator</returns>
	IEnumerator FadeIn(Image image, float fadeInTime = 0.5f)
	{
		float elapsedTime = 0.0f;

		BackgroundNode backgroundNode = m_sceneManager.GetCurrentNode() as BackgroundNode;
		image.sprite = backgroundNode.GetBackground();

		// instantly proceed to next node if "wait for finish" is false
		if (!backgroundNode.GetWaitForFinish())
			m_sceneManager.NextNode();

		while (elapsedTime < fadeInTime)
		{
			// set transparency
			float percentage = elapsedTime / fadeInTime;
			image.color = new Color(1, 1, 1, percentage);

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		// proceed to next node if "wait for finish" is true
		if (backgroundNode.GetWaitForFinish())
			m_sceneManager.NextNode();
	}

	/// <summary>
	/// helper function which performs a fade-out on an image over a
	/// certain amount of time, and performs a fade-in of a new background
	/// if required
	/// </summary>
	/// <param name="image">the background to fade out</param>
	/// <param name="fadeOutTime">the amount of time to fade out</param>
	/// <param name="isExit">if true, no consequence fade-in will be performed</param>
	/// <returns>coroutine IEnumerator</returns>
	IEnumerator FadeOut(Image image, float fadeOutTime = 0.5f, bool isExit = false)
	{
		float elapsedTime = 0.0f;

		while (elapsedTime < fadeOutTime)
		{
			// set transparency
			float percentage = 1 - (elapsedTime / fadeOutTime);
			image.color = new Color(1, 1, 1, percentage);

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}
		
		image.sprite = null; // clear the background

		// perform fade in if required
		if (!isExit)
			StartCoroutine(FadeIn(image, fadeOutTime));
		else
			m_sceneManager.NextNode();
	}
}

}
