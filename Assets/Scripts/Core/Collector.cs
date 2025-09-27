using System;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public CollectorType CollectorType;
    private float collectionTimer;
    private bool isCollecting;

    private ColonyTypeEnum _colonyType;

    private void Start()
    {
        _colonyType = GetComponentInParent<Colony>().ColonyType.ColonyTypeName;
    }

    void Update()
    {
        if (isCollecting)
        {
            collectionTimer += Time.deltaTime;

            EventBus.Publish(new CollectorProgressEvent() 
            { 
                collectorType = CollectorType.CollectorTypeName, 
                progress = Mathf.Clamp01(collectionTimer /(float)CollectorType.Speed) , timeRemaining = (float)CollectorType.Speed - collectionTimer 
            });

            if (collectionTimer >= CollectorType.Speed)
            {
                collectionTimer = 0f;
                CollectorType.Resource.AddAmount(CollectorType.CollectionRate);
                isCollecting = false;
                EventBus.Publish(new CollectorFinishedEvent() 
                { 
                    collectorType = this.CollectorType, 
                    colonyType = GetComponentInParent<Colony>().ColonyType.ColonyTypeName 
                });

                EventBus.Publish(new CollectorProgressEvent() 
                { 
                    collectorType = CollectorType.CollectorTypeName,
                    colonyType = _colonyType,
                    progress = Mathf.Clamp01(0f), timeRemaining = 0f
                });
            }
        }
    }

    public void Collect()
    {
        isCollecting = true;
    }
}
