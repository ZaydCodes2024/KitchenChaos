using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;

    public event EventHandler OnPlateRemoved;
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int plateSpawnAmount;
    private int plateSpawnAmountMax = 4;

    private void Update()
    {
        if (!IsServer)  return;
        
        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if (GameManager.Instance.IsGamePlaying() && plateSpawnAmount < plateSpawnAmountMax)
            {
               SpawnPlaterServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlaterServerRpc()
    {
        SpawnPlaterClientRpc();
    }

    [ClientRpc]
    private void SpawnPlaterClientRpc()
    {
        plateSpawnAmount++;
        OnPlateSpawned?.Invoke(this,EventArgs.Empty);
    }
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (plateSpawnAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        plateSpawnAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
