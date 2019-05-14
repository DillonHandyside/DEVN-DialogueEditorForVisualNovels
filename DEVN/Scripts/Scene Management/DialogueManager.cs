using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// singleton manager class responsible for managing all of the dialogue
/// nodes with-in a DEVN scene
/// </summary>
public class DialogueManager : MonoBehaviour
{
	// singleton
	private static DialogueManager m_instance;

	// scene manager ref
	private SceneManager m_sceneManager;

	// reference to dialogue box UI elements
    [Header("Dialogue")]
	[SerializeField] private GameObject m_dialogueBox;
	[SerializeField] private Text m_speaker;
	[SerializeField] private Text m_dialogue;

	[Header("Branches")]
	[SerializeField] private GameObject m_branchPrefab;
	[SerializeField] private GameObject m_branchPanel;
	[SerializeField] private Transform m_branchContent;
		
	// reference to typewrite coroutine, so StopCoroutine can be used
	private IEnumerator m_typewriteEvent;

	// other text typing relevant variables
	private float m_textSpeed;
	private bool m_isTyping = false;

    private float m_autoSpeed;
    private bool m_isAutoEnabled = false;

	#region getters
		
	public static DialogueManager GetInstance() { return m_instance; }
	public bool GetIsTyping() { return m_isTyping; }

	#endregion

	#region setters

	public void SetDialogueSpeed(float speed) { m_textSpeed = speed; }
    public void SetAutoSpeed(float speed) { m_autoSpeed = speed; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	void Awake ()
	{
		m_instance = this; // intialise singleton

		// cache scene manager reference
		m_sceneManager = GetComponent<SceneManager>();
		Debug.Assert(m_sceneManager != null, "SceneManager cache unsuccessful!");
	}

	/// <summary>
	/// 
	/// </summary>
	public void SetDialogue(DialogueNode dialogueNode)
	{
		// allow input during dialogue, to allow skip & continue
		m_sceneManager.SetIsInputAllowed(true);
			
		// attempt to get this dialogue node's character, log an error if there is none
		Character character = dialogueNode.GetCharacter();
		Debug.Assert(character != null, "DEVN: Dialogue requires a speaking character!");

		// update speaker text field
		m_speaker.text = character.m_name; 
		
		CharacterManager characterManager = CharacterManager.GetInstance();
		Debug.Assert(characterManager != null, "DEVN: A CharacterManager must exist!");
		
		Sprite currentSprite = dialogueNode.GetSprite();
		GameObject characterObject = characterManager.TryGetCharacter(character);

		if (currentSprite != null)
		{
			if (characterObject != null)
				characterManager.SetSprite(characterObject, currentSprite);
			else
				Debug.LogWarning("DEVN: Do not attempt to change the sprite of a character that is not in the scene.");
		}
		else if (characterObject != null)
			currentSprite = characterObject.GetComponent<Image>().sprite;
		else
			currentSprite = null;
		
		if (characterObject)
			characterManager.HighlightSpeakingCharacter(character);

		LogManager.GetInstance().LogDialogue(currentSprite, character.m_name, dialogueNode.GetDialogue());
		
		StartCoroutine(m_typewriteEvent = TypewriteText(dialogueNode.GetDialogue()));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="branches"></param>
	public void DisplayChoices(List<string> branches)
	{
		for (int i = 0; i < branches.Count; i++)
		{
			int branchIndex = i;
			GameObject branch = Instantiate(m_branchPrefab, m_branchContent);
			
			Button branchButton = branch.GetComponent<Button>();
			branchButton.onClick.AddListener(() => m_sceneManager.NextNode(branchIndex));
			branchButton.onClick.AddListener(() => { m_branchPanel.SetActive(false); });

			Text branchText = branch.GetComponentInChildren<Text>();
			branchText.text = branches[i];
		}

		m_branchPanel.SetActive(true);
	}

	/// <summary>
	/// 
	/// </summary>
	public void ToggleDialogueBox()
	{
		ClearDialogueBox();
		m_dialogueBox.SetActive(!m_dialogueBox.activeSelf);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="toggle"></param>
	public void ToggleDialogueBox(bool toggle)
	{
		ClearDialogueBox();
		m_dialogueBox.SetActive(toggle);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="dialogueBoxNode"></param>
	public void SetDialogueBox(DialogueBoxNode dialogueBoxNode)
	{
		if (dialogueBoxNode.GetToggleSelection() == 0)
			ToggleDialogueBox(true);
		else
			ToggleDialogueBox(false);

		m_sceneManager.NextNode();
	}

	/// <summary>
	/// 
	/// </summary>
	private void ClearDialogueBox()
	{
		m_speaker.text = "";
		m_dialogue.text = "";
	}

    /// <summary>
    /// 
    /// </summary>
    public void ToggleAuto()
    {
        m_isAutoEnabled = !m_isAutoEnabled;
        m_sceneManager.SetIsInputAllowed(!m_isAutoEnabled);

        if (m_sceneManager.GetCurrentNode() is DialogueNode &&
            m_isAutoEnabled && m_isTyping == false)
            StartCoroutine(WaitForAuto());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForAuto()
    {
        // determine wait time
        float waitTime = m_dialogue.text.Length * ((1.1f - m_autoSpeed) * 0.1f);
        yield return new WaitForSeconds(waitTime); // wait

        // if auto is still enabled at the end of wait, proceed
        if (m_isAutoEnabled)
            m_sceneManager.NextNode();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator TypewriteText(string dialogue)
	{
		m_dialogue.text = "";
		m_isTyping = true;

		for (int i = 0; i < dialogue.Length; i++)
		{
			m_dialogue.text += dialogue[i];
			yield return new WaitForSeconds((1 - m_textSpeed) * 0.1f);
		}
		
		m_isTyping = false;

        if (m_isAutoEnabled)
            StartCoroutine(WaitForAuto());
	}

	/// <summary>
	/// 
	/// </summary>
	public void SkipTypewrite()
	{
		StopCoroutine(m_typewriteEvent);
			
		DialogueNode dialogueNode = m_sceneManager.GetCurrentNode() as DialogueNode;
		m_dialogue.text = dialogueNode.GetDialogue();

		m_isTyping = false;
	}
}

}
