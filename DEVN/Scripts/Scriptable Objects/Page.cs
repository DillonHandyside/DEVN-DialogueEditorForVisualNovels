using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class Page : ScriptableObject
{
    [SerializeField] private int m_currentID = 0;

    [SerializeField] private List<BaseNode> m_nodes = new List<BaseNode>();
    
    #region getters

    public List<BaseNode> GetNodes() { return m_nodes; }
    public int GetCurrentNodeID() { return m_currentID; }

    #endregion

    #region setters

    public void SetCurrentNodeID(int currentID) { m_currentID = currentID; }

    #endregion

#if UNITY_EDITOR

    /// <summary>
    /// constructor
    /// </summary>
    public void Init()
    {
        BaseNode startNode = NodeEditor.GetNodeManager().AddNode(typeof(StartNode));
        startNode.m_rectangle.position = new Vector2(10, 24);
        m_nodes.Add(startNode); // add a StartNode to page
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodes"></param>
    public void SaveNodes(List<BaseNode> nodes)
    {
        m_nodes.Clear();

        for (int i = 0; i < nodes.Count; i++)
            m_nodes.Add(nodes[i]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<BaseNode> LoadNodes()
    {
        List<BaseNode> nodes = new List<BaseNode>();

        for (int i = 0; i < m_nodes.Count; i++)
            nodes.Add(m_nodes[i]);

        return nodes;
    }

#endif
}

}