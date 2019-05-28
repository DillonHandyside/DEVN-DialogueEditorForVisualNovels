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
	// references to branch UI elements
	private readonly GameObject m_branchPrefab;
	private readonly GameObject m_branchPanel;
	private readonly Transform m_branchContent;

	public BranchManager(BranchComponent branchComponent)
	{
		// assign references to all the relevant branch elements
		m_branchPrefab = branchComponent.GetBranchPrefab();
		m_branchPanel = branchComponent.GetBranches();
		m_branchContent = branchComponent.GetBranchContent();
	}
		
	/// <summary>
	/// 
	/// </summary>
	/// <param name="branches"></param>
    /// <param name="nextNode"></param>
	public void DisplayChoices(List<string> branches, bool nextNode = false)
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
			branchButton.onClick.AddListener(() => { m_branchPanel.SetActive(false); });
			branchButton.onClick.AddListener(() => { ClearChoices(); });

            SceneManager sceneManager = SceneManager.GetInstance();
            if (sceneManager != null && nextNode)
			    branchButton.onClick.AddListener(() => sceneManager.NextNode(branchIndex));
		}

		// display the branch panel
		m_branchPanel.SetActive(true);
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