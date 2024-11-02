using System;
using System.Collections.Generic;
using Project.Core.CharacterCreation;
using Project.UI.CharacterCreation.UIElements.Scripts;
using TMPro;
using UnityEngine;

namespace Project.UI.CharacterCreation.Classes.Scripts
{
    public class ClassSelectionPanel : MonoBehaviour
    {
        [SerializeField] Transform classContainer;
        [SerializeField] GameObject classButtonPrefab;
        [SerializeField] TextMeshProUGUI classDescriptionText;
        Action<CharacterClass> onClassSelected;

        CharacterClass? selectedClass;

        public void Initialize(List<StartingClass> availableClasses, Action<CharacterClass> classSelectedCallback)
        {
            onClassSelected = classSelectedCallback;

            foreach (var classData in availableClasses)
            {
                var buttonGO = Instantiate(classButtonPrefab, classContainer);
                var classButton = buttonGO.GetComponent<ClassSelectionButton>();
                classButton.Setup(classData, OnClassButtonClicked);
            }
        }

        void OnClassButtonClicked(StartingClass classData)
        {
            // Update selected class
            selectedClass = (CharacterClass)Enum.Parse(typeof(CharacterClass), classData.className);

            // Update description
            classDescriptionText.text = $"{classData.className}\n\n{classData.description}";

            // Notify listeners
            onClassSelected?.Invoke(selectedClass.Value);
        }
    }
}
