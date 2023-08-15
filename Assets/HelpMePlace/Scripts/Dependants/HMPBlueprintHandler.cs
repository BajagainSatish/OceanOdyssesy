using System;
using UnityEngine;
using static HelpMePlace.HMPEnums;
using static HelpMePlace.HMPConstants;

namespace HelpMePlace
{
    /// <summary>
    /// Handles any changes to the blueprint.
    /// </summary>
    internal class HMPBlueprintHandler
    {
#if UNITY_EDITOR
        // The current visible blueprints gameobject
        private GameObject bluePrintGo;
        // Mouse button handling. This will take care of any r/l holding click events.
        private bool LMouseIsDown = false, RMouseIsDown = false;

        // v3 Todo: Make settings for this. 
        private bool ShowForwardVector = true;

        // Amount of sinkage to be applied to the blueprint
        private float sinkAmount;

        /// <summary>
        /// Get the current position of the blueprint.
        /// </summary>
        /// <returns>Current position of the blueprint.</returns>
        internal Vector3 GetPosition() =>
            bluePrintGo.transform.position;

        /// <summary>
        /// Get the current rotation of the blueprint.
        /// </summary>
        /// <returns>Current rotation of the blueprint.</returns>
        internal Quaternion GetRotation() =>
            bluePrintGo.transform.rotation;

        /// <summary>
        /// Gets the current scale of the blueprint
        /// </summary>
        /// <returns>Local scale of the blueprint</returns>
        internal Vector3 GetScale() => bluePrintGo.transform.localScale;

        /// <summary>
        /// Sets the local scale of the blueprint
        /// </summary>
        /// <param name="scale">Scale to set</param>
        internal void SetScale(Vector3 scale)
        {
            Vector3 newScale = HelpMePlaceSystem.Instance.Selection.SelectedPrefab.transform.localScale;

            newScale.x *= scale.x;
            newScale.y *= scale.y;
            newScale.z *= scale.z;
             
            bluePrintGo.transform.localScale = newScale;
        }

        /// <summary>
        /// Get the current euler rotation of the blueprint.
        /// </summary>
        /// <returns>Current euler rotation of the blueprint.</returns>
        internal Vector3 GetRotationEuler() =>
            bluePrintGo.transform.eulerAngles;

        /// <summary>
        /// Set the current euler rotation of the blueprint.
        /// </summary>
        /// <param name="euler">The desired euler rotation.</param>
        internal void SetRotationEuler(Vector3 euler) =>
            bluePrintGo.transform.eulerAngles = euler;

        /// <summary>
        /// Get the current blueprint GameObject. 
        /// </summary>
        /// <returns>The current Blueprint object if it exists</returns>
        internal GameObject GetCurrentBlueprintGameObject()
        {
            try
            {
                if (bluePrintGo == null && HelpMePlaceSystem.Instance != null && HelpMePlaceSystem.Instance.gameObject && HelpMePlaceSystem.Instance.gameObject.activeInHierarchy)
                    // If not already cached, look for one in the heirarchy.
                    bluePrintGo = GameObject.Find(HMP_BLUEPRINT_DEFAULT_NAME);
            }
            catch(Exception)
            {
                // "Assertation failed errors sometimes happen on these lines. Nothing we can do about this so just swallow the error and return null. "
                return bluePrintGo;
            }
            return bluePrintGo;
        }

        /// <summary>
        /// Updates the visible sink amount of the blueprint.
        /// </summary>
        /// <param name="sinkAmount">The amount to sink by</param>
        internal void UpdateSinkAmt(float sinkAmount) => this.sinkAmount = sinkAmount;

        /// <summary>
        /// Handles the rotation input changes for the blueprint
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleBlueprintRotation(Event e)
        {
            // As long as the blueprint is active / not empty.
            if (bluePrintGo != null && bluePrintGo.activeInHierarchy)
            {
                // Switches between modes
                switch (HelpMePlaceSystem.Instance.Configuration.RotationMode)
                {
                    case HMPEnums.RotationMode.LookTowards:
                        HandleLookTowards();
                        break;
                    case HMPEnums.RotationMode.LookAwayFrom:
                        HandleLookAwayFrom();
                        break;
                    case HMPEnums.RotationMode.Smooth:
                        HandleSmoothRotation(e);
                        break;
                    case HMPEnums.RotationMode.Snap:
                        HandleSnapRotation(e);
                        break;
                    case HMPEnums.RotationMode.Fixed:
                        HandleFixedRotation();
                        break;
                }

                // Update the fixed amount regardless of what was selected. 
                UpdateRotationFixedAmount();
            }
        }

