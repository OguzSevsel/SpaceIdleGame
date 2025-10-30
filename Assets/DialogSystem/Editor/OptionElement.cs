using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class OptionElement
{
    private TextField _optionTextField;
    public DropdownField NextIdField { get; private set; }
    private Button _addOptionButton;
    private VisualElement _optionContainer;
    public VisualElement OptionCard { get; private set; }
    public DialogOption DialogOption { get; private set; }
    public NodeElement ParentNode { get; set; }

    public event Action<OptionElement> OnOptionCreated;

    public OptionElement()
    {
        OptionCard = CreateOptionCard();
    }

    private VisualElement CreateOptionCard()
    {
        _optionTextField = new TextField("Option Text:");
        NextIdField = new DropdownField("Next Id:");
        _addOptionButton = new Button();
        _addOptionButton.text = "Create Option Dialog";

        _optionContainer = new VisualElement();
        _optionContainer.name = "OptionContainer";

        _optionContainer.Add(_optionTextField);
        _optionContainer.Add(NextIdField);
        _optionContainer.Add(_addOptionButton);
        _addOptionButton.clicked += AddOption;

        return _optionContainer;
    }

    private void AddOption()
    {
        DialogOption = new DialogOption
        {
            OptionText = _optionTextField.value,
            NextNodeId = NextIdField.value
        };

        OnOptionCreated?.Invoke(this);
    }

    public void PopulateNodeId(List<NodeElement> nodes)
    {
        NextIdField.choices.Clear();
        foreach (var node in nodes)
        {
            if (node != ParentNode && node.NodeIdField.value != string.Empty)
            {
                NextIdField.choices.Add(node.NodeIdField.value);
            }
        }
    }
}