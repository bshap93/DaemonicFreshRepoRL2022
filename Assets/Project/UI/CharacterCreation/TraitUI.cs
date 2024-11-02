using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation
{
// UI component for a single trait
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI traitNameText;
        [SerializeField] Toggle traitToggle;
        [SerializeField] Button infoButton;
        Action<CharacterTrait> onInfoRequested;
        Action<CharacterTrait> onSelected;

        CharacterTrait traitData;
        public CharacterTrait Trait { get; set; }

        public void Initialize(CharacterTrait trait, Action<CharacterTrait> onSelect, Action<CharacterTrait> onInfo)
        {
            traitData = trait;
            onSelected = onSelect;
            onInfoRequested = onInfo;

            traitNameText.text = trait.traitName;
            traitToggle.onValueChanged.AddListener(OnToggleChanged);
            infoButton.onClick.AddListener(OnInfoClicked);
        }

        void OnToggleChanged(bool isOn)
        {
            if (isOn)
                onSelected?.Invoke(traitData);
        }

        void OnInfoClicked()
        {
            onInfoRequested?.Invoke(traitData);
        }
        public void SetToggleWithoutNotify(bool b)
        {
            throw new NotImplementedException();
        }
    }
}
