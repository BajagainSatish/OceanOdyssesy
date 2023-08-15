using System;
using System.Linq;
using UnityEngine;

namespace HelpMePlace
{
    /// <summary>
    /// Handles selection / traversal of the Prefab groups and their associated prefabs and settings. 
    /// Also handles updates to the properties in the Configuration sections.
    /// </summary>
    internal class HMPSelectedPrefabHandler
    {
#if UNITY_EDITOR
        // Holds the Prefab Index for each prefab.
        internal int PrefabIndex = 0;

        // Holds the Column index for each of the prefabs. 
        internal int ColumnIndex = 0;
        
        // Stores the currently selected Prefab.
        internal GameObject SelectedPrefab;

        // Used in detecting changes to the current GameObject without using the UI.
        internal string SelectedPrefabCachedName = "";

        /// <summary>
        /// Any changes to the column selected are handled here. 
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleColumnChange(Event e)
        {
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.SelectNextGroup)
            {
                // Reset the prefab index.
                PrefabIndex = 0;
                // Increment the column index.
                ColumnIndex++;
                // If the end of the list is reached, scroll back to the first index.
                if (ColumnIndex > (HelpMePlaceSystem.Instance.PrefabGroups.Count - 1)) 
                    ColumnIndex = 0;

                // Cache the selected prefab.
                SelectedPrefab = HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].Prefab;
                UpdateCachedName();

                // Handle updates to the blueprint system
                HelpMePlaceSystem.Instance.UpdateBlueprintAppearance(SelectedPrefab);
                HelpMePlaceSystem.Instance.UpdateBlueprintSinkDepth(HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount);
                HelpMePlaceSystem.Instance.Blueprint.SetBlueprintPositionToMousePosition(e);

                HelpMePlaceSystem.Instance.EventHooks.InvokePrefabGroupChange();
            }

