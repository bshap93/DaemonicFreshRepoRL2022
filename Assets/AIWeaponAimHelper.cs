using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
///     Add this script to your weapon prefab to configure its aim behavior
/// </summary>
[RequireComponent(typeof(WeaponAim))]
public class WeaponAimConfiguration : MonoBehaviour
{
    protected Weapon _weapon;
    protected WeaponAim _weaponAim;

    protected virtual void Awake()
    {
        // Get references
        _weaponAim = GetComponent<WeaponAim>();
        _weapon = GetComponent<Weapon>();

        // Configure WeaponAim for AI use
        if (_weaponAim != null)
        {
            _weaponAim.AimControl = WeaponAim.AimControls.Script; // This is key for AI control
            _weaponAim.RotationMode = WeaponAim.RotationModes.Free;
            _weaponAim.MinimumAngle = -180f;
            _weaponAim.MaximumAngle = 180f;
            _weaponAim.WeaponRotationSpeed = 10f;

            // Disable reticle for AI weapons
            _weaponAim.ReticleType = WeaponAim.ReticleTypes.None;
        }

        Debug.Log($"Weapon Aim configured on {gameObject.name}");
    }

    protected virtual void Reset()
    {
        // This helps setup when the script is first added in the editor
        if (_weaponAim == null) _weaponAim = GetComponent<WeaponAim>();
    }
}
