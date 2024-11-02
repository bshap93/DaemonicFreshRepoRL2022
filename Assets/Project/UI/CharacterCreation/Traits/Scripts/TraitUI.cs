using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.Traits.Scripts
{
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] Toggle selectionToggle;
        [SerializeField] Button infoButton;
        [SerializeField] Image traitIcon;
        Action<CharacterTrait> onInfoRequested;
        Action<CharacterTrait> onSelected;

        public CharacterTrait Trait { get; private set; }

        public void Initialize(CharacterTrait traitData,
            Action<CharacterTrait> onSelect,
            Action<CharacterTrait> onInfo, bool isClassSpecific)
        {
            Trait = traitData;
            onSelected = onSelect;
            onInfoRequested = onInfo;

            // Setup UI elements
            nameText.text = Trait.traitName;
            if (Trait.icon != null)
                traitIcon.sprite = Trait.icon;

            // Setup listeners
            selectionToggle.onValueChanged.AddListener(OnToggleChanged);
            infoButton.onClick.AddListener(OnInfoClicked);
        }

        void OnToggleChanged(bool isOn)
        {
            onSelected?.Invoke(Trait);
        }

        void OnInfoClicked()
        {
            onInfoRequested?.Invoke(Trait);
        }

        public void SetToggleWithoutNotify(bool value)
        {
            selectionToggle.SetIsOnWithoutNotify(value);
        }
    }
}
