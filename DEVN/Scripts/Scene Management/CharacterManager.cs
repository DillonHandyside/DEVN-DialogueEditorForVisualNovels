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

	private CharacterTransformer m_characterTransformer;

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

		m_characterTransformer = new CharacterTransformer();

		m_characters = new List<GameObject>();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="node"></param>
	public void EvaluateCharacterNode(BaseNode node)
	{
		if (node is CharacterNode)
		{
			CharacterNode characterNode = node as CharacterNode;

			if (characterNode.GetToggleSelection() == 0)
				EnterCharacter(characterNode);
			else
				ExitCharacter(characterNode);
		}
		else
			m_characterTransformer.EvaluateTransformNode(node);
		
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterNode"></param>
	public void EnterCharacter(CharacterNode characterNode)
	{
		// check to see if the character already exists in the scene
		if (TryGetCharacter(characterNode.GetCharacter()) != null)
		{
			Debug.LogWarning("DEVN: Do not attempt to enter two of the same characters.");
			m_sceneManager.NextNode();
			return;
		}

		// create new character and parent it to canvas
		GameObject character = Instantiate(m_characterPrefab);
		character.transform.SetParent(m_inactiveCharacterPanel.transform, false);

		// invert if necessary
		if (characterNode.GetIsInverted())
			m_characterTransformer.InvertCharacter(character);

		SetSprite(character, characterNode.GetSprite()); // set sprite

		Vector2 position = new Vector2(characterNode.GetXPosition(), 0);
		m_characterTransformer.SetCharacterPosition(character, position); // set position

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
		GameObject character = TryGetCharacter(characterNode.GetCharacter());

		if (character != null)
		{
			// perform fade-out and removal
			StartCoroutine(FadeOut(character, characterNode.GetFadeTime()));
			return; // exit succeeded
		}

		Debug.LogWarning("DEVN: Do not attempt to exit a character that is not in the scene.");
		m_sceneManager.NextNode();
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
			CharacterInfo characterInfo = existingCharacter.GetComponent<CharacterInfo>();

			if (characterInfo.GetCharacter() == character)
				return existingCharacter; // character found
		}

		return null; // character not found
	}

	/// <summary>
	/// 
	/// </summary>
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
			character.transform.SetParent(m_inactiveCharacterPanel.transform);
			character.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
		}

		GameObject characterToHighlight = TryGetCharacter(speakingCharacter);
		characterToHighlight.transform.SetParent(m_activeCharacterPanel.transform);
		characterToHighlight.GetComponent<Image>().color = new Color(1, 1, 1);
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

		// if not "wait for finish", jump straight to next node
		if (!dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode();

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
		if (dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode(); 
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

		// if not "wait for finish", jump straight to next node
		if (!dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode();

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeOutTime)
		{
			// adjust character transparency
			float percentage = 1 - (elapsedTime / fadeOutTime);
			character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);
				
			elapsedTime += Time.deltaTime; // increment time
			yield return null;
		}

		m_characters.Remove(character);
		Destroy(character);

		// if "wait for finish", go to next node after fade
		if (dialogueNode.GetWaitForFinish())
			m_sceneManager.NextNode();
	}
}

}
