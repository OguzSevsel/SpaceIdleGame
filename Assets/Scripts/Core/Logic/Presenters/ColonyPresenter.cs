using System;
using System.Collections.Generic;
using UnityEngine;

public class ColonyPresenter : MonoBehaviour
{
    private ColonyModel _colonyModel;
    private ColonyView _colonyView;
    private List<CollectorPresenter> _collectorPresenters = new();

    public void Initialize(ColonyModel colonyModel, ColonyView colonyView)
    {
        _colonyModel = colonyModel;
        _colonyView = colonyView;

        for (int i = 0; i < _colonyModel.Collectors.Count; i++)
        {
            var collectorModel = _colonyModel.Collectors[i];
            var collectorView = _colonyView.CollectorPanels[i];
            var collectorPresenter = new CollectorPresenter(collectorModel, collectorView);
            _collectorPresenters.Add(collectorPresenter);
        }
    }

    private void ChangeResourceText(Resource resource)
    {
        _colonyView.UpdateResourceText(resource.ResourceSO, resource.ResourceAmount);
    }













    private void OnResourceAddedHandler(Resource resource)
    {
        ChangeResourceText(resource);
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        ColonyModel.OnResourceAdded += OnResourceAddedHandler;
    }

    private void UnSubscribe()
    {
        ColonyModel.OnResourceAdded -= OnResourceAddedHandler;
    }
}
