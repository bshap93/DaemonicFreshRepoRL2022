using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.CharacterCreation.SaveData
{
    [CreateAssetMenu(fileName = "NewCharacterSave", menuName = "Roguelike/Character Save")]
    public class CharacterSaveData : ScriptableObject
    {
        public string characterName;
        public StartingClass selectedClass;
        public List<CharacterTrait> selectedTraits;
        public CharacterStats attributes;

        public void Initialize(CharacterCreationData creationData)
        {
            characterName = creationData.characterName;
            selectedClass = creationData.selectedClass;
            selectedTraits = new List<CharacterTrait>(creationData.selectedTraits);
            attributes = creationData.attributes;
        }
    }
}
