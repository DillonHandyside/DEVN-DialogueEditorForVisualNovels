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
        startNode.m_rectangle.position = new Vector2(4, 24);
        m_nodes.Add(startNode); // add a StartNode to page
    }

#endif
}

}