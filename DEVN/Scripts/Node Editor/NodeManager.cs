using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace DEVN
{

public class NodeManager
{
    // collection of all nodes in graph
    public List<BaseNode> m_nodes;
    public int m_currentID = 0;

    // clipboard used for copy and paste
    public BaseNode m_clipboard;

    public NodeManager()
    {
        m_nodes = new List<BaseNode>();
    }

    /// <summary>
    /// function which iterates through all nodes and calls draw
    /// </summary>
    public void DrawNodes()
    {
        for (int i = 0; i < m_nodes.Count; i++)
            m_nodes[i].Draw();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeID"></param>
    /// <returns></returns>
    public BaseNode GetNode(int nodeID)
    {
        for (int i = 0; i < m_nodes.Count; i++)
        {
            if (m_nodes[i].GetNodeID() == nodeID)
                return m_nodes[i];
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeType"></param>
    /// <returns></returns>
    public BaseNode AddNode(System.Type type)
    {
        GUI.changed = true;
        BaseNode node;

        node = ScriptableObject.CreateInstance(type) as BaseNode;
        node.Init(NodeEditor.GetMousePosition());

        string path = AssetDatabase.GetAssetPath(NodeEditor.GetScene());
        AssetDatabase.AddObjectToAsset(node, path);
		node.hideFlags = HideFlags.HideInHierarchy;
		AssetDatabase.SaveAssets();
        m_nodes.Add(node);

        return node;
    }

    /// <summary>
    /// handles node copying
    /// </summary>
    /// <param name="node">the desired node to copy</param>
    public void CopyNode(BaseNode node)
    {
        m_clipboard = node;
        GUI.changed = true;
    }

    /// <summary>
    /// handles node pasting
    /// </summary>
    public void PasteNode()
    {
        BaseNode node = ScriptableObject.CreateInstance(m_clipboard.GetType()) as BaseNode;
        node.Copy(m_clipboard, NodeEditor.GetMousePosition());

        string path = AssetDatabase.GetAssetPath(NodeEditor.GetScene());
        AssetDatabase.AddObjectToAsset(node, path);

        m_nodes.Add(node);
        GUI.changed = true;
    }

    /// <summary>
    /// handles node removal
    /// </summary>
    /// <param name="node">the desired node to remove</param>
    public void RemoveNode(BaseNode node)
    {
        if (node is StartNode)
            return; // disallow removal of starting node

        // iterate through all connected input nodes
        for (int i = 0; i < node.m_inputs.Count; i++)
        {
            BaseNode connectedNode = NodeEditor.GetNodeManager().GetNode(node.m_inputs[i]);

            // find the index of the desired node to remove in the connected node's outputs
            int indexOfConnectedNode = connectedNode.m_outputs.IndexOf(node.GetNodeID());

            // disconnect connection
            connectedNode.m_outputs[indexOfConnectedNode] = -1;
			node.m_inputs.Remove(connectedNode.GetNodeID());
        }

        // iterate through all connected output nodes
        for (int i = 0; i < node.m_outputs.Count; i++)
        {
            if (node.m_outputs[i] == -1)
                continue; // skip empty outputs

			// disconnect connection
			NodeEditor.GetConnectionManager().RemoveConnection(node, i);
        }

        m_nodes.Remove(node); // perform removal
        Object.DestroyImmediate(node, true);
    }
}

}

#endif