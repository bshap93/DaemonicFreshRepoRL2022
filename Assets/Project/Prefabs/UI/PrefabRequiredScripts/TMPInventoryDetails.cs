using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace MoreMountains.InventoryEngine
{
    public class TMPInventoryDetails : InventoryDetails
    {
        [Header("TMP Components")]
        [MMInformation(
            "Bind your TextMeshPro components here. These will be used instead of the basic Text components.",
            MMInformationAttribute.InformationType.Info, false)]
        /// the title container TMP object
        public TMP_Text TMPTitle;
        /// the short description container TMP object
        public TMP_Text TMPShortDescription;
        /// the description container TMP object
        public TMP_Text TMPDescription;
        /// the quantity container TMP object
        public TMP_Text TMPQuantity;
        /// the weight container TMP object
        public TMP_Text TMPWeight;
        /// reference to the weight icon image component
        [Header("Default Values")]
        /// the default weight to display when none is provided
        public string DefaultWeight = "0.0";

        protected virtual void OnValidate()
        {
            // Warn if both regular Text and TMP_Text components are assigned
            if (TMPTitle != null && Title != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for Title. Only TMP_Text will be used.");

            if (TMPShortDescription != null && ShortDescription != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for ShortDescription. Only TMP_Text will be used.");

            if (TMPDescription != null && Description != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for Description. Only TMP_Text will be used.");

            if (TMPQuantity != null && Quantity != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for Quantity. Only TMP_Text will be used.");
        }

        protected virtual float GetItemWeight(InventoryItem item)
        {
            if (item == null) return 0f;

            // Try to get the Weight property through reflection
            var weightProperty = item.GetType().GetProperty("Weight");
            if (weightProperty != null)
                try
                {
                    return (float)weightProperty.GetValue(item, null);
                }
                catch
                {
                    return 0f;
                }

            // If no Weight property exists, return 0
            return 0f;
        }

        protected override IEnumerator FillDetailFields(InventoryItem item, float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);

            if (TMPTitle != null) TMPTitle.text = item.ItemName;
            if (TMPShortDescription != null) TMPShortDescription.text = item.ShortDescription;
            if (TMPDescription != null) TMPDescription.text = item.Description;
            if (TMPQuantity != null) TMPQuantity.text = item.Quantity.ToString();
            if (Icon != null) Icon.sprite = item.Icon;

            // Handle weight display
            if (TMPWeight != null)
            {
                var itemWeight = GetItemWeight(item);
                var totalWeight = itemWeight * item.Quantity;
                TMPWeight.text = totalWeight.ToString("F1");
            }

            if (HideOnEmptySlot && !Hidden && item.Quantity == 0)
            {
                StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup, _fadeDelay, 0f));
                Hidden = true;
            }
        }

        protected override IEnumerator FillDetailFieldsWithDefaults(float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);

            if (TMPTitle != null) TMPTitle.text = DefaultTitle;
            if (TMPShortDescription != null) TMPShortDescription.text = DefaultShortDescription;
            if (TMPDescription != null) TMPDescription.text = DefaultDescription;
            if (TMPQuantity != null) TMPQuantity.text = DefaultQuantity;
            if (TMPWeight != null) TMPWeight.text = DefaultWeight;
            if (Icon != null) Icon.sprite = DefaultIcon;
        }
    }
}
