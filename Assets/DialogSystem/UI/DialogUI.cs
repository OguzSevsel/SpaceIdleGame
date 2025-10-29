using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DialogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private TextMeshProUGUI _speakerNameText;
    [SerializeField] private UnityEngine.UI.Image _speakerPortrait;
    [SerializeField] private GameObject _choicesContainer;
    [SerializeField] private GameObject _choiceButtonPrefab;
    [SerializeField] private DialogVariables variables;
    private Coroutine typingCoroutine;

    public void CreateNewDialog(DialogNode dialogNode)
    {
        string dialogText = dialogNode.DialogText;

        foreach (Transform child in _choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        _speakerNameText.text = dialogNode.SpeakerName;
        _speakerPortrait.sprite = dialogNode.Portrait;

        dialogText = variables.ReplaceVariables(dialogText);

        typingCoroutine = StartCoroutine(TypeText(dialogText, () =>
        {
            foreach (var option in dialogNode.DialogOptions)
            {
                GameObject choiceButtonObj = Instantiate(_choiceButtonPrefab, _choicesContainer.transform);
                TextMeshProUGUI choiceButtonText = choiceButtonObj.GetComponentInChildren<TextMeshProUGUI>();
                Button choiceButton = choiceButtonObj.GetComponent<Button>();

                string optionText = dialogNode.DialogOptions[dialogNode.DialogOptions.IndexOf(option)].OptionText;
                choiceButtonText.text = variables.ReplaceVariables(optionText);

                choiceButton.onClick.AddListener(() =>
                {
                    if (string.IsNullOrEmpty(option.NextNodeId))
                    {
                        EndDialog();
                    }
                    else
                    {
                        CreateNewDialog(DialogManager.Instance.GetDialogNodeById(option.NextNodeId));
                    }
                });
            }
        })); 
    }

    private IEnumerator TypeText(string text, System.Action onComplete)
    {
        _dialogText.text = "";
        foreach (char c in text)
        {
            _dialogText.text += c;
            yield return new WaitForSeconds(0.015f);
        }
        onComplete?.Invoke();
    }

    public void EndDialog()
    {
        this.gameObject.SetActive(false);
    }
}
