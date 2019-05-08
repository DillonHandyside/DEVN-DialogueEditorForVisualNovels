using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

public class CharacterManager : MonoBehaviour
{
	// singleton
	private static CharacterManager m_instance;

	// scene manager ref
	private SceneManager m_sceneManager;

	public GameObject m_inactiveCharacterPanel;
	public GameObject m_activeCharacterPanel;

	public GameObject m_characterPrefab;
	private List<GameObject> m_characters;

	#region getters

	public static CharacterManager GetInstance() { return m_instance; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	void Awake()
	{
		m_instance = this; // initialise singleton

		// cache scene manager reference
		m_sceneManager = GetComponent<SceneManager>();

		m_characters = new List<GameObject>();
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isEnter"></param>
    public void UpdateCharacter(bool isEnter)
    {
        CharacterNode currentNode = m_sceneManager.GetCurrentNode() as CharacterNode;

        if (isEnter)
            EnterCharacter(currentNode);
        else
            ExitCharacter(currentNode);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterNode"></param>
	public void EnterCharacter(CharacterNode characterNode)
	{
		// check to see if the character already exists in the scene
		if (FindCharacter(characterNode.GetCharacter()) != null)
		{
			Debug.LogWarning("Don't attempt to enter two of the same characters!");
			m_sceneManager.NextNode();
			return;
		}

		// create new character and parent it to canvas
		GameObject character = Instantiate(m_characterPrefab);
		character.transform.SetParent(m_inactiveCharacterPanel.transform, false);

		// handle inverting
        if (characterNode.GetIsInverted())
            character.transform.localScale = new Vector3(-1, 1, 1);

        // set sprite
        Image characterImage = character.GetComponent<Image>();
		characterImage.sprite = characterNode.GetSprite();
		characterImage.preserveAspect = true;
		
		// adjust position
		RectTransform characterTransform = character.GetComponent<RectTransform>();
		float screenExtent = m_inactiveCharacterPanel.GetComponent<RectTransform>().rect.width * 0.5f;
		float xScalar = (characterNode.GetXPosition() * 2 - 100.0f) * 0.01f; // between -1 and 1
		Vector2 position = new Vector2(screenExtent * xScalar, characterTransform.anchoredPosition.y);
		characterTransform.anchoredPosition = position;
			
		CharacterInfo characterInfo = character.GetComponent<CharacterInfo>();
		characterInfo.SetCharacter(characterNode.GetCharacter());

		m_characters.Add(character);

		// perform fade-in
		StartCoroutine(FadeIn(character, characterNode.GetFadeTime()));
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterNode"></param>
	public void ExitCharacter(CharacterNode characterNode)
	{
		GameObject character = FindCharacter(characterNode.GetCharacter());

		if (character != null)
		{
			// perform fade-out and removal
			StartCoroutine(FadeOut(character, characterNode.GetFadeTime()));
			return; // exit succeeded
		}

		Debug.LogWarning("Don't attempt to exit a character which doesn't exist!");
		m_sceneManager.NextNode();
	}

	/// <summary>
	/// 
	/// </summary>
	public void ChangeSprite()
	{
		DialogueNode currentNode = m_sceneManager.GetCurrentNode() as DialogueNode;
		Character character = currentNode.GetCharacter();
		GameObject characterObject = FindCharacter(character);

		characterObject.GetComponent<Image>().sprite = currentNode.GetSprite();
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
			character.transform.SetParent(m_inactiveCharacterPanel.transform);
			character.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
		}
		
		GameObject characterToHighlight = FindCharacter(speakingCharacter);
		characterToHighlight.transform.SetParent(m_activeCharacterPanel.transform);
		characterToHighlight.GetComponent<Image>().color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	public GameObject FindCharacter(Character character)
	{
		// iterate over all characters in scene
		for (int i = 0; i < m_characters.Count; i++)
		{
			GameObject existingCharacter = m_characters[i];
			CharacterInfo characterInfo = existingCharacter.GetComponent<CharacterInfo>();

			if (characterInfo.GetCharacter() == character)
				return existingCharacter; // character found
		}

		return null; // character not found
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="image"></param>
	/// <param name="fadeInTime"></param>
	/// <returns></returns>
	IEnumerator FadeIn(GameObject character, float fadeInTime = 0.5f)
	{
		CharacterNode dialogueNode = m_sceneManager.GetCurrentNode() as CharacterNode;

		if (!dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode();

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeInTime)
		{
			float percentage = elapsedTime / fadeInTime;

			character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		if (dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode(); // jump straight to next node
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="image"></param>
	/// <param name="fadeOutTime"></param>
	/// <returns></returns>
	IEnumerator FadeOut(GameObject character, float fadeOutTime = 0.5f)
	{
		CharacterNode dialogueNode = m_sceneManager.GetCurrentNode() as CharacterNode;

		if (!dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode();

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeOutTime)
		{
			float percentage = 1 - (elapsedTime / fadeOutTime);

			character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		m_characters.Remove(character);
		Destroy(character);

		if (dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode();
	}
}

}
