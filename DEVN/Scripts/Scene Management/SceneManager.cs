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

	[SerializeField] private Scene m_startScene;
	[HideInInspector]
    [SerializeField] private Scene m_currentScene;
    private BaseNode m_currentNode;
    private List<BaseNode> m_sceneNodes;

    private List<Blackboard> m_blackboards;
	
	private bool m_isInputAllowed = false;

	#region getters

	public static SceneManager GetInstance() { return m_instance; }
	public BaseNode GetCurrentNode() { return m_currentNode; }
    public List<Blackboard> GetBlackboards() { return m_blackboards; }

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

		NewScene(m_startScene);
	}
	
	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
		DialogueManager dialogueManager = DialogueManager.GetInstance();

		// dialogue box toggle
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			SetIsInputAllowed(!m_isInputAllowed);
			dialogueManager.ToggleDialogueBox();
		}

		if (m_isInputAllowed && Input.GetKeyDown(KeyCode.Space))
		{
			if (dialogueManager.GetIsTyping())
				dialogueManager.SkipTypewrite();
			else
				NextNode();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="scene"></param>
	public void NewScene(Scene scene)
	{
		m_currentScene = scene;

		m_sceneNodes = m_currentScene.GetNodes(0);
		m_currentNode = m_sceneNodes[0]; // start node

		NextNode();
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNumber"></param>
    public void NewPage(int pageNumber)
    {
        m_sceneNodes = m_currentScene.GetNodes(pageNumber);
        m_currentNode = m_sceneNodes[0]; // start node

        NextNode();
    }

	/// <summary>
	/// 
	/// </summary>
	public BaseNode GetNode(int nodeID)
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
	/// <param name="outputIndex"></param>
    public void NextNode(int outputIndex = 0)
    {
		m_currentNode = GetNode(m_currentNode.m_outputs[outputIndex]);
        UpdateScene();
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
			AudioManager.GetInstance().SetBGM(bgmNode.GetBGM(), bgmNode.GetAmbientAudio(), true);
		}
		else if (m_currentNode is SFXNode)
		{
			SFXNode sfxNode = m_currentNode as SFXNode;
			StartCoroutine(AudioManager.GetInstance().PlaySFX(sfxNode.GetSFX(), true, sfxNode.GetWaitForFinish()));
		}

		// character nodes
		else if (m_currentNode is CharacterNode)
			EnterExitCharacter();
		else if (m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
			EvaluateTransformNode();

		// variable nodes
		else if (m_currentNode is ConditionNode)
			VariableManager.GetInstance().EvaluateCondition(m_currentNode as ConditionNode);
		else if (m_currentNode is ModifyNode)
			VariableManager.GetInstance().PerformModify(m_currentNode as ModifyNode);

		// dialogue nodes
		else if (m_currentNode is BranchNode)
			DialogueManager.GetInstance().DisplayChoices((m_currentNode as BranchNode).GetBranches());
		else if (m_currentNode is DialogueNode)
			DialogueManager.GetInstance().SetDialogue(m_currentNode as DialogueNode);
		else if (m_currentNode is DialogueBoxNode)
			DialogueManager.GetInstance().SetDialogueBox(m_currentNode as DialogueBoxNode);

		// transition nodes
		else if (m_currentNode is PageNode || m_currentNode is EndNode)
			Transition();
    }

	/// <summary>
	/// 
	/// </summary>
	private void UpdateBackground()
	{
		// get BackgroundManager singleton instance and log any potential errors
		BackgroundManager backgroundManager = BackgroundManager.GetInstance();
		Debug.Assert(backgroundManager != null, "DEVN: BackgroundManager singleton not found!");

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
			backgroundManager.EnterBackground(background, fadeColour, fadeTime, true, waitForFinish);
		}
		else // exit
			backgroundManager.ExitBackground(fadeColour, fadeTime);
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void EnterExitCharacter()
	{
		// get CharacterManager singleton instance
		CharacterManager characterManager = CharacterManager.GetInstance();
		Debug.Assert(characterManager != null, "DEVN: CharacterManager singleton not found!");
			
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
			characterManager.EnterCharacter(character, sprite, xPosition, fadeTime, waitForFinish, isInvert);
		}
		else // exit
			characterManager.ExitCharacter(character, sprite, fadeTime, waitForFinish);
	}

	/// <summary>
	/// 
	/// </summary>
	private void EvaluateTransformNode()
	{
		// get CharacterManager singleton instance
		CharacterManager characterManager = CharacterManager.GetInstance();
		Debug.Assert(characterManager != null, "DEVN: CharacterManager singleton not found!");

		// get CharacterTransformer
		CharacterTransformer characterTransformer = characterManager.GetCharacterTransformer();
		Debug.Assert(characterTransformer != null, "DEVN: CharacterTransformer does not exist!");
		
		if (m_currentNode is CharacterScaleNode)
			ScaleCharacter(characterManager, characterTransformer);
		else if (m_currentNode is CharacterTranslateNode)
			TranslateCharacter(characterManager, characterTransformer);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="characterManager"></param>
	/// <param name="characterTransformer"></param>
	private void ScaleCharacter(CharacterManager characterManager, CharacterTransformer characterTransformer)
	{
		CharacterScaleNode scaleNode = m_currentNode as CharacterScaleNode;
		GameObject character = characterManager.TryGetCharacter(scaleNode.GetCharacter());
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
	/// <param name="characterManager"></param>
	/// <param name="characterTransformer"></param>
	private void TranslateCharacter(CharacterManager characterManager, CharacterTransformer characterTransformer)
	{
		CharacterTranslateNode translateNode = m_currentNode as CharacterTranslateNode;
		GameObject character = characterManager.TryGetCharacter(translateNode.GetCharacter());
		Vector2 position = translateNode.GetTranslation();

		if (translateNode.GetIsLerp())
			characterManager.StartCoroutine(characterTransformer.LerpCharacterPosition(character, position, translateNode.GetLerpTime()));
		else
			characterTransformer.SetCharacterPosition(character, position);

		NextNode();
	}
}

}