using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogNode
{
    public string NodeId;
    public Sprite Portrait;
    public string SpeakerName;
    [Header("")]
    [Header("Dont Add More Than 4 Options")]
    [Header("")]
    public List<DialogOption> DialogOptions;
    [TextArea(2, 6)] public string DialogText;
}

[System.Serializable]
public class DialogOption
{
    [TextArea(2, 6)] public string OptionText;
    public string NextNodeId;
}


