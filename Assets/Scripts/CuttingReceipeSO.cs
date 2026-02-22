using UnityEngine;

[CreateAssetMenu()]
public class CuttingReceipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}
