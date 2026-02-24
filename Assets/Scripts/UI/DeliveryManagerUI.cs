using System;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
   [SerializeField] private Transform container;
   [SerializeField] private Transform receipeTemplate;

    private void Start()
    {
        DeliveryManager.Instance.OnReceipeSpawned += Delivery_OnReceipeSpawned;
        DeliveryManager.Instance.OnReceipeCompleted += Delivery_OnReceipeCompleted;
        UpdateVisual();
    }

    private void Delivery_OnReceipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void Delivery_OnReceipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void Awake()
    {
        receipeTemplate.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == receipeTemplate) continue;
                Destroy(child.gameObject);
        }

       foreach (ReceipeSO receipeSO in DeliveryManager.Instance.GetWaitingReceipeSOList())
       {
            Transform receipeTransform = Instantiate(receipeTemplate, container);
            receipeTransform.gameObject.SetActive(true);
            receipeTransform.GetComponent<DeliveryManagerSingleUI>().SetReceipeSO(receipeSO);
       } 
    }
}
