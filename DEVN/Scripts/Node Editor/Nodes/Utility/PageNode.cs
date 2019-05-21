using UnityEngine;
using UnityEditor;
using DEVN.Editor;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// a utility node for handling page transitions
/// </summary>
[System.Serializable]
public class PageNode : BaseNode
{
    [SerializeField] private int m_pageNumber; // page to jump to

    #region getter

    public int GetPageNumber() { return m_pageNumber; }

    #endregion

#if UNITY_EDITOR

    /// <summary>
    /// overridden constructor, no output points
    /// </summary>
    /// <param name="position">position of creation</param>
    public override void Init(Vector2 position)
    {
        base.Init(position);
            
        m_title = "Page";

        m_rectangle.width = 100;
        m_rectangle.height = 58;
    }

    /// <summary>
    /// overridden copy constructor, copies the desired page number to jump to
    /// </summary>
    /// <param name="node">the node to copy</param>
    /// <param name="position">the position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

        PageNode pageNode = node as PageNode;

        // copy "jump to" page number
        m_pageNumber = pageNode.m_pageNumber; 
    }

    /// <summary>
    /// overridden draw function, gets all existing pages and lists them in a popup for the user to select
    /// </summary>
    /// <param name="id">the node window ID</param>
    protected override void DrawNodeWindow(int id)
	{
        // draw "Jump To:" label
        EditorGUILayout.LabelField("Jump To:");

        // get a string list of all pages
        string[] pages = new string[NodeEditor.GetScene().GetPages().Count];
        for (int i = 0; i < pages.Length; i++)
            pages[i] = "Page " + (i + 1);

        // draw page list popup
        m_pageNumber = EditorGUILayout.Popup(m_pageNumber, pages);

        // perform clamp to prevent any index out of range errors
        m_pageNumber = Mathf.Clamp(m_pageNumber, 0, pages.Length - 1);

		base.DrawNodeWindow(id);
	}

#endif
}

}

}