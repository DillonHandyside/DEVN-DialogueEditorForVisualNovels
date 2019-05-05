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
    public int GetCurrentPage() { return m_currentPage; }
    public List<Page> GetPages() { return m_pages; }
    public List<BaseNode> GetNodes(int pageNumber) { return m_pages[pageNumber].GetNodes(); }

	#endregion

	#region setters

	public void SetCurrentNodeID(int currentID) { m_pages[m_currentPage].SetCurrentNodeID(currentID); }
    public void SetCurrentPage(int currentPage) { m_currentPage = currentPage; }

	#endregion

#if UNITY_EDITOR
        
	/// <summary>
	/// saves all of the given nodes to the member variable m_nodes
	/// </summary>
	/// <param name="nodes">the collection of nodes to save</param>
	public void SaveNodes(List<BaseNode> nodes)
    {
        m_pages[m_currentPage].SaveNodes(nodes);
        EditorUtility.SetDirty(m_pages[m_currentPage]);
    }

    /// <summary>
    /// loads all of the nodes in this Scene object
    /// </summary>
    /// <returns>a collection of all nodes in this object</returns>
    public List<BaseNode> LoadNodes()
    {
        if (m_pages.Count == 0)
            NewPage();

        return m_pages[m_currentPage].LoadNodes();
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void NewPage()
    {
        Page newPage = CreateInstance<Page>(); // create page

        // add page to scene
        string path = AssetDatabase.GetAssetPath(this);
        AssetDatabase.AddObjectToAsset(newPage, path);
        AssetDatabase.SaveAssets();

        m_pages.Add(newPage); // add to list of pages
        SetCurrentPage(m_pages.Count - 1);
        newPage.Init(); // initialise page
    }

#endif
}

}
