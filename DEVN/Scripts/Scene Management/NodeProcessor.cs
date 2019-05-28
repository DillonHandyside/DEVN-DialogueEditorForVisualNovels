using UnityEngine;
using DEVN.Nodes;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace SceneManagement
{

public class NodeProcessor
{
    private BaseNode m_currentNode;

    // scene manager ref
    private readonly SceneManager m_sceneManager;

    // references to all of the different component managers
    private readonly AudioManager m_audioManager;
    private readonly BackgroundManager m_backgroundManager;
    private readonly CharacterManager m_characterManager;
    private readonly DialogueManager m_dialogueManager;
    private readonly LogManager m_logManager;
    private readonly VariableManager m_variableManager;

    public NodeProcessor(SceneManager sceneManager)
    {
        m_sceneManager = sceneManager;

        // cache manager references
        m_audioManager = m_sceneManager.GetAudioManager();
        m_backgroundManager = m_sceneManager.GetBackgroundManager();
        m_characterManager = m_sceneManager.GetCharacterManager();
        m_dialogueManager = m_sceneManager.GetDialogueManager();
        m_logManager = m_sceneManager.GetLogManager();
        m_variableManager = m_sceneManager.GetVariableManager();
    }

    public void Update(BaseNode currentNode)
    {
        m_currentNode = currentNode;
    }

    #region node processing functions

    /// <summary>
    /// 
    /// </summary>
    public void ProcessBackground()
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
    public void ProcessBGM()
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

        m_sceneManager.NextNode();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ProcessCharacter()
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
    public void ProcessCharacterTransform()
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
            m_sceneManager.StartCoroutine(characterTransformer.LerpCharacterScale(character, scale, scaleNode.GetLerpTime()));
        else
            characterTransformer.SetCharacterScale(character, scale);

        m_sceneManager.NextNode();
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
            m_sceneManager.StartCoroutine(characterTransformer.LerpCharacterPosition(character, position, translateNode.GetLerpTime()));
        else
            characterTransformer.SetCharacterPosition(character, position);

        m_sceneManager.NextNode();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ProcessDialogue()
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
    public void ProcessDialogueBox()
    {
        DialogueBoxNode dialogueBoxNode = m_currentNode as DialogueBoxNode;

        if (dialogueBoxNode.GetToggleSelection() == 0)
            m_dialogueManager.ToggleDialogueBox(true);
        else
            m_dialogueManager.ToggleDialogueBox(false);

        m_sceneManager.NextNode();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ProcessCondition()
    {
        ConditionNode conditionNode = m_currentNode as ConditionNode;
        Blackboard blackboardA = conditionNode.GetBlackboardA();
        string keyA = conditionNode.GetKeyA();
        bool output = true;

        if (conditionNode.GetSourceSelection() == 0)
        {
            // blackboard v value
            switch (conditionNode.GetValueType())
            {
                case ValueType.Boolean:
                    VariableManager.BoolComparison boolComparison = VariableManager.BoolComparison.EqualTo;
                    output = m_variableManager.EvaluateBlackboardVsValue(blackboardA, keyA, conditionNode.GetBooleanValue(), boolComparison);
                    break;

                case ValueType.Float:
                    VariableManager.FloatComparison floatComparison = (VariableManager.FloatComparison)conditionNode.GetFloatSelection();
                    output = m_variableManager.EvaluateBlackboardVsValue(blackboardA, keyA, conditionNode.GetFloatValue(), floatComparison);
                    break;

                case ValueType.String:
                    VariableManager.StringComparison stringComparison = VariableManager.StringComparison.EqualTo;
                    output = m_variableManager.EvaluateBlackboardVsValue(blackboardA, keyA, conditionNode.GetStringValue(), stringComparison);
                    break;
            }
        }
        else
        {
            // blackboard v blackboard
            Blackboard blackboardB = conditionNode.GetBlackboardB();
            string keyB = conditionNode.GetKeyB();

            switch (conditionNode.GetValueType())
            {
                case ValueType.Boolean:
                    VariableManager.BoolComparison boolComparison = VariableManager.BoolComparison.EqualTo;
                    output = m_variableManager.EvaluateBlackboardVsBlackboard(blackboardA, keyA, blackboardB, keyB, boolComparison);
                    break;

                case ValueType.Float:
                    VariableManager.FloatComparison floatComparison = (VariableManager.FloatComparison)conditionNode.GetFloatSelection();
                    output = m_variableManager.EvaluateBlackboardVsBlackboard(blackboardA, keyA, blackboardB, keyB, floatComparison);
                    break;

                case ValueType.String:
                    VariableManager.StringComparison stringComparison = VariableManager.StringComparison.EqualTo;
                    output = m_variableManager.EvaluateBlackboardVsBlackboard(blackboardA, keyA, blackboardB, keyB, stringComparison);
                    break;
            }
        }

        m_sceneManager.NextNode(output ? 0 : 1);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ProcessModify()
    {
        ModifyNode modifyNode = m_currentNode as ModifyNode;
        Blackboard blackboard = modifyNode.GetBlackboard();
        string key = modifyNode.GetKey();
        Value value = blackboard.GetValue(key);

        switch (modifyNode.GetValueType())
        {
            case ValueType.Boolean:
                bool booleanValue;

                if (modifyNode.GetBooleanSelection() == 0)
                    booleanValue = modifyNode.GetBooleanValue(); // set
                else
                    booleanValue = !value.m_boolean; // toggle

                m_variableManager.PerformModify(blackboard, key, booleanValue); // perform modify
                break;

            case ValueType.Float:
                float floatValue;

                if (modifyNode.GetFloatSelection() == 0)
                    floatValue = modifyNode.GetFloatValue(); // set
                else
                    floatValue = value.m_float + modifyNode.GetFloatValue(); // increment

                m_variableManager.PerformModify(blackboard, key, floatValue); // perform modify
                break;

            case ValueType.String:
                m_variableManager.PerformModify(blackboard, key, modifyNode.GetStringValue()); // set
                break;
        }

        m_sceneManager.NextNode();
    }

    #endregion
}

}

}