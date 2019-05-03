using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

public class CharacterManager : MonoBehaviour
{
	private static CharacterManager m_instance;

	private SceneManager m_sceneManager;

	public GameObject m_characterPanel;

	public GameObject m_characterPrefab;
	private List<GameObject> m_characters;

	#region getters

	public static CharacterManager GetInstance() { return m_instance; }

	#endregion

	// Use this for initialization
	void Awake()
	{
		m_instance = this; // initialise singleton
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
		if (FindCharacter(characterNode.GetCharacter()) != -1)
		{
			Debug.LogWarning("Don't attempt to enter two of the same characters!");
			m_sceneManager.NextNode();
			return;
		}

		// create new character and parent it to canvas
		GameObject character = Instantiate(m_characterPrefab);
		character.transform.SetParent(m_characterPanel.transform, false);

		// set sprite
		Image characterImage = character.GetComponent<Image>();
		characterImage.sprite = characterNode.GetSprite();
		characterImage.preserveAspect = true;
		
		// adjust position
		RectTransform characterTransform = character.GetComponent<RectTransform>();
		float screenExtent = m_characterPanel.GetComponent<RectTransform>().rect.width * 0.5f;
		float xScalar = (characterNode.GetXPosition() * 2 - 100.0f) * 0.01f; // between -1 and 1
		Vector2 position = new Vector2(screenExtent * xScalar, characterTransform.anchoredPosition.y);
		characterTransform.anchoredPosition = position;

		// perform fade-in
		StartCoroutine(FadeIn(character, characterNode.GetFadeTime()));

		//
		CharacterInfo characterInfo = character.GetComponent<CharacterInfo>();
		characterInfo.SetCharacter(characterNode.GetCharacter());

		m_characters.Add(character);
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterNode"></param>
	public void ExitCharacter(CharacterNode characterNode)
	{
		int characterIndex = FindCharacter(characterNode.GetCharacter());

		if (characterIndex != -1)
		{
			GameObject character = m_characters[characterIndex];

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
		int characterIndex = FindCharacter(character);

		m_characters[characterIndex].GetComponent<Image>().sprite = currentNode.GetSprite();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	public int FindCharacter(Character character)
	{
		// iterate over all characters in scene
		for (int i = 0; i < m_characters.Count; i++)
		{
			GameObject existingCharacter = m_characters[i];
			CharacterInfo characterInfo = existingCharacter.GetComponent<CharacterInfo>();

			if (characterInfo.GetCharacter() == character)
				return i; // character found
		}

		return -1; // character not found
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="image"></param>
	/// <param name="fadeInTime"></param>
	/// <returns></returns>
	IEnumerator FadeIn(GameObject character, float fadeInTime = 0.5f)
	{
		float elapsedTime = 0.0f;

		while (elapsedTime < fadeInTime)
		{
			float percentage = elapsedTime / fadeInTime;

			character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}

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

		m_sceneManager.NextNode();
	}
}

}
