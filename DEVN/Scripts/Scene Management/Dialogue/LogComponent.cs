using UnityEngine;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
public class LogComponent : MonoBehaviour
{
	// log message prefab reference
	[Header("Log")]
	[Tooltip("Plug in the \"Log Message\" prefab found in \"DEVN\\Prefabs\"")]
	[SerializeField] private GameObject m_logPrefab;

	// log content panel
	[Tooltip("Plug in a scroll-view content panel, the log messages will be added to this")]
	[SerializeField] private Transform m_logContent;

	#region getters

	public GameObject GetLogPrefab() { return m_logPrefab; }
	public Transform GetLogContent() { return m_logContent; }

	#endregion
}

}
