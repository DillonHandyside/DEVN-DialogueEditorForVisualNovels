using UnityEngine;
using DEVN.SceneManagement;

namespace DEVN
{

namespace Components
{

/// <summary>
/// 
/// </summary>
public class SaveComponent : MonoBehaviour
{
	[Header("Save Slot Prefab")]
	[SerializeField] private GameObject m_saveSlotPrefab;

	[Header("Save UI Element/s")]
	[SerializeField] private GameObject m_savePanel;
	[SerializeField] private RectTransform m_saveContent;

	[Header("Settings")]
	[SerializeField] private int m_numberOfSaves;

	#region getters

	public GameObject GetSaveSlotPrefab() { return m_saveSlotPrefab; }
	public GameObject GetSavePanel() { return m_savePanel; }
	public RectTransform GetSaveContent() { return m_saveContent; }
	public int GetNumberOfSaves() { return m_numberOfSaves; }

	#endregion
	
	public void OpenSaveMenu()
	{
		SceneManager.GetInstance().GetSaveManager().UpdateSaveLoadSlots();
	}

	public void OpenLoadMenu()
	{
		SceneManager.GetInstance().GetSaveManager().UpdateSaveLoadSlots(true);
	}
}

}

}