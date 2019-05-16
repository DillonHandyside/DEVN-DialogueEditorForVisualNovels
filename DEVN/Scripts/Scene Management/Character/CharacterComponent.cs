using UnityEngine;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
public class CharacterComponent : MonoBehaviour
{
	[Header("Character Prefab")]
	[SerializeField] private GameObject m_characterPrefab;

	[Header("Canvas Panels")]
	[SerializeField] private RectTransform m_backgroundPanel;
	[SerializeField] private RectTransform m_foregroundPanel;

	#region getters

	public GameObject GetCharacterPrefab() { return m_characterPrefab; }
	public RectTransform GetBackgroundPanel() { return m_backgroundPanel; }
	public RectTransform GetForegroundPanel() { return m_foregroundPanel; }

		#endregion
}

}