using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class PageNode : BaseNode
{
    [SerializeField] private int m_pageNumber;

    #region getter

    public int GetPageNumber() { return m_pageNumber; }

    #endregion

#if UNITY_EDITOR

    public override void Init(Vector2 position)
    {
        base.Init(position);
            
        m_title = "Page";

        m_rectangle.width = 100;
        m_rectangle.height = 58;
    }

    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

        PageNode pageNode = node as PageNode;
        
        m_pageNumber = pageNode.m_pageNumber;
    }

    protected override void DrawNodeWindow(int id)
	{
        float fieldWidth = m_rectangle.width - 10;
        float fieldHeight = 16;

        Rect fieldRect = new Rect(5, 20, fieldWidth, fieldHeight);

        GUI.Label(fieldRect, "Jump To:");
        fieldRect.y += fieldHeight;

        string[] pages = new string[NodeEditor.GetScene().GetPages().Count];
        for (int i = 0; i < pages.Length; i++)
            pages[i] = "Page " + (i + 1);

        m_pageNumber = EditorGUI.Popup(fieldRect, m_pageNumber, pages);

		base.DrawNodeWindow(id);
	}

#endif
}

}
