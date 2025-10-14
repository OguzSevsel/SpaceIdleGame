using System;
using System.Collections.Generic;
using UnityEngine;

public class ColonyPresenter : MonoBehaviour
{
    private ColonyModel _colonyModel;
    private ColonyView _colonyView;
    private List<CollectorPresenter> _collectorPresenters = new();

    private bool _isShowInfo;

    private void Update()
    {
        if (_isShowInfo)
        {
            if (_colonyModel != null && _colonyView != null)
            {
                _colonyView.ChangeSellButtonUI();
                _colonyView.SetSellText();
            }
        }
    }

    public void Initialize(ColonyModel colonyModel, ColonyView colonyView)
    {
        _colonyModel = colonyModel;
        _colonyView = colonyView;

        _colonyView.OnSelectedResourceSell += SelectedResourceSellHandler;
        _colonyModel.OnResourceAdded += OnResourceAddedHandler;

        for (int i = 0; i < _colonyModel.Collectors.Count; i++)
        {
            var collectorModel = _colonyModel.Collectors[i];
            var collectorView = _colonyView.CollectorPanels[i];
            var collectorPresenter = new CollectorPresenter(collectorModel, collectorView);
            _collectorPresenters.Add(collectorPresenter);
        }

        InitializeSellButtons(_colonyModel.Resources);
    }

    private void SelectedResourceSellHandler(Resource resource)
    {
        _colonyModel.SellResource(resource, resource.ResourceAmount);
    }

    private void InitializeSellButtons(List<Resource> resources)
    {
        _colonyView.InitializeSellButtons(resources);
    }

    private void ChangeResourceText(Resource resource, int index)
    {
        _colonyView.UpdateResourceText(resource.ResourceSO, resource.ResourceAmount, index);
    }













    private void OnResourceAddedHandler(Resource resource, int index)
    {
        ChangeResourceText(resource, index);
    }

    private void SellResourceShowHandler()
    {
        _isShowInfo = true;
    }

    private void SellResourceHideHandler()
    {
        _isShowInfo = false;
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
        TabGroup.OnSellResourceHide += SellResourceHideHandler;
        TabGroup.OnSellResourceShow += SellResourceShowHandler;
    }

    private void UnSubscribe()
    {
        TabGroup.OnSellResourceHide -= SellResourceHideHandler;
        TabGroup.OnSellResourceShow -= SellResourceShowHandler;
    }

    
}
