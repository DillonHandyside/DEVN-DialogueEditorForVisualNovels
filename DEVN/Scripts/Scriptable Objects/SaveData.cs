using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class SaveData
{
	// store blackboards
	[SerializeField] private List<Blackboard> m_blackboards = new List<Blackboard>();

	// store current scene (which scene did we save on?)
	[SerializeField] private Scene m_currentScene;

	// store current node (when in the scene did we save?)
	[SerializeField] private BaseNode m_currentNode;

	[SerializeField] private Sprite m_background;
	[SerializeField] private List<GameObject> m_characters = new List<GameObject>();

    [SerializeField] private Sprite m_snapshot;

	// store date saved and play-time
    [SerializeField] private string m_playTime;
    [SerializeField] private string m_dateTime;

	#region getters

	public Scene GetCurrentScene() { return m_currentScene; }
	public BaseNode GetCurrentNode() { return m_currentNode; }
	public Sprite GetBackground() { return m_background; }
	public List<GameObject> GetCharacters() { return m_characters; }
    public Sprite GetSnapshot() { return m_snapshot; }
    public string GetDateTime() { return m_dateTime; }

	#endregion

	#region setters

	public void SetBlackboards(List<Blackboard> blackboards) { m_blackboards = blackboards; }
	public void SetCurrentScene(Scene currentScene) { m_currentScene = currentScene; }
	public void SetCurrentNode(BaseNode currentNode) { m_currentNode = currentNode; }
	public void SetBackground(Sprite background) { m_background = background; }
    public void SetSnapshot(Sprite snapshot) { m_snapshot = snapshot; }
    public void SetDateTime(string dateTime) { m_dateTime = dateTime; }

	#endregion

	public void SetCharacters(List<GameObject> characters)
		{
			m_characters.Clear();

			for (int i = 0; i < characters.Count; i++)
			{
				m_characters.Add(characters[i]);
			}
		}
}

}