            // Handle traversal back through the system
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.SelectPreviousGroup)
            {
                // If user is hitting CTRL+S we don't wanna traverse, as the intention is to save the scene.
                if (HMPKeyBinds.SelectPreviousGroup == KeyCode.S && e.control) 
                    return;

                PrefabIndex = 0;
                // Reverse the column selection
                ColumnIndex--;

                // If the column index is below zero, then we should go to the maximum from the prefab groups list.
                if (ColumnIndex < 0) 
                    ColumnIndex = (HelpMePlaceSystem.Instance.PrefabGroups.Count - 1);

                // Cache the selected prefab.
                SelectedPrefab = HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].Prefab;
                UpdateCachedName();

                // Handle updates to the blueprint system
                HelpMePlaceSystem.Instance.UpdateBlueprintAppearance(SelectedPrefab);
                HelpMePlaceSystem.Instance.UpdateBlueprintSinkDepth(HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount);
                HelpMePlaceSystem.Instance.Blueprint.SetBlueprintPositionToMousePosition(e);

                HelpMePlaceSystem.Instance.EventHooks.InvokePrefabGroupChange();
            }
        }

        internal float GetCurrentSinkAmount() => HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount;

        internal float GetCurrentWidthOfObject() => HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].WidthOfObject;
        internal float SetCurrentWidthOfObject(float amt) => HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].WidthOfObject += amt;

        internal void UpdateCachedName() => SelectedPrefabCachedName = SelectedPrefab != null ? SelectedPrefab.name : "";

        internal void ResetToFirstColumn()
        {
            ColumnIndex = 0;

            if (HelpMePlaceSystem.Instance.PrefabGroups.Count == 0 || HelpMePlaceSystem.Instance.PrefabGroups[0].PlaceablePrefabs.Count == 0)
            {
                HelpMePlaceSystem.Instance.Validator.Error(HMPEnums.Validations.ResetPreventedNoPrefabGroups);
                HelpMePlaceSystem.Instance.enabled = false;
                return;
            }

            SelectedPrefab = HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].Prefab;
            UpdateCachedName();

            // Handle updates to the blueprint system
            HelpMePlaceSystem.Instance.UpdateBlueprintAppearance(SelectedPrefab);
            HelpMePlaceSystem.Instance.UpdateBlueprintSinkDepth(HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount);
        }

        /// <summary>
        /// Handles changes to the selected prefab
        /// </summary>
        /// <param name="e">Current event</param>
        /// <param name="index">the prefab index to select</param>
        internal void ChangeSelectedPrefab(Event e, int index)
        {
            Vector3 oldRotationEuler = Vector3.zero;
            // If a prefab was previously selected, copy the rotation euler;
            if (HelpMePlaceSystem.Instance != null && HelpMePlaceSystem.Instance.Blueprint != null && HelpMePlaceSystem.Instance.Blueprint.GetCurrentBlueprintGameObject() != null)
                oldRotationEuler = HelpMePlaceSystem.Instance.Blueprint.GetCurrentBlueprintGameObject().transform.rotation.eulerAngles;

            // Set the prefab index
            PrefabIndex = index;

            // If the index is more than the amount of prefabs available, reset the selected prefab to the first in the list.
            if (index > HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs.Count - 1) 
                PrefabIndex = 0;

            // if the index is less than zero, go to the previous maximum
            if (index < 0) 
                PrefabIndex = HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs.Count - 1;

            if (HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs.Count == 0) return;

            // Cache the selected prefab.
            SelectedPrefab = HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].Prefab;

            UpdateCachedName();

            // Handle blueprint updates.
            HelpMePlaceSystem.Instance.UpdateBlueprintAppearance(SelectedPrefab);
            HelpMePlaceSystem.Instance.UpdateBlueprintSinkDepth(HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount);

            // Sometimes E may be null because of some modifications to the inner lists in Settings require the blueprint to update, without an event current within sceneview.
            if(e != null) HelpMePlaceSystem.Instance.Blueprint.SetBlueprintPositionToMousePosition(e);

            // Refresh the rotation to what it was before changing prefab
            HelpMePlaceSystem.Instance.Blueprint.SetRotationEuler(oldRotationEuler);
        }

        /// <summary>
        /// For unexpected updates to the selection, i.e. when a prefab is replaced whilst selected - this method will update the blueprint to the newly selected prefab.
        /// </summary>
        internal void ForceUpdateBlueprint()
        {
            HelpMePlaceSystem.Instance.UpdateBlueprintAppearance(SelectedPrefab);
            HelpMePlaceSystem.Instance.UpdateBlueprintSinkDepth(HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount);
        }

        /// <summary>
        /// Initialise the prefab selection and cache the selected prefab to zero.
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleInitialize(Event e) => ChangeSelectedPrefab(e, 0);

        /// <summary>
        /// Handle the selection change keyboard events.
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleChangeSelection(Event e)
        {
            // Forward the event to the appropriate method with the selection changes 
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.SelectionMoveLeft)
                ChangeSelectedPrefab(e, PrefabIndex - 1);
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.SelectionMoveRight)
                ChangeSelectedPrefab(e, PrefabIndex + 1);
        }

        /// <summary>
        /// Handles the toggle Random From Row keyboard event.
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleChangeRandomFromRow(Event e)
        {
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.RandomFromRowToggle)
                // Update the selection in the prefab groups
                HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].RandomFromRow = !HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].RandomFromRow;
        }

        /// <summary>
        /// Handles the toggle Rotation to Normal keyboard event.
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleChangeMatchNormalOfGround(Event e)
        {
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.HandleChangeMatchNormalOfGround)
                // Update the rotation to normal flag for the selected group
                HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].RotationToNormalEnabled = !HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].RotationToNormalEnabled;
        }

        /// <summary>
        /// Handles rotation toggle keyboard events. 
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleCycleRotationMode(Event e)
        {
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.CycleRotationModes)
            {
                // Hotkey for cycling rotation mode
                if (e.shift)
                {
                    // Traverse back through the rotation modes if shift is held. 
                    int currentRotationMode = (int)HelpMePlaceSystem.Instance.Configuration.RotationMode;

                    // As above, but in reverse. 
                    currentRotationMode--;
                    if (currentRotationMode < 0)
                        // Set to maximum
                        HelpMePlaceSystem.Instance.Configuration.RotationMode = (HMPEnums.RotationMode)Enum.GetNames(typeof(HMPEnums.RotationMode)).Length - 1;
                    else
                        HelpMePlaceSystem.Instance.Configuration.RotationMode--;

                    return;
                }
                else
                {
                    // Cache the current rotation mode.
                    int currentRotationMode = (int)HelpMePlaceSystem.Instance.Configuration.RotationMode;

                    // increase the number
                    currentRotationMode++;

                    // If the current rotation mode is above the amount of modes we have, set the rotation mode to zero. otherwise increase by one.
                    if (currentRotationMode > Enum.GetNames(typeof(HMPEnums.RotationMode)).Length - 1)
                        HelpMePlaceSystem.Instance.Configuration.RotationMode = 0;
                    else
                        // Move to next rotation mode. 
                        HelpMePlaceSystem.Instance.Configuration.RotationMode++;
                }
            }
        }

        internal void HandleCycleRotationAxis(Event e)
        {
            if (e.type == EventType.KeyDown && e.keyCode == HMPKeyBinds.CycleRotationAxis)
            {
                // Cache the current rotation axis.
                int currentAxisRotation = (int)HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis;

                // increase the number
                currentAxisRotation++;

                // If the current rotation Axis is above the amount of modes we have, set the rotation Axis to zero. otherwise increase by one.
                if (currentAxisRotation > Enum.GetNames(typeof(HMPEnums.RotationAxis)).Length - 1)
                    HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis = 0;
                else
                    // Move to next rotation Axis. 
                    HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis++;

                return;
            }
        }

        float currentScaleRangeMin = 1, currentScaleRangeMax = 1;
        
        void ResetCurrentScaleRanges() {
            currentScaleRangeMin = HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].ScaleRangeMin;
            currentScaleRangeMax = HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].ScaleRangeMax;
        }

        internal void DetectChangesToScale()
        {
            if(currentScaleRangeMin != HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].ScaleRangeMin ||
               currentScaleRangeMax !=  HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].ScaleRangeMax)
            {
                ResetCurrentScaleRanges();
                HelpMePlaceSystem.Instance.Blueprint.SetScale(HelpMePlaceSystem.Instance.GetScaleFromSelectedRange());
            }
        }

        internal void HandleKeybindButtonPresses(Event e)
        {
            if (e.type == EventType.KeyDown)
            {
                if (!(e.keyCode == HMPKeyBinds.Keybind1 ||
                    e.keyCode == HMPKeyBinds.Keybind2 ||
                    e.keyCode == HMPKeyBinds.Keybind3 ||
                    e.keyCode == HMPKeyBinds.Keybind4 ||
                    e.keyCode == HMPKeyBinds.Keybind5 ||
                    e.keyCode == HMPKeyBinds.Keybind6 ||
                    e.keyCode == HMPKeyBinds.Keybind7 ||
                    e.keyCode == HMPKeyBinds.Keybind8 ||
                    e.keyCode == HMPKeyBinds.Keybind9 ||
                    e.keyCode == HMPKeyBinds.Keybind0 )) return;

                var kb = HelpMePlaceSystem.Instance.PrefabGroups.FirstOrDefault(o => o.HotKeyBind == e.keyCode);
                if (kb != null)
                {
                    // Reset the prefab index.
                    PrefabIndex = 0;
                    // Setup the Column index to match whats in prefab groups
                    ColumnIndex = HelpMePlaceSystem.Instance.PrefabGroups.IndexOf(kb);

                    // Cache the selected prefab.
                    SelectedPrefab = HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].Prefab;
                    UpdateCachedName();

                    // Handle updates to the blueprint system
                    HelpMePlaceSystem.Instance.UpdateBlueprintAppearance(SelectedPrefab);
                    HelpMePlaceSystem.Instance.UpdateBlueprintSinkDepth(HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount);
                    HelpMePlaceSystem.Instance.Blueprint.SetBlueprintPositionToMousePosition(e);

                    HelpMePlaceSystem.Instance.EventHooks.InvokePrefabGroupChange();
                }
            }
        }

        /// <summary>
        /// Sets the sink Amount for the selected prefab. 
        /// </summary>
        /// <param name="amt"></param>
        internal void AdjustSinkAmount(float amt)
        {
            // If no value change is required, just return 0.
            if (amt == 0) return;

            // If negative amount is provided add by sensitivity
            if (amt < 1)
                HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount += 
                    HelpMePlaceSystem.Instance.Configuration.ALTAdjustmentSinkAmountSensitivity;
            else
                // If positive amount is provided remove by sensitivity
                HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount += 
                    -HelpMePlaceSystem.Instance.Configuration.ALTAdjustmentSinkAmountSensitivity;

            // Update the blueprint appearance to reflect the change in sink amount.
            HelpMePlaceSystem.Instance.Blueprint.UpdateSinkAmt(
                HelpMePlaceSystem.Instance.PrefabGroups[ColumnIndex].PlaceablePrefabs[PrefabIndex].SinkAmount
            );
        }
#endif
    }
}