        /// <summary>
        /// Snaps the rotation of the blueprint by the value given in the configuration.
        /// </summary>
        void SnapLeft() => bluePrintGo.transform.rotation = Quaternion.Euler(GetRotationEuler() - GetRotationSnapAmountForSelectedAxis());

        /// <summary>
        /// Snaps the blueprint rotation by the value given in the configuration.
        /// </summary>
        void SnapRight() => bluePrintGo.transform.rotation = Quaternion.Euler(GetRotationEuler() + GetRotationSnapAmountForSelectedAxis());

        /// <summary>
        /// Calculates the snap amount for the selected axis.
        /// </summary>
        /// <returns>The euler to snap to.</returns>
        Vector3 GetRotationSnapAmountForSelectedAxis()
        {
            Vector3 amount = Vector3.zero;

            switch (HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis)
            {
                case HMPEnums.RotationAxis.X:
                    amount = new Vector3(HelpMePlaceSystem.Instance.Configuration.RotationSnapAmount, 0, 0);
                    break;
                case HMPEnums.RotationAxis.Y:
                    amount = new Vector3(0, HelpMePlaceSystem.Instance.Configuration.RotationSnapAmount, 0);
                    break;
                case HMPEnums.RotationAxis.Z:
                    amount = new Vector3(0, 0, HelpMePlaceSystem.Instance.Configuration.RotationSnapAmount);
                    break;
                case HMPEnums.RotationAxis.All:
                    amount = new Vector3(HelpMePlaceSystem.Instance.Configuration.RotationSnapAmount, HelpMePlaceSystem.Instance.Configuration.RotationSnapAmount, HelpMePlaceSystem.Instance.Configuration.RotationSnapAmount);
                    break;
            }

            return amount;
        }

        /// <summary>
        /// Updates the position of the blueprint to the mouse position.
        /// </summary>
        /// <param name="e">Current event</param>
        internal void HandleBlueprintPositionUpdate(Event e)
        {
            if (e.type == EventType.MouseMove)
            {
                // If the control button is pressed, and the configuration value CtrlAdjustsSinkAmount is set, adjust the sinkamount of the prefab (and the selected gameobject/prefab)
                if (HelpMePlaceSystem.Instance.Configuration.ALTAdjustsSinkAmount && e.alt)
                {
                    // Adjust the Selection object to reflect this change.
                    HelpMePlaceSystem.Instance.Selection.AdjustSinkAmount(e.delta.y);
                    // Adjust the blueprint visibility.
                    SetBlueprintPositionWithNewSinkAmountWithoutMoving(e);
                    // We don't want the blueprint position to go to the mouse position in this case, so halt execution.
                    return;
                }

                // If the mouse is moving, ensure the blueprint follows it. 
                SetBlueprintPositionToMousePosition(e);
            }

            // If the configuration for Rotation match to Normal is enabled
            if (HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotationToNormalEnabled)
                MatchRotationToNormal(e);
        }

        /// <summary>
        ///  Sets the blueprints current position to the mouse position hit on the selected gameobject.
        /// </summary>
        /// <param name="e">Current event</param>
        internal void SetBlueprintPositionToMousePosition(Event e)
        {
            // Gets the current position of the mouse vs the Selected Ground Layer
            Vector3 res = HelpMePlaceSystem.Instance.Helpers.GetMouseOnTerrainPosition(e);

            // If a position is found
            if (res != Vector3.zero)
            {
                // If the blueprint is initialised
                if (bluePrintGo != null)
                {
                    // Set the blueprint to active if not already
                    if (!bluePrintGo.activeInHierarchy)
                        bluePrintGo.SetActive(true);

                    // Set the position to the mouse position.
                    // bluePrintGo.transform.position = res + new Vector3(0, sinkAmount, 0);
                    bluePrintGo.transform.position = res + (HelpMePlaceSystem.Instance.Helpers.GetNormalOfGroundAtPosition(e) * sinkAmount); // new Vector3(0, sinkAmount, 0);
                    return;
                }
            }

            if (bluePrintGo != null && bluePrintGo.activeInHierarchy)
            {
                // If no position was found, deactivate the blueprint.
                bluePrintGo.SetActive(false);
                return;
            }
        }

