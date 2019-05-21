using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DEVN.Components;

namespace DEVN
{

namespace SceneManagement
{

public class BranchManager
{
	// scene manager ref
	private SceneManager m_sceneManager;

	// references to branch UI elements
	private GameObject m_branchPrefab;
	private GameObject m_branches;
	private Transform m_branchContent;

	public BranchManager(SceneManager sceneManager, BranchComponent branchComponent)
	{
		m_sceneManager = sceneManager; // assign scene manager reference

		// assign references to all the relevant branch elements
		m_branchPrefab = branchComponent.GetBranchPrefab();
		m_branches = branchComponent.GetBranches();
		m_branchContent = branchComponent.GetBranchContent();
	}
		
	/// <summary>
	/// 
	/// </summary>
	/// <param name="branches"></param>
	public void DisplayChoices(List<string> branches)
	{
		for (int i = 0; i < branches.Count; i++)
		{
			GameObject branch = Object.Instantiate(m_branchPrefab, m_branchContent); // create branch
			Button branchButton = branch.GetComponent<Button>(); // get reference to button component
			
			// set branch text
			Text branchText = branch.GetComponentInChildren<Text>();
			branchText.text = branches[i];

			// add an onClick event to each button so that they hide the panel and proceed to the desired node
			int branchIndex = i;
			branchButton.onClick.AddListener(() => m_sceneManager.NextNode(branchIndex));
			branchButton.onClick.AddListener(() => { m_branches.SetActive(false); });
			branchButton.onClick.AddListener(() => { ClearChoices(); });
		}

		// display the branch panel
		m_branches.SetActive(true);
	}

	/// <summary>
	/// 
	/// </summary>
	public void ClearChoices()
	{
		Button[] branches = m_branchContent.GetComponentsInChildren<Button>();

		for (int i = 0; i < branches.Length; i++)
			Object.Destroy(branches[i].gameObject);
	}
}

}

}