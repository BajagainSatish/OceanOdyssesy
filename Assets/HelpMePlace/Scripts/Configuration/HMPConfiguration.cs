using UnityEngine;
using System;
using static HelpMePlace.HMPConstants;
using static HelpMePlace.HMPEnums;
using System.Linq;

namespace HelpMePlace
{
    /// <summary>
    /// Configuration values for the Help Me Place system.
    /// </summary>
    [Serializable]
    public class HMPConfiguration
    {
#if UNITY_EDITOR
#pragma warning disable 649

        [SerializeField, Tooltip("The layer that the HelpMePlace system can interact with. Usually this should be your Ground layer, Surface of a table layer, etc. ")]
        internal LayerMask GroundLayer;

        [Header("Rotation")]
        [SerializeField, Tooltip("The rotation mode to use (Smooth, Random, Snap.. etc). See documentation for descriptions of each type.")]
        public RotationMode RotationMode = HMP_ROTATIONMODE_DEFAULT;

        [SerializeField, Tooltip("The amount to rotate per click during Snap Rotation mode.")]
        public float RotationSnapAmount = HMP_ROTATION_SNAP_AMOUNT_DEFAULT;

        [SerializeField, Tooltip("The fixed rotation amount. (In Euler Angles.)")]
        public Vector3 RotationFixed = HMP_ROTATION_FIXED_AMOUNT_DEFAULT;

        [SerializeField, Range(0, 8), Tooltip("How sensitive is the rotation during whilst using the smooth mode.")]
        public float RotationSmoothSensitivity = HMP_ROTATION_SMOOTH_DEFAULT;
        
        [SerializeField, Tooltip("(E) The relative position to look towards, or look away from for Look At and Look Away From rotation modes.")]
        internal Vector3 LookAtPosition;

        [Header("After Placement Options")]
        [SerializeField, Tooltip("If an object is specified here, it will be used to parent all spawned objects. ")]
        private Transform AllSpawnedObjectParent;

        [SerializeField, Tooltip("Do you want the Static flag to be activated after the objects are spawned?")]
        public bool MakePrefabStatic = HMP_STATIC_AFTER_PLACEMENT_DEFAULT;

        [Header("Preview Images")]
        [SerializeField, Range(10, 100), Tooltip("Adjust this value to increase the size of the preview images shown whilst HelpMePlace is active.")]
        internal int PreviewImageSize = HMP_PREVIEW_IMAGE_SIZE_DEFAULT;

        [SerializeField, Tooltip("Should the keybinds be visible under each setting in the scene view.")]
        internal bool ShowKeybinds = HMP_SHOW_KEYBINDS_DEFAULT;

        [Header("Helper Controls - Sinking")]
        [SerializeField, Tooltip("Holding Ctrl and moving the mouse up/down will adjust the amount of sink to apply to the selected prefab.")]
        internal bool ALTAdjustsSinkAmount = HMP_ALT_ADJUSTS_SINK_AMOUNT_DEFAULT;

        [SerializeField, Tooltip("The sensitivity of the Sinking activity whilst holding CTRL.")]
        internal float ALTAdjustmentSinkAmountSensitivity = HMP_SINK_AMOUNT_ADJUSTMENT_SENSITIVITY_DEFAULT;
#pragma warning restore 649

        /// <summary>
        /// Gets the object parent selected by the user. 
        /// If no parent has been selected, create a master, and all groups parents below it. 
        /// </summary>
        /// <param name="GroupName">The parent group name to get</param>
        /// <returns>An existing, or new transform to parent the gameobjects to.</returns>
        public Transform GetSpawnedObjectParent(string GroupName)
        {
            // If no override was selected, we need to create/find an existing default one.
            if(AllSpawnedObjectParent == null)
            {
                // Get the existing Default holder
                GameObject existingMasterHolder = GameObject.Find(HMP_HOLDER_DEFAULT_NAME);
                if (existingMasterHolder != null)
                {
                    // If a master holder exists, see if there is an existing group parent already created.
                    Transform existingGroupParent = existingMasterHolder.GetComponentsInChildren<Transform>()?.FirstOrDefault(o => o.name == HMP_GROUP_HOLDER_DEFAULT_NAME_PREFIX + GroupName);
                    if (existingGroupParent != null)
                        // If an existing group parent already exists, return that.
                        return existingGroupParent.transform;
                    else
                    {
                        // If no group parent exists, but a master has already been created, lets create a new group parent for this group, and set its parent to the existing master group.
                        GameObject newGroupParent = new GameObject(HMP_GROUP_HOLDER_DEFAULT_NAME_PREFIX + GroupName);
                        newGroupParent.transform.SetParent(existingMasterHolder.transform);
                        return newGroupParent.transform;
                    }
                }
                else
                {
                    // No master holder exists, create one with a group inside. 
                    GameObject newMasterHolder = new GameObject(HMP_HOLDER_DEFAULT_NAME);
                    GameObject newGroupParent = new GameObject(HMP_GROUP_HOLDER_DEFAULT_NAME_PREFIX + GroupName);
                    newGroupParent.transform.SetParent(newMasterHolder.transform);
                    return newGroupParent.transform;
                }
            } else
                // Override was selected by the user, so return desired transform
                return AllSpawnedObjectParent;
        }
#endif
    }
}