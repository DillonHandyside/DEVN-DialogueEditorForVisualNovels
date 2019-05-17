using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{
	
/// <summary>
/// component to be placed on the gameobject containing "SceneManager", contains
/// references to the branch panel and it's content, and a branch prefab 
/// </summary>
public class BranchComponent : MonoBehaviour
{
	// reference to branch prefab
	[Header("Branch Prefab")]
	[Tooltip("Plug in the \"Branch\" prefab found in \"DEVN\\Prefabs\"")]
	[SerializeField] private GameObject m_branchPrefab;

	// references to branch UI elements
	[Header("Branch UI Element/s")]
	[Tooltip("Plug in a parent panel, this is used to enable/disable the branches when a choice occurs")]
	[SerializeField] private GameObject m_branches;
	[Tooltip("Plug in a scroll-view content panel, the branch prefabs will be added to this")]
	[SerializeField] private Transform m_branchContent;

	#region getters
	
	public GameObject GetBranchPrefab() { return m_branchPrefab; }
	public GameObject GetBranches() { return m_branches; }
	public Transform GetBranchContent() { return m_branchContent; }

	#endregion
}

}