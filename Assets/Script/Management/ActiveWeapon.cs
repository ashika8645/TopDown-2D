using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    private int activeSlotIndexNum = 0;
    private Transform[] weapons;
    private bool isInputProcessed = false;
    private void Start()
    {
        weapons = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }

        ToggleActiveWeapon(activeSlotIndexNum);
    }

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
        ToggleActiveWeapon(slotIndex);
    }

    private void ScrollInventory(float scrollValue)
    {
        if (scrollValue > 0)
        {
            activeSlotIndexNum = (activeSlotIndexNum + 1) % weapons.Length;
        }
        else if (scrollValue < 0)
        {
            activeSlotIndexNum = (activeSlotIndexNum - 1 + weapons.Length) % weapons.Length;
        }
        ToggleActiveWeapon(activeSlotIndexNum);
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
        ToggleActiveWeapon(activeSlotIndexNum);
    }

    private void ToggleActiveWeapon(int indexNum)
    {
        activeSlotIndexNum = indexNum;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == indexNum);

            if (i == indexNum)
            {
                weapons[i].SendMessage("ResetState", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
