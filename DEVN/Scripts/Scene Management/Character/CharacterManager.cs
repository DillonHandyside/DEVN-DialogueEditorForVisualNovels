using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DEVN.Components;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace SceneManagement
{

/// <summary>
/// 
/// </summary>
public class CharacterManager
{
    private readonly CharacterComponent m_characterComponent;
	private CharacterTransformer m_characterTransformer;
		
	private readonly GameObject m_characterPrefab;
	private readonly RectTransform m_backgroundPanel;
	private readonly RectTransform m_foregroundPanel;
		
	private List<GameObject> m_characters;

	#region getters
        
	public CharacterTransformer GetCharacterTransformer() { return m_characterTransformer; }
    public RectTransform GetBackgroundPanel() { return m_backgroundPanel; }
	public List<GameObject> GetCharacters() { return m_characters; }

	#endregion

	/// <summary>
	/// are you sure you want to construct your own CharacterManager? You may want to use 
	/// SceneManager.GetInstance().GetCharacterManager() instead
	/// </summary>
	/// <param name="characterComponent">a character component which houses the relevant prefab
	/// and UI elements</param>
	public CharacterManager(CharacterComponent characterComponent)
	{
        m_characterComponent = characterComponent;

		// assign references to all the relevant character elements
		m_characterPrefab = characterComponent.GetCharacterPrefab();
		m_backgroundPanel = characterComponent.GetBackgroundPanel();
		m_foregroundPanel = characterComponent.GetForegroundPanel();

		// create character transformer for scaling/translation
		m_characterTransformer = new CharacterTransformer();
		m_characters = new List<GameObject>(); // initialise list of characters
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <param name="sprite"></param>
	/// <param name="xPosition"></param>
	/// <param name="fadeInTime"></param>
	/// <param name="waitForFinish"></param>
    /// <param name="nextNode"></param>
	/// <param name="isInvert"></param>
	public void EnterCharacter(Character character, Sprite sprite, float xPosition,
		float fadeInTime = 0.5f, bool nextNode = false, bool waitForFinish = false, bool isInvert = false)
	{
		// check to see if the character already exists in the scene
		if (TryGetCharacter(character) != null)
		{
			Debug.LogWarning("DEVN: Do not attempt to enter two of the same characters.");

            SceneManager sceneManager = SceneManager.GetInstance();
            if (sceneManager != null && nextNode)
                sceneManager.NextNode();
			return;
		}

		// create new character and parent it to canvas
		GameObject characterObject = Object.Instantiate(m_characterPrefab, m_backgroundPanel);

		// invert if necessary
		if (isInvert)
			m_characterTransformer.SetCharacterScale(characterObject, new Vector2(-1, 1));

		// set sprite
		SetSprite(characterObject, sprite); 

		// set position
		m_characterTransformer.SetCharacterPosition(characterObject, new Vector2(xPosition, 0)); 

		// set character info
		Components.CharacterInfo characterInfo = characterObject.GetComponent<Components.CharacterInfo>();
		characterInfo.SetCharacter(character);

		m_characters.Add(characterObject);

		// perform fade-in
		m_characterComponent.StartCoroutine(FadeIn(characterObject, fadeInTime, nextNode, waitForFinish));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <param name="sprite"></param>
	/// <param name="fadeOutTime"></param>
    /// <param name="nextNode"></param>
	/// <param name="waitForFinish"></param>
	public void ExitCharacter(Character character, Sprite sprite, float fadeOutTime = 0.5f, 
		bool nextNode = false, bool waitForFinish = false)
	{
		GameObject characterObject = TryGetCharacter(character);

		if (characterObject == null)
		{
		    Debug.LogWarning("DEVN: Do not attempt to exit a character that is not in the scene.");

            SceneManager sceneManager = SceneManager.GetInstance();
            if (sceneManager != null && nextNode)
                sceneManager.NextNode();
            return;
		}

		SetSprite(characterObject, sprite); // set sprite

		// perform fade-out and removal
		m_characterComponent.StartCoroutine(FadeOut(characterObject, fadeOutTime, nextNode, waitForFinish));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	public GameObject TryGetCharacter(Character character)
	{
		// iterate over all characters in scene
		for (int i = 0; i < m_characters.Count; i++)
		{
			GameObject existingCharacter = m_characters[i];
			Components.CharacterInfo characterInfo = existingCharacter.GetComponent<Components.CharacterInfo>();

			if (characterInfo.GetCharacter() == character)
				return existingCharacter; // character found
		}

		return null; // character not found
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <param name="sprite"></param>
	public void SetSprite(GameObject character, Sprite sprite)
	{
		Image characterImage = character.GetComponent<Image>();
		characterImage.sprite = sprite;
		characterImage.preserveAspect = true;
	}
		
	/// <summary>
	/// 
	/// </summary>
	/// <param name="speakingCharacter"></param>
	public void HighlightSpeakingCharacter(Character speakingCharacter)
	{
		for (int i = 0; i < m_characters.Count; i++)
		{
			GameObject character = m_characters[i];
			character.transform.SetParent(m_backgroundPanel.transform);
			character.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
		}

		GameObject characterToHighlight = TryGetCharacter(speakingCharacter);
		characterToHighlight.transform.SetParent(m_foregroundPanel.transform);
		characterToHighlight.GetComponent<Image>().color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="characters"></param>
	public void SetCharacters(List<GameObject> characters)
	{
		for (int i = 0; i < m_characters.Count; i++)
			Object.Destroy(m_characters[i]);
		
		m_characters.Clear();

		for (int i = 0; i < characters.Count; i++)
		{
			GameObject character = Object.Instantiate(characters[i], m_backgroundPanel);
			m_characters.Add(character);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <param name="fadeInTime"></param>
    /// <param name="nextNode"></param>
	/// <param name="waitForFinish"></param>
	/// <returns></returns>
	private IEnumerator FadeIn(GameObject character, float fadeInTime = 0.5f, 
        bool nextNode = false, bool waitForFinish = false)
	{
        SceneManager sceneManager = SceneManager.GetInstance();

		// if not "wait for finish", jump straight to next node
		if (sceneManager != null && nextNode && !waitForFinish)
			sceneManager.NextNode();

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeInTime)
		{
			// adjust character transparency
			float percentage = elapsedTime / fadeInTime;
			character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);
				
			elapsedTime += Time.deltaTime; // increment time
			yield return null;
		}

		// if "wait for finish", go to next node after fade
		if (sceneManager != null && nextNode && waitForFinish)
			sceneManager.NextNode();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <param name="fadeOutTime"></param>
    /// <param name="nextNode"></param>
	/// <param name="waitForFinish"></param>
	/// <returns></returns>
	private IEnumerator FadeOut(GameObject character, float fadeOutTime = 0.5f, 
        bool nextNode = false, bool waitForFinish = false)
	{
        SceneManager sceneManager = SceneManager.GetInstance();

		// if not "wait for finish", jump straight to next node
		if (sceneManager != null && nextNode && !waitForFinish)
			sceneManager.NextNode();

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeOutTime)
		{
			// adjust character transparency
			float percentage = 1 - (elapsedTime / fadeOutTime);
			character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);
				
			elapsedTime += Time.deltaTime; // increment time
			yield return null;
		}

		// destroy the character
		m_characters.Remove(character);
		Object.Destroy(character);

		// if "wait for finish", go to next node after fade
		if (sceneManager != null && nextNode && waitForFinish)
			sceneManager.NextNode();
	}
}

}

}
