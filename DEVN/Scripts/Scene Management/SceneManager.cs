using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

#region required components
[RequireComponent(typeof(LogManager))]
[RequireComponent(typeof(DialogueManager))]
[RequireComponent(typeof(CharacterManager))]
[RequireComponent(typeof(BackgroundManager))]
[RequireComponent(typeof(AudioManager))]
#endregion
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
    private CharacterManager m_characterManager;
    private DialogueManager m_dialogueManager;
    private LogManager m_logManager;
    private VariableManager m_variableManager;
	
	private bool m_isInputAllowed = false;

	#region getters

	public static SceneManager GetInstance() { return m_instance; }
	public BaseNode GetCurrentNode() { return m_currentNode; }
    public List<Blackboard> GetBlackboards() { return m_blackboards; }
    public AudioManager GetAudioManager() { return m_audioManager; }
    public BackgroundManager GetBackgroundManager() { return m_backgroundManager; }
    public CharacterManager GetCharacterManager() { return m_characterManager; }
    public DialogueManager GetDialogueManager() { return m_dialogueManager; }
    public LogManager GetLogManager() { return m_logManager; }

	#endregion

	#region setters

	public void SetIsInputAllowed(bool isInputAllowed) { m_isInputAllowed = isInputAllowed; }

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

        // cache references to all required managers
        m_audioManager = GetComponent<AudioManager>();
        m_backgroundManager = GetComponent<BackgroundManager>();
        m_characterManager = GetComponent<CharacterManager>();
        m_dialogueManager = GetComponent<DialogueManager>();
        m_logManager = GetComponent<LogManager>();

        // check if all managers were cached successfully
        Debug.Assert(m_audioManager != null, "DEVN: AudioManager cache unsuccessful!");
        Debug.Assert(m_backgroundManager != null, "DEVN: BackgroundManager cache unsuccessful!");
        Debug.Assert(m_characterManager != null, "DEVN: CharacterManager cache unsuccessful!");
        Debug.Assert(m_dialogueManager != null, "DEVN: DialogueManager cache unsuccessful!");
        Debug.Assert(m_logManager != null, "DEVN: LogManager cache unsuccessful!");
        
        // create variable manager
        m_variableManager = new VariableManager();

        // delete this later
		NewScene(m_startScene);
	}
	
	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
		// dialogue box toggle
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			SetIsInputAllowed(!m_isInputAllowed);
            m_dialogueManager.ToggleDialogueBox();
		}

		if (m_isInputAllowed && Input.GetKeyDown(KeyCode.Space))
		{
			if (m_dialogueManager.GetIsTyping())
                m_dialogueManager.SkipTypewrite();
			else
				NextNode();
		}
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
	private void Transition()
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
    private void UpdateScene()
	{
		SetIsInputAllowed(false);

		// background node
		if (m_currentNode is BackgroundNode)
			UpdateBackground();

		// audio nodes
		else if (m_currentNode is BGMNode)
		{
			BGMNode bgmNode = m_currentNode as BGMNode;
			m_audioManager.SetBGM(bgmNode.GetBGM(), bgmNode.GetAmbientAudio(), true);
		}
		else if (m_currentNode is SFXNode)
		{
			SFXNode sfxNode = m_currentNode as SFXNode;
			StartCoroutine(m_audioManager.PlaySFX(sfxNode.GetSFX(), true, sfxNode.GetWaitForFinish()));
		}

		// character nodes
		else if (m_currentNode is CharacterNode)
			EnterExitCharacter();
		else if (m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
			EvaluateTransformNode();

		// variable nodes
		else if (m_currentNode is ConditionNode)
			m_variableManager.EvaluateCondition(m_currentNode as ConditionNode);
		else if (m_currentNode is ModifyNode)
			m_variableManager.PerformModify(m_currentNode as ModifyNode);

		// dialogue nodes
		else if (m_currentNode is BranchNode)
			m_dialogueManager.DisplayChoices((m_currentNode as BranchNode).GetBranches());
		else if (m_currentNode is DialogueNode)
			m_dialogueManager.SetDialogue(m_currentNode as DialogueNode);
		else if (m_currentNode is DialogueBoxNode)
			m_dialogueManager.SetDialogueBox(m_currentNode as DialogueBoxNode);

		// transition nodes
		else if (m_currentNode is PageNode || m_currentNode is EndNode)
			Transition();
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