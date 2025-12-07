using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{

#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bars/Rectangle White")]
    public static void AddLinearProgressBarRectangleWhite ()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Linear Progress Bar Rectangle White"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Linear Progress Bars/Rounded Blue")]
    public static void AddLinearProgressBarRoundedBlue()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Linear Progress Bar Rounded Blue"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Linear Progress Bars/Rounded Pink")]
    public static void AddLinearProgressBarRoundedPink()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Linear Progress Bar Rounded Pink"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Linear Progress Bars/Rounded White")]
    public static void AddLinearProgressBarRoundedWhite()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Linear Progress Bar Rounded White"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Radial Progress Bars/Iconed")]
    public static void AddRadialProgressBarIconedWithoutBorder()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Radial Progress Bar Iconed"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Radial Progress Bars/Iconed With Border")]
    public static void AddRadialProgressBarIconedWithBorder()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Radial Progress Bar Iconed With Border"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Radial Progress Bars/Full Blue")]
    public static void AddRadialProgressBarFullBlue()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Radial Progress Bar Full Blue"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Radial Progress Bars/Full White With Border")]
    public static void AddRadialProgressBarWithoutIconWithBorderFullWhite()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Radial Progress Bar Full White With Border"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }

    [MenuItem("GameObject/UI/Radial Progress Bars/Full White")]
    public static void AddRadialProgressBarFullWhite()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Progress Bars/Radial Progress Bar Full White"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif

    [SerializeField] private Image Fill;
    [SerializeField] private Gradient gradient;
    private float currentValue;
    private Color currentColor;

    public float progressValue
    {
        get => currentValue;
        set
        {
            if (this.currentValue != value)
            {
                this.currentValue = value;
                UpdateUI();
            }
        }
    }

    public Color progressColor
    {
        get => currentColor;
        set
        {
            if (this.currentColor != value)
            {
                this.currentColor = value;
                UpdateUI();
            }
        }
    }
    
    private void UpdateUI()
    {
        SetProgressBarColor(progressColor);
        SetProgressBarValue(progressValue);
    }

    private void SetProgressBarColor(Color color)
    {
        Fill.color = gradient.Evaluate(progressValue);
    }

    private void SetProgressBarValue(float value)
    {
        if (value >= 0f && value <= 1f)
        {
            Fill.fillAmount = value;
        }
        else
        {
            Debug.LogWarning("Progress Value should be between 0 and 1");
        }    
    }
}
