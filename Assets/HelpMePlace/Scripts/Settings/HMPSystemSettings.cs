using System;
using UnityEngine;
using static HelpMePlace.HMPEnums;

namespace HelpMePlace
{
    /// <summary>
    /// Help Me Place core System settings.
    /// </summary>
    [System.Serializable]
    public class HMPSystemSettings
    {
#if UNITY_EDITOR
#pragma warning disable 649
        /// <summary>
        /// These are Projectors that will show the images over the terrain to use as guides.
        /// These can be toggled on/off using the key assigned to ToggleOverlayMap in KeyBinds.cs (Feel free to change the keybinds there)
        /// </summary>
        [Header("Map projections")]
        [Tooltip("All projector maps that you wish to toggle on/off should be added to this.")]
        [SerializeField]
        private GameObject[] OverlayMapGameObjects;

        [Header("Positioning")]
        [Tooltip("Change these values to move the Help me Place interface in the Scene View")]
        [SerializeField]
        internal Vector2 InterfaceStartPosition = new Vector2(0, 0);

        [Header("Blueprint")]
        [Tooltip("The blueprint material that will be applied to the Blueprint object.")]
        [SerializeField]
        private Material BluePrintMaterial;

        [Header("Gizmos")]
        [SerializeField, Tooltip("Color of the Rotation Lookat mode Gizmo.")]
        internal Color GizmoColor;

        [Header("Logging")]
        [SerializeField, Tooltip("Enable logging of Debug actions for Active/Deactive instructions.")]
        internal bool ShowActivityNotifications;

        [Header("Placement Animation")]
        [SerializeField, Tooltip("Should the placement of prefabs / gameobjects be animated?")]
        internal bool AnimatePlacement = true;

        [Header("Textures for drawing editor GUI")]
        [SerializeField, Tooltip("")] private Texture2D InactiveSelectionTexture;                        // Sys
        [SerializeField, Tooltip("")] private Texture2D ActiveSelectionTexture;                          // Sys
        
        [SerializeField, Tooltip("")] private Texture2D InstructionLeftTexture;                          // A
        [SerializeField, Tooltip("")] private Texture2D InstructionRightTexture;                         // D
        [SerializeField, Tooltip("")] private Texture2D InstructionChangeColumnTexture;                  // W

        [SerializeField, Tooltip("")] private Texture2D XRotationAxisTexture;                            // G
        [SerializeField, Tooltip("")] private Texture2D YRotationAxisTexture;                            // G
        [SerializeField, Tooltip("")] private Texture2D ZRotationAxisTexture;                            // G
        [SerializeField, Tooltip("")] private Texture2D AllRotationAxisTexture;                          // G

        [SerializeField, Tooltip("")] private Texture2D FixedRotationModeTexture;                        // R
        [SerializeField, Tooltip("")] private Texture2D RandomRotationModeTexture;                       // R
        [SerializeField, Tooltip("")] private Texture2D SmoothRotationModeTexture;                       // R
        [SerializeField, Tooltip("")] private Texture2D SnapRotationModeTexture;                         // R

        [SerializeField, Tooltip("")] private Texture2D RotationToNormalActiveTexture;                   // Q
        [SerializeField, Tooltip("")] private Texture2D RotationToNormalInactiveTexture;                 // Q

        [SerializeField, Tooltip("")] private Texture2D InstructionRandomFromRowActiveTexture;           // T
        [SerializeField, Tooltip("")] private Texture2D InstructionRandomFromRowInactiveTexture;         // T
        
        [SerializeField, Tooltip("")] private Texture2D InstructionShowHideOverlayMapActiveTexture;      // Tab
        [SerializeField, Tooltip("")] private Texture2D InstructionShowHideOverlayMapInactiveTexture;    // Tab
        
        [SerializeField, Tooltip("")] private Texture2D InstructionPlacePrefabTexture;                   // Space
        
        [SerializeField, Tooltip("")] private Texture2D InstructionPreviewMissing;                   // For unavailable previews.
        
