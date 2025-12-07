using System.Collections.Generic;
using UnityEngine;

//This file is creating colony presenters from the lists it holds. Then colony presenters create other lower level presenters such as collector presenters.
public class ColonyManager : MonoBehaviour
{
    [SerializeField] private List<ColonyModel> colonyModels;
    [SerializeField] private List<ColonyView> colonyViews;

    private List<ColonyPresenter> colonyPresenters = new();

    private void Start()
    {
        for (int i = 0; i < colonyModels.Count; i++)
        {
            ColonyPresenter presenter = new GameObject($"ColonyPresenter_{i}").AddComponent<ColonyPresenter>();
            presenter.Initialize(colonyModels[i], colonyViews[i]);
            colonyPresenters.Add(presenter);
        }
    }

    public void AddToColonyModels(ColonyModel modelToAdd)
    {
        colonyModels.Add(modelToAdd);
    }

    public void AddToColonyViews(ColonyView viewToAdd)
    {
        colonyViews.Add(viewToAdd);
    }
}
