using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace DEVN
{

public class ConnectionManager
{
    public BaseNode m_selectedLeftNode;
    public BaseNode m_selectedRightNode;
    public int m_selectedOutput;

    //
    public GUIStyle m_inputStyle;
    public GUIStyle m_outputStyle;

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
        m_selectedLeftNode.m_outputs[m_selectedOutput] = m_selectedRightNode.GetNodeID();
        m_selectedRightNode.m_inputs.Add(m_selectedLeftNode.GetNodeID());
    }

    /// <summary>
    /// 
    /// </summary>
    public void RemoveConnection(BaseNode inputNode, int outputIndex)
    {
		NodeManager nodeManager = NodeEditor.GetNodeManager();
		nodeManager.GetNode(inputNode.m_outputs[outputIndex]).m_inputs.Remove(inputNode.GetNodeID());
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
}

}

#endif