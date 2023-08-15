using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static HelpMePlace.HMPConstants;
using static HelpMePlace.HMPEnums;

namespace HelpMePlace
{
    /// <summary>
    /// The core HelpMePlace System. 
    /// Adding this to a GameObject will allow you to use the HelpMePlace system in your scene. 
    /// </summary>
    [ExecuteInEditMode]
    public class HelpMePlaceSystem : MonoBehaviour
    {
#if UNITY_EDITOR
#pragma warning disable 649

        /// <summary>
        /// Singleton instance accessor for this system. 
        /// </summary>
        public static HelpMePlaceSystem Instance;

        #region Settings
        [Header("Prefab placement settings")]
        [SerializeField, Tooltip("Group containing prefabs and their spawn rules.")]
        public List<HMPPrefabGroup> PrefabGroups = new List<HMPPrefabGroup>();

        [SerializeField, Tooltip("Configuration options and usability control")]
        public HMPConfiguration Configuration;

        [SerializeField, Tooltip("System options for Help Me Place. Avoid changing these unless absolutely nescessary.")]
        internal HMPSystemSettings HMPSystem;
        #endregion

        #region Internal

        // Handles all visualization functionality
        public HMPVisualizer Visualizer = new HMPVisualizer();

        // Holds a list of all transforms created by the system.
        private List<HMPSpawnedTransform> spawnedTransforms = new List<HMPSpawnedTransform>();

        // Selection Handler
        internal HMPSelectedPrefabHandler Selection = new HMPSelectedPrefabHandler();

        // Blueprint Handler
        internal HMPBlueprintHandler Blueprint = new HMPBlueprintHandler();

        // Helper functions such as GetMousePosition
        internal HMPHelperFunctions Helpers = new HMPHelperFunctions();

        // Outputs validation messages to the User
        internal HMPValidator Validator = new HMPValidator();

        // Fencing system for helping place walls and fences.
        internal HMPFencePlacement FencePlacement = new HMPFencePlacement();

        // Event hook system to allow you to add your own functionality to the events that happen within HMP.
        public HMPEditorEventHooks EventHooks = new HMPEditorEventHooks();

        // Holds the number for the ticks used by the subscribed event. 
        int tickCounter = 0;

        // Holds the ground layer for changes to be listened to.
        LayerMask CachedGroundLayer;
        #endregion
#pragma warning restore 649

        #region Blueprint Actions
        /// <summary>
        /// Updates the sink amount of the blueprint system to whatever is selected / set by the user.
        /// </summary>
        /// <param name="sinkAmount">Default of 0, but can be updated using user input.</param>
        internal void UpdateBlueprintSinkDepth(float sinkAmount) => Blueprint.UpdateSinkAmt(sinkAmount);

        /// <summary>
        /// Updates the appearance of the blueprint to match what is selected in the Selection Handler.
        /// </summary>
        /// <param name="selectedPrefab">The currently selected prefab.</param>
        internal void UpdateBlueprintAppearance(GameObject selectedPrefab)
        {
            // If there is no selected prefab, it hasn't be selected yet. 
            if (selectedPrefab == null) return;

            // If a blueprint exists, destroy it. 
            GameObject existingBlueprint = Blueprint.GetCurrentBlueprintGameObject();
            if (existingBlueprint != null)
                DestroyImmediate(existingBlueprint);

            // Create a new blueprint with the newly selected prefab.
            Blueprint.UpdateBlueprintGO(Instantiate(selectedPrefab));

            // Update the materials to the transparent blue material. (or whatever overridden object from the system settings.)
            Blueprint.UpdateBlueprintMaterialsToNewMat();

            // Updates the scale of the blueprint to a random within range of selected in the inspector.
            Blueprint.SetScale(Instance.GetScaleFromSelectedRange());

            EventHooks.InvokePrefabChange();
        }

        /// <summary>
        /// Is there a currently selected gameobject
        /// </summary>
        /// <returns>True if exists</returns>
        public bool HasSelection() => Selection.SelectedPrefab != null;

        /// <summary>
        /// Resets the current objects sink amount incase of bad values being set.
        /// </summary>
        public void ResetCurrentObjectSinkAmount()
        {
            // Reset the sink amount to 0.
            PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs[Selection.PrefabIndex].SinkAmount = 0;
            // Update the blueprint.
            Blueprint.UpdateSinkAmt(PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs[Selection.PrefabIndex].SinkAmount);
        }

        #endregion

