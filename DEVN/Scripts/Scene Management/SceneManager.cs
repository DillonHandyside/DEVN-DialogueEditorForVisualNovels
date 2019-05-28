using System.Collections.Generic;
using UnityEngine;
using DEVN.Nodes;
using DEVN.Components;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace SceneManagement
{
	
/// <summary>
/// 
/// </summary>
public class SceneManager : MonoBehaviour
{
	// singleton
	private static SceneManager m_instance;

    // delete later
	[SerializeField] private Scene m_startScene;
    
    [Header("Scene Components")]
    [SerializeField] private AudioComponent m_audioComponent;
    [SerializeField] private BackgroundComponent m_backgroundComponent;
    [SerializeField] private BranchComponent m_branchComponent;
    [SerializeField] private CharacterComponent m_characterComponent;
    [SerializeField] private DialogueComponent m_dialogueComponent;
    [SerializeField] private LogComponent m_logComponent;
    [SerializeField] private SaveComponent m_saveComponent;

    private NodeEvaluator m_nodeEvaluator;

    // scene management variables
	private Scene m_currentScene; 
    private BaseNode m_currentNode;
    private List<BaseNode> m_sceneNodes;

    private List<Blackboard> m_blackboards;

    #region component managers

    private AudioManager m_audioManager;
    private BackgroundManager m_backgroundManager;
	private BranchManager m_branchManager;
    private CharacterManager m_characterManager;
    private DialogueManager m_dialogueManager;
    private LogManager m_logManager;
	private SaveManager m_saveManager;
	private UtilityManager m_utilityManager;
    private VariableManager m_variableManager;

    #endregion

    #region getters

    public static SceneManager GetInstance() { return m_instance; }
    public Scene GetCurrentScene() { return m_currentScene; }
	public BaseNode GetCurrentNode() { return m_currentNode; }
    public List<Blackboard> GetBlackboards() { return m_blackboards; }
    public AudioManager GetAudioManager() { return m_audioManager; }
    public BackgroundManager GetBackgroundManager() { return m_backgroundManager; }
    public BranchManager GetBranchManager() { return m_branchManager; }
    public CharacterManager GetCharacterManager() { return m_characterManager; }
    public DialogueManager GetDialogueManager() { return m_dialogueManager; }
    public LogManager GetLogManager() { return m_logManager; }
	public SaveManager GetSaveManager() { return m_saveManager; }
	public UtilityManager GetUtilityManager() { return m_utilityManager; }
    public VariableManager GetVariableManager() { return m_variableManager; }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    void Start ()
    {
		m_instance = this; // initialise singleton

        Blackboard[] blackboards = Resources.FindObjectsOfTypeAll<Blackboard>();

        m_blackboards = new List<Blackboard>();
        for (int i = 0; i < blackboards.Length; i++)
        {
            Blackboard blackboard = ScriptableObject.CreateInstance<Blackboard>();
            blackboard.Copy(blackboards[i]);
            m_blackboards.Add(blackboard);
        }

		// retrieve manager from each component
		if (m_audioComponent != null)
			m_audioManager = m_audioComponent.GetAudioManager();
		if (m_backgroundComponent != null)
			m_backgroundManager = m_backgroundComponent.GetBackgroundManager();
		if (m_branchComponent != null)
			m_branchManager = m_branchComponent.GetBranchManager();
		if (m_characterComponent != null)
			m_characterManager = m_characterComponent.GetCharacterManager();
		if (m_dialogueComponent != null)
			m_dialogueManager = m_dialogueComponent.GetDialogueManager();
		if (m_logComponent != null)
			m_logManager = m_logComponent.GetLogManager();
		if (m_saveComponent != null)
			m_saveManager = new SaveManager(this, m_saveComponent);
		
		m_utilityManager = new UtilityManager();
        m_variableManager = new VariableManager();

        m_nodeEvaluator = new NodeEvaluator(this);

        // delete this later
		NewScene(m_startScene);
	}

	/// <summary>
	/// use this function to start a new DEVN scene!
	/// </summary>
	/// <param name="scene">pass in the scene you want to start here</param>
	public void NewScene(Scene scene)
	{
		m_currentScene = scene; // set scene
        NewPage(0); // page 1
    }

    /// <summary>
    /// use this function to proceed to the next node in a DEVN scene! Be sure you know
    /// what you're doing when manually calling this function
    /// </summary>
    /// <param name="outputIndex">the index of the output path you want to traverse. 
    /// For linear nodes with only one output, use the default parameter (0). For
    /// any node with divergence (more than one output) pass in the desired parameter</param>
    public void NextNode(int outputIndex = 0)
    {
        // prevent index out of range error
        if (outputIndex < m_currentNode.m_outputs.Count)
        {
            // update node and scene
            m_currentNode = GetNode(m_currentNode.m_outputs[outputIndex]);
            m_nodeEvaluator.Evaluate(m_currentNode);
        }
        else 
            Debug.LogWarning("DEVN: Given index is out of range! Can not proceed to next node");
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="node"></param>
	public void JumpToNode(BaseNode node)
	{
		m_currentNode = node;
		m_nodeEvaluator.Evaluate(m_currentNode);
	}

    /// <summary>
    /// helper function which jumps to a new page, and proceeds from it's start node
    /// </summary>
    /// <param name="pageNumber">the page to jump to</param>
    public void NewPage(int pageNumber)
    {
        m_sceneNodes = m_currentScene.GetNodes(pageNumber); // get all nodes on page

        // get start node and jump instantly to next node        
        m_currentNode = m_sceneNodes[0];
        NextNode();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeID"></param>
    /// <returns></returns>
	private BaseNode GetNode(int nodeID)
	{
		for (int i = 0; i < m_sceneNodes.Count; i++)
		{
			if (m_sceneNodes[i].GetNodeID() == nodeID)
				return m_sceneNodes[i];
		}

		return null;
	}
}

}

}