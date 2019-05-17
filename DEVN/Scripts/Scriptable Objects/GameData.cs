using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

/// <summary>
/// a singleton scriptableobject class which houses the important data related to
/// the game
/// </summary>
[System.Serializable]
public class GameData
{
	// store default blackboards
	[SerializeField] private List<Blackboard> m_defaultBlackboards = new List<Blackboard>();

    // store save data
	[SerializeField] private SaveData m_autoSave;
    [SerializeField] private List<SaveData> m_saves = new List<SaveData>();

	// -- store settings/options here --
	//

	#region getters

	public List<Blackboard> GetDefaultBlackboards() { return m_defaultBlackboards; }
	public SaveData GetAutoSave() { return m_autoSave; }
	public List<SaveData> GetSaves() { return m_saves; }

	#endregion

	#region setters

	public void SetDefaultBlackboards(List<Blackboard> blackboards) { m_defaultBlackboards = blackboards; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	/// <param name="numberOfSaves"></param>
	public GameData(int numberOfSaves)
	{
		for (int i = 0; i < numberOfSaves; i++)
			m_saves.Add(null);
	}

	public void SetSave(SaveData saveData, int saveSlot)
	{
		m_saves[saveSlot] = saveData;
	}
}

}