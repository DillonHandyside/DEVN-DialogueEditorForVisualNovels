using System.Collections;
using UnityEngine;

namespace DEVN
{

namespace SceneManagement
{

/// <summary>
/// utility manager class responsible for performing various miscellaneous functions
/// </summary>
public class UtilityManager
{
	// scene manager ref
	SceneManager m_sceneManager;

	/// <summary>
	/// are you sure you want to construct your own UtilityManager? You may want to use 
	/// SceneManager.GetUtilityManager() instead
	/// </summary>
	/// <param name="sceneManager">reference to the scene manager instance</param>
	public UtilityManager(SceneManager sceneManager)
	{
		m_sceneManager = sceneManager; // assign scene manager reference
	}

	/// <summary>
	/// simple coroutine which performs a delay for an arbritrary amount of time. Call
	/// StartCoroutine on this to perform said delay!
	/// </summary>
	/// <param name="delayTime">the amount of time to delay for</param>
	/// <param name="nextNode">do you want to proceed to the next node of the current scene?
	/// If you're calling this function manually, you probably want to leave this as false</param>
	public IEnumerator Delay(float delayTime, bool nextNode = false)
	{
		yield return new WaitForSeconds(delayTime);

		if (nextNode)
			m_sceneManager.NextNode(); 
	}
}

}

}
