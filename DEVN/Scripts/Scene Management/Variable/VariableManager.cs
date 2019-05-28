using System.Collections.Generic;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace SceneManagement
{

public class VariableManager
{
    public enum BoolComparison
    {
        EqualTo
    }

    public enum FloatComparison
    {
        LessThan,
        EqualTo,
        GreaterThan
    }

    public enum StringComparison
    {
        EqualTo
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void PerformModify(Blackboard blackboard, string key, bool value)
    {
        Blackboard sceneBlackboard = FindBlackboard(blackboard);
        sceneBlackboard.SetValue(key, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void PerformModify(Blackboard blackboard, string key, float value)
    {
        Blackboard sceneBlackboard = FindBlackboard(blackboard);
        sceneBlackboard.SetValue(key, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void PerformModify(Blackboard blackboard, string key, string value)
    {
        Blackboard sceneBlackboard = FindBlackboard(blackboard);
        sceneBlackboard.SetValue(key, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool EvaluateBlackboardVsValue(Blackboard blackboard, string key, bool value, BoolComparison boolComparison)
    {
        Blackboard sceneBlackboard = FindBlackboard(blackboard);
        Value valueA = sceneBlackboard.GetValue(key);

        return CompareBoolean(valueA.m_boolean, value, boolComparison);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="floatComparison"></param>
    /// <returns></returns>
    public bool EvaluateBlackboardVsValue(Blackboard blackboard, string key, float value, FloatComparison floatComparison)
    {
        Blackboard sceneBlackboard = FindBlackboard(blackboard);
        Value valueA = sceneBlackboard.GetValue(key);

        return CompareFloat(valueA.m_float, value, floatComparison);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool EvaluateBlackboardVsValue(Blackboard blackboard, string key, string value, StringComparison stringComparison)
    {
        Blackboard sceneBlackboard = FindBlackboard(blackboard);
        Value valueA = sceneBlackboard.GetValue(key);

        return CompareString(valueA.m_string, value, stringComparison);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboardA"></param>
    /// <param name="keyA"></param>
    /// <param name="blackboardB"></param>
    /// <param name="keyB"></param>
    /// <param name="boolComparison"></param>
    /// <returns></returns>
    public bool EvaluateBlackboardVsBlackboard(Blackboard blackboardA, string keyA, Blackboard blackboardB, string keyB, BoolComparison boolComparison)
    {
        Blackboard sceneBlackboardA = FindBlackboard(blackboardA);
        Value valueA = sceneBlackboardA.GetValue(keyA);
        Blackboard sceneBlackboardB = FindBlackboard(blackboardB);
        Value valueB = sceneBlackboardB.GetValue(keyB);

        return CompareBoolean(valueA.m_boolean, valueB.m_boolean, boolComparison);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboardA"></param>
    /// <param name="keyA"></param>
    /// <param name="blackboardB"></param>
    /// <param name="keyB"></param>
    /// <param name="floatComparison"></param>
    /// <returns></returns>
    public bool EvaluateBlackboardVsBlackboard(Blackboard blackboardA, string keyA, Blackboard blackboardB, string keyB, FloatComparison floatComparison)
    {
        Blackboard sceneBlackboardA = FindBlackboard(blackboardA);
        Value valueA = sceneBlackboardA.GetValue(keyA);
        Blackboard sceneBlackboardB = FindBlackboard(blackboardB);
        Value valueB = sceneBlackboardB.GetValue(keyB);
                
        return CompareFloat(valueA.m_float, valueB.m_float, floatComparison);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboardA"></param>
    /// <param name="keyA"></param>
    /// <param name="blackboardB"></param>
    /// <param name="keyB"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public bool EvaluateBlackboardVsBlackboard(Blackboard blackboardA, string keyA, Blackboard blackboardB, string keyB, StringComparison stringComparison)
    {
        Blackboard sceneBlackboardA = FindBlackboard(blackboardA);
        Value valueA = sceneBlackboardA.GetValue(keyA);
        Blackboard sceneBlackboardB = FindBlackboard(blackboardB);
        Value valueB = sceneBlackboardB.GetValue(keyB);

        return CompareString(valueA.m_string, valueB.m_string, stringComparison);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueA"></param>
    /// <param name="valueB"></param>
    /// <returns></returns>
    private bool CompareBoolean(bool valueA, bool valueB, BoolComparison boolComparison)
    {
        switch (boolComparison)
        {
            default:
                return valueA == valueB;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueA"></param>
    /// <param name="valueB"></param>
    /// <param name="floatComparison"></param>
    /// <returns></returns>
    private bool CompareFloat(float valueA, float valueB, FloatComparison floatComparison)
    {
        switch (floatComparison)
        {
            case FloatComparison.LessThan:
                return valueA < valueB; // float is less than value

            case FloatComparison.EqualTo:
                return valueA == valueB; // float is equal to value

            case FloatComparison.GreaterThan:
                return valueA > valueB; // float is greater than value
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueA"></param>
    /// <param name="valueB"></param>
    /// <returns></returns>
    private bool CompareString(string valueA, string valueB, StringComparison stringComparison)
    {
        switch (stringComparison)
        {
            default:
                return valueA == valueB;
        }
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

}

}