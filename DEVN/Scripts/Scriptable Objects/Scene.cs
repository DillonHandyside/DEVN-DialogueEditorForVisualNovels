using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

/// <summary>
/// scriptable object which is to be created by the user. Upon highlighting/selection
/// the user can access the Node Editor in order to edit the scene
/// </summary>
[CreateAssetMenu(fileName = "New Scene", menuName = "DEVN/Scene")]
[System.Serializable]
public class Scene : ScriptableObject
{
    [HideInInspector]
    [SerializeField] private int m_currentID;

    [HideInInspector]
    [SerializeField] private List<BaseNode> m_nodes = new List<BaseNode>();

	#region getters

	public int GetCurrentNodeID() { return m_currentID; }
	public List<BaseNode> GetNodes() { return m_nodes; }

	#endregion

	#region setters

	public void SetCurrentNodeID(int currentID) { m_currentID = currentID; }

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// saves all of the given nodes to the member variable m_nodes
	/// </summary>
	/// <param name="nodes">the collection of nodes to save</param>
	public void SaveNodes(List<BaseNode> nodes)
    {
        m_nodes.Clear();

        for (int i = 0; i < nodes.Count; i++)
            m_nodes.Add(nodes[i]);
    }

    /// <summary>
    /// loads all of the nodes in this Scene object
    /// </summary>
    /// <returns>a collection of all nodes in this object</returns>
    public List<BaseNode> LoadNodes()
    {
        List<BaseNode> nodes = new List<BaseNode>();

        // initialise start node if necessary
        if (m_nodes.Count == 0)
        {
            BaseNode startNode = NodeEditor.GetNodeManager().AddNode(typeof(StartNode));
            startNode.m_rectangle.position = new Vector2(10, 10);
            nodes.Add(startNode);
        }

        // load all nodes
        for (int i = 0; i < m_nodes.Count; i++)
            nodes.Add(m_nodes[i]);

        return nodes;
    }

#endif
}

}
