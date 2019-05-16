using UnityEngine;
using UnityEngine.EventSystems;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
public class InputManager
{
	// scene manager ref
	private SceneManager m_sceneManager;
		
	private EventSystem m_eventSystem;

	// directional axes
	private string m_horizontalAxis;
	private string m_verticalAxis;
	
	// submit/cancel axes
	private string m_submitButton;
	private string m_cancelButton;
		
	private bool m_isInputAllowed = false;

	#region getters
		
	public bool GetIsInputAllowed() { return m_isInputAllowed; }

	#endregion

	#region setters
		
	public void SetIsInputAllowed(bool isInputAllowed) { m_isInputAllowed = isInputAllowed; }

	#endregion

	/// <summary>
	/// are you sure you want to construct your own InputManager? You may want to use 
	/// SceneManager.GetInputManager() instead
	/// </summary>
	/// <param name="sceneManager"></param>
	/// <param name="inputComponent"></param>
	public InputManager(SceneManager sceneManager, InputComponent inputComponent)
	{
		m_sceneManager = sceneManager; // assign scene manager reference

		m_eventSystem = inputComponent.GetEventSystem(); // assign event system reference

		// assign references to all relevant input variables
		m_horizontalAxis = inputComponent.GetHorizontal();
		m_verticalAxis = inputComponent.GetVertical();
		m_submitButton = inputComponent.GetSubmit();
		m_cancelButton = inputComponent.GetCancel();
	}

	/// <summary>
	/// 
	/// </summary>
	public void Update()
	{
		if (!m_isInputAllowed)
			return;
		
		if (Input.GetButton(m_submitButton))
		{
			DialogueManager dialogueManager = m_sceneManager.GetDialogueManager();

			if (dialogueManager.GetIsTyping())
				dialogueManager.SkipTypewrite();
			else
				m_sceneManager.NextNode();
		}
	}

	public void SetSelectedGameObject(GameObject gameObject)
	{
		m_eventSystem.SetSelectedGameObject(gameObject);
	}
}

}