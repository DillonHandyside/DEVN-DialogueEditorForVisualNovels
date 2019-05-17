using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

public class SaveManager
{
	// scene manager ref
	private SceneManager m_sceneManager;
		
	private GameObject m_saveSlotPrefab;
	private GameObject m_savePanel;
	private RectTransform m_saveContent;
	
	private int m_numberOfSaves;

	private GameData m_gameData;
	private SaveData m_currentData = new SaveData();

	public SaveManager(SceneManager sceneManager, SaveComponent saveComponent)
	{
		m_sceneManager = sceneManager;

		m_saveSlotPrefab = saveComponent.GetSaveSlotPrefab();
		m_savePanel = saveComponent.GetSavePanel();
		m_saveContent = saveComponent.GetSaveContent();
		m_numberOfSaves = saveComponent.GetNumberOfSaves();

		m_gameData = LoadGameData();

		if (m_gameData == null)
			m_gameData = new GameData(saveComponent.GetNumberOfSaves());
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="currentScene"></param>
	/// <param name="node"></param>
	public void Update(Scene currentScene, BaseNode node)
	{
		//m_currentData.GetNodePath().Add(node);
		m_currentData.SetBlackboards(m_sceneManager.GetBlackboards());

		m_currentData.SetCurrentScene(currentScene);
		m_currentData.SetCurrentNode(m_sceneManager.GetCurrentNode());

		// save background
		Sprite background = m_sceneManager.GetBackgroundManager().GetBackground();
		m_currentData.SetBackground(background);
		
		// save characters
		List<GameObject> characters = m_sceneManager.GetCharacterManager().GetCharacters();
		m_currentData.SetCharacters(characters);
	}

	/// <summary>
	/// 
	/// </summary>
	public void OpenSaveLoadMenu(bool isLoad = false)
	{
		List<SaveData> saveData = m_gameData.GetSaves();
		
		for (int i = 0; i < saveData.Count; i++)
		{
			GameObject saveSlot = Object.Instantiate(m_saveSlotPrefab, m_saveContent);
			Button saveButton = saveSlot.GetComponent<Button>();
			Text saveText = saveSlot.GetComponentInChildren<Text>();

			if (saveData[i] != null && saveData[i].GetCurrentScene() != null)
				saveText.text = saveData[i].GetCurrentScene().name;
			else
				saveText.text = "Empty Save Slot";
			
			int saveIndex = i;
			saveButton.onClick.AddListener(CloseSaveLoadMenu);

			if (isLoad)
				saveButton.onClick.AddListener(() => Load(saveIndex));
			else
				saveButton.onClick.AddListener(() => Save(saveIndex));

			//
		}

		m_savePanel.SetActive(true); // open save menu
	}

	/// <summary>
	/// 
	/// </summary>
	public void CloseSaveLoadMenu()
	{
		m_savePanel.SetActive(false); // close menu
		Button[] saveSlots = m_saveContent.GetComponentsInChildren<Button>();

		for (int i = 0; i < saveSlots.Length; i++)
			Object.Destroy(saveSlots[i].gameObject);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="saveIndex"></param>
	public void Save(int saveIndex)
	{
		m_gameData.SetSave(m_currentData, saveIndex);

		string path = Application.persistentDataPath + "/data.sav";

		string data = JsonUtility.ToJson(m_gameData);
		File.WriteAllText(path, data);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="saveIndex"></param>
	public void Load(int saveIndex)
	{
		string path = Application.persistentDataPath + "/data.sav";

		if (!File.Exists(path))
			return; // save data doesn't exist

		string data = File.ReadAllText(path);
		GameData gameData = JsonUtility.FromJson<GameData>(data);

		SaveData saveData = gameData.GetSaves()[saveIndex];
		m_sceneManager.GetBackgroundManager().SetBackground(saveData.GetBackground());
		m_sceneManager.GetCharacterManager().SetCharacters(saveData.GetCharacters());

		m_sceneManager.JumpToNode(saveData.GetCurrentNode());
	}

	/// <summary>
	/// 
	/// </summary>
	private void CreateGameData()
	{
		string path = Application.persistentDataPath + "/data.sav";

		string data = JsonUtility.ToJson(m_gameData);
		File.WriteAllText(path, data);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private GameData LoadGameData()
	{
		string path = Application.persistentDataPath + "/data.sav";

		if (!File.Exists(path))
			CreateGameData();

		string data = File.ReadAllText(path);
		GameData gameData = JsonUtility.FromJson<GameData>(data);

		return gameData; // load successful
	}
}

}