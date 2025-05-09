using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveInventory : MonoBehaviour
{
    private int activeSlotIndexNum = 0;
    private bool isInputProcessed = false;
    private void Update()
    {
        if (Input.GetButtonDown("Slot1"))
        {
            SelectSlot(0);
        }
        if (Input.GetButtonDown("Slot2"))
        {
            SelectSlot(1);
        }
        if (Input.GetButtonDown("Slot3"))
        {
            SelectSlot(2);
        }

        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue != 0)
        {
            ScrollInventory(scrollValue);
        }

        float controlValue = Input.GetAxis("ControllerInventory");

        if (controlValue != 0 && !isInputProcessed)
        {
            ControllerInventory(controlValue);
            isInputProcessed = true; 
        }

        if (controlValue == 0)
        {
            isInputProcessed = false;
        }
    }

    private void SelectSlot(int slotIndex)
    {
        ToggleActiveHighlight(slotIndex);
    }

    private void ScrollInventory(float scrollValue)
    {
        if (scrollValue > 0)
        {
            activeSlotIndexNum = (activeSlotIndexNum + 1) % transform.childCount;
        }
        else if (scrollValue < 0)
        {
            activeSlotIndexNum = (activeSlotIndexNum - 1 + transform.childCount) % transform.childCount;
        }
        ToggleActiveHighlight(activeSlotIndexNum);
    }

    private void ControllerInventory(float controlValue)
    {
        if (controlValue > 0)
        {
            activeSlotIndexNum = (activeSlotIndexNum + 1) % transform.childCount;
        }
        else if (controlValue < 0)
        {
            activeSlotIndexNum = (activeSlotIndexNum - 1 + transform.childCount) % transform.childCount;
        }
        ToggleActiveHighlight(activeSlotIndexNum);
    }
    private void ToggleActiveHighlight(int indexNum)
    {
        activeSlotIndexNum = indexNum;
        foreach (Transform inventorySlot in this.transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }
        this.transform.GetChild(indexNum).GetChild(0).gameObject.SetActive(true);
    }
}
