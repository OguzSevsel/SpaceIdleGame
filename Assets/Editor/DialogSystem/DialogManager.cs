using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
    [SerializeField] private DialogUI _dialogUI;
    [SerializeField] private DialogGraph _dialogGraph;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartDialog();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public DialogNode GetDialogNodeById(string nodeId)
    {
        return _dialogGraph.GetNodeById(nodeId);
    }

    public void StartDialog()
    {
        _dialogUI.CreateNewDialog(_dialogGraph.GetNodeById("deneme"));
    }
}
