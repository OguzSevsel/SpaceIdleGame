using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveUtility : MonoBehaviour
{
    public TextMeshProUGUI saveNameInputField;
    public Button saveButton1;
    public Button saveButton2;
    public Button saveButton3;
    public Button loadButton1;
    public Button loadButton2;
    public Button loadButton3;
    public Button deleteButton1;
    public Button deleteButton2;
    public Button deleteButton3;
    public TextMeshProUGUI saveSlotText1;
    public TextMeshProUGUI saveSlotText2;
    public TextMeshProUGUI saveSlotText3;
    public GameObject savePanel;
    public SaveManager saveManager;

    public void OnSavePanelOpenClicked()
    {
        if (savePanel.activeSelf)
        {
            savePanel.SetActive(false);
            return;
        }

        savePanel.SetActive(true);

        IEnumerable<GameData> saveFiles = GetAllSaves();

        foreach (var save in saveFiles)
        {
            if (save.metadata.saveSlot == 1)
            {
                saveSlotText1.text = save.metadata.slotName;
            }

            if (save.metadata.saveSlot == 2)
            {
                saveSlotText2.text = save.metadata.slotName;
            }

            if (save.metadata.saveSlot == 3)
            {
                saveSlotText3.text = save.metadata.slotName;
            }
        }

        saveButton1.onClick.AddListener(() => {
            saveManager.Save(saveNameInputField.text, 1);
            saveSlotText1.text = saveNameInputField.text;
        });

        saveButton2.onClick.AddListener(() => {
            saveManager.Save(saveNameInputField.text, 2);
            saveSlotText2.text = saveNameInputField.text;
        });

        saveButton3.onClick.AddListener(() => {
            saveManager.Save(saveNameInputField.text, 3);
            saveSlotText3.text = saveNameInputField.text;
        });

        deleteButton1.onClick.AddListener(() =>
        {
            saveManager.DeleteSave(saveSlotText1.text);
            saveSlotText1.text = "Save Slot 1";
        });

        deleteButton2.onClick.AddListener(() =>
        {
            saveManager.DeleteSave(saveSlotText2.text);
            saveSlotText2.text = "Save Slot 2";
        });

        deleteButton3.onClick.AddListener(() =>
        {
            saveManager.DeleteSave(saveSlotText3.text);
            saveSlotText3.text = "Save Slot 3";
        });

        loadButton1.onClick.AddListener(() => saveManager.Load(saveSlotText1.text));
        loadButton2.onClick.AddListener(() => saveManager.Load(saveSlotText2.text));
        loadButton3.onClick.AddListener(() => saveManager.Load(saveSlotText3.text));
    }

    public static IEnumerable<GameData> GetAllSaves()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "save_*.json");
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            GameData data = JsonConvert.DeserializeObject<GameData>(json, settings);
            if (data != null) yield return data;
        }
    }
}
