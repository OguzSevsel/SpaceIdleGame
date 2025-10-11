using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SpriteObjectField : ObjectField
{
    public new class UxmlFactory : UxmlFactory<SpriteObjectField, UxmlTraits> { }
}

public class ScriptableObjectField : ObjectField
{
    public new class UxmlFactory : UxmlFactory<ScriptableObjectField, UxmlTraits> { }
}
