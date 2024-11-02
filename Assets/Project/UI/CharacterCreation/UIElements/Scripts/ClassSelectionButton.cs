using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.UIElements.Scripts
{
    public class ClassSelectionButton : MonoBehaviour
    {
        [SerializeField] Image classIcon;
        [SerializeField] TMP_Text className;
        [SerializeField] TMP_Text description;

        public void Setup(StartingClass classData, Action<StartingClass> onSelected)
        {
            classIcon.sprite = classData.classIcon;
            className.text = classData.className;
            description.text = classData.description;

            GetComponent<Button>().onClick.AddListener(() => onSelected?.Invoke(classData));
        }
    }
}
