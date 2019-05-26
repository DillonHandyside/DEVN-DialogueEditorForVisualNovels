using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DEVN.Components;

namespace DEVN
{

namespace SceneManagement
{

/// <summary>
/// background manager class responsible for fading backgrounds in and out
/// </summary>
public class BackgroundManager
{
	// references to the background elements in the scene
	private Image m_imageBackground;
	private Image m_colourBackground;

    private BackgroundComponent m_backgroundComponent;

	#region getters

	public Sprite GetBackground() { return m_imageBackground.sprite; }

	#endregion

	/// <summary>
	/// are you sure you want to construct your own BackgroundManager? You may want to use 
	/// SceneManager.GetInstance().GetBackgroundManager() instead
	/// </summary>
	/// <param name="backgroundComponent">a background component which houses the relevent UI elements</param>
	public BackgroundManager(BackgroundComponent backgroundComponent)
	{
        m_backgroundComponent = backgroundComponent;

        // assign references to the relevant UI elements
        m_imageBackground = backgroundComponent.GetImageBackground();
		m_colourBackground = backgroundComponent.GetColourBackground();
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

		// perform fade in
		m_backgroundComponent.StartCoroutine(FadeIn(background, fadeInTime, nextNode, waitForFinish)); 
	}

	/// <summary>
	/// use this to perform a fade-out of the current background/environment!
	/// </summary>
	/// <param name="fadeColour">the colour of fade-in (e.g. Color.black or Color.white)</param>
	/// <param name="fadeOutTime">the time it takes to fade out, don't make this too long! Recommended: 0.5f to 3.0f</param>
	public void ExitBackground(Color fadeColour, float fadeOutTime = 0.5f, bool nextNode = false)
	{
		m_colourBackground.color = fadeColour; // set fade colour

		m_backgroundComponent.StartCoroutine(FadeOut(fadeOutTime, nextNode)); // perform fade out
	}

	/// <summary>
	/// use this to instantly set a background, without fade-in!
	/// </summary>
	/// <param name="background">the background/environment image</param>
	public void SetBackground(Sprite background)
	{
		m_imageBackground.sprite = background;
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

        SceneManager sceneManager = SceneManager.GetInstance();

		// instantly proceed to next node if "wait for finish" is false
		if (sceneManager != null && !waitForFinish && nextNode)
			sceneManager.NextNode();

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
		if (sceneManager != null && waitForFinish && nextNode)
			sceneManager.NextNode();
	}

	/// <summary>
	/// helper function which performs a fade-out on an image over a
	/// certain amount of time, and performs a fade-in of a new background
	/// if required
	/// </summary>
	/// <param name="fadeOutTime">the amount of time to fade out</param>
	private IEnumerator FadeOut(float fadeOutTime = 0.5f, bool nextNode = false)
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

        SceneManager sceneManager = SceneManager.GetInstance();
		if (sceneManager != null && nextNode)
			sceneManager.NextNode();
	}
}

}

}