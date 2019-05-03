using System.Collections;
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
	private DialogueNode m_currentNode;

	// reference to dialogue box UI elements
	[SerializeField] private GameObject m_dialogueBox;
	[SerializeField] private Text m_speaker;
	[SerializeField] private Text m_dialogue;
		
	// reference to typewrite coroutine, so StopCoroutine can be used
	private IEnumerator m_typewriteEvent;

	// other text typing relevant variables
	private float m_textSpeed;
	private bool m_isTyping = false;

	#region getters
		
	public static DialogueManager GetInstance() { return m_instance; }
	public bool GetIsTyping() { return m_isTyping; }

	#endregion

	#region setters

	public void SetDialogueSpeed(float speed) { m_textSpeed = speed; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	void Awake ()
	{
		m_instance = this; // intialise singleton

		// cache reference to scene manager
		m_sceneManager = GetComponent<SceneManager>();
	}

	/// <summary>
	/// 
	/// </summary>
	public void SetDialogue()
	{
		// allow input during dialogue, to allow skip & continue
		m_sceneManager.SetIsInputAllowed(true);

		// convert current node to dialogue node
		m_currentNode = m_sceneManager.GetCurrentNode() as DialogueNode;

		CharacterManager characterManager = CharacterManager.GetInstance();
		Character character = m_currentNode.GetCharacter();
		
		m_speaker.text = character.m_name; // update speaker text field
		characterManager.ChangeSprite();
		characterManager.HighlightSpeakingCharacter(character);
		StartCoroutine(m_typewriteEvent = TypewriteText());
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
	private void ClearDialogueBox()
	{
		m_speaker.text = "";
		m_dialogue.text = "";
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	IEnumerator TypewriteText()
	{
		m_dialogue.text = "";
		m_isTyping = true;

		for (int i = 0; i < m_currentNode.GetDialogue().Length; i++)
		{
			m_dialogue.text += m_currentNode.GetDialogue()[i];
			yield return new WaitForSeconds((1 - m_textSpeed) * 0.1f);
		}
		
		m_isTyping = false;
	}

	/// <summary>
	/// 
	/// </summary>
	public void SkipTypewrite()
	{
		StopCoroutine(m_typewriteEvent);
		m_dialogue.text = m_currentNode.GetDialogue();
		m_isTyping = false;
	}
}

}