        [SerializeField, Tooltip("")] private Texture2D HMPLogo;

        [SerializeField, Tooltip("")] private Texture2D LookAwayFromModeTexture;
        [SerializeField, Tooltip("")] private Texture2D LookTowardsModeTexture;
        
        [SerializeField, Tooltip("")] private Texture2D InstructionLookAtSetPointRotationModeHelperTexture;

#pragma warning restore 649

        /// <summary>
        /// Helper for getting the Help Me Place editor UI.
        /// </summary>
        /// <param name="texture">Enum for ease of access to HMP system images.</param>
        /// <returns>The corresponding texture.</returns>
        public Texture2D GetTexture(TextureSettings texture)
        {
            switch(texture)
            {
                case TextureSettings.InactiveSelection: return InactiveSelectionTexture; // Sys
                case TextureSettings.ActiveSelection: return ActiveSelectionTexture; // Sys
                case TextureSettings.InstructionLeft: return InstructionLeftTexture; // Q
                case TextureSettings.InstructionRight: return InstructionRightTexture; // E
                case TextureSettings.InstructionChangeColumn: return InstructionChangeColumnTexture; // W

                case TextureSettings.XRotationAxis: return XRotationAxisTexture;  // G
                case TextureSettings.YRotationAxis: return YRotationAxisTexture;  // G
                case TextureSettings.ZRotationAxis: return ZRotationAxisTexture;  // G
                case TextureSettings.AllRotationAxis: return AllRotationAxisTexture;  // G

                case TextureSettings.FixedRotationMode: return FixedRotationModeTexture;  // R
                case TextureSettings.RandomRotationMode: return RandomRotationModeTexture;  // R
                case TextureSettings.SmoothRotationMode: return SmoothRotationModeTexture;  // R
                case TextureSettings.SnapRotationMode: return SnapRotationModeTexture;  // R
                case TextureSettings.LookAwayFromRotationMode: return LookAwayFromModeTexture;  // R
                case TextureSettings.LookTowardsRotationMode: return LookTowardsModeTexture;  // R

                case TextureSettings.InstructionLookAtSetPointRotationModeHelper: return InstructionLookAtSetPointRotationModeHelperTexture;  // E

                case TextureSettings.RotationToNormalActive: return RotationToNormalActiveTexture;  // Q
                case TextureSettings.RotationToNormalInactive: return RotationToNormalInactiveTexture;  // Q

                case TextureSettings.InstructionRandomFromRowActive: return InstructionRandomFromRowActiveTexture;  // T
                case TextureSettings.InstructionRandomFromRowInactive: return InstructionRandomFromRowInactiveTexture;  // T
                case TextureSettings.InstructionShowHideOverlayMapActive: return InstructionShowHideOverlayMapActiveTexture;  // Tab
                case TextureSettings.InstructionShowHideOverlayMapInactive: return InstructionShowHideOverlayMapInactiveTexture;  // Tab

                case TextureSettings.InstructionPlacePrefab: return InstructionPlacePrefabTexture;  // Space

                case TextureSettings.PreviewMissing: return InstructionPreviewMissing;  // Space

                case TextureSettings.HMPLogo: return HMPLogo;

                default: return null;
            }
        }

        /// <summary>
        /// Transparent blue material for the blueprint gameobject.
        /// </summary>
        /// <returns>The material assigned for the blueprint gameobject</returns>
        public Material GetBlueprintMaterial() => BluePrintMaterial;

        /// <summary>
        /// Gets all projector gameobjects from the settings. Any new / extra that you wish to add should be done in the HMP System Settings.
        /// </summary>
        /// <returns>A gameobject array of all objects</returns>
        public GameObject[] GetOverlayMaps() => OverlayMapGameObjects;

        public void UpdateOverlayMapsEffectedLayer(LayerMask layersToIgnore)
        {
            foreach(GameObject go in GetOverlayMaps())
                go.GetComponentInChildren<Projector>().ignoreLayers =~ layersToIgnore;
        }

        internal Color GetGizmoColor() => GizmoColor;
#endif
    }
}