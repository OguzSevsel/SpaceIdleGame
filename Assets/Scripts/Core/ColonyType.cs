using System.Collections.Generic;
using UnityEngine;

public enum ColonyTypeEnum
{
    Earth,
    TheMoon,
    Mars,
    Titan,
    Mercury,
    Ceres
}

[CreateAssetMenu(fileName = "New Colony Type", menuName = "Scriptable Objects/ New Colony Type")]
public class ColonyType : ScriptableObject
{
    public ColonyTypeEnum ColonyTypeName;
}
