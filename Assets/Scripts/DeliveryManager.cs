using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set;}
    private List<ReceipeSO> waitingReceipeSOList;
    [SerializeField] private ReceipeListSO receipeListSO;
    private float spawnReceipeTimer;
    private float spawnReceipeTimerMax = 4f;
    private int waitingReceipeMax = 4;

    private void Awake()
    {
        Instance = this;
        waitingReceipeSOList = new List<ReceipeSO>();
    }
    private void Update()
    {
        spawnReceipeTimer -= Time.deltaTime;
        if (spawnReceipeTimer <= 0f)
        {
            spawnReceipeTimer = spawnReceipeTimerMax;

            if (waitingReceipeSOList.Count < waitingReceipeMax)
            {
                ReceipeSO waitingReceipeSO = receipeListSO.receipeSOList[Random.Range(0, receipeListSO.receipeSOList.Count)];
                Debug.Log(waitingReceipeSO.receipeName);
                waitingReceipeSOList.Add(waitingReceipeSO);
            }
        }
    }

    public void DeliverReceipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingReceipeSOList.Count; i++)
        {
            ReceipeSO waitingReceipeSO = waitingReceipeSOList[i];

            if (waitingReceipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) // Has the same number of ingredients
            {
                bool plateContentMatchesReceipe = true;

                foreach (KitchenObjectSO receipeKitchenObjectSO in waitingReceipeSO.kitchenObjectSOList) // Cycles through all ingredients in the receipe
                {
                    bool ingredientFound = false;

                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) // Cycles through all ingredients in the receipe
                    {
                        if (plateKitchenObjectSO == receipeKitchenObjectSO) // Ingredient Matched!
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound) // This receipe ingredient was not found on the plate
                    {
                        plateContentMatchesReceipe = false;
                    }
                }

                if (plateContentMatchesReceipe) // player delivered the correct receipe
                {
                    Debug.Log("Player Delivered the correct receipe!");
                    waitingReceipeSOList.RemoveAt(i);
                    return;
                }
            }
        }
        // no matches found, player didn'y deliver the correct receipe
        Debug.Log("Incorrect receipe");
    }
}
