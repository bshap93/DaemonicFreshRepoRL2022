using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelectionButton : MonoBehaviour
{
    [SerializeField] Image classIcon;
    [SerializeField] TMP_Text className;
    [SerializeField] TMP_Text description;

    void Awake()
    {
        // Validate required references
        if (classIcon == null) Debug.LogError("Class Icon not assigned on ClassSelectionButton", this);
        if (className == null) Debug.LogError("Class Name text not assigned on ClassSelectionButton", this);
        if (description == null) Debug.LogError("Description text not assigned on ClassSelectionButton", this);
    }

    public void Setup(StartingClass classData, Action<StartingClass> onSelected)
    {
        if (classData == null)
        {
            Debug.LogError("Null class data passed to ClassSelectionButton");
            return;
        }

        if (classIcon != null) classIcon.sprite = classData.classIcon;
        if (className != null) className.text = classData.className;
        if (description != null) description.text = classData.description;

        var button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(() => onSelected?.Invoke(classData));
    }
}
