using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
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
		Button firstBranch = null;
		Button previousBranch = null;

		for (int i = 0; i < branches.Count; i++)
		{
			GameObject branch = Object.Instantiate(m_branchPrefab, m_branchContent); // create branch
			Button branchButton = branch.GetComponent<Button>(); // get reference to button component
			
			if (i == 0)
				firstBranch = branchButton;
			
			// set branch text
			Text branchText = branch.GetComponentInChildren<Text>();
			branchText.text = branches[i];

			// add an onClick event to each button so that they hide the panel and proceed to the desired node
			int branchIndex = i;
			branchButton.onClick.AddListener(() => m_sceneManager.NextNode(branchIndex));
			branchButton.onClick.AddListener(() => { m_branches.SetActive(false); });
			branchButton.onClick.AddListener(() => { ClearChoices(); });

			if (previousBranch != null)
			{
				Navigation verticalNavigation = previousBranch.navigation;
				verticalNavigation.selectOnDown = branchButton;
				previousBranch.navigation = verticalNavigation;

				verticalNavigation.selectOnDown = null;
				verticalNavigation.selectOnUp = previousBranch;
				branchButton.navigation = verticalNavigation;

			}

			//
			previousBranch = branchButton;
		}
		
		Navigation firstNavigation = firstBranch.navigation;
		firstNavigation.selectOnUp = previousBranch;
		firstBranch.navigation = firstNavigation;

		Navigation lastNavigation = previousBranch.navigation;
		lastNavigation.selectOnDown = firstBranch;
		previousBranch.navigation = lastNavigation;

		m_sceneManager.GetInputManager().SetSelectedGameObject(firstBranch.gameObject);

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