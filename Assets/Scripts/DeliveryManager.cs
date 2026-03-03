using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public static DeliveryManager Instance { get; private set;}
    public event EventHandler OnReceipeSpawned;
    public event EventHandler OnReceipeCompleted;
    public event EventHandler OnReceipeSuccess;
    public event EventHandler OnReceipeFailed;
    private List<ReceipeSO> waitingReceipeSOList;
    [SerializeField] private ReceipeListSO receipeListSO;
    private float spawnReceipeTimer = 4f;
    private float spawnReceipeTimerMax = 4f;
    private int waitingReceipeMax = 4;
    private int successfullReceipeAmount;

    private void Awake()
    {
        Instance = this;
        waitingReceipeSOList = new List<ReceipeSO>();
    }
    private void Update()
    {
        if (!IsServer)  return;

        spawnReceipeTimer -= Time.deltaTime;
        if (spawnReceipeTimer <= 0f)
        {
            spawnReceipeTimer = spawnReceipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingReceipeSOList.Count < waitingReceipeMax)
            {
                int waitingReceipeSOIndex = UnityEngine.Random.Range(0, receipeListSO.receipeSOList.Count);
                
                SpawnNewWaitingReceipeClientRpc(waitingReceipeSOIndex);
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingReceipeClientRpc(int waitingReceipeSOIndex)
    {
        ReceipeSO waitingReceipeSO = receipeListSO.receipeSOList[waitingReceipeSOIndex];
        waitingReceipeSOList.Add(waitingReceipeSO);
        OnReceipeSpawned?.Invoke(this,EventArgs.Empty);
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
                    waitingReceipeSOList.RemoveAt(i);

                    OnReceipeCompleted?.Invoke(this,EventArgs.Empty);
                    OnReceipeSuccess?.Invoke(this,EventArgs.Empty);
                    successfullReceipeAmount++;
                    return;
                }
            }
        }
        // no matches found, player didn'y deliver the correct receipe
        OnReceipeFailed?.Invoke(this,EventArgs.Empty);
    }

    public List<ReceipeSO> GetWaitingReceipeSOList()
    {
        return waitingReceipeSOList;
    }

    public int GetSuccessfullReceipeAmount()
    {
        return successfullReceipeAmount;
    }
}
