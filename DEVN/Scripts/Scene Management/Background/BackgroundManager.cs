using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// background manager class responsible for fading backgrounds in and out
/// </summary>
public class BackgroundManager : MonoBehaviour
{
	// scene manager ref
	private SceneManager m_sceneManager;

	// references to the background elements in the scene
    [Header("Background Images")]
	[SerializeField] private Image m_imageBackground;
	[SerializeField] private Image m_colourBackground;

	/// <summary>
	/// constructor
	/// </summary>
	void Awake ()
	{
		// cache scene manager reference
		m_sceneManager = GetComponent<SceneManager>();
		Debug.Assert(m_sceneManager != null, "DEVN: SceneManager cache unsuccessful!");
	}
		
	/// <summary>
	/// use this to set a new background/environment and perform a fade-in!
	/// </summary>
	/// <param name="background">the image sprite of the new background</param>
	/// <param name="fadeColour">the colour of fade-in (e.g. Color.black or Color.white)</param>
	/// <param name="fadeInTime">the time it takes to fade in, don't make this too long! Recommended: 0.5f to 3.0f</param>
	/// <param name="nextNode">do you want to proceed to the next node of the current scene?
	/// If you're calling this function manually, you probably want to leave this as false</param>
	/// <param name="waitForFinish">this only matters if nextNode is true. If true, the scene waits
	/// until the background has finished fading in before proceeding to the next node</param>
	public void EnterBackground(Sprite background, Color fadeColour, float fadeInTime = 0.5f, 
		bool nextNode = false, bool waitForFinish = false)
	{
		m_colourBackground.color = fadeColour; // set fade colour
			
		StartCoroutine(FadeIn(background, fadeInTime, nextNode, waitForFinish)); // perform fade in
	}

	/// <summary>
	/// use this to perform a fade-out of the current background/environment!
	/// </summary>
	/// <param name="fadeColour">the colour of fade-in (e.g. Color.black or Color.white)</param>
	/// <param name="fadeOutTime">the time it takes to fade out, don't make this too long! Recommended: 0.5f to 3.0f</param>
	public void ExitBackground(Color fadeColour, float fadeOutTime = 0.5f)
	{
		m_colourBackground.color = fadeColour; // set fade colour

		StartCoroutine(FadeOut(fadeOutTime)); // perform fade out
	}
	

	/// <summary>
	/// helper function which performs a fade-in on an image over a certain amount of time
	/// </summary>
	/// <param name="background">the background/environment image</param>
	/// <param name="fadeInTime">the amount of time to fade in</param>
	/// <param name="nextNode">proceed to next node?</param>
	/// <param name="waitForFinish">wait for fade to finish before proceeding to next node?</param>
	private IEnumerator FadeIn(Sprite background, float fadeInTime = 0.5f, bool nextNode = false,
		bool waitForFinish = false)
	{
		if (m_imageBackground.sprite != null)
			Debug.LogWarning("DEVN: Perform Background \"Exit\" before entering another background!");

		m_imageBackground.sprite = background;

		// instantly proceed to next node if "wait for finish" is false
		if (!waitForFinish && nextNode)
			m_sceneManager.NextNode();

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeInTime)
		{
			// set transparency
			float percentage = elapsedTime / fadeInTime;
			m_imageBackground.color = new Color(1, 1, 1, percentage);

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		// proceed to next node if "wait for finish" is true
		if (waitForFinish && nextNode)
			m_sceneManager.NextNode();
	}

	/// <summary>
	/// helper function which performs a fade-out on an image over a
	/// certain amount of time, and performs a fade-in of a new background
	/// if required
	/// </summary>
	/// <param name="fadeOutTime">the amount of time to fade out</param>
	private IEnumerator FadeOut(float fadeOutTime = 0.5f)
	{
		float elapsedTime = 0.0f;

		while (elapsedTime < fadeOutTime)
		{
			// set transparency
			float percentage = 1 - (elapsedTime / fadeOutTime);
			m_imageBackground.color = new Color(1, 1, 1, percentage);

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		m_imageBackground.sprite = null; // clear the background
	}
}

}