        /// DEPRECATED
        /// <summary>
        /// This is used to ensure the blueprint doesn't move when the sink amount is being set by mouse movement.
        /// </summary>
        internal void XSetBlueprintPositionWithNewSinkAmountWithoutMoving()
        {
            if (bluePrintGo == null) return;
            // Shoots a ray downward and where it hits, gets the height so that we can relativly move the position of the blueprint to that vector, and include the sinkamount. 
            Vector3 res = HelpMePlaceSystem.Instance.Helpers.GetPositionOnTerrainFromAbove(bluePrintGo.transform.position);

            // if a position is hit, adjust the sinkamount. 
            if (res != Vector3.zero)
                bluePrintGo.transform.position = res + new Vector3(0, sinkAmount, 0);
        }

        /// <summary>
        /// This is used to ensure the blueprint doesn't move when the sink amount is being set by mouse movement.
        /// </summary>
        internal void SetBlueprintPositionWithNewSinkAmountWithoutMoving(Event e)
        {
            if (bluePrintGo == null) 
                return;

            // Shoots a ray downward and where it hits, gets the height so that we can relativly move the position of the blueprint to that vector, and include the sinkamount. 
            Vector3 res = HelpMePlaceSystem.Instance.Helpers.GetMouseOnTerrainPosition(e);

            // if a position is hit, adjust the sinkamount. 
            if (res != Vector3.zero)
                bluePrintGo.transform.position = res + (HelpMePlaceSystem.Instance.Helpers.GetNormalOfGroundAtPosition(e) * sinkAmount); // new Vector3(0, sinkAmount, 0);
        }

        internal void HandleShowForwardVector(Event e)
        {
            if (!ShowForwardVector) return;

            if (e.keyCode == KeyCode.LeftControl) {
                var t = bluePrintGo.transform;
                Debug.DrawLine(t.position, t.position + (t.forward * 100), Color.blue);
            }
        }

        /// <summary>
        /// Updates the blueprint to a new object.
        /// </summary>
        /// <param name="go">The existing blueprint Gameobject.</param>
        internal void UpdateBlueprintGO(GameObject go)
        {
            // Sets the name of the new object
            go.name = HMP_BLUEPRINT_DEFAULT_NAME;

            // Caches the item
            bluePrintGo = go;

            // Removes any colliders so that we can place on effected layers, even if that effected layer is the one used by the blueprint. 
            foreach (Collider col in bluePrintGo.GetComponentsInChildren<Collider>())
                col.enabled = false;

            // Remove any rigidbodies from the blueprint so as the blueprint is not effected by physics.
            foreach (Rigidbody rb in bluePrintGo.GetComponentsInChildren<Rigidbody>())
                HelpMePlaceSystem.Instance.DestroyProxy(rb);

            // Remove the static flag from the object.
            bluePrintGo.isStatic = false;
        }

        internal void MatchRotationToNormal(Event e)
        {
            if (bluePrintGo != null)
            {
                Vector3 groundNormalAtPoint = HelpMePlaceSystem.Instance.Helpers.GetNormalOfGroundAtPosition(e);
                bluePrintGo.transform.rotation = Quaternion.FromToRotation(bluePrintGo.transform.up, groundNormalAtPoint) * bluePrintGo.transform.rotation;
            }
        }

        /// <summary>
        /// Updates the materials used by the blueprint object to the transparent blue. 
        /// </summary>
        internal void UpdateBlueprintMaterialsToNewMat()
        {
            // This should apply to any renderers in the mesh.
            foreach (Renderer r in bluePrintGo.GetComponentsInChildren<Renderer>(true))
            {
                // Updating shared materials to the transparent blue.
                r.sharedMaterial = HelpMePlaceSystem.Instance.HMPSystem.GetBlueprintMaterial();

                var x = r.sharedMaterials;

                for (int i = 0; i < x.Length; i++)
                    x[i] = HelpMePlaceSystem.Instance.HMPSystem.GetBlueprintMaterial();

                r.sharedMaterials = x;
            }
        }

        #region Rotation Helpers
        private void UpdateRotationFixedAmount() =>
            HelpMePlaceSystem.Instance.Configuration.RotationFixed = bluePrintGo.transform.localRotation.eulerAngles;

        private void HandleFixedRotation()
        {
            // Fixed rotations will ensure that the rotation will remain the same for any gameobjects created. 
            bluePrintGo.transform.localRotation = Quaternion.Euler(HelpMePlaceSystem.Instance.Configuration.RotationFixed);
                            //Quaternion.Euler(
                            //    bluePrintGo.transform.localRotation.eulerAngles.x,
                            //    ,
                            //    bluePrintGo.transform.localRotation.eulerAngles.z
                            //);
        }

