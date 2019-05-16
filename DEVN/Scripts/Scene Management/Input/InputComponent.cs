using UnityEngine;
using UnityEngine.EventSystems;

namespace DEVN
{

public class InputComponent : MonoBehaviour
{
	[SerializeField] private EventSystem m_eventSystem;

	// directional axes
	[SerializeField] private string m_horizontalAxis = "Horizontal";
	[SerializeField] private string m_verticalAxis = "Vertical";
	
	// submit/cancel axes
	[SerializeField] private string m_submitButton = "Submit";
	[SerializeField] private string m_cancelButton = "Cancel";

	#region getters

	public EventSystem GetEventSystem() { return m_eventSystem; }
	public string GetHorizontal() { return m_horizontalAxis; }
	public string GetVertical() { return m_verticalAxis; }
	public string GetSubmit() { return m_submitButton; }
	public string GetCancel() { return m_cancelButton; }

		#endregion

	#region setters

	public void SetIsInputAllowed(bool isInputAllowed) { GetComponent<SceneManager>().GetInputManager().SetIsInputAllowed(isInputAllowed); }

	#endregion
}

}
