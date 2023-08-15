using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HelpMePlace
{
    /// <summary>
    /// Custom editor for the Help Me Place system. 
    /// Creates a clean looking UI for the Prefab Group / Prefab selection and its configuration.
    /// </summary>
    [CustomEditor(typeof(HelpMePlaceSystem))]
    public class HelpMePlaceEditor : Editor
    {
        readonly string currentVersion = "3.0.0";

        // Initial size padding for each of the array size from inner lists on the editor
        internal const int HMP_ARRAY_SIZE_PADDING_DEFAULT = 30, HMP_EDITOR_PREFABS_HEADER_HEIGHT = 25, HMP_EDITOR_PREFABS_ACTION_BUTTONS_HEIGHT = 25;

        // Left padding initial for inner list in settings
        internal const int HMP_LEFT_OF_INNER_LIST_DEFAULT = 10;

        // Height of the settings from Prefab Group upper group of settings.
        internal const int HMP_HEIGHT_OF_UPPER_SETTINGS_GROUP_DEFAULT = 200;

        // Limit for the amount of prefabs per group.
        internal const int HMP_LIMIT_PREFAB_COUNT = 13;

#pragma warning disable 649
        [SerializeField]
        Texture HMPLogo;
#pragma warning restore 649

        // Data related to the Prefab Groups
        private SerializedProperty prefabGroupData;

        // Reorderable List for the outer Prefab Groups.
        private ReorderableList ReorderableList;

        // inner reorderable list will hold all the prefabs for use in each group.
        private Dictionary<string, ReorderableList> innerListDict = new Dictionary<string, ReorderableList>();

        string groupName = "Buildings";
        string numToDivideInto = "10";

        private void OnEnable()
        {
            if (Application.isPlaying) return;

            // Convert the Prefab Groups to a Reorderable List
            prefabGroupData = serializedObject.FindProperty("PrefabGroups");

            ReorderableList = new ReorderableList(serializedObject: serializedObject, elements: prefabGroupData, draggable: true, displayHeader: true,
                displayAddButton: true, displayRemoveButton: true)
            {
                drawHeaderCallback = DrawHeaderCallback,
                drawElementCallback = DrawElementCallback
            };

            // Subscribe to the events for redrawing.
            ReorderableList.elementHeightCallback += PrefabGroupsElementHeightCallback;
            ReorderableList.onAddCallback += OnAddCallback;
        }

        HelpMePlaceSystem HMPSystem;

        // Internal tab number to keep track of what UI to show.
        int CurrentTab = 0;
        
        // Show / hide the utilities section.
        bool ShowPrefabGroupUtilities, ObjectPlacementVisualizer;

        /// <summary>
        /// Formats the Component look and feel. 
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Draw the logo at the top.
            GUILayout.Box(HMPLogo);

            if (Application.isPlaying) return;

            // Grab the system class to check for enabled/disabled properties. 
            HMPSystem = (HelpMePlaceSystem)target;

            // If the system could not be found, return.
            if (HMPSystem == null) return;

            // Cache the background color.
            Color defaultGUIColor = GUI.backgroundColor;

            // Create a style for the button text.
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;

            // Change the color for the button
            GUI.backgroundColor = HMPSystem.enabled ? Color.red : Color.green;

            // Only show the button if prefab groups exist.
            if (HMPSystem.PrefabGroups.Count > 0)
            {
                // Add a button to enable / disable the system.
                if (GUILayout.Button(HMPSystem.enabled ? "Disable" : "Enable"))
                    HMPSystem.enabled = !HMPSystem.enabled;
            }

            // Revert the color to the cached version
            GUI.backgroundColor = defaultGUIColor;

            // Ensure values are up to date.
            serializedObject.Update();

            Rect rect = EditorGUILayout.GetControlRect(false, 10);
            rect.height = 0;
            EditorGUI.DrawRect(rect, GUI.backgroundColor);

            // Tab layout
            string[] options = new string[] { "Prefab Groups", "Configuration", "HMP System" };
            CurrentTab = GUILayout.Toolbar(CurrentTab, options);

            if (CurrentTab == 0)
            {
                // Utility for creating a prefab group from folder selection.
                if (GUILayout.Button("Create new prefab group from folder"))
                {
                    // Show the folder dialog
                    var filePath = EditorUtility.OpenFolderPanel("Select the folder with your Prefabs.", Application.dataPath, "Prefabs");

                    // New list for holding prefab groups to be created.
                    Dictionary<string, List<GameObject>> GOsFromSelectedPath = new Dictionary<string, List<GameObject>>();

                    // If a folder is selected..
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        // Grab the file path to get directory info
                        DirectoryInfo directoryInfo = new DirectoryInfo(filePath);

                        // Search for files within the directory
                        FileInfo[] fileInfos = directoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);

                        // iterate all found prefabs
                        foreach (FileInfo fi in fileInfos)
                        {
                            // Ensure that the prefab is not a meta prefab.
                            if (fi.Extension == ".prefab")
                            {
                                // Load it as an asset so that we can cast as a GameObject
                                Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath:
                                    fi.FullName.Substring(
                                        fi.FullName.IndexOf("Assets")
                                    )
                                );

                                // If theres a problem with the prefab, skip it.
                                if (prefab == null) continue;

                                // If there is a group already created, skip over, otherwise add it to the dictionary
                                if (!GOsFromSelectedPath.ContainsKey(fi.Directory.Name))
                                    GOsFromSelectedPath.Add(fi.Directory.Name, new List<GameObject>());

                                // Add the prefab to the dictionary for the prefab group.
                                GOsFromSelectedPath[fi.Directory.Name].Add((GameObject)prefab);
                            }
                        }

                        foreach (KeyValuePair<string, List<GameObject>> kvp in GOsFromSelectedPath)
                        {
                            Debug.Log($"<color=blue>Help Me Place</color> Added new group {kvp.Key.ToString()} with {kvp.Value.Count} prefabs.");

                            // Create a new group with the name of the folder and all the prefabs
                            HMPPrefabGroup newGroup = new HMPPrefabGroup(kvp.Key.ToString(), kvp.Value);

                            // Add the new prefab groups to the collection.
                            HMPSystem.PrefabGroups.Add(newGroup);
                        }
                    }
                }

                ReorderableList.DoLayoutList();

                var offsetProperty = serializedObject.FindProperty("Offset");

                ObjectPlacementVisualizer = EditorGUILayout.Foldout(ObjectPlacementVisualizer, "Prefab Group Visualization");
                if(ObjectPlacementVisualizer)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Visualizer.VisualizerShape"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Visualizer.VisualiserShapeSize"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Visualizer.AutoRefreshEnabled"));

                    if (GUILayout.Button(HMPSystem.Visualizer.VisualizerEnabled ? "Disable" : "Enable"))
                    {
                        // If the instance hasnt' been set - you must enable Help Me Place at least once.
                        if (HMPSystem.Visualizer.VisualizerEnabled == false && HelpMePlaceSystem.Instance == null)
                        {
                            HMPSystem.Visualizer.VisualizerEnabled = false;
                            Debug.LogError("<color=blue>Help Me Place</color> Cannot enable the Visualizer. Please enable the Help Me Place system at least once before using the Visualiser.");
                        }
                        else
                        {
                            HMPSystem.Visualizer.VisualizerEnabled = !HMPSystem.Visualizer.VisualizerEnabled;
                        }
                    }
                }

                // Prefab Utilities Section
                ShowPrefabGroupUtilities = EditorGUILayout.Foldout(ShowPrefabGroupUtilities, "Prefab Group Utilities");
                if (ShowPrefabGroupUtilities)
                {
                    // Divide Groups Section.
                    EditorGUILayout.LabelField("Divide Groups into smaller Groups.", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    // Inputs
                    groupName = EditorGUILayout.TextField("Group Name", groupName);
                    numToDivideInto = EditorGUILayout.TextField("Number per group", numToDivideInto);

                    // Action button for Division of groups - to split into smaller prefab groups.
                    if (GUILayout.Button("Divide Group"))
                    {
                        // Name must match exactly.
                        HMPPrefabGroup grp = HMPSystem.PrefabGroups.FirstOrDefault(o => o.GroupName == groupName);
                        if (grp == null)
                        {
                            Debug.LogError("No group found with name: " + groupName);
                            return;
                        }

                        // Get the number of parts that will be created.
                        string numParts = (grp.PlaceablePrefabs.Count / float.Parse(numToDivideInto)).ToString();

                        // If any remainder comes from previous calculation, create an extra group for the overflow.
                        string remainder = numParts.Contains('.') ?
                            numParts.Substring(numParts.IndexOf('.'), numParts.Length - numParts.IndexOf('.')) :
                            "0";

                        // Get total number of groups
                        int numGroups = numParts.Contains('.') ?
                            int.Parse(numParts.Substring(0, numParts.IndexOf('.'))) :
                            int.Parse(numParts);

                        if (float.Parse(remainder) > 0)
                            numGroups++;

                        // Output of what is happening.
                        Debug.Log($"Dividing group [{grp}] ({grp.PlaceablePrefabs.Count} items) into [{numGroups}] groups.");

                        // Create a new list to hold the new groups.
                        Dictionary<int, List<HMPPrefabGroup.HMPPlaceablePrefab>> newLists = new Dictionary<int, List<HMPPrefabGroup.HMPPlaceablePrefab>>();
                        for (int i = 0; i < numGroups; i++)
                        {
                            newLists.Add(i, new List<HMPPrefabGroup.HMPPlaceablePrefab>());
                            for (int j = 0; j < int.Parse(numToDivideInto); j++)
                            {
                                int x = (int.Parse(numToDivideInto) * i) + j;
                                if (x >= grp.PlaceablePrefabs.Count) break;
                                // This will copy the sink amount and prefab to a new list.
                                newLists[i].Add(grp.PlaceablePrefabs[x]);
                            }
                        }

                        foreach (KeyValuePair<int, List<HMPPrefabGroup.HMPPlaceablePrefab>> kvp in newLists)
                        {
                            // Add the new split groups to the new prefab groups.
                            List<GameObject> prefabs = kvp.Value.Select(o => o.Prefab).ToList();

                            HMPSystem.PrefabGroups.Add(
                                new HMPPrefabGroup(grp.GroupName + kvp.Key.ToString(), prefabs)
                            );

                            var LastCreated = HMPSystem.PrefabGroups.Last();

                            // Copy the sink amounts
                            foreach (HMPPrefabGroup.HMPPlaceablePrefab go in LastCreated?.PlaceablePrefabs)
                            {
                                var p = kvp.Value.FirstOrDefault(o => o.Prefab.name == go.Prefab.name);
                                if (p != null)
                                    go.SinkAmount = p.SinkAmount;
                            }

                            // Copy all properties from the origonal to the new rows.
                            LastCreated.RandomFromRow = grp.RandomFromRow;
                            LastCreated.DesiredParent = grp.DesiredParent;
                            LastCreated.RotateAroundAxis = grp.RotateAroundAxis;
                            LastCreated.RotationToNormalEnabled = grp.RotationToNormalEnabled;
                            LastCreated.ScaleRangeMax = grp.ScaleRangeMax;
                            LastCreated.ScaleRangeMin = grp.ScaleRangeMin;
                            LastCreated.Color = grp.Color;
                        }

                        // Remove the origonal
                        HMPSystem.PrefabGroups.Remove(grp);
                    }

                    rect = EditorGUILayout.GetControlRect(false, 10);
                    rect.height = 0;
                    EditorGUI.DrawRect(rect, GUI.backgroundColor);

                    EditorGUI.indentLevel--;

                    // As we can't show the Sink Amount with each element, we provide the ability to reset the currently selected object. 
                    if (HMPSystem.enabled && HMPSystem.HasSelection() && GUILayout.Button("Reset current object Sink Amount"))
                    {
                        EditorGUILayout.LabelField("Sink amount Reset.", EditorStyles.boldLabel);
                        HMPSystem.ResetCurrentObjectSinkAmount();
                    }

                    if (GUILayout.Button("Line up all Prefabs per Group."))
                    {
                        if (HelpMePlaceSystem.Instance == null)
                        {
                            Debug.LogError("<color=blue>Help Me Place</color> Cannot enable the Visualizer. Please enable the Help Me Place system at least once before using Visualisers.");
                            return;
                        }
                        Transform PrefabPreviewHolder = new GameObject("Prefab Preview Holder").transform;
                        int xAxis = 0;
                        int zAxis = 0;

                        HelpMePlaceSystem.Instance.StringsToDrawGizmos = new List<HelpMePlaceSystem.StringsToDrawGizmo>();

                        foreach (HMPPrefabGroup p in HelpMePlaceSystem.Instance.PrefabGroups)
                        {
                            xAxis = 0;
                            HelpMePlaceSystem.Instance.StringsToDrawGizmos.Add(new HelpMePlaceSystem.StringsToDrawGizmo() { Color = p.Color, Pos = (new Vector3(xAxis - 100, 0, zAxis)), Text = p.GroupName, IsHeader = true });
                            Transform groupHolder = new GameObject(p.GroupName).transform;
                            groupHolder.SetParent(PrefabPreviewHolder);

                            foreach (var details in p.PlaceablePrefabs)
                            {
                                if (details.Prefab != null)
                                {
                                    Transform pi = (PrefabUtility.InstantiatePrefab(details.Prefab) as GameObject).transform;
                                    pi.position = new Vector3(xAxis, 0, zAxis);
                                    pi.rotation = Quaternion.identity;
                                    pi.SetParent(groupHolder);
                                    HelpMePlaceSystem.Instance.StringsToDrawGizmos.Add(new HelpMePlaceSystem.StringsToDrawGizmo() { Color = p.Color, Pos = (new Vector3(xAxis, 0, zAxis)), Text = details.Prefab.name });
                                }

                                xAxis += 40;
                            }
                            zAxis += 100;
                        }
                    }

                    if (GUILayout.Button("Clear Gizmo Text"))
                    {
                        if (HelpMePlaceSystem.Instance == null)
                        {
                            Debug.LogError("<color=blue>Help Me Place</color> Please enable the Help Me Place system at least once before trying this function.");
                            return;
                        }

                        HelpMePlaceSystem.Instance.StringsToDrawGizmos = new List<HelpMePlaceSystem.StringsToDrawGizmo>();
                    }
                }
            }

            if (CurrentTab == 1)
            {
                //// Spacer
                //rect.height = 0;
                //EditorGUI.DrawRect(rect, GUI.backgroundColor);

                // Shows the Configuration serialized fields.
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Configuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("EventHooks"));
            }

            //// Spacer
            //rect = EditorGUILayout.GetControlRect(false, 20);
            //rect.height = 0;
            //EditorGUI.DrawRect(rect, GUI.backgroundColor);

            //// Spacer line between the Config and the System values.
            //rect = EditorGUILayout.GetControlRect(false, 1);
            //rect.height = 1;

            if (CurrentTab == 2)
            {
                // Warn the user that they shouldn't change the system settings.
                EditorGUILayout.HelpBox("Help Me Place internal settings - Don't change below this line unless absolutely nescessary.", MessageType.Warning);

                // Draw the System properties.
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HMPSystem"));
            }

            var vstyle = new GUIStyle(GUI.skin.button);
            vstyle.normal.textColor = Color.grey;
            vstyle.padding = new RectOffset(0, 0, 0, 0);
            vstyle.border = new RectOffset(0, 0, 0, 0);
            vstyle.fontSize = 8;
            GUI.Label(new Rect(22, 8, 50, 15), $"HMP v{currentVersion}", vstyle);
            // Revert the color to the cached version
            GUI.backgroundColor = defaultGUIColor;
            // Apply any modified props.
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the Prefab Groups header
        /// </summary>
        /// <param name="rect">Positioning</param>
        private void DrawHeaderCallback(Rect rect) => EditorGUI.LabelField(rect, "Prefab Groups");

        /// <summary>
        /// Element draw callbacks for any reorderable list drawing / redrawing
        /// </summary>
        /// <param name="rect">Positioning</param>
        /// <param name="index">The index of the property to draw</param>
        /// <param name="isactive">Is this active?</param>
        /// <param name="isfocused">Is this Focused?</param>
        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            // Grab the property
            SerializedProperty element = ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            // Set the header name to include the Group Name if its been set.
            SerializedProperty elementName = element.FindPropertyRelative("GroupName");
            string elementTitle = string.IsNullOrEmpty(elementName.stringValue)
                ? "New Prefab Group"
                : $"Prefab Group: {elementName.stringValue} ";

            Rect rowHeaderRect = new Rect(rect.x += 15, rect.y, rect.width * .8f, height: EditorGUIUtility.singleLineHeight);

            HMPPrefabGroup currentGroup = HMPSystem.PrefabGroups[index];

            Rect colorPickerRect = new Rect(rowHeaderRect.xMax - 50f, rect.y, 50, rowHeaderRect.height);
            currentGroup.Color = EditorGUI.ColorField(colorPickerRect, currentGroup.Color == default ? Color.white : currentGroup.Color);

            // Place the property.
            EditorGUI.PropertyField(position: rowHeaderRect
                , property:
                element, label: new GUIContent(elementTitle), includeChildren: true);

            // Find the Placeable Prefabs list if available
            var InnerList = element.FindPropertyRelative("PlaceablePrefabs");

            // For storing the list in the dictionary we now store the path as its unique.
            string listKey = element.propertyPath;

            // Initialise the new reorderable inner list.
            ReorderableList prefabsList;

            // if it already exists, reset it.
            if (innerListDict.ContainsKey(listKey))
            {
                // Grab the existing list
                prefabsList = innerListDict[listKey];
            }
            else
            {
                // Create a new inner reorderable list
                prefabsList = new ReorderableList(element.serializedObject, InnerList)
                {
                    // Need to specify these values so that we can move the prefabs around.
                    displayAdd = true,
                    displayRemove = true,
                    draggable = true,

                    // Draw each row.
                    drawElementCallback = (innerRect, innerIndex, innerA, innerH) =>
                    {
                        // Get element of inner list
                        var innerElement = InnerList.GetArrayElementAtIndex(innerIndex);
                        var name = innerElement.FindPropertyRelative("Prefab");
                        EditorGUI.PropertyField(innerRect, name);
                    },

                    drawHeaderCallback = innerRect =>
                    {
                        EditorGUI.LabelField(innerRect, "Prefabs"); // Draw the header of inner list Placable prefabs
                    },
                };
                // Store in the Dictionary
                innerListDict[listKey] = prefabsList;

                prefabsList.onAddCallback += OnAddInnerListCallback;
                prefabsList.onRemoveCallback += OnRemoveInnerListCallback;
            }

            // Set up the inner list.
            var height = EditorGUIUtility.singleLineHeight;

            // If its been expanded, ensure the space is applied to the total row height.
            if (element.isExpanded)
            {
                Rect buttonRect = new Rect(rowHeaderRect.xMax - 120 - 50f, rect.y, 120, rowHeaderRect.height);
                if (GUI.Button(buttonRect, "Reset This Group"))
                {
                    Undo.RecordObject(HMPSystem, "Undo reset Prefab Group");
                    currentGroup.ResetAll();
                }

                height = (InnerList.arraySize + HMP_ARRAY_SIZE_PADDING_DEFAULT) * EditorGUIUtility.singleLineHeight;

                var ctrlWidth = Screen.width * .8f / EditorGUIUtility.pixelsPerPoint;

                Rect innerListRect = new Rect(
                        rect.x + HMP_LEFT_OF_INNER_LIST_DEFAULT,
                        rect.y + HMP_HEIGHT_OF_UPPER_SETTINGS_GROUP_DEFAULT,
                        ctrlWidth,
                        height: height // EditorGUIUtility.singleLineHeight
                    );

                prefabsList.DoList(
                    innerListRect
                );
            }
        }

        /// <summary>
        /// Ensure the height of the prefabs group is sufficiently spaced and that all elements within are visble.
        /// </summary>
        /// <param name="index">The index of the property being drawn</param>
        /// <returns>The height</returns>
        private float PrefabGroupsElementHeightCallback(int index)
        {
            if (HMPSystem.PrefabGroups.Count <= 0) return default;

            // Get the element
            var el = ReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            // Get the origonal height
            float propertyHeight = EditorGUI.GetPropertyHeight(el, true);

            // Apply half line height spacing
            float spacing = EditorGUIUtility.singleLineHeight / 2;

            float height = 0;

            // If there is a list here, we need to add the height of it to the expanded drawer height.
            if (el.isExpanded && innerListDict.ContainsKey(el.propertyPath))
            {
                var element = innerListDict[el.propertyPath];
                // Multiply the amount of elements by single line height, add 3 for extra padding.
                height = element.count * element.elementHeight + HMP_ARRAY_SIZE_PADDING_DEFAULT + HMP_EDITOR_PREFABS_HEADER_HEIGHT + HMP_EDITOR_PREFABS_ACTION_BUTTONS_HEIGHT;
                
                // * (.arraySize + HMP_ARRAY_SIZE_PADDING_DEFAULT) * EditorGUIUtility.singleLineHeight;
            }

            return height + propertyHeight + spacing;
        }

        /// <summary>
        /// Every time something is added, it will need to be readded to the serialized property. 
        /// </summary>
        /// <param name="list">The list that something was added to</param>
        private void OnAddCallback(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            // Increase the size of the array.
            list.serializedProperty.arraySize++;
            list.index = index;
        }

        /// <summary>
        /// Every time something is added, it will need to be readded to the serialized property. 
        /// This is for the Inner Lists - If the amount of elements exceed 13, we will notify the user and prevent more elements being added. 
        /// </summary>
        /// <param name="list">The list that something was added to</param>
        private void OnAddInnerListCallback(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            //if (list.serializedProperty.arraySize >= HMP_LIMIT_PREFAB_COUNT)
            //{
            //    Debug.Log("Maximum amount of prefabs exceeded. No more prefabs may be added to this list. Create another prefab group instead.");
            //    list.displayAdd = false;
            //}
            //else
            //    list.displayAdd = true;
            // Increase the size of the array.
            list.serializedProperty.arraySize++;
            list.index = index;
        }

        /// <summary>
        /// We need to limit the amount of elements that can be added to the system for draw limitations.
        /// </summary>
        /// <param name="list">The inner list.</param>
        private void OnRemoveInnerListCallback(ReorderableList list)
        {
            // Delete the element.
            var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
            element.DeleteCommand();

            // If the list has been brought to acceptable size, allow adding more.
            //if (list.serializedProperty.arraySize <= HMP_LIMIT_PREFAB_COUNT)
            //    list.displayAdd = true;
            //else
            //    list.displayAdd = false;
        }
    }
}