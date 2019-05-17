using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

[System.Serializable]
public class SaveData
{
	// store date saved and play-time

	// store blackboards
	[SerializeField] private List<Blackboard> m_blackboards = new List<Blackboard>();

	// store current scene (which scene did we save on?)
	[SerializeField] private Scene m_currentScene;

	// store current node (when in the scene did we save?)
	[SerializeField] private BaseNode m_currentNode;

	[SerializeField] private Sprite m_background;
	[SerializeField] private List<GameObject> m_characters = new List<GameObject>();

	#region getters

	public Scene GetCurrentScene() { return m_currentScene; }
	public BaseNode GetCurrentNode() { return m_currentNode; }
	public Sprite GetBackground() { return m_background; }
	public List<GameObject> GetCharacters() { return m_characters; }

	#endregion

	#region setters

	public void SetBlackboards(List<Blackboard> blackboards) { m_blackboards = blackboards; }
	public void SetCurrentScene(Scene currentScene) { m_currentScene = currentScene; }
	public void SetCurrentNode(BaseNode currentNode) { m_currentNode = currentNode; }
	public void SetBackground(Sprite background) { m_background = background; }

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