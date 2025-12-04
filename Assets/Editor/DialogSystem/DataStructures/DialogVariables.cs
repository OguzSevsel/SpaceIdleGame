using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog System/Variables")]
public class DialogVariables : ScriptableObject
{
    [System.Serializable]
    public class Variable
    {
        public string key;
        public string value;
    }

    public List<Variable> variables = new List<Variable>();

    private Dictionary<string, string> _variableDictionary;

    private void OnEnable()
    {
        _variableDictionary = new Dictionary<string, string>();
        foreach (var v in variables)
            _variableDictionary[v.key] = v.value;
    }

    public string Get(string key)
    {
        if (_variableDictionary.TryGetValue(key, out var val))
            return val;
        return $"{{{key}}}";
    }

    public void Set(string key, string value)
    {
        _variableDictionary[key] = value;
    }

    public string ReplaceVariables(string text)
    {
        foreach (var kvp in _variableDictionary)
            text = text.Replace($"{{{kvp.Key}}}", kvp.Value);
        return text;
    }
}
