using UnityEngine;
using UnityEditor;
using DEVN.Nodes;

#if UNITY_EDITOR

namespace DEVN
{

namespace Editor
{

/// <summary>
/// connection manager class which is responsible for the creation and removal of connections between nodes.
/// A node will always have only one input point, however those input points can receive as many connections
/// as they like (convergence). A node can have as many output points as they like (divergence), however those 
/// output points can only produce one connection, otherwise it would be difficult to traverse the graph
/// </summary>
public class ConnectionManager
{
    // selection variables
    private BaseNode m_selectedLeftNode;
    private BaseNode m_selectedRightNode;
    private int m_selectedOutput;

    // input/output style variables
    private GUIStyle m_inputStyle;
    private GUIStyle m_outputStyle;

	#region getters

	public BaseNode GetSelectedLeftNode() { return m_selectedLeftNode; }
	public BaseNode GetSelectedRightNode() { return m_selectedRightNode; }
	public int GetSelectedOutput() { return m_selectedOutput; }
	public GUIStyle GetInputStyle() { return m_inputStyle; }
	public GUIStyle GetOutputStyle() { return m_outputStyle; }

		#endregion

	#region setters
            
	public void SetSelectedRightNode(BaseNode node) { m_selectedRightNode = node; }

	#endregion

    /// <summary>
    /// constructor, initialises the style of the input/output points
    /// </summary>
	public ConnectionManager()
    {
        // input point style initialisation
        m_inputStyle = new GUIStyle();
        m_inputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        m_inputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        m_inputStyle.border = new RectOffset(4, 4, 12, 12);

        // output point style initialisation
        m_outputStyle = new GUIStyle();
        m_outputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        m_outputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        m_outputStyle.border = new RectOffset(4, 4, 12, 12);
    }

    /// <summary>
    /// update function which gets called by NodeEditor OnGUI
    /// </summary>
    public void Update()
    {
        DrawBezierToMouse();
    }

    /// <summary>
    /// connection creation function
    /// </summary>
    public void CreateConnection()
    {
        // record the state of the nodes that are about to be connected
        Undo.RecordObject(m_selectedLeftNode, "Create Connection");
        Undo.RecordObject(m_selectedRightNode, "Create Connection");
        
        // if the left node already has a connection, ensure it is removed before creating a new connection
		if (m_selectedLeftNode.m_outputs[m_selectedOutput] != -1)
			RemoveConnection(m_selectedLeftNode, m_selectedOutput);

        // connect the two nodes, such that each node's list of inputs and outputs points to eachothers node ID
        m_selectedLeftNode.m_outputs[m_selectedOutput] = m_selectedRightNode.GetNodeID(); // left points to right
        m_selectedRightNode.m_inputs.Add(m_selectedLeftNode.GetNodeID()); // right points to left
    }

    /// <summary>
    /// helper function which creates a linear connect between two nodes
    /// </summary>
    /// <param name="leftNode">the first node to connect</param>
    /// <param name="rightNode">the second node to connect</param>
    public void CreateLinearConnection(BaseNode leftNode, BaseNode rightNode)
    {
        Debug.Assert(leftNode.m_outputs.Count == 1, "DEVN: Do not try and create a linear output for a non-linear node");

        // update relevant selection variables
        m_selectedLeftNode = leftNode;
        m_selectedRightNode = rightNode;
        m_selectedOutput = 0;

        // create the connection and clear selection variables
        CreateConnection();
        ClearConnectionSelection();
    }

    /// <summary>
    /// connection removal function
    /// </summary>
    /// <param name="leftNode">the reference to the left node in the connection</param>
    /// <param name="outputIndex">the left node's output point of the connection</param>
    public void RemoveConnection(BaseNode leftNode, int outputIndex)
    {
		NodeManager nodeManager = NodeEditor.GetNodeManager();
        BaseNode rightNode = nodeManager.FindNode(leftNode.m_outputs[outputIndex]);
            
        // record the state of the nodes that are about to be disconnected
        Undo.RecordObject(leftNode, "Remove Connection");
        Undo.RecordObject(rightNode, "Remove Connection");

        // disconnect the two nodes, such that neither points to eachother's node ID anymore
        leftNode.m_outputs[outputIndex] = -1; // left node output points to nothing
        rightNode.m_inputs.Remove(leftNode.GetNodeID()); // left node removed from right nodes list of inputs
    }
	
    /// <summary>
    /// helper function which clears the current left/right node selection
    /// </summary>
    public void ClearConnectionSelection()
    {
        m_selectedLeftNode = null;
        m_selectedRightNode = null;
    }

    /// <summary>
    /// on click event callback function which occurs when a node's input point is clicked
    /// </summary>
    /// <param name="node">the node whose input point was clicked on</param>
    public void OnClickInput(BaseNode node)
    {
        // update the selected input point
        m_selectedRightNode = node;

        // continue if an output point has been selected
        if (m_selectedLeftNode != null)
        {
            // disallow connecting node with self
            if (m_selectedRightNode != m_selectedLeftNode)
                CreateConnection(); // connect left and right node

            ClearConnectionSelection(); // connection complete, clear selections
        }
    }

    /// <summary>
    /// on click event callback function which occurs when one of a node's output points is clicked
    /// </summary>
    /// <param name="node">the node whose output point was clicked on</param>
    /// <param name="outputIndex">the index of the clicked output point</param>
    public void OnClickOutput(BaseNode node, int outputIndex)
    {
        // update the selected output point
        m_selectedLeftNode = node;
        m_selectedOutput = outputIndex;

        // continue if an input point has been selected
        if (m_selectedRightNode != null)
        {
            // disallow connecting node with self
            if (m_selectedLeftNode != m_selectedRightNode)
                CreateConnection(); // connect left and right node

            ClearConnectionSelection(); // connection complete, clear selections
        }
    }

	/// <summary>
	/// helper function which draws a bezier to the user's mouse position if only one input/output point
    /// has been selected. This is used to give the user a visual indication that they're creating a connection
	/// </summary>
	private void DrawBezierToMouse()
	{
		Color colour = Color.white; // bezier is white by default

		if (m_selectedLeftNode != null)
		{
			// an output point was clicked the the mouse is the end position
			Vector3 startPos = m_selectedLeftNode.m_outputPoints[m_selectedOutput].center;
			Vector3 endPos = NodeEditor.GetMousePosition();
			Vector3 startTangent = startPos + Vector3.right * 50;
			Vector3 endTangent = endPos + Vector3.left * 50;

			// different bezier colours for condition nodes
			if (m_selectedLeftNode is ConditionNode)
			{
				if (m_selectedOutput == 0)
					colour = Color.green; // true
				else
					colour = Color.red; // false
			}

			// draw the connection
			Handles.DrawBezier(startPos, endPos, startTangent, endTangent, colour, null, 2);
		}
		else if (m_selectedRightNode != null)
		{
			// an input point was clicked, so the mouse is the start position
			Vector3 startPos = NodeEditor.GetMousePosition();
			Vector3 endPos = m_selectedRightNode.m_inputPoint.center;
			Vector3 startTangent = startPos + Vector3.right * 50;
			Vector3 endTangent = endPos + Vector3.left * 50;

			// draw the connection
			Handles.DrawBezier(startPos, endPos, startTangent, endTangent, colour, null, 2);
		}
	}
}

}

}

#endif