using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelectionButton : MonoBehaviour
{
    [Header("UI Elements")] [SerializeField]
    Image classIcon;
    [SerializeField] TMP_Text className;
    [SerializeField] TMP_Text description;

    [Header("Styling")] [SerializeField] Color selectedColor = new(0.8f, 0.8f, 1f);
    [SerializeField] Color normalColor = Color.white;

    Image backgroundImage;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        backgroundImage = GetComponent<Image>();

        // Set up button colors
        var colors = button.colors;
        colors.normalColor = normalColor;
        colors.selectedColor = selectedColor;
        colors.highlightedColor = Color.Lerp(normalColor, selectedColor, 0.5f);
        button.colors = colors;
    }

    public void Setup(StartingClass classData, Action<StartingClass> onSelected)
    {
        if (classData == null) return;

        if (classIcon != null)
        {
            classIcon.sprite = classData.classIcon;
            classIcon.preserveAspect = true;
        }

        if (className != null)
        {
            className.text = classData.className;
            className.fontStyle = FontStyles.Bold;
        }

        if (description != null)
        {
            description.text = classData.description;
            description.fontSize = className.fontSize * 0.8f;
        }

        if (button != null)
            button.onClick.AddListener(
                () =>
                {
                    onSelected?.Invoke(classData);
                    SetSelected(true);
                });
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage != null) backgroundImage.color = selected ? selectedColor : normalColor;
    }
}
