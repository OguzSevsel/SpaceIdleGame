using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Graph", menuName = "Dialog System/Dialog Graph")]
public class DialogGraph : ScriptableObject
{
    public string GraphId;
    public List<DialogNodeSO> DialogNodes;

    public DialogNode GetNodeById(string nodeId)
    {
        DialogNodeSO node = DialogNodes.Find(node => node.DialogNode.NodeId == nodeId);
        return node.DialogNode;
    }

    private void OnEnable()
    {
        if (DialogNodes == null)
            DialogNodes = new List<DialogNodeSO>();
    }
}