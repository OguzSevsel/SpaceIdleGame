using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogNode
{
    public string NodeId;
    public Sprite Portrait;
    public string SpeakerName;
    [TextArea(2, 6)] public string DialogText;
    public List<DialogOption> DialogOptions;
}

[System.Serializable]
public class DialogOption
{
    [TextArea(2, 6)] public string OptionText;
    public string NextNodeId;
}


