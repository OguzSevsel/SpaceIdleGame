using UnityEngine;

public enum ColonyType
{
    Earth,
    TheMoon,
    Mars,
    Titan,
    Mercury,
    Ceres
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Colony", fileName = "New Colony")]
public class ColonySO : ScriptableObject
{
    public ColonyType ColonyType;
}
