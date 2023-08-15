using UnityEngine;
using static HelpMePlace.HMPEnums;

namespace HelpMePlace
{
    /// <summary>
    /// Holds all constant values for use by the HelpMePlace system
    /// </summary>
    public class HMPConstants
    {
#if UNITY_EDITOR
        #region Preview Images in Editor Properties
        // Default space between images.
        internal const int PREVIEW_IMAGE_SPACER = 10;

        // Default preview image size.
        internal const int PREVIEW_IMAGE_DEFAULT_SIZE = 10;

        // Padding between the left of the scene view and the first image.
        internal const int HMP_XPOS_INITIAL_PADDING = 10;

        // Padding between the top of the scene view and the images
        internal const int HMP_PREVIEW_IMAGE_TOP_PADDING = 5;
        
        // Default size of the preview images (Configurable)
        internal const int HMP_PREVIEW_IMAGE_SIZE_DEFAULT = 45;

        // Default value for the keybinds show/hide (Configuratable)
        internal const bool HMP_SHOW_KEYBINDS_DEFAULT = true;

        // Width of the HMP Logo drawn in the top left of the Scene View
        internal const int HMP_LOGO_WIDTH = 55;
        #endregion

        #region Spawner settings
        // Height that the item will spawn above the mouse.
        internal const int SPAWN_HEIGHT_FOR_ANIM_MODIFIER = 5;

        // Sink speed for animation of the falling GameObjects
        internal const float PREFAB_PLACED_SINK_SPEED = 0.3f;

        // Default rotation axis for the Rotation Around Axis feature.
        internal const RotationAxis HMP_ROTATION_AROUND_AXIS_DEFAULT = RotationAxis.Y;

        // Default min scale to apply to placed prefabs.
        internal const int HMP_SCALE_RANGE_MIN_DEFAULT = 1;

        // Default max scale to apply to placed prefabs.
        internal const int HMP_SCALE_RANGE_MAX_DEFAULT = 1;

        // Default holder for the spawned items name
        internal const string HMP_HOLDER_DEFAULT_NAME = "HMPSpawnedObjectHolder";

        // Prefix for the automatically created group heirarchy gameobjects for storing each group. 
        internal const string HMP_GROUP_HOLDER_DEFAULT_NAME_PREFIX = "HMPSpawnedGroup-";

        // Default value for the Static after placement configuration value
        internal const bool HMP_STATIC_AFTER_PLACEMENT_DEFAULT = true;

        // Default value for the Rotation mode configuration value
        internal const RotationMode HMP_ROTATIONMODE_DEFAULT = RotationMode.Smooth;

        // Default value for the Rotation mode Smooth configuration value
        internal const float HMP_ROTATION_SMOOTH_DEFAULT = 1;

        // Default value for the Rotation mode Snap configuration value
        internal const int HMP_ROTATION_SNAP_AMOUNT_DEFAULT = 20;

        // Default value for the Rotation mode Fixed configuration value
        internal static Vector3 HMP_ROTATION_FIXED_AMOUNT_DEFAULT = Vector3.zero;

        // Default value for the Sink using MouseMove & CTRL configuration value
        internal const bool HMP_ALT_ADJUSTS_SINK_AMOUNT_DEFAULT = true;

        // Default value for the Sink using MouseMove & CTRL Sensitivity configuration value
        internal const float HMP_SINK_AMOUNT_ADJUSTMENT_SENSITIVITY_DEFAULT = 0.1f;
        #endregion

        #region
        public const VisualizerShape HMP_VISUALIZER_SHAPE_DEFAULT = VisualizerShape.Sphere;
        public const int HMP_VISUALIZER_SHAPE_SIZE_DEFAULT = 2;
        #endregion

        #region System
        // Blueprint gameobject default name
        internal const string HMP_BLUEPRINT_DEFAULT_NAME = "HMPBlueprint";

        // Prefix for the undo registration - visible in the edit > undo menu.
        internal const string HMP_UNDO_ITEM_PREFIX = "HMP Spawn ";
        #endregion
#endif
    }
}