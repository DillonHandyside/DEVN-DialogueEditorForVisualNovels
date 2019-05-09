using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// scriptable object which is to be created by the user. Upon highlighting/selection
/// the user can access the Node Editor in order to edit the scene
/// </summary>
[CreateAssetMenu(fileName = "New Scene", menuName = "DEVN/Scene")]
[System.Serializable]
public class Scene : ScriptableObject
{
    [SerializeField] private int m_currentPage = 0;
        
    [SerializeField] private List<Page> m_pages = new List<Page>();

	#region getters

	public int GetCurrentNodeID() { return m_pages[m_currentPage].GetCurrentNodeID(); }
    public int GetCurrentPageID() { return m_currentPage; }
    public Page GetCurrentPage() { return m_pages[m_currentPage]; }
    public List<Page> GetPages() { return m_pages; }
    public List<BaseNode> GetNodes(int pageNumber) { return m_pages[pageNumber].GetNodes(); }

	#endregion

	#region setters

	public void SetCurrentNodeID(int currentID) { m_pages[m_currentPage].SetCurrentNodeID(currentID); }
    public void SetCurrentPage(int currentPage) { m_currentPage = currentPage; }

	#endregion

#if UNITY_EDITOR
   
    /// <summary>
    /// initialisation function which creates the first page if necessary
    /// </summary>
    public void Init()
    {
        if (m_pages.Count == 0)
            NewPage();
    }
        
    /// <summary>
    /// page creation function which creates a page asset, adds it to the
	/// scene asset and records said creation for undo functionality
    /// </summary>
    public void NewPage()
    {
        Page newPage = CreateInstance<Page>(); // create page

		Undo.RegisterCreatedObjectUndo(newPage, "New Page");

        // add page to scene
        string path = AssetDatabase.GetAssetPath(this);
        AssetDatabase.AddObjectToAsset(newPage, path);

		// hide page asset from unity project window
		newPage.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.SaveAssets();

		Undo.RecordObject(this, "New Node");
        m_pages.Add(newPage); // add to list of pages
        m_currentPage = m_pages.Count - 1;
        newPage.Init(); // initialise page
    }

    /// <summary>
    /// page deletion function which deletes a page asset and it's nodes,
	/// and records said deletion for undo functionality
    /// </summary>
    public void RemovePage()
    {
		// print warning box to prevent accidental page deletion
        if (!EditorUtility.DisplayDialog("Wait!",
            "Are you sure you want to delete this page?", "Yes", "No"))
            return;

        Page currentPage = m_pages[m_currentPage];
        List<BaseNode> nodes = currentPage.GetNodes();
        int nodeCount = nodes.Count;

		// remove all of the nodes in the page
		Undo.RegisterFullObjectHierarchyUndo(currentPage, "Remove Page");
        for (int i = nodeCount - 1; i >= 0; i--)
            NodeEditor.GetNodeManager().RemoveNode(nodes[i], true);
        
		// record scene and remove page from scene
        Undo.RecordObject(this, "Remove Page");
        m_pages.Remove(currentPage);

        // perform clamp to ensure no index out of range errors
        m_currentPage = Mathf.Clamp(m_currentPage, 0, m_pages.Count - 1);

		// destroy page and record
        Undo.DestroyObjectImmediate(currentPage);
        AssetDatabase.SaveAssets();
    }

#endif
}

}
