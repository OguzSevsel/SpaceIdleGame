using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using Unity.Android.Types;

public static class Helpers
{
    public static string ColorBackground = "#0B0C0D";
    public static string ColorHighlight = "#161718";
    public static string ColorText = "#DFDCD8";
    public static string ColorBorder = "#6B6459";

    public static Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;

        Debug.LogWarning($"Invalid hex code: {hex}");
        return Color.white;
    }

    public static void SetFieldWidthPercentages(VisualElement Field, string leftClassName, string rightClassName)
    {
        Field.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            var children = Field.Children().ToList();
            if (children.Count >= 2)
            {
                children[0].AddToClassList(leftClassName);
                children[1].AddToClassList(rightClassName);
            }
        });
    }

    public static void AddClass(VisualElement element, string className)
    {
        element.AddToClassList(className);
    }

    public static void RemoveClass(VisualElement element, string className)
    {
        element.RemoveFromClassList(className);
    }

    public static void SetMarginsAndPadding(VisualElement element, int value)
    {
        element.style.marginTop = value;
        element.style.marginBottom = value;
        element.style.marginLeft = value;
        element.style.marginRight = value;
        element.style.paddingBottom = value;
        element.style.paddingLeft = value;
        element.style.paddingRight = value;
        element.style.paddingTop = value;
    }

    public static void SetPadding(VisualElement element, int padding)
    {
        element.style.paddingTop = padding;
        element.style.paddingBottom = padding;
        element.style.paddingLeft = padding;
        element.style.paddingRight = padding;
    }

    public static void SetMargin(VisualElement element, int margin)
    {
        element.style.marginTop = margin;
        element.style.marginBottom = margin;
        element.style.marginLeft = margin;
        element.style.marginRight = margin;
    }

    public static void SetBorderWidth(VisualElement element, int width)
    {
        element.style.borderTopWidth = width;
        element.style.borderLeftWidth = width;
        element.style.borderBottomWidth = width;
        element.style.borderRightWidth = width;
    }

    public static void SetBorderRadius(VisualElement element, int radius)
    {
        element.style.borderTopRightRadius = radius;
        element.style.borderTopLeftRadius = radius;
        element.style.borderBottomLeftRadius = radius;
        element.style.borderBottomRightRadius = radius;
    }

    public static void SetBorderColor(VisualElement element, Color color)
    {
        element.style.borderTopColor = color;
        element.style.borderLeftColor = color;
        element.style.borderBottomColor = color;
        element.style.borderRightColor = color;
    }

    public static void OnMouseEnter(VisualElement element, string highlightClass = null, Color color = default)
    {
        if (highlightClass == null && color != default)
        {
            element.style.backgroundColor = color;
            return;
        }

        element.RegisterCallback<MouseEnterEvent>(evt => AddClass(element, highlightClass));
    }

    public static void OnMouseLeave(VisualElement element, string highlightClass = null, Color color = default)
    {
        if (highlightClass == null && color != default)
        {
            element.style.backgroundColor = color;
            return;
        }

        element.RegisterCallback<MouseLeaveEvent>(evt => RemoveClass(element, highlightClass));
    }
}
