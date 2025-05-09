using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public int slotIndex;
    private ActiveInventory activeInventory;

    private void Start()
    {
        activeInventory = GetComponentInParent<ActiveInventory>();
    }
}
