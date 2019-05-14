using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DEVN;


public class VariableManager
{
    // singleton
    private static VariableManager m_instance;

	// scene manager ref
	private SceneManager m_sceneManager;

    #region getters

    public static VariableManager GetInstance()
    {
        if (m_instance == null)
            m_instance = new VariableManager();
        return m_instance;
    }

    #endregion
	
	/// <summary>
	/// 
	/// </summary>
	public VariableManager()
	{
		// cache scene manager instance
		m_sceneManager = SceneManager.GetInstance();
		Debug.Assert(m_sceneManager != null, "DEVN: SceneManager cache unsuccessful!");
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public void PerformModify(ModifyNode modifyNode)
    {
        string key = modifyNode.GetKey();
        Blackboard blackboard = FindBlackboard(modifyNode.GetBlackboard());
        Value value = blackboard.GetValue(key);

        switch (modifyNode.GetValueType())
        {
            case ValueType.Boolean:
                if (modifyNode.GetBooleanSelection() == 0) // set
                    blackboard.SetValue(key, modifyNode.GetBooleanValue());
                else // toggle
                    blackboard.SetValue(key, !value.m_boolean);

                break;

            case ValueType.Float:
                if (modifyNode.GetFloatSelection() == 0) // set
                    blackboard.SetValue(key, modifyNode.GetFloatValue());
                else // increment
                    blackboard.SetValue(key, value.m_float + modifyNode.GetFloatValue());

                break;

            case ValueType.String:
                blackboard.SetValue(key, modifyNode.GetStringValue());

                break;
        }

        m_sceneManager.NextNode();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public void EvaluateCondition(ConditionNode conditionNode)
    {
        string keyA = conditionNode.GetKeyA();
        Blackboard blackboardA = conditionNode.GetBlackboardA();
        Value valueA = FindBlackboard(blackboardA).GetValue(keyA);
		int outputIndex = 0;

        if (conditionNode.GetSourceSelection() == 0)
			outputIndex = BlackboardAndValue(conditionNode, valueA);
        else
			outputIndex = BlackboardAndBlackboard(conditionNode, valueA);

		m_sceneManager.NextNode(1 - outputIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueA"></param>
    /// <returns></returns>
    private int BlackboardAndValue(ConditionNode conditionNode, Value valueA)
    {
        switch (conditionNode.GetValueType())
        {
            case ValueType.Boolean:
                bool booleanValue = conditionNode.GetBooleanValue();
                if (valueA.m_boolean == booleanValue)
                    return 1; // bool is equal to value
                break;

            case ValueType.Float:
                float floatValue = conditionNode.GetFloatValue();
                switch (conditionNode.GetFloatSelection())
                {
                    case 0:
                        if (valueA.m_float < floatValue)
                            return 1; // float is less than value
                        break;
                    case 1:
                        if (valueA.m_float == floatValue)
                            return 1; // float is equal to value
                        break;
                    case 2:
                        if (valueA.m_float > floatValue)
                            return 1; // float is greater than value
                        break;
                }
                        
                break;

            case ValueType.String:
                string stringValue = conditionNode.GetStringValue();
                if (valueA.m_string == stringValue)
                    return 1; // string is equal to value
                break;
        }

        return 0; // condition is false
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueA"></param>
    /// <returns></returns>
    private int BlackboardAndBlackboard(ConditionNode conditionNode, Value valueA)
    {
        string keyB = conditionNode.GetKeyB();
        Blackboard blackboardB = conditionNode.GetBlackboardB();
        Value valueB = FindBlackboard(blackboardB).GetValue(keyB);

        switch (conditionNode.GetValueType())
        {
            case ValueType.Boolean:
                if (valueA.m_boolean == valueB.m_boolean)
                    return 1; // bool is equal to value

                break;

            case ValueType.Float:
                switch (conditionNode.GetFloatSelection())
                {
                    case 0:
                        if (valueA.m_float < valueB.m_float)
                            return 1; // float is less than value
                        break;
                    case 1:
                        if (valueA.m_float == valueB.m_float)
                            return 1; // float is equal to value
                        break;
                    case 2:
                        if (valueA.m_float > valueB.m_float)
                            return 1; // float is greater than value
                        break;
                }

                break;

            case ValueType.String:
                if (valueA.m_string == valueB.m_string)
                    return 1; // string is equal to value

                break;
        }

        return 0; // condition is false
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <returns></returns>
    private Blackboard FindBlackboard(Blackboard blackboard)
    {
        SceneManager sceneManager = SceneManager.GetInstance();
        List<Blackboard> blackboards = sceneManager.GetBlackboards();

        for (int i = 0; i < blackboards.Count; i++)
        {
            if (blackboard.GetInstanceID() == blackboards[i].GetBlackboardID())
                return blackboards[i]; // blackboard found
        }

        return null; // blackboard not found
    }
}
