using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.UIElements.Scripts
{
    public class AttributeRowUI : MonoBehaviour
    {
        [SerializeField] TMP_Text statNameText;
        [SerializeField] TMP_Text valueText;
        [SerializeField] Button incrementButton;
        [SerializeField] Button decrementButton;
        public Action OnPointsChanged;

        public int CurrentPoints { get; private set; }

        public void Initialize()
        {
            incrementButton.onClick.AddListener(OnIncrease);
            decrementButton.onClick.AddListener(OnDecrease);
            UpdateUI();
        }

        void OnIncrease()
        {
            CurrentPoints++;
            UpdateUI();
            OnPointsChanged?.Invoke();
        }

        void OnDecrease()
        {
            if (CurrentPoints > 0)
            {
                CurrentPoints--;
                UpdateUI();
                OnPointsChanged?.Invoke();
            }
        }


        void UpdateUI()
        {
            valueText.text = CurrentPoints.ToString();
            decrementButton.interactable = CurrentPoints > 0;
        }
    }
}
