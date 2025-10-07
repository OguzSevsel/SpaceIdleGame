using System.Collections.Generic;
using UnityEngine;

public class ColonyManager : MonoBehaviour
{
    [SerializeField] private List<ColonyModel> colonyModels;
    [SerializeField] private List<ColonyView> colonyViews;
    private List<ColonyPresenter> colonyPresenters = new();

    void Start()
    {
        for (int i = 0; i < colonyModels.Count; i++)
        {
            ColonyPresenter presenter = new GameObject($"ColonyPresenter_{i}").AddComponent<ColonyPresenter>();
            presenter.Initialize(colonyModels[i], colonyViews[i]);
            colonyPresenters.Add(presenter);
        }
    }
}
