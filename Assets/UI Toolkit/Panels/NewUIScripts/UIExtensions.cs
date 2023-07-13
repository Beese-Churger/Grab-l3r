using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIExtensions
{
    public static void Display(this VisualElement element, bool enabled)
    {
        if (element == null)
        {
            Debug.Log("Element not found");
            return;
        }
        element.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