        /// <summary>
        /// Enables the HelpMePlace system and initialises all nescessary handlers / dependants. 
        /// Also subscribes the handlers.
        /// </summary>
        private void OnEnable()
        {
            currentStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                stretchHeight = true
            };

#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            if (PrefabGroups.Count == 0 || PrefabGroups[0].PlaceablePrefabs.Count == 0)
            {
                Validator.Error(Validations.NoGroupsAdded);
                enabled = false;
                return;
            }

            if (HMPSystem.ShowActivityNotifications)
            {
                // Notify the user
                Debug.Log("(1/2) <color=blue>Help Me Place!</color> activated. " +
                    "This will lock some editor functionality. " +
                    "Please click into the scene view and ensure Gizmos are enabled to continue. ");

                Debug.Log("(2/2) " +
                    "To deactivate, hit ESC whilst the scene view is in focus. " +
                    "You can also deactivate or delete the HelpMePlace GameObject, " +
                    "alternativly you can simply disable the HelpMePlaceSystem component");
            }

            // Ensure the singleton instance is initialised.
            if (Instance == null) Instance = this;

            // Ensure the Selection system has a blueprint to work with.
            InitialiseBlueprint(Event.current);

            // Add the handler to the event. 
            SceneView.duringSceneGui += QueueEditorUpdate;
            SceneView.duringSceneGui -= QueueEditorHMPInactiveUpdate;

            RightMouseButtonHeld = false;

            EventHooks.InvokeHMPSystemActivated();
#endif
        }

        /// <summary>
        /// On disable will run and clean up all HelpMePlace objects, and notify the user that it has been disabled. 
        /// </summary>
        private void OnDisable()
        {
            // If the configuration is enabled, show activity.
            if (HMPSystem.ShowActivityNotifications)
            {
                // Notify the user
                Debug.Log("(1/1) <color=blue>Help Me Place!</color> deactivated. Visit the HelpMePlace GameObject to re-enable!", transform);
            }

            // Clean up the blueprint if it exists.
            if (Blueprint?.GetCurrentBlueprintGameObject() != null)
                DestroyImmediate(Blueprint.GetCurrentBlueprintGameObject());

            // Return the tool from the Hand tool.
            Tools.current = Tool.Move;

            // Unsubscribe from the event. 
            SceneView.duringSceneGui -= QueueEditorUpdate;
            SceneView.duringSceneGui += QueueEditorHMPInactiveUpdate;

            RightMouseButtonHeld = false;

            spawnedTransforms = new List<HMPSpawnedTransform>();

            EventHooks.InvokeHMPSystemDeactivated();
        }

        /// <summary>
        /// Initialises the Blueprint object.
        /// </summary>
        /// <param name="e">Current event</param>
        private void InitialiseBlueprint(Event e)
        {
            // Updates the appearance to the selected prefab.
            UpdateBlueprintAppearance(Selection.SelectedPrefab);
            // Ensures the selection is initialised correctly.
            Selection.HandleInitialize(e);
        }

        bool RightMouseButtonHeld = false;

        private void OnDestroy() => UnregisterEvents();

        void UnregisterEvents()
        {
            SceneView.duringSceneGui -= QueueEditorHMPInactiveUpdate;
            SceneView.duringSceneGui -= QueueEditorUpdate;
        }

