using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN.Editor;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// branch node used to allow for player choice and divergence
/// </summary>
[System.Serializable]
public class BranchNode : BaseNode
{
    [SerializeField] private List<string> m_branches;

	#region getters

	public List<string> GetBranches() { return m_branches; }

		#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor, initialises two branches by default
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Branch";

        m_rectangle.width = 340;
			
		m_branches = new List<string>(); // initialise list of branches

		// two branches by default
		AddBranch();
        AddBranch();
    }

	/// <summary>
	/// overridden copy constructor which copies all branches
	/// </summary>
	/// <param name="node">the node to copy</param>
	/// <param name="position">position to copy to</param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		BranchNode branchNode = node as BranchNode;

		// copy all branches/options
		m_branches = new List<string>();
		for (int i = 0; i < branchNode.m_branches.Count; i++)
			m_branches.Add(branchNode.m_branches[i]);
	}

	/// <summary>
	/// overridden draw function. Draws as many text fields as there are branches
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
    {
        // draw all text fields
        for (int i = 0; i < m_branches.Count; i++)
        {
            EditorGUILayout.LabelField("Branch " + (i + 1));
		    m_branches[i] = EditorGUILayout.TextField(m_branches[i]);
        }
        
        // determine button position
        Rect buttonRect = new Rect(m_rectangle.width - 48, 0, 21, 16);
        buttonRect.y = GUILayoutUtility.GetLastRect().y + buttonRect.height + 4;

        // draw the remove button
        if (GUI.Button(buttonRect, "+"))
            AddBranch();

        buttonRect.x += 22;

        // draw the add button
        if (GUI.Button(buttonRect, "-"))
            RemoveBranch();

        // resize node height
        if (Event.current.type == EventType.Repaint)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            m_rectangle.height = lastRect.y + lastRect.height + 24;
        }

        base.DrawNodeWindow(id);
    }

    /// <summary>
    /// helper function which adds a new branch, output point and resizes the node window
    /// </summary>
    private void AddBranch()
    {
		// add new text area and output point 
		m_branches.Add("");
        AddOutputPoint();
	}

    /// <summary>
    /// helper function which removes the last branch, the last output point and resizes the node window
    /// </summary>
    private void RemoveBranch()
    {
		// can't remove branch if there's only 2 branches left
		if (m_branches.Count <= 2)
			return;
		
		int removalIndex = m_branches.Count - 1;

		// remove last text area and output point
		m_branches.RemoveAt(removalIndex);
		
		// disconnect connection if necessary
		if (m_outputs[removalIndex] != -1)
			NodeEditor.GetConnectionManager().RemoveConnection(this, removalIndex);

		// remove the output/output point
		m_outputPoints.RemoveAt(removalIndex);
        m_outputs.RemoveAt(removalIndex);
    }

#endif
}

}

}