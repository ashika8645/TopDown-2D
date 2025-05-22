using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonHoverHandler : MonoBehaviour
{
    [System.Serializable]
    public class HoverButton
    {
        public GameObject buttonObject;
        [HideInInspector] public GameObject selectedBorder;
        [HideInInspector] public Button buttonComponent;
    }

    public List<HoverButton> buttons = new List<HoverButton>();

    private int currentIndex = 0;
    private float inputCooldown = 0.3f;
    private float nextInputTime = 0f;

    private bool usingGamepad = false;

    void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var hb = buttons[i];
            if (hb.buttonObject == null) continue;

            hb.buttonComponent = hb.buttonObject.GetComponent<Button>();

            var border = hb.buttonObject.transform.Find("Selected border");
            if (border != null)
            {
                hb.selectedBorder = border.gameObject;
                hb.selectedBorder.SetActive(false);
            }

            EventTrigger trigger = hb.buttonObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = hb.buttonObject.AddComponent<EventTrigger>();

            int index = i;

            EventTrigger.Entry enterEvent = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEvent.callback.AddListener((data) =>
            {
                usingGamepad = false;
                SetActiveBorder(index);
            });
            trigger.triggers.Add(enterEvent);

            EventTrigger.Entry exitEvent = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEvent.callback.AddListener((data) =>
            {
                if (!usingGamepad && buttons[index].selectedBorder != null)
                    buttons[index].selectedBorder.SetActive(false);
            });
            trigger.triggers.Add(exitEvent);
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Vertical");

        if (Mathf.Abs(x) > 0.5f && Time.unscaledTime >= nextInputTime)
        {
            usingGamepad = true;

            List<int> activeIndices = GetActiveButtonIndices();
            if (activeIndices.Count == 0)
                return;

            if (!AnyBorderActive())
            {
                int highestIndex = activeIndices[activeIndices.Count - 1];
                SetActiveBorder(highestIndex);
                currentIndex = highestIndex;
                nextInputTime = Time.unscaledTime + inputCooldown;
                return;
            }

            int idxInActive = activeIndices.IndexOf(currentIndex);
            if (idxInActive == -1)
            {
                idxInActive = activeIndices.Count - 1;
            }

            if (x > 0)
                idxInActive = (idxInActive - 1 + activeIndices.Count) % activeIndices.Count;
            else
                idxInActive = (idxInActive + 1) % activeIndices.Count;

            currentIndex = activeIndices[idxInActive];
            SetActiveBorder(currentIndex);
            Debug.Log("Current Index: " + currentIndex);
            nextInputTime = Time.unscaledTime + inputCooldown;
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (currentIndex >= 0 && currentIndex < buttons.Count)
            {
                if (buttons[currentIndex].buttonObject.activeInHierarchy && buttons[currentIndex].buttonComponent != null)
                {
                    buttons[currentIndex].buttonComponent.onClick.Invoke();
                }
            }
        }
    }


    public void ResetSelection()
    {
        List<int> activeIndices = GetActiveButtonIndices();
        if (activeIndices.Count > 0)
        {
            SetActiveBorder(activeIndices[0]); // Đặt lại nút đầu tiên làm mặc định
        }
    }

    void SetActiveBorder(int index)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].selectedBorder != null)
                buttons[i].selectedBorder.SetActive(i == index);
        }

        currentIndex = index;
    }

    List<int> GetActiveButtonIndices()
    {
        List<int> active = new List<int>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].buttonObject.activeInHierarchy)
                active.Add(i);
        }
        return active;
    }

    bool AnyBorderActive()
    {
        foreach (var b in buttons)
        {
            if (b.selectedBorder != null && b.selectedBorder.activeSelf)
                return true;
        }
        return false;
    }
}
