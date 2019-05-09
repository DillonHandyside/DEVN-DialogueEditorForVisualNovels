using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

public class CharacterTransformer
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="node"></param>
	public void EvaluateTransformNode(BaseNode node)
	{
		SceneManager sceneManager = SceneManager.GetInstance();
		CharacterManager characterManager = CharacterManager.GetInstance();

		if (node is CharacterInvertNode)
		{
			CharacterInvertNode invertNode = node as CharacterInvertNode;
			InvertCharacter(characterManager.TryGetCharacter(invertNode.GetCharacter()));
			sceneManager.NextNode(); // proceed to next node
		}
		else if (node is CharacterScaleNode)
		{
			CharacterScaleNode scaleNode = node as CharacterScaleNode;
			GameObject character = characterManager.TryGetCharacter(scaleNode.GetCharacter());
			Vector2 scale = scaleNode.GetScale();

			if (scaleNode.GetIsLerp())
				characterManager.StartCoroutine(LerpCharacterScale(character, scale, scaleNode.GetLerpTime()));
			else
				SetCharacterScale(character, scale);

			sceneManager.NextNode();
		}
		else if (node is CharacterTranslateNode)
		{
			CharacterTranslateNode translateNode = node as CharacterTranslateNode;
			GameObject character = characterManager.TryGetCharacter(translateNode.GetCharacter());
			Vector2 position = translateNode.GetTranslation();

			if (translateNode.GetIsLerp())
				characterManager.StartCoroutine(LerpCharacterPosition(character, position, translateNode.GetLerpTime()));
			else
				SetCharacterPosition(character, position);

			sceneManager.NextNode();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	public void InvertCharacter(GameObject character)
	{
		float currentX = character.transform.localScale.x;
		character.transform.localScale = new Vector3(-currentX, 1, 1);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <param name="xPosition"></param>
	public void SetCharacterPosition(GameObject character, Vector2 position)
	{
        Vector2 desiredPosition = GetScreenPosition(position);

		// perform set
		RectTransform characterTransform = character.GetComponent<RectTransform>();
		characterTransform.anchoredPosition = desiredPosition;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="character"></param>
	/// <param name="position"></param>
	/// <param name="lerpTime"></param>
	/// <returns></returns>
	public IEnumerator LerpCharacterPosition(GameObject character, Vector2 position, float lerpTime)
	{
		RectTransform characterTransform = character.GetComponent<RectTransform>();
        Vector2 currentPosition = characterTransform.anchoredPosition;
        Vector2 desiredPosition = GetScreenPosition(position);

		float elapsedTime = 0;

		while (elapsedTime < lerpTime)
		{
			float percentage = elapsedTime / lerpTime;

            float positionX = Mathf.Lerp(currentPosition.x, desiredPosition.x, percentage);
            float positionY = Mathf.Lerp(currentPosition.y, desiredPosition.y, percentage);

            Vector3 newPosition = new Vector3(positionX, positionY, 0);
            characterTransform.anchoredPosition = newPosition;

			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector2 GetScreenPosition(Vector2 position)
    {
        GameObject inactiveCharacterPanel = CharacterManager.GetInstance().m_inactiveCharacterPanel;

        //
        RectTransform panelTransform = inactiveCharacterPanel.GetComponent<RectTransform>();
        float screenExtentX = panelTransform.rect.width * 0.5f;
        float screenExtentY = panelTransform.rect.height * 0.5f;

        // get x and y scalars between -1 and 1 (left is -1, center is 0, right is 1)
        float xScalar = (position.x * 2 - 100) * 0.01f;
        float yScalar = (position.y * 2 - 100) * 0.01f;

        return new Vector2(screenExtentX * xScalar, screenExtentY * yScalar);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="character"></param>
    /// <param name="scale"></param>
    public void SetCharacterScale(GameObject character, Vector2 scale)
	{
		Vector3 desiredScale = new Vector3(scale.x, scale.y, 1);
		character.transform.localScale = desiredScale;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="character"></param>
    /// <param name="scale"></param>
    /// <param name="lerpTime"></param>
    /// <returns></returns>
    public IEnumerator LerpCharacterScale(GameObject character, Vector2 scale, float lerpTime)
	{
        Vector2 currentScale = character.transform.localScale;
		Vector2 desiredScale = new Vector3(scale.x, scale.y);
		float elapsedTime = 0;

		while (elapsedTime < lerpTime)
		{
			float percentage = elapsedTime / lerpTime;

            float scaleX = Mathf.Lerp(currentScale.x, desiredScale.x, percentage);
            float scaleY = Mathf.Lerp(currentScale.y, desiredScale.y, percentage);

            character.transform.localScale = new Vector3(scaleX, scaleY, 1);

			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}
}

}
