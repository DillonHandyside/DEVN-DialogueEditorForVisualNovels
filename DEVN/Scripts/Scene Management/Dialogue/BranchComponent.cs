using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

public class BranchComponent : MonoBehaviour
{
	// references to branch UI elements
	[Header("Branches")]
	[Tooltip("Plug in the \"Branch\" prefab found in \"DEVN\\Prefabs\"")]
	[SerializeField] private GameObject m_branchPrefab;
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