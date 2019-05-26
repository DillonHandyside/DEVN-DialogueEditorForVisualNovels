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
    public CharacterManager GetCharacterManager() { return m_characterManager; }
    public DialogueManager GetDialogueManager() { return m_dialogueManager; }
	public SaveManager GetSaveManager() { return m_saveManager; }
	public UtilityManager GetUtilityManager() { return m_utilityManager; }
    public LogManager GetLogManager() { return m_logManager; }

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
		
		m_utilityManager = new UtilityManager(this);
        m_variableManager = new VariableManager(this);

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
            UpdateScene();
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
		UpdateScene();
	}

    /// <summary>
    /// helper function which jumps to a new page, and proceeds from it's start node
    /// </summary>
    /// <param name="pageNumber">the page to jump to</param>
    private void NewPage(int pageNumber)
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

	/// <summary>
	/// 
	/// </summary>
    private void UpdateScene()
	{
		// evaluate all the different node types
		if (EvaluateAudioNode())
			return;
		else if (EvaluateBackgroundNode())
			return;
        else if (EvaluateBranchNode())
            return;
		else if (EvaluateCharacterNode())
			return;
		else if (EvaluateDialogueNode())
			return;
		else if (EvaluateUtilityNode())
			return;
		else if (EvaluateVariableNode())
			return;
		EvaluateTransitionNodes();
    }

    #region node evaluation functions

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateAudioNode()
	{
		if (m_audioManager != null)
		{
			if (m_currentNode is BGMNode)
			{
                ProcessBGM();
                return true;
			}
			else if (m_currentNode is SFXNode)
			{
				SFXNode sfxNode = m_currentNode as SFXNode;
				StartCoroutine(m_audioManager.PlaySFX(sfxNode.GetSFX(), true, sfxNode.GetWaitForFinish()));
				return true;
			}
		}
		else
		{
			if (m_currentNode is BGMNode || m_currentNode is SFXNode)
				Debug.LogError("DEVN: SceneManager needs an AudioComponent if you are using audio nodes!");
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private bool EvaluateBackgroundNode()
	{
		if (m_backgroundManager != null)
		{
			if (m_currentNode is BackgroundNode)
			{
				ProcessBackground();
				return true;
			}
		}
		else
		{
			if (m_currentNode is BackgroundNode)
				Debug.LogError("DEVN: SceneManager needs a BackgroundComponent if you are using background nodes!");
		}

		return false;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateBranchNode()
    {
        if (m_branchManager != null)
        {
            if (m_currentNode is BranchNode)
            {
                m_branchManager.DisplayChoices((m_currentNode as BranchNode).GetBranches(), true);
                return true;
            }
        }
        else
        {
            if (m_currentNode is BranchNode)
                Debug.LogError("DEVN: SceneManager needs a BranchComponent is you are using branch nodes!");
        }

        return false;
    }

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private bool EvaluateCharacterNode()
	{
		if (m_characterManager != null)
		{
			if (m_currentNode is CharacterNode)
			{
				ProcessCharacter();
				return true;
			}
			else if (m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
			{
				ProcessCharacterTransform();
				return true;
			}
		}
		else
		{
			if (m_currentNode is CharacterNode || m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
				Debug.LogError("DEVN: SceneManager needs a CharacterComponent if you are using character nodes!");
		}
		
		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private bool EvaluateDialogueNode()
	{
		if (m_dialogueManager != null)
		{
			if (m_currentNode is DialogueNode)
			{
                ProcessDialogue();
				return true;
			}
			else if (m_currentNode is DialogueBoxNode)
			{
                ProcessDialogueBox();
                return true;
			}
		}
		else
		{
			if (m_currentNode is BranchNode || m_currentNode is DialogueNode || m_currentNode is DialogueBoxNode)
				Debug.LogError("DEVN: SceneManager needs a DialogueComponent if you are using dialogue nodes!");
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private bool EvaluateUtilityNode()
	{
		if (m_currentNode is DelayNode)
		{
			StartCoroutine(m_utilityManager.Delay((m_currentNode as DelayNode).GetDelayTime(), true));
			return true;
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private bool EvaluateVariableNode()
	{
		if (m_currentNode is ConditionNode)
		{
			m_variableManager.EvaluateCondition(m_currentNode as ConditionNode);
			return true;
		}
		else if (m_currentNode is ModifyNode)
		{
			m_variableManager.PerformModify(m_currentNode as ModifyNode);
			return true;
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	private void EvaluateTransitionNodes()
	{
        if (m_currentNode is PageNode)
        {
            PageNode pageNode = m_currentNode as PageNode;
            NewPage((pageNode).GetPageNumber());
        }
		else if (m_currentNode is EndNode)
		{
			EndNode endNode = m_currentNode as EndNode;

			if (endNode.GetNextScene() == null)
			{
				// -- place custom scene transition code here -- 
			}
			else
				NewScene(endNode.GetNextScene());
		}
    }

    #endregion

    #region node processing functions

    /// <summary>
    /// 
    /// </summary>
    private void ProcessBackground()
	{
		// get background node
		BackgroundNode backgroundNode = m_currentNode as BackgroundNode;
		Color fadeColour = backgroundNode.GetFadeColour();
		float fadeTime = backgroundNode.GetFadeTime();

		// enter or exit?
		if (backgroundNode.GetToggleSelection() == 0) // enter
		{
			// perform background fade-in
			Sprite background = backgroundNode.GetBackground().GetBackground();
			bool waitForFinish = backgroundNode.GetWaitForFinish();
			m_backgroundManager.EnterBackground(background, fadeColour, fadeTime, true, waitForFinish);
		}
		else // exit
			m_backgroundManager.ExitBackground(fadeColour, fadeTime, true);
	}
	
   /// <summary>
   /// 
   /// </summary>
   private void ProcessBGM()
   {
       BGMNode bgmNode = m_currentNode as BGMNode;

       switch (bgmNode.GetToggleSelection())
       {
           case 0:
               m_audioManager.SetBGM(bgmNode.GetBGM(), bgmNode.GetAmbientAudio());
               break;

           case 1:
               m_audioManager.PauseBGM();
               break;

           case 2:
               m_audioManager.ResumeBGM();
               break;
       }
       
       NextNode();
   }

	/// <summary>
	/// 
	/// </summary>
	private void ProcessCharacter()
	{
		// get character enter/exit details such as fadeTime, default sprite etc.
		CharacterNode characterNode = m_currentNode as CharacterNode;
		Character character = characterNode.GetCharacter();
		Sprite sprite = characterNode.GetSprite();
		float fadeTime = characterNode.GetFadeTime();
		bool waitForFinish = characterNode.GetWaitForFinish();

		// enter or exit?
		if (characterNode.GetToggleSelection() == 0) // enter
		{
			// get xPosition and invert, then enter character
			float xPosition = characterNode.GetXPosition();
			bool isInvert = characterNode.GetIsInverted();
			m_characterManager.EnterCharacter(character, sprite, xPosition, fadeTime, true, waitForFinish, isInvert);
		}
		else // exit
			m_characterManager.ExitCharacter(character, sprite, fadeTime, true, waitForFinish);
	}

	/// <summary>
	/// 
	/// </summary>
	private void ProcessCharacterTransform()
	{
		// get CharacterTransformer
		CharacterTransformer characterTransformer = m_characterManager.GetCharacterTransformer();
		Debug.Assert(characterTransformer != null, "DEVN: CharacterTransformer does not exist!");
		
		if (m_currentNode is CharacterScaleNode)
			ScaleCharacter(characterTransformer);
		else if (m_currentNode is CharacterTranslateNode)
			TranslateCharacter(characterTransformer);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="characterTransformer"></param>
	private void ScaleCharacter(CharacterTransformer characterTransformer)
	{
		CharacterScaleNode scaleNode = m_currentNode as CharacterScaleNode;
		GameObject character = m_characterManager.TryGetCharacter(scaleNode.GetCharacter());
		Vector2 scale = scaleNode.GetScale();

		if (scaleNode.GetIsLerp())
			StartCoroutine(characterTransformer.LerpCharacterScale(character, scale, scaleNode.GetLerpTime()));
		else
			characterTransformer.SetCharacterScale(character, scale);

		NextNode();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="characterTransformer"></param>
	private void TranslateCharacter(CharacterTransformer characterTransformer)
	{
		CharacterTranslateNode translateNode = m_currentNode as CharacterTranslateNode;
		GameObject character = m_characterManager.TryGetCharacter(translateNode.GetCharacter());
		Vector2 position = new Vector2(translateNode.GetXPosition(), 0);

		if (translateNode.GetIsLerp())
            StartCoroutine(characterTransformer.LerpCharacterPosition(character, position, translateNode.GetLerpTime()));
		else
			characterTransformer.SetCharacterPosition(character, position);

		NextNode();
	}

    /// <summary>
    /// 
    /// </summary>
    private void ProcessDialogue()
    {
        DialogueNode dialogueNode = m_currentNode as DialogueNode;

        // attempt to get this dialogue node's character, log an error if there is none
        Character character = dialogueNode.GetCharacter();
        Debug.Assert(character != null, "DEVN: Dialogue requires a speaking character!");

        m_dialogueManager.SetDialogue(character, dialogueNode.GetSprite(), dialogueNode.GetDialogue(),
            m_characterManager, m_logManager);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ProcessDialogueBox()
    {
        DialogueBoxNode dialogueBoxNode = m_currentNode as DialogueBoxNode;

        if (dialogueBoxNode.GetToggleSelection() == 0)
            m_dialogueManager.ToggleDialogueBox(true);
        else
            m_dialogueManager.ToggleDialogueBox(false);

        NextNode();
    }

    #endregion
}

}

}