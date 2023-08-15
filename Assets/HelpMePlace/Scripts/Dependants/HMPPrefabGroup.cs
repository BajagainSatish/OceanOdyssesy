using System.Collections.Generic;
using UnityEngine;
using static HelpMePlace.HMPEnums;
using static HelpMePlace.HMPConstants;

namespace HelpMePlace
{
    /// <summary>
    /// A Prefab Group is a collection of properties that group placable prefabs together. These can then be scrolled through in the UI.
    /// </summary>
    [System.Serializable]
    public class HMPPrefabGroup
    {
#if UNITY_EDITOR
        [SerializeField, Tooltip("Give this group a short descriptive name for ease of identification. E.g. Trees; Rocks; Houses; Rocks and Trees; etc")]
        public string GroupName = "";

        [SerializeField, Tooltip("Keypad keybind to use. Can be Keypad 1-9")]
        public KeyCode HotKeyBind = KeyCode.Keypad0;

        [HideInInspector, Tooltip("A color to represent this group. Will be used in heatmap mode.")]
        public Color Color = Color.green;

        [SerializeField, Tooltip("Random From Row means, when this Group is selected, the system will choose a random prefab from the list each time one is placed.")]
        public bool RandomFromRow = false;

        [SerializeField, Tooltip("Match the rotation of the object to the normal of the ground. Useful for Fences.")]
        public bool RotationToNormalEnabled = false;

        [SerializeField, Tooltip("What world axis should items in this group rotate around.")]
        public RotationAxis RotateAroundAxis = HMP_ROTATION_AROUND_AXIS_DEFAULT;

        [SerializeField, Range(0, 30), Tooltip("Minimum scale difference to apply to placed prefabs")]
        public float ScaleRangeMin = HMP_SCALE_RANGE_MIN_DEFAULT;

        [SerializeField, Range(0, 30), Tooltip("Maximum scale difference to apply to placed prefabs.")]
        public float ScaleRangeMax = HMP_SCALE_RANGE_MAX_DEFAULT;
        
        [SerializeField, Tooltip("If you wish for items from this group to be given a parent gameobject, set this here.")]
        public Transform DesiredParent = null;

        // This list will be handled by the Editor script and turned into a reorderable list.
        [SerializeField, HideInInspector, Tooltip("Add the prefabs that you would like to place when using this group.")]
        public List<HMPPlaceablePrefab> PlaceablePrefabs = new List<HMPPlaceablePrefab>();

        public void ClearPrefabs() => PlaceablePrefabs = new List<HMPPlaceablePrefab>();

        public void ResetAll() {

            GroupName = "";
            RandomFromRow = false;
            RotationToNormalEnabled = false;
            RotateAroundAxis = HMP_ROTATION_AROUND_AXIS_DEFAULT;
            ScaleRangeMin = HMP_SCALE_RANGE_MIN_DEFAULT;
            ScaleRangeMax = HMP_SCALE_RANGE_MAX_DEFAULT;
            DesiredParent = null;
            PlaceablePrefabs = new List<HMPPlaceablePrefab>();
        }

        /// <summary>
        /// Placable prefabs and their associated properties are initialised into these objects.
        /// </summary>
        [System.Serializable]
        public class HMPPlaceablePrefab
        {
            [SerializeField, Tooltip("Any GameObject or Prefab")]
            public GameObject Prefab;

            [SerializeField, Tooltip("The amount of height to be removed from a gameobject when its being placed.")]
            public float SinkAmount;

            [SerializeField, Tooltip("The width of the object that will be used by the fencing system.")]
            public float WidthOfObject = 3;
        }

        /// <summary>
        /// Constructor for the Prefab Groups - only used by the Folder adding option.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="placables"></param>
        public HMPPrefabGroup(string groupName, List<GameObject> placables)
        {
            GroupName = groupName;

            // Add a new placable prefab per gameobject passed.
            foreach (GameObject go in placables) 
                PlaceablePrefabs.Add(new HMPPlaceablePrefab() { Prefab = go }); 
        }
#endif
    }
}