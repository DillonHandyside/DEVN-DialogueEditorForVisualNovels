using UnityEngine;

namespace DEVN
{

/// <summary>
/// component to be placed on the gameobject containing "SceneManager", contains
/// references to the character prefab and the character panel UI elements
/// </summary>
public class CharacterComponent : MonoBehaviour
{
	// reference to character prefab
	[Header("Character Prefab")]
	[SerializeField] private GameObject m_characterPrefab;

	// reference to the character UI panels, used for foreground/background functionality
	[Header("Character UI Element/s")]
	[SerializeField] private RectTransform m_backgroundPanel;
	[SerializeField] private RectTransform m_foregroundPanel;

	#region getters

	public GameObject GetCharacterPrefab() { return m_characterPrefab; }
	public RectTransform GetBackgroundPanel() { return m_backgroundPanel; }
	public RectTransform GetForegroundPanel() { return m_foregroundPanel; }

		#endregion
}

}