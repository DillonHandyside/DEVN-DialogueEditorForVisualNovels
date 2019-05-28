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

        SceneManager sceneManager = SceneManager.GetInstance();
		if (sceneManager != null && nextNode)
			sceneManager.NextNode(); 
	}

    /// <summary>
    /// simple utility function which calls Application.Quit()
    /// </summary>
    public void ApplicationQuit()
    {
        Application.Quit();
    }
}

}

}