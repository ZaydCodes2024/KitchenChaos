using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ReceipeSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string receipeName;
}
