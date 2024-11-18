using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Magic
{
    public class MagicResourceUI : MonoBehaviour
    {
        public MMProgressBar HealthBar;
        public MMProgressBar KinemaBar;
        public MMProgressBar FavourBar;
        [SerializeField] MagicSystem _magicSystem;
        void Update()
        {
            if (_magicSystem != null)
            {
                UpdateBar(
                    HealthBar, _magicSystem.GetComponent<Health>().CurrentHealth,
                    _magicSystem.GetComponent<Health>().MaximumHealth);

                UpdateBar(KinemaBar, _magicSystem.Kinema.CurrentResource, _magicSystem.Kinema.MaxResource);
                UpdateBar(FavourBar, _magicSystem.Favour.CurrentResource, _magicSystem.Favour.MaxResource);
            }
        }


        public void SetMagicSystem(MagicSystem magicSystem)
        {
            Debug.Log("Magic system set.");
            _magicSystem = magicSystem;
        }

        void UpdateBar(MMProgressBar bar, float currentValue, float maxValue)
        {
            if (bar != null) bar.UpdateBar(currentValue, 0f, maxValue);
        }
    }
}