        /// <summary>
        /// If the system has been de-activated, allow the user to re-enable on keypress.
        /// </summary>
        /// <param name="scene"></param>
        void QueueEditorHMPInactiveUpdate(SceneView scene)
        {
            try
            {
                if ((!ReferenceEquals(gameObject, null)) && !enabled) {
                    var e = Event.current;

                    // Re-enable button.
                    if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.ReEnableHMP)
                        GameObject.FindObjectOfType<HelpMePlaceSystem>().enabled = true;
                }
            } catch (Exception) {
                UnregisterEvents();
            }
        }

        /// <summary>
        /// Listens to any input from the user if enabled and Gizmos are enabled. 
        /// </summary>
        /// <param name="scene">The current scene view.</param>
        void QueueEditorUpdate(SceneView scene)
        {
#if UNITY_EDITOR
            // Only run if the application is not in Play mode.
            if (Application.isPlaying)
                return;

            // If the tool is not enabled, remove the blueprint and pause execution.
            if (!enabled) return;

            // Tick counter so that we can stutter some operations.
            tickCounter++;

            // Ensure the Instance is never null by checking every 100 ticks. 
            if (tickCounter % 100 == 1) if (Instance == null) Instance = this;

            // Ensure the instance has been created before the system activates. 
            if (Instance == null) return;

            // Force the hand tool - This is to prevent selecting items when rotation is what is required.
            Tools.current = Tool.View;

            // Get the current user input
            Event e = Event.current;

            HandleExitTool(e);

            HandleRightMouseButtonDetection(e);

            // If the mouse button is held, the user may be moving around the scene using WASD - so disable the tool traversal.
            if (!RightMouseButtonHeld)
            {
                // Selection handlers and control.
                Selection.HandleColumnChange(e);
                Selection.HandleChangeSelection(e);
                Selection.HandleKeybindButtonPresses(e);

                Selection.HandleChangeRandomFromRow(e);
                Selection.HandleChangeMatchNormalOfGround(e);
                Selection.HandleCycleRotationMode(e);
                Selection.HandleCycleRotationAxis(e);
            }

            // Simulate physics
            HandleSimulatePhysics(e);

            // Handle visibility / activity of the overlay map.
            HandleToggleOverlayMap(e);

            // Handle the placement initialisation of the prefabs.
            HandlePrefabPlacement(e);

            // Handle position / rotation of the blueprint. 
            Blueprint.HandleBlueprintRotation(e);
            Blueprint.HandleBlueprintPositionUpdate(e);

            // Feature to help identify the forward vector of the selected gameobject.
            Blueprint.HandleShowForwardVector(e);

            // Handle any changes to the LookAtPoint
            HandleSetRotationModeLookAtPoint(e);

            // Handle any fencing inputs.
            FencePlacement.HandleInput(e);

            // State updates for the transforms that need activity.
            if (tickCounter % 10 == 1)
                HandleUpdateLoops();


            // Stagger the Ground Layer listener
            if (tickCounter % 100 == 1)
            {
                if (Selection.SelectedPrefab != null)
                    Selection.DetectChangesToScale();
            }
                    // Stagger the Ground Layer listener
            if (tickCounter % 1000 == 1)
            {
                HandleOnChangeGroundLayer();
                if (Selection.SelectedPrefab != null)
                    HandleChangesToCurrentSelectedObject();
            }
#endif
        }

        /// <summary>
        /// Detects if the Right Mouse button is down. 
        /// </summary>
        /// <param name="e"></param>
        void HandleRightMouseButtonDetection(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 1)
                RightMouseButtonHeld = true;

            if (e.type == EventType.MouseUp && e.button == 1)
                RightMouseButtonHeld = false;
        }

        /// <summary>
        /// If the current selected gameobject changes, update the blueprint to reflect that. 
        /// </summary>
        void HandleChangesToCurrentSelectedObject()
        {
            if (PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs[Selection.PrefabIndex].Prefab.name != Selection.SelectedPrefabCachedName)
            {
                // Update the selected prefab & redraw the blueprint. 
                Selection.ChangeSelectedPrefab(null, Selection.PrefabIndex);

                // Ensure the cache is up to date.
                Selection.UpdateCachedName();

                // Force a redraw, as sometimes the frame can be skipped due to editor latency. 
                Selection.ForceUpdateBlueprint();
            }
        }

        /// <summary>
        /// Handler for the LookAtMode point setter.
        /// </summary>
        /// <param name="e">Current event</param>
        void HandleSetRotationModeLookAtPoint(Event e)
        {
            if (e.type == EventType.KeyDown && e.keyCode == HMPKeyBinds.SetLookAtPosition)
                // Update the configuration value to the mouse position
                Configuration.LookAtPosition = Helpers.GetMouseOnTerrainPosition(e);
        }

        /// <summary>
        /// Detects changes to the Ground Layer and ensures the Overlay maps will effect the same layers
        /// </summary>
        void HandleOnChangeGroundLayer()
        {
            // If the Ground Layer has changed
            if (Configuration.GroundLayer != CachedGroundLayer)
            {
                // Recache the new value.
                CachedGroundLayer = Configuration.GroundLayer;

                // Update the effected layers
                HMPSystem.UpdateOverlayMapsEffectedLayer(Configuration.GroundLayer);
            }
        }

        /// <summary>
        /// Handles the Physics simulation frames
        /// </summary>
        /// <param name="e">The current Event</param>
        void HandleSimulatePhysics(Event e)
        {
            // Only available if Animate Placement is disabled.
            if (!HMPSystem.AnimatePlacement && e.type == EventType.KeyDown && e.keyCode == HMPKeyBinds.SimulatePhysics)
            {
                Physics.autoSimulation = false;
                // Simulate physics for each frame.
                Physics.Simulate(Time.fixedDeltaTime);
                Physics.autoSimulation = true;
            }
        }

        /// <summary>
        /// State updates for the transforms that need activity.
        /// </summary>
        void HandleUpdateLoops()
        {
            // If a transform exists in the spawned list, update its current state. 
            foreach (HMPSpawnedTransform st in spawnedTransforms)
                st.Update();

            // Ensure that the isStatic flag has been set to desired. 
            foreach (HMPSpawnedTransform st in spawnedTransforms.FindAll(o => o.ActionNotRequired()))
            {
                if (st.SpawnedGo != null && Configuration.MakePrefabStatic) st.SpawnedGo.isStatic = true;
                EventHooks.InvokePrefabPlaceFinished(st.SpawnedGo);
            }

            // Remove all items that have reached the end of their state cycle.
            spawnedTransforms.RemoveAll(o => o.ActionNotRequired());
        }

        /// <summary>
        /// If ESC is hit, exit the tool by deactivating the component.
        /// </summary>
        /// <param name="e">Current input event of user</param>
        void HandleExitTool(Event e)
        {
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.ExitTool)
            {
                // Delete the blueprint if it exists.
                if (Blueprint.GetCurrentBlueprintGameObject() != null)
                    DestroyImmediate(Blueprint.GetCurrentBlueprintGameObject());

                // Unsubscribe the editor queue
                SceneView.duringSceneGui -= QueueEditorUpdate;
                SceneView.duringSceneGui += QueueEditorHMPInactiveUpdate;

                // Disable the component.
                enabled = false;
            }
        }

        /// <summary>
        /// Toggle all the projector gameobjects
        /// </summary>
        /// <param name="e">Current input event of user</param>
        void HandleToggleOverlayMap(Event e)
        {
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.ToggleOverlayMap)
            {
                // If any overlays are active, turn them all off, or vice versa.
                bool anyAreOn = HMPSystem.GetOverlayMaps().Any(o => o.activeInHierarchy);
                foreach (GameObject go in HMPSystem.GetOverlayMaps())
                    go.SetActive(!anyAreOn);
            }
        }

        /// <summary>
        /// Handle placement of the selected prefab.
        /// </summary>
        /// <param name="e">Current input event of user</param>
        void HandlePrefabPlacement(Event e)
        {
            if (!HMPFencePlacement.FenceStarted && (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.PlaceObject))
            {
                // If full screen toggling is happening, don't place an object. (Shift & Spacebar)
                if (Helpers.IsShiftHeld(e) && HMPKeyBinds.PlaceObject == KeyCode.Space) return;

                // If the selection has not been made - throw an error and halt execution.
                if (Selection.SelectedPrefab == null)
                {
                    Validator.Error(Validations.SelectedPrefabIsNull);
                    return;
                }

                // Refresh the current blueprint object incase of unexpected ALT+Tabbing.
                Blueprint.GetCurrentBlueprintGameObject();

                // Force a refresh the blueprint position. 
                Blueprint.SetBlueprintPositionToMousePosition(e);

                // If the blueprint is missing for some reason - throw an error and halt execution.
                if (Blueprint.GetCurrentBlueprintGameObject() == null)
                {
                    Validator.Error(Validations.BlueprintDoesNotExist);
                    return;
                }
                Transform parent = GetParentForPlacedPrefab();

                // Spawn the GameObject using the PrefabUtility.
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(Selection.SelectedPrefab, parent);

                // If the GameObject is not a Prefab, the above line will return Null. So, instantiate normally as it must be a raw GameObject from the Scene.
                if (go == null)
                    go = Instantiate(Selection.SelectedPrefab, parent);

                // if it still has not been created, there is something wrong with this prefab item. Throw an error and halt execution. 
                if (go == null)
                {
                    // If the spawned gameobject is STILL empty, execution should not continue.
                    Validator.Error(Validations.FailedToSpawn);
                    return;
                }

                // Register the event creation with Undo. 
                Undo.RegisterCreatedObjectUndo(go, HMP_UNDO_ITEM_PREFIX + go.name);

                // Add the newly created item to the spawned transforms so that the system can animate them falling over time.
                spawnedTransforms.Add(
                    new HMPSpawnedTransform(
                        go,
                        Blueprint.GetPosition(),
                        Blueprint.GetRotation(),
                        Blueprint.GetScale()
                    )
                );

                // If RandomFromRow has been selected by the user, select a random prefab.
                if (PrefabGroups[Selection.ColumnIndex].RandomFromRow)
                    Selection.ChangeSelectedPrefab(e, UnityEngine.Random.Range(0, PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs.Count));

                // If the rotation mode is Random, rotate the blueprint to a random 360 degree amount. 
                if (Configuration.RotationMode == RotationMode.Random)
                {
                    Vector3 rotation = Blueprint.GetRotationEuler();
                    float rnd = UnityEngine.Random.Range(0, 360);

                    switch (PrefabGroups[Selection.ColumnIndex].RotateAroundAxis)
                    {
                        case RotationAxis.X:
                            rotation.x = rnd;
                            break;
                        case RotationAxis.Y:
                            rotation.y = rnd;
                            break;
                        case RotationAxis.Z:
                            rotation.z = rnd;
                            break;
                        case RotationAxis.All:
                            rotation = UnityEngine.Random.rotation.eulerAngles;
                            break;
                        default:
                            rotation = UnityEngine.Random.rotation.eulerAngles;
                            break;
                    }

                    Blueprint.RotateByRandomAmount(rotation); //.SetRotationEuler(rotation);
                }

                Blueprint.SetScale(GetScaleFromSelectedRange());
            }
        }

        internal Transform GetParentForPlacedPrefab() => 
            // Get the desired parent or the default if selected.
            PrefabGroups[Selection.ColumnIndex]?.DesiredParent == null ?
                Configuration.GetSpawnedObjectParent(PrefabGroups[Selection.ColumnIndex].GroupName) :
                PrefabGroups[Selection.ColumnIndex]?.DesiredParent;

        /// <summary>
        /// Gets a random scale size between min and max of whats set in the inspector.
        /// </summary>
        /// <returns>A vector3 for scale between min and max</returns>
        internal Vector3 GetScaleFromSelectedRange()
        {
            float rnd = UnityEngine.Random.Range(
                PrefabGroups[Selection.ColumnIndex].ScaleRangeMin,
                PrefabGroups[Selection.ColumnIndex].ScaleRangeMax
            );

            return new Vector3(rnd, rnd, rnd);
        }

        /// <summary>
        /// As Destroy Immediate can only be called from a MonoBehavior, this will be used to strip items from the BluePrint.
        /// </summary>
        /// <param name="o">The Unity object to destroy.</param>
        internal void DestroyProxy(UnityEngine.Object o) => DestroyImmediate(o);


        static void drawString(string text, Vector3 worldPos, Color? colour = null, bool isHeader = false)
        {
            Handles.BeginGUI();
            Color restoreColor = GUI.color;

            if (colour.HasValue) GUI.color = colour.Value;
            
            var view = SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

            if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
            {
                Handles.EndGUI();
                return;
            }

            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUIStyle s = new GUIStyle
            {
                fontStyle = isHeader ? FontStyle.Bold : FontStyle.Normal,
            };

            s.normal.textColor = colour.Value;
            
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, s);
            GUI.color = restoreColor;
            
            Handles.EndGUI();
        }

        public List<StringsToDrawGizmo> StringsToDrawGizmos = new List<StringsToDrawGizmo>();
        public class StringsToDrawGizmo
        {
            public string Text;
            public Color? Color;
            public Vector3 Pos;
            public bool IsHeader = false;
        }

        /// <summary>
        /// Draws the Editor UI for the user to see the selected prefabs.
        /// Also draws a gizmo for rotation lookat features.
        /// Also outputs the hotkeys. Feature can be turned off in system configuration.
        /// </summary>
        void OnDrawGizmos()
        {

            // Should only happen in the Editor.
            #if UNITY_EDITOR
            if (Visualizer.VisualizerEnabled)
                Visualizer.Draw();
            #endif

            if (StringsToDrawGizmos.Any())
                foreach (var x in StringsToDrawGizmos)
                    drawString(x.Text, x.Pos, x.Color, x.IsHeader);

            if (!enabled) return;
        
            #if UNITY_EDITOR
            // Should only happen in the editor when the application is Not running. 
            if (Application.isPlaying)
                return;

            SceneView.RepaintAll();
            Handles.BeginGUI();

            FencePlacement.OnDrawGizmos();

            // Maintains the counters so that we know what is selected and can draw accordingly. 
            int rowCounter = 0;

            // Draw the Logo in the top left of the editor scene view.
            Rect LogoRect = new Rect(HMPSystem.InterfaceStartPosition.x + HMP_XPOS_INITIAL_PADDING, HMPSystem.InterfaceStartPosition.y + HMP_PREVIEW_IMAGE_TOP_PADDING, HMP_LOGO_WIDTH, HMP_LOGO_WIDTH);
            GUI.DrawTexture(LogoRect, HMPSystem.GetTexture(TextureSettings.HMPLogo));

            // Maintains the X position of the UI elements. 
            xImageSpawnPosition = HMPSystem.InterfaceStartPosition.x + HMP_XPOS_INITIAL_PADDING * 2 + HMP_LOGO_WIDTH;

            Rect instrLeftRect = GetNextRectForimage(Configuration.PreviewImageSize / 2);
            // Draws the Left Instruction
            DrawTextureWithHelp(
                instrLeftRect,
                HMPSystem.GetTexture(TextureSettings.InstructionLeft),
                HMPKeyBinds.SelectionMoveLeft
            );

            // Halves the space between previous and next UI Elements.
            HalfSpacerFromPlacement();

            // Add the group name & the hotkey to the UI.
            Color oldColor = GUI.color;
            GUI.color = Color.blue;

            // If the current selected column is removed from the list, reset to the first column.
            if (Selection.ColumnIndex > PrefabGroups.Count - 1)
                Selection.ResetToFirstColumn();

            // If no groups have been created, or the group that was selected is empty, stop execution. 
            if (PrefabGroups.Count == 0 || PrefabGroups[Selection.ColumnIndex] == null)
                return;

            // If the prefab has been removed as its selected, revert to the first prefab.
            if (Selection.PrefabIndex > PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs.Count - 1)
                Selection.ChangeSelectedPrefab(null, 0);

            // If the selection is still not possible, no placement prefabs are available still - so halt execution.
            if (Selection.PrefabIndex > PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs.Count - 1)
            {
                Validator.Error(Validations.AllPrefabsRemoved);
                enabled = false;
                return;
            }

            // Get the currently selected prefab name. 
            string SelectedPrefabName = "";
            if (PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs[Selection.PrefabIndex]?.Prefab == null)
                // If the prefab name is unavailable as the prefab is null, show 'Invalid' as this will fail to place anyway.
                SelectedPrefabName = "Invalid";
            else
                // Otherwise get the prefab name as the object thats assigned name.
                SelectedPrefabName = PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs[Selection.PrefabIndex].Prefab.name;

            // Show the Next/Previous hotkeys and the Group Name currently selected to the UI.
            string selectedGroupNameWithHotkey =
                (Configuration.ShowKeybinds ? "(" + HMPKeyBinds.SelectNextGroup + "/" + HMPKeyBinds.SelectPreviousGroup + ") " : "")
                + PrefabGroups[Selection.ColumnIndex].GroupName
                + ": "
                + "" + SelectedPrefabName + "";

            AddTextAtPosition(
                new Rect(
                        xImageSpawnPosition,
                        HMPSystem.InterfaceStartPosition.y + HMP_PREVIEW_IMAGE_TOP_PADDING + Configuration.PreviewImageSize,
                        // Set the size of the information to the same size as the width of all placable prefabs.
                        (PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs.Count * Configuration.PreviewImageSize) +
                        (PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs.Count * (PREVIEW_IMAGE_SPACER / 2)) - (PREVIEW_IMAGE_SPACER / 2),
                        40
                    ),
                   selectedGroupNameWithHotkey
                );

            // Reset the color to what it was before
            GUI.color = oldColor;

            // Draw all added prefabs if available
            foreach (HMPPrefabGroup.HMPPlaceablePrefab pp in PrefabGroups[Selection.ColumnIndex].PlaceablePrefabs)
            {
                // Get the next available position.
                Rect thisRectPos = GetNextRectForimage();

                // Get the asset preview for the current gameobject.
                Texture2D assetPreview = AssetPreview.GetAssetPreview(pp.Prefab);

                // If no preview is available..
                if (assetPreview == null)
                {
                    GUI.DrawTexture(thisRectPos, HMPSystem.GetTexture(TextureSettings.PreviewMissing));
                }
                else
                    GUI.DrawTexture(thisRectPos, assetPreview);

                // Check what is selected.
                if (rowCounter == Selection.PrefabIndex)
                {
                    Selection.SelectedPrefab = pp.Prefab;
                    // This is the selected prefab, so draw the selection texture above it. 
                    GUI.DrawTexture(thisRectPos, HMPSystem.GetTexture(TextureSettings.ActiveSelection));
                }
                else
                {
                    // This is not the selected prefab, draw the inactive selection texture above it.
                    GUI.DrawTexture(thisRectPos, HMPSystem.GetTexture(TextureSettings.InactiveSelection));
                }

                // Increment rowcounter so we know when we'll hit the selected prefab.
                rowCounter++;

                // Tweak the distance a little between what we just created. 
                HalfSpacerFromPlacement();
            }

            Rect instrRightRect = GetNextRectForimage(Configuration.PreviewImageSize / 2);

            // Draw the selection move right
            DrawTextureWithHelp(
                instrRightRect,
                HMPSystem.GetTexture(TextureSettings.InstructionRight),
                HMPKeyBinds.SelectionMoveRight
            );

            Rect BkgRect = new Rect(
                instrLeftRect.x + instrLeftRect.width + PREVIEW_IMAGE_SPACER,
                HMPSystem.InterfaceStartPosition.y + HMP_PREVIEW_IMAGE_TOP_PADDING + Configuration.PreviewImageSize +2, 
                instrRightRect.x - instrLeftRect.x - instrLeftRect.width - (PREVIEW_IMAGE_SPACER * 2), 
                1
            );

            GUIStyle currentStyle = null;
            currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = MakeTex(2, 2, PrefabGroups[Selection.ColumnIndex].Color);
            GUI.Box(BkgRect, "", currentStyle);

            // Half the last space
            // Draw the Random From Row selected / deselected Texture
            xImageSpawnPosition += 10;

            DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(
                            PrefabGroups[Selection.ColumnIndex].RotationToNormalEnabled ?
                                TextureSettings.RotationToNormalActive :
                                TextureSettings.RotationToNormalInactive
                            ),
                        HMPKeyBinds.HandleChangeMatchNormalOfGround
                    );

            HalfSpacerFromPlacement();

            // Depending on what RotationMode is selected - Draw an appropriate Texture
            switch (Configuration.RotationMode)
            {
                case RotationMode.LookTowards:
                    // Draw the LookTowards Rotation image.
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.LookTowardsRotationMode),
                        HMPKeyBinds.CycleRotationModes
                    );
                    // If LookTowards is selected, add the helper for the Set Rotation Point. 
                    HalfSpacerFromPlacement();
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.InstructionLookAtSetPointRotationModeHelper),
                        HMPKeyBinds.SetLookAtPosition
                    );
                    break;
                case RotationMode.LookAwayFrom:
                    // Draw the LookAwayFrom Rotation image.
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.LookAwayFromRotationMode),
                        HMPKeyBinds.CycleRotationModes
                    );
                    // If LookAwayFrom is selected, add the helper for the Set Rotation Point. 
                    HalfSpacerFromPlacement();
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.InstructionLookAtSetPointRotationModeHelper),
                        HMPKeyBinds.SetLookAtPosition
                    );
                    break;
                case RotationMode.Fixed:
                    // Draw the Fixed Rotation image.
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.FixedRotationMode),
                        HMPKeyBinds.CycleRotationModes
                    );
                    break;
                case RotationMode.Random:
                    // Draw the Random Rotation image.
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.RandomRotationMode),
                        HMPKeyBinds.CycleRotationModes
                    );
                    break;
                case RotationMode.Smooth:
                    // Draw the Smooth Rotation image.
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.SmoothRotationMode),
                        HMPKeyBinds.CycleRotationModes
                    );
                    break;
                case RotationMode.Snap:
                    // Draw the Snap Rotation image.
                    DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(TextureSettings.SnapRotationMode),
                        HMPKeyBinds.CycleRotationModes
                    );
                    break;
            }

            // Half the last space
            HalfSpacerFromPlacement();

            // Draw the Random From Row selected / deselected Texture
            DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(
                            PrefabGroups[Selection.ColumnIndex].RandomFromRow ?
                                TextureSettings.InstructionRandomFromRowActive :
                                TextureSettings.InstructionRandomFromRowInactive
                            ),
                        HMPKeyBinds.RandomFromRowToggle
                    );



            // Half the last space
            // HalfSpacerFromPlacement();
            xImageSpawnPosition += 10;

            switch (PrefabGroups[Selection.ColumnIndex].RotateAroundAxis)
            {
                case RotationAxis.X:
                    DrawTextureWithHelp(
                                GetNextRectForimage(),
                                HMPSystem.GetTexture(TextureSettings.XRotationAxis),
                                HMPKeyBinds.CycleRotationAxis
                            );
                    break;
                case RotationAxis.Y:
                    DrawTextureWithHelp(
                                GetNextRectForimage(),
                                HMPSystem.GetTexture(TextureSettings.YRotationAxis),
                                HMPKeyBinds.CycleRotationAxis
                            );
                    break;
                case RotationAxis.Z:
                    DrawTextureWithHelp(
                                GetNextRectForimage(),
                                HMPSystem.GetTexture(TextureSettings.ZRotationAxis),
                                HMPKeyBinds.CycleRotationAxis
                            );
                    break;
                case RotationAxis.All:
                    DrawTextureWithHelp(
                                GetNextRectForimage(),
                                HMPSystem.GetTexture(TextureSettings.AllRotationAxis),
                                HMPKeyBinds.CycleRotationAxis
                            );
                    break;
                default:
                    break;
            }

            // Half the last space
            HalfSpacerFromPlacement();

            // Draw the Show/Hide overlay selected / deselected Texture
            DrawTextureWithHelp(
                        GetNextRectForimage(),
                        HMPSystem.GetTexture(
                            HMPSystem.GetOverlayMaps().Any(o => o.activeInHierarchy) ?
                                TextureSettings.InstructionShowHideOverlayMapActive :
                                TextureSettings.InstructionShowHideOverlayMapInactive
                            ),
                        HMPKeyBinds.ToggleOverlayMap
                    );

            // Thats the end of the 2D UI
            Handles.EndGUI();

            // Draw the Look at location
            if (Configuration.RotationMode == RotationMode.LookAwayFrom || Configuration.RotationMode == RotationMode.LookTowards)
            {
                Color col = Gizmos.color;
                Gizmos.color = HMPSystem.GetGizmoColor();
                Gizmos.DrawSphere(Configuration.LookAtPosition, 1f);
                Gizmos.color = col;
            }

