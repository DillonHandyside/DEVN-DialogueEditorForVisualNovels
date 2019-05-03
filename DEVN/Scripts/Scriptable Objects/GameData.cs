using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

/// <summary>
/// a singleton scriptableobject class which houses the important data related to
/// the game
/// </summary>
public class GameData : ScriptableObject
{
    #region singleton

    private static GameData m_instance;

    public static GameData GetInstance()
    {
		if (m_instance == null)
			{
                m_instance = FindObjectOfType<GameData>();
			}

        if (m_instance == null)
            m_instance = CreateInstance<GameData>();

        return m_instance;
    }

	#endregion

	// store default blackboards
	[SerializeField] private List<Blackboard> m_defaultBlackboards = new List<Blackboard>();

    // store save data
    [SerializeField] private List<SaveData> m_saves = new List<SaveData>();

	// -- store settings/options here --
	//

	#region getters

	public List<Blackboard> GetDefaultBlackboards() { return m_defaultBlackboards; }
	public List<SaveData> GetSaves() { return m_saves; }

	#endregion

	#region setters

	public void SetDefaultBlackboards(List<Blackboard> blackboards) { m_defaultBlackboards = blackboards; }

	#endregion
}

[System.Serializable]
public class SaveData
{
    // store date saved and play-time

    // store blackboards
    [SerializeField] private List<Blackboard> m_blackboards = new List<Blackboard>();

    // store current scene (which scene did we save on?)
    [SerializeField] private Scene m_currentScene;
    public Scene GetCurrentScene() { return m_currentScene; }

    // store path of nodes (when in the scene did we save?)
    [SerializeField] private List<BaseNode> m_nodePath;
    public List<BaseNode> GetNodePath() { return m_nodePath; }
}

}