using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN.Nodes;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Editor
{

#if UNITY_EDITOR

/// <summary>
/// node manager class which is responsible for drawing, adding, copying, pasting and removing nodes.
/// </summary>
public class NodeManager
{
    // collection of all nodes in graph
    [SerializeField] private List<BaseNode> m_nodes;

    // clipboard used for copy and paste
    [SerializeField] private BaseNode m_clipboard;

    #region getters

    public List<BaseNode> GetNodes() { return m_nodes; }
	public BaseNode GetLastNode() { return m_nodes[m_nodes.Count - 1]; }
    public BaseNode GetClipboard() { return m_clipboard; }

    #endregion

    #region setters

    //public void SetNodes(List<BaseNode> nodes) { m_nodes = nodes; }

    #endregion
            
    /// <summary>
    /// update function which gets called by NodeEditor OnGUI
    /// </summary>
    public void Update()
    {
        Scene currentScene = NodeEditor.GetScene(); // retrieve current scene
        m_nodes = currentScene.GetNodes(currentScene.GetCurrentPageID()); // update nodes depending on the page
    }

    /// <summary>
    /// function which iterates through all nodes and calls draw
    /// </summary>
    public void DrawNodes()
    {
        for (int i = 0; i < m_nodes.Count; i++)
        {
            BaseNode node = m_nodes[i];
            Undo.RecordObject(node, "Node Changes"); // record any changes
            node.Draw(); // draw the node
        }
    }

    /// <summary>
    /// helper function which performs a linear search to find a node using a node ID
    /// </summary>
    /// <param name="nodeID">the ID of the desired node</param>
    /// <returns>if search was unsuccessful, this will return null, otherwise it returns the found node</returns>
    public BaseNode FindNode(int nodeID)
    {
        for (int i = 0; i < m_nodes.Count; i++)
        {
            if (m_nodes[i].GetNodeID() == nodeID)
                return m_nodes[i]; // node found!
        }

        return null; // node not found
    }

    /// <summary>
    /// function which handles node creation
    /// </summary>
    /// <param name="nodeType">the type of node to create, e.g. typeof(DialogueNode)</param>
    /// <param name="nodeToCopy">optional argument used to handle copying of nodes</param>
    public BaseNode AddNode(System.Type type, BaseNode nodeToCopy = null)
    {
        Scene currentScene = NodeEditor.GetScene(); // retrieve current scene

        // create a new node asset
        BaseNode node = ScriptableObject.CreateInstance(type) as BaseNode;
        if (nodeToCopy != null)
            node.Copy(nodeToCopy, NodeEditor.GetMousePosition()); // perform copy
        else
            node.Init(NodeEditor.GetMousePosition()); // perform initialisation

        // disallow undo functionality for StartNode
        if (type != typeof(StartNode))
        {
            Undo.RecordObject(currentScene.GetCurrentPage(), "New Node"); // record changes to the current page
            m_nodes.Add(node); // add node to list of all nodes in scene

            Undo.RegisterCreatedObjectUndo(node, "New Node"); // record creation
        }

        // if an input point is selected, immediately connect new node with the selected node
		ConnectionManager connectionManager = NodeEditor.GetConnectionManager();
		BaseNode selectedLeftNode = connectionManager.GetSelectedLeftNode();
		if (selectedLeftNode != null)
		{
			connectionManager.SetSelectedRightNode(node);
			connectionManager.CreateConnection(); // connect the two nodes
			connectionManager.ClearConnectionSelection(); // clear selection
		}

		// parent node to scene asset
        string path = AssetDatabase.GetAssetPath(currentScene);
        AssetDatabase.AddObjectToAsset(node, path);

		// hide node asset from unity project window
        node.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.SaveAssets(); // save!
                
		return node;
    }

    /// <summary>
    /// handles node copying
    /// </summary>
    /// <param name="node">the desired node to copy</param>
    public void CopyNode(BaseNode node)
    {
        m_clipboard = node;
    }

    /// <summary>
    /// handles node pasting
    /// </summary>
    public void PasteNode()
    {
        AddNode(m_clipboard.GetType(), m_clipboard);
    }

    /// <summary>
    /// handles node removal
    /// </summary>
    /// <param name="node">the desired node to remove</param>
    /// <param name="saveAssets">optional argument, used to determine whether to save assets after removal</param>
    public void RemoveNode(BaseNode node, bool saveAssets = true)
    {
        ConnectionManager connectionManager = NodeEditor.GetConnectionManager(); // retrieve connection manager

		// iterate through all connected input nodes
		int noOfInputs = node.m_inputs.Count;
		for (int i = 0; i < noOfInputs; i++)
        {
            BaseNode connectedNode = FindNode(node.m_inputs[0]);

            // find the index of the desired node to remove in the connected node's outputs
            int indexOfThisNode = connectedNode.m_outputs.IndexOf(node.GetNodeID());
            connectionManager.RemoveConnection(connectedNode, indexOfThisNode);
        }

        // iterate through all connected output nodes
        for (int i = 0; i < node.m_outputs.Count; i++)
        {
            if (node.m_outputs[i] == -1)
                continue; // skip empty outputs
            
            // disconnect connection
            connectionManager.RemoveConnection(node, i);
        }

        // record changes to the page
        Page currentPage = NodeEditor.GetScene().GetCurrentPage();
        Undo.RecordObject(currentPage, "Remove Node");
        m_nodes.Remove(node); // perform removal

        // destroy node and record
        Undo.DestroyObjectImmediate(node);
        
        if (saveAssets)
            AssetDatabase.SaveAssets(); // save, if required
    }
}

#endif

}

}