#endif
        }

        #region Editor UI Element helpers

        // Maintain the Xposition for all Rectangles used by the Editor UI.
        float xImageSpawnPosition = 0;

        /// <summary>
        /// Gets the next rectangle for placements.
        /// </summary>
        /// <param name="widthOverride">Overrides the width to custom amounts</param>
        /// <returns>Returns a rectangle in the sequence for the next rectangle required.</returns>
        Rect GetNextRectForimage(int widthOverride = -1)
        {
            widthOverride = widthOverride == -1 ? _ = Configuration.PreviewImageSize : widthOverride;

            // Create a rectangle to the right of the previous Image.
            Rect rtn = new Rect(
                xImageSpawnPosition,
                HMPSystem.InterfaceStartPosition.y + HMP_PREVIEW_IMAGE_TOP_PADDING,
                widthOverride,
                Configuration.PreviewImageSize
            );

            // If width override is given, throw that value onto the X pos for next iteration.
            if (widthOverride == -1)
                IncreaseSpacerImageWidthWithSpacer();
            else
                IncreaseSpacerWithCustomAmount(widthOverride);

            return rtn;
        }

        // Helper to increase by the Spacer
        void IncreaseSpacerImageWidthWithSpacer() => xImageSpawnPosition += Configuration.PreviewImageSize + PREVIEW_IMAGE_SPACER;

        // Helper to increase by a custom amount
        void IncreaseSpacerWithCustomAmount(int widthOverride) => xImageSpawnPosition += widthOverride + PREVIEW_IMAGE_SPACER;

        // Remove half the space from the XPosition
        void HalfSpacerFromPlacement() => xImageSpawnPosition -= PREVIEW_IMAGE_SPACER / 2;

        /// <summary>
        /// Draws the texture with the hotkey below.
        /// </summary>
        /// <param name="rect">The rectangle to draw the Image at</param>
        /// <param name="tex">The texture to draw</param>
        /// <param name="keyCode">The KeyCode to draw below the texture</param>
        private void DrawTextureWithHelp(Rect rect, Texture2D tex, KeyCode keyCode)
        {
            if (tex != null)
                GUI.DrawTexture(rect, tex);

            Rect instructionBox = new Rect(rect.x, rect.y + rect.height, rect.width, rect.height / 2);
            // Only draw the keycode if the configuration is desired. 
            if (Configuration.ShowKeybinds)
            {
                // Change the UI Color temporarily until the instruction is created.
                Color oldColor = GUI.color;
                GUI.color = Color.blue;
                instructionBox.height = 20;

                // Transparent Background
                GUI.Box(instructionBox, image: null);

                GUI.color = Color.black;
                // The hotkey
                GUI.Label(instructionBox, new GUIContent(keyCode.ToString()), currentStyle);

                // Reset the colors.
                GUI.color = oldColor;
            }
        }

        GUIStyle currentStyle = null;

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color32[] pix = new Color32[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels32(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Creates a text box at position.
        /// </summary>
        /// <param name="rect">The position of the screen to use</param>
        /// <param name="message">The message to display</param>
        private void AddTextAtPosition(Rect rect, string message)
        {
            Color oldColor = GUI.color;
            // Transparent Background
            GUI.color = Color.blue;
            GUI.Box(rect, image: null);

            GUI.color = Color.black;
            GUI.Box(rect, message, currentStyle);
            GUI.color = oldColor;
        }

        #endregion
#endif
    }
}