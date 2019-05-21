using System.Collections.Generic;
using UnityEngine;
using DEVN.Editor;
using DEVN.Nodes;

namespace DEVN
{

namespace ScriptableObjects
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
        m_nodes.Add(startNode); // add a StartNode to page
    }

#endif
}

}

}