namespace HelpMePlace
{
    /// <summary>
    /// All Enums used by the Help Me Place system
    /// </summary>
    public class HMPEnums
    {
#if UNITY_EDITOR
        /// <summary>
        /// Any textures are referenced via this enum, instead of by string / gameobject to keep it cleaner & more robust.
        /// </summary>
        public enum TextureSettings
        {
            // Prefab selection
            InactiveSelection,                              // Sys
            ActiveSelection,                                // Sys
            
            // Group/prefab traversal
            InstructionLeft,                                // Q
            InstructionRight,                               // E
            InstructionChangeColumn,                        // W

            // Rotation Axis
            XRotationAxis,
            YRotationAxis,
            ZRotationAxis,
            AllRotationAxis,

            // Rotation Modes
            FixedRotationMode,                              // R
            RandomRotationMode,                             // R
            SmoothRotationMode,                             // R
            SnapRotationMode,                               // R
            LookTowardsRotationMode,                        // R
            LookAwayFromRotationMode,                       // R
            
            // Set point
            InstructionLookAtSetPointRotationModeHelper,    // E

            // Random from Row
            InstructionRandomFromRowActive,                 // T
            InstructionRandomFromRowInactive,               // T

            // Overlay map
            InstructionShowHideOverlayMapActive,            // Tab
            InstructionShowHideOverlayMapInactive,          // Tab

            // Unused as of yet
            InstructionPlacePrefab,                         // Space

            // Sometimes the preview cannot be shown.
            PreviewMissing,

            // Logo is always visible.
            HMPLogo,
            RotationToNormalActive,
            RotationToNormalInactive,
        }

        /// <summary>
        /// Spawned Transform States for animation in the editor and management.
        /// </summary>
        public enum SpawnedTransformState
        {
            Spawned,
            Sinking,
            Settling,
            Finished,
            Error,
            Unspawned
        }

        /// <summary>
        /// Different modes for rotating your game objects spawned. 
        /// </summary>
        public enum RotationMode : int
        {
            Smooth,
            Snap,
            Fixed,
            Random,
            LookTowards,
            LookAwayFrom
        }

        /// <summary>
        /// Validation messages driven by Enum for ease of localization.
        /// </summary>
        public enum Validations {
            SelectedPrefabIsNull,
            BlueprintDoesNotExist,
            FailedToSpawn,
            NoGroupsAdded,
            AllPrefabsRemoved,
            ResetPreventedNoPrefabGroups
        }

        /// <summary>
        /// Severity will drive the brand name color in console messages.
        /// </summary>
        public enum ValidationSeverity
        {
            Info,
            Warning,
            Error
        }

        /// <summary>
        /// The axis of rotation for the selected group
        /// </summary>
        public enum RotationAxis
        {
            X,
            Y,
            Z,
            All
        }

        public enum VisualizerShape
        {
            Sphere,
            WireSphere,
            Cube,
            WireCube,
            SkyLine
        }
#endif
    }
}