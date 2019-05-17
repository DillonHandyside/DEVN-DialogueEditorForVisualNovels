using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace DEVN
{

public class ConnectionManager
{
    private BaseNode m_selectedLeftNode;
    private BaseNode m_selectedRightNode;
    private int m_selectedOutput;

    //
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

	public void SetSelectedLeftNode(BaseNode node) { m_selectedLeftNode = node; }
	public void SetSelectedRightNode(BaseNode node) { m_selectedRightNode = node; }

	#endregion

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
    /// 
    /// </summary>
    public void CreateConnection()
    {
        Undo.RecordObject(m_selectedLeftNode, "Create Connection");
        Undo.RecordObject(m_selectedRightNode, "Create Connection");
        
		if (m_selectedLeftNode.m_outputs[m_selectedOutput] != -1)
			RemoveConnection(m_selectedLeftNode, m_selectedOutput);

        m_selectedLeftNode.m_outputs[m_selectedOutput] = m_selectedRightNode.GetNodeID();
        m_selectedRightNode.m_inputs.Add(m_selectedLeftNode.GetNodeID());
    }

    /// <summary>
    /// 
    /// </summary>
    public void RemoveConnection(BaseNode inputNode, int outputIndex)
    {
		NodeManager nodeManager = NodeEditor.GetNodeManager();
        BaseNode connectedNode = nodeManager.FindNode(inputNode.m_outputs[outputIndex]);
            
        Undo.RecordObject(inputNode, "Remove Connection");
        Undo.RecordObject(connectedNode, "Remove Connection");

        connectedNode.m_inputs.Remove(inputNode.GetNodeID());
        inputNode.m_outputs[outputIndex] = -1;
    }
	
    /// <summary>
    /// 
    /// </summary>
    public void ClearConnectionSelection()
    {
        m_selectedLeftNode = null;
        m_selectedRightNode = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public void OnClickInput(BaseNode node)
    {
        // update the selected input point
        m_selectedRightNode = node;

        // continue if an output point has been selected
        if (m_selectedLeftNode != null)
        {
            // disallow connecting node with self
            if (m_selectedRightNode != m_selectedLeftNode)
                CreateConnection();

            ClearConnectionSelection();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="output"></param>
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
                CreateConnection();

            ClearConnectionSelection();
        }
    }

	/// <summary>
	/// 
	/// </summary>
	public void DrawBezierToMouse()
	{
		Color colour = Color.white;

		if (m_selectedLeftNode != null)
		{
			// define positional variables
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
			Handles.color = colour;
			Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Handles.color, null, 2);
		}
		else if (m_selectedRightNode != null)
		{
			// define positional variables
			Vector3 startPos = NodeEditor.GetMousePosition();
			Vector3 endPos = m_selectedRightNode.m_inputPoint.center;
			Vector3 startTangent = startPos + Vector3.right * 50;
			Vector3 endTangent = endPos + Vector3.left * 50;

			// draw the connection
			Handles.color = colour;
			Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Handles.color, null, 2);
		}
	}
}

}

#endif