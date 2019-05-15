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
        CharacterManager characterManager = SceneManager.GetInstance().GetCharacterManager();
        GameObject inactiveCharacterPanel = characterManager.GetBackgroundPanel();

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
