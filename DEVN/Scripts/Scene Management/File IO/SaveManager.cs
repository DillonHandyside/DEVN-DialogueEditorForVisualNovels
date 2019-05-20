using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
public class SaveManager
{
	// scene manager ref
	private SceneManager m_sceneManager;
		
    // UI elements
	private GameObject m_saveSlotPrefab;
	private GameObject m_savePanel;
	private RectTransform m_saveContent;

	private GameData m_gameData;
	private SaveData m_currentData = new SaveData();

    private List<GameObject> m_saveSlots = new List<GameObject>();

    private Sprite m_snapshot;

	public SaveManager(SceneManager sceneManager, SaveComponent saveComponent)
	{
		m_sceneManager = sceneManager;

		m_saveSlotPrefab = saveComponent.GetSaveSlotPrefab();
		m_savePanel = saveComponent.GetSavePanel();
		m_saveContent = saveComponent.GetSaveContent();

		int numberOfSaves = saveComponent.GetNumberOfSaves();

        // load game data
		m_gameData = LoadGameData();

        // init game data if necessary
		if (m_gameData == null)
			m_gameData = new GameData(numberOfSaves);

        InitSaveLoadSlots(numberOfSaves);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numberOfSaves"></param>
    public void InitSaveLoadSlots(int numberOfSaves)
    {
        for (int i = 0; i < numberOfSaves; i++)
        {
            GameObject saveSlotObject = UnityEngine.Object.Instantiate(m_saveSlotPrefab, m_saveContent);
            m_saveSlots.Add(saveSlotObject);
        }

        UpdateSaveLoadSlots();
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isLoad"></param>
    public void UpdateSaveLoadSlots(bool isLoad = false)
    {
        List<SaveData> saveDatas = m_gameData.GetSaves();
        
        for (int i = 0; i < m_saveSlots.Count; i++)
        {
            GameObject saveSlotObject = m_saveSlots[i];
            SaveData saveData = saveDatas[i];
            SaveSlot saveSlot = saveSlotObject.GetComponent<SaveSlot>();

            // assign slot number and scene
            Text scene = saveSlot.GetSlotScene();
            scene.text = "Slot " + (i + 1) + " - ";

            if (saveData != null)
            {
                if (saveData.GetCurrentScene() != null)
                    scene.text += saveData.GetCurrentScene().name;
                else
                    scene.text += "Empty";

                //
                Image snapshot = saveSlot.GetSnapshot();
                snapshot.sprite = saveData.GetSnapshot();

                // assign playtime


                // assign date
                Text dateTime = saveSlot.GetDate();
                dateTime.text = saveData.GetDateTime();
            }

            // add listeners
            Button saveButton = saveSlotObject.GetComponent<Button>();
            int saveIndex = i;
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(() => m_savePanel.SetActive(false));

            if (isLoad)
            {
                saveButton.onClick.AddListener(() => Load(saveIndex));

                if (saveData == null)
                    saveButton.interactable = false;
            }
            else
            {
                saveButton.onClick.AddListener(() => Save(saveIndex));
                TakeSnapshot();
            }
        }
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="saveIndex"></param>
    /// <param name="currentScene"></param>
	public void Save(int saveIndex)
	{
		m_currentData.SetBlackboards(m_sceneManager.GetBlackboards());

		m_currentData.SetCurrentScene(m_sceneManager.GetCurrentScene());
		m_currentData.SetCurrentNode(m_sceneManager.GetCurrentNode());

		// save background
		Sprite background = m_sceneManager.GetBackgroundManager().GetBackground();
		m_currentData.SetBackground(background);
		
		// save characters
		List<GameObject> characters = m_sceneManager.GetCharacterManager().GetCharacters();
		m_currentData.SetCharacters(characters);
        
        //
        m_currentData.SetSnapshot(m_snapshot);

        // save date/time
        DateTime now = DateTime.Now;
        string dateTime = now.ToShortDateString() + "   " + now.ToShortTimeString();
        m_currentData.SetDateTime(dateTime);

		m_gameData.SetSave(m_currentData, saveIndex);
        m_currentData = new SaveData();

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

    /// <summary>
    /// 
    /// </summary>
    private void TakeSnapshot()
    {
        Texture2D snapshot = ScreenCapture.CaptureScreenshotAsTexture();

        Rect snapshotRect = new Rect(0, 0, snapshot.width, snapshot.height);
        Vector2 snapshotPivot = new Vector2(0.5f, 0.5f);
        m_snapshot = Sprite.Create(snapshot, snapshotRect, snapshotPivot);
    }
}

}