        private void HandleSnapRotation(Event e)
        {
            // Snap will increment the rotation of the blueprint by the given values in configuration.
            if (e.type == EventType.MouseDown)
            {
                // Based off mouse clicking. 
                if (Event.current.button == 0)
                    SnapLeft();
                if (Event.current.button == 1)
                    SnapRight();
            }
        }

        Vector3 GetWorldRotationAroundVector3(RotationAxis rotationAxis)
        {
            switch(rotationAxis)
            {
                case RotationAxis.X:
                    return Vector3.right;
                case RotationAxis.Y:
                    return Vector3.up;
                case RotationAxis.Z:
                    return Vector3.forward;
                case RotationAxis.All:
                    // For true random, uncomment this line. This will cause strange behavior but it may be required in some use cases. 
                    // For now, we will treat All as Y.
                    //return new Vector3(
                    //    UnityEngine.Random.Range(0, 2),
                    //    UnityEngine.Random.Range(0, 2),
                    //    UnityEngine.Random.Range(0, 2)
                    //);
                    return Vector3.up;
                default:
                    return Vector3.up;
            }
        }

        private void HandleSmoothRotation(Event e)
        {
            // Smooth rotation will activate when the mouse buttons are held down. 
            if (LMouseIsDown)
            {
                bluePrintGo.transform.Rotate(
                    GetWorldRotationAroundVector3(HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis),
                    HelpMePlaceSystem.Instance.Configuration.RotationSmoothSensitivity *
                    // If shift is held, double the speed of rotation.
                    (HelpMePlaceSystem.Instance.Helpers.IsShiftHeld(e) ? 2 : 1)
                , Space.Self);
            }

            // Reverse the movement if it is right clicked. 
            if (RMouseIsDown)
            {
                bluePrintGo.transform.Rotate(
                    GetWorldRotationAroundVector3(HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis),
                    -HelpMePlaceSystem.Instance.Configuration.RotationSmoothSensitivity *
                    // If shift is held, double the speed of rotation.
                    (HelpMePlaceSystem.Instance.Helpers.IsShiftHeld(e) ? 2 : 1)
                , Space.Self);
            }

            // Handle the mouse down events for both mouse buttons.
            if (e.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                    LMouseIsDown = true;
                if (Event.current.button == 1)
                    RMouseIsDown = true;
            }

            // ensure the MouseUp will turn off the held positions.
            if (e.type == EventType.MouseUp)
            {
                if (Event.current.button == 0)
                    LMouseIsDown = false;
                if (Event.current.button == 1)
                    RMouseIsDown = false;
            }
        }

        // Look Towards will use transform lookat to continually ensure the transform is looking towards a given position.
        private void HandleLookTowards()
        {
            Vector3 stareAtPosition = HelpMePlaceSystem.Instance.Configuration.LookAtPosition;
            if (HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotationToNormalEnabled)
                bluePrintGo.transform.LookAt(stareAtPosition);
            else
            {
                // Ensure angle remains flat.
                stareAtPosition.y = bluePrintGo.transform.position.y;
                bluePrintGo.transform.LookAt(stareAtPosition,
                    GetWorldRotationAroundVector3(HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis));
            }
        }

        // Look away from will use transform lookat to continually ensure the transform is looking away from a given position.
        private void HandleLookAwayFrom()
        {
            Vector3 stareAwayFromPosition = HelpMePlaceSystem.Instance.Configuration.LookAtPosition;

            if (HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotationToNormalEnabled)
                bluePrintGo.transform.LookAt(2 * bluePrintGo.transform.position - stareAwayFromPosition);
            else
            {
                // Ensure angle remains flat.
                stareAwayFromPosition.y = bluePrintGo.transform.position.y;
                // Reverse the lookat
                bluePrintGo.transform.LookAt(2 * bluePrintGo.transform.position - stareAwayFromPosition,
                GetWorldRotationAroundVector3(HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotateAroundAxis));
            }
        }

        /// <summary>
        ///  Amount is discovered in the after-placement actions.
        /// </summary>
        /// <param name="rotation">The euler amount to rotate by</param>
        internal void RotateByRandomAmount(Vector3 rotation)
        {
            // Rotate by self space - euler already discoveres using Rotation Axis selected.
            bluePrintGo.transform.Rotate(rotation, Space.Self);
        }
        #endregion
#endif
    }
}