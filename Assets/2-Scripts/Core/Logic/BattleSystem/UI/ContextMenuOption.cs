using System.Collections.Generic;

public class ContextMenuOption
{
    public string Label;
    public System.Action Callback;
    public List<ContextMenuOption> SubOptions;
    public bool HasSubmenu => SubOptions != null && SubOptions.Count > 0;

    public ContextMenuOption(string label, System.Action callback = null, List<ContextMenuOption> subOptions = null)
    {
        Label = label;
        Callback = callback;
        SubOptions = subOptions;
    }
}
