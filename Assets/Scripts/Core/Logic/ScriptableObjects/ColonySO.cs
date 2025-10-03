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
    //[HideInInspector] public string _guid;

    //private void OnEnable()
    //{
    //    if (string.IsNullOrEmpty(_guid))
    //        _guid = System.Guid.NewGuid().ToString();
    //}

    //public string ColonyGUID => _guid;
    public ColonyType ColonyType;
    public ResourceSO Money;
}
