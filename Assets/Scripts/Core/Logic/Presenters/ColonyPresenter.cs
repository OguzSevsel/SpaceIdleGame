using System;
using System.Collections.Generic;
using UnityEngine;

public class ColonyPresenter : MonoBehaviour
{
    private ColonyModel _colonyModel;
    private ColonyView _colonyView;
    private List<CollectorPresenter> _collectorPresenters = new();
    private bool _isShowInfo;

    //Actual logic that showing the sell panel info.
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

    /// <summary>
    /// Initialization of the UI, Events, and collector MVP (Model, view, presenters), our only presenter structure that live in the scene are colony presenters, and there is a cascading scructure that, colony presenters create our collector presenters and upgrades. 
    /// </summary>
    /// <param name="colonyModel"></param>
    /// <param name="colonyView"></param>
    public void Initialize(ColonyModel colonyModel, ColonyView colonyView)
    {
        _colonyModel = colonyModel;
        _colonyView = colonyView;

        _colonyView.OnSelectedResourceSell += SelectedResourceSellHandler;
        _colonyModel.OnResourceAdded += OnResourceAddedHandler;
        _colonyModel.OnResourceSpend += OnResourceSpendHandler;
        
        for (int i = 0; i < _colonyModel.Collectors.Count; i++)
        {
            var collectorModel = _colonyModel.Collectors[i];
            var collectorView = _colonyView.CollectorPanels[i];
            var collectorPresenter = new CollectorPresenter(collectorModel, collectorView);
            _collectorPresenters.Add(collectorPresenter);
        }
        
        InitializeSellButtons(_colonyModel.Resources);
        InitializeResourceTexts(_colonyModel.Resources);
    }

    private void OnResourceSpendHandler(Resource resource, int index)
    {
        _colonyView.UpdateResourceText(resource.ResourceSO, resource.ResourceAmount, index);
    }

    /// <summary>
    /// Sell the resource 
    /// </summary>
    /// <param name="resource"></param>
    private void SelectedResourceSellHandler(Resource resource)
    {
        _colonyModel.SellResource(resource, resource.ResourceAmount);
    }

    /// <summary>
    /// Creating the resource selection buttons in the sell screen.  
    /// </summary>
    /// <param name="resources"></param>
    private void InitializeSellButtons(List<Resource> resources)
    {
        _colonyView.InitializeSellButtons(resources);
    }


    /// <summary>
    /// Initializing resource texts according to the resources colony has. 
    /// </summary>
    /// <param name="colonyResources"></param>
    private void InitializeResourceTexts(List<Resource> colonyResources)
    {
        _colonyView.InitializeResourceTexts(colonyResources);
    }


    /// <summary>
    /// Updating resource texts after the collection happens. 
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="index"></param>
    private void OnResourceAddedHandler(Resource resource, int index)
    {
        _colonyView.UpdateResourceText(resource.ResourceSO, resource.ResourceAmount, index);
    }

    /// <summary>
    /// In this files update method we do handle the changing sell button ui according to the ongoing resource collection. This is the show method for it because of the performance we only updating it when player opens that panel. 
    /// </summary>
    private void SellResourceShowHandler()
    {
        _isShowInfo = true;
    }


    /// <summary>
    /// In this files update method we do handle the changing sell button ui according to the ongoing resource collection. This is the hide method of it because of the performance constantly updating it. 
    /// </summary>
    private void SellResourceHideHandler()
    {
        _isShowInfo = false;
    }


    //Generic event subscribe and unsubscribe methods.
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
