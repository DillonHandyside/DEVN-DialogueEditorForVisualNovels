using UnityEngine;

namespace DEVN
{

/// <summary>
/// component to be placed on the gameobject containing "SceneManager", contains
/// references to the log prefab and the log panel
/// </summary>
public class LogComponent : MonoBehaviour
{
	// log message prefab reference
	[Header("Log Prefab")]
	[Tooltip("Plug in the \"Log Message\" prefab found in \"DEVN\\Prefabs\"")]
	[SerializeField] private GameObject m_logPrefab;

	// log content panel
	[Header("Log UI Element/s")]
	[Tooltip("Plug in a scroll-view content panel, the log messages will be added to this")]
	[SerializeField] private Transform m_logContent;

	#region getters

	public GameObject GetLogPrefab() { return m_logPrefab; }
	public Transform GetLogContent() { return m_logContent; }

	#endregion
}

}