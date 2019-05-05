using UnityEngine;
using UnityEditor;


namespace DEVN
{

/// <summary>
/// a node which dictates the end of a scene. No outputs
/// </summary>
[System.Serializable]
public class EndNode : BaseNode
{
    [SerializeField] private Scene m_nextScene;
    public Scene GetNextScene() { return m_nextScene; }

#if UNITY_EDITOR

	public override void Init(Vector2 position)
    {
        base.Init(position);
        
        m_rectangle.width = 100;
        m_rectangle.height = 58;

        m_title = "End";
    }

    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);
        
        // copy relevant variable/s
        m_nextScene = (node as EndNode).GetNextScene();
    }

    protected override void DrawNodeWindow(int id)
    {
        float fieldWidth = m_rectangle.width - 10;
        float fieldHeight = 16;

		Rect fieldRect = new Rect(5, 20, fieldWidth, fieldHeight);

		// draw label and object field for "next scene"
		GUI.Label(fieldRect, "Next Scene");
		fieldRect.y += 16;
        m_nextScene = EditorGUI.ObjectField(fieldRect, m_nextScene, typeof(Scene), false) as Scene;

        base.DrawNodeWindow(id);
	}

#endif
}

}