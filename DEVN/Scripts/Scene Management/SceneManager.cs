using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{
	
public class SceneManager : MonoBehaviour
{
	// singleton
	private static SceneManager m_instance;

    // delete later
	[SerializeField] private Scene m_startScene;
    
    // scene management variables
	private Scene m_currentScene; 
    private BaseNode m_currentNode;
    private List<BaseNode> m_sceneNodes;

    private List<Blackboard> m_blackboards;

    // reference to all of the different managers
    private AudioManager m_audioManager;
    private BackgroundManager m_backgroundManager;
	private BranchManager m_branchManager;
    private CharacterManager m_characterManager;
    private DialogueManager m_dialogueManager;
	private InputManager m_inputManager;
    private LogManager m_logManager;
    private VariableManager m_variableManager;
	

	#region getters

	public static SceneManager GetInstance() { return m_instance; }
	public BaseNode GetCurrentNode() { return m_currentNode; }
    public List<Blackboard> GetBlackboards() { return m_blackboards; }
    public AudioManager GetAudioManager() { return m_audioManager; }
    public BackgroundManager GetBackgroundManager() { return m_backgroundManager; }
    public CharacterManager GetCharacterManager() { return m_characterManager; }
    public DialogueManager GetDialogueManager() { return m_dialogueManager; }
	public InputManager GetInputManager() { return m_inputManager; }
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

		// cache references to all possible components
		AudioComponent audioComponent = GetComponent<AudioComponent>();
		BackgroundComponent backgroundComponent = GetComponent<BackgroundComponent>();
		BranchComponent branchComponent = GetComponent<BranchComponent>();
		CharacterComponent characterComponent = GetComponent<CharacterComponent>();
		DialogueComponent dialogueComponent = GetComponent<DialogueComponent>();
		InputComponent inputComponent = GetComponent<InputComponent>();
		LogComponent logComponent = GetComponent<LogComponent>();

		if (audioComponent != null)
			m_audioManager = new AudioManager(this, audioComponent);
		if (backgroundComponent != null)
			m_backgroundManager = new BackgroundManager(this, backgroundComponent);
		if (branchComponent != null)
			m_branchManager = new BranchManager(this, branchComponent);
		if (characterComponent != null)
			m_characterManager = new CharacterManager(this, characterComponent);
		if (dialogueComponent != null)
			m_dialogueManager = new DialogueManager(this, dialogueComponent);
		if (inputComponent != null)
			m_inputManager = new InputManager(this, inputComponent);
		if (logComponent != null)
			m_logManager = new LogManager(logComponent);

        m_variableManager = new VariableManager();

        // delete this later
		NewScene(m_startScene);
	}
	
	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
		if (m_inputManager != null)
			m_inputManager.Update();
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
		if (m_inputManager != null)
			m_inputManager.SetIsInputAllowed(false);
			
		// evaluate all the different node types
		if (EvaluateAudioNode())
			return;
		if (EvaluateBackgroundNode())
			return;
		if (EvaluateCharacterNode())
			return;
		if (EvaluateDialogueNode())
			return;
		if (EvaluateVariableNode())
			return;
		EvaluateTransitionNodes();
    }
	
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
				BGMNode bgmNode = m_currentNode as BGMNode;
				m_audioManager.SetBGM(bgmNode.GetBGM(), bgmNode.GetAmbientAudio(), true);
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
			{
				Debug.LogWarning("DEVN: SceneManager needs an AudioComponent if you are using audio nodes!");
				NextNode();
			}
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
				UpdateBackground();
				return true;
			}
		}
		else
		{
			if (m_currentNode is BackgroundNode)
			{
				Debug.LogWarning("DEVN: SceneManager needs a BackgroundComponent if you are using background nodes!");
				NextNode();
			}
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
				EnterExitCharacter();
				return true;
			}
			else if (m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
			{
				EvaluateTransformNode();
				return true;
			}
		}
		else
		{
			if (m_currentNode is CharacterNode || m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
			{
				Debug.LogWarning("DEVN: SceneManager needs a CharacterComponent if you are using character nodes!");
				NextNode();
			}
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
			if (m_currentNode is BranchNode)
			{
				m_branchManager.DisplayChoices((m_currentNode as BranchNode).GetBranches());
				return true;
			}
			else if (m_currentNode is DialogueNode)
			{
				m_dialogueManager.SetDialogue(m_currentNode as DialogueNode);
				return true;
			}
			else if (m_currentNode is DialogueBoxNode)
			{
				m_dialogueManager.SetDialogueBox(m_currentNode as DialogueBoxNode);
				return true;
			}
		}
		else
		{
			if (m_currentNode is BranchNode || m_currentNode is DialogueNode || m_currentNode is DialogueBoxNode)
			{
				Debug.LogWarning("DEVN: SceneManager needs a DialogueComponent if you are using dialogue nodes!");
				NextNode();
			}
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

	/// <summary>
	/// 
	/// </summary>
	private void UpdateBackground()
	{
		// get background node
		BackgroundNode backgroundNode = m_currentNode as BackgroundNode;
		Color fadeColour = backgroundNode.GetFadeColour();
		float fadeTime = backgroundNode.GetFadeTime();

		// enter or exit?
		if (backgroundNode.GetToggleSelection() == 0) // enter
		{
			// perform background fade-in
			Sprite background = backgroundNode.GetBackground();
			bool waitForFinish = backgroundNode.GetWaitForFinish();
			m_backgroundManager.EnterBackground(background, fadeColour, fadeTime, true, waitForFinish);
		}
		else // exit
			m_backgroundManager.ExitBackground(fadeColour, fadeTime);
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void EnterExitCharacter()
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
			m_characterManager.EnterCharacter(character, sprite, xPosition, fadeTime, waitForFinish, isInvert);
		}
		else // exit
			m_characterManager.ExitCharacter(character, sprite, fadeTime, waitForFinish);
	}

	/// <summary>
	/// 
	/// </summary>
	private void EvaluateTransformNode()
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
		Vector2 position = translateNode.GetTranslation();

		if (translateNode.GetIsLerp())
            StartCoroutine(characterTransformer.LerpCharacterPosition(character, position, translateNode.GetLerpTime()));
		else
			characterTransformer.SetCharacterPosition(character, position);

		NextNode();
	}
}

}