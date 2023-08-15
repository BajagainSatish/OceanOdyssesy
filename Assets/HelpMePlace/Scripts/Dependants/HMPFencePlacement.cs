using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HelpMePlace
{
    /// <summary>
    /// Fence placement tool can create chains of fencing whilst the button for place fence is held.
    /// </summary>
    internal class HMPFencePlacement
    {
#if UNITY_EDITOR
        // Color setup.
        Color FencePreviewColor = Color.yellow, FencePreviewAlternativeColor = Color.cyan, ErrorFenceColor = Color.red;

        // Multiplier when the Shift Button is Held. 
        const int SHIFT_HELD_MULTIPLIER = 5;

        // Limits the amount of fences, preventing huge amounts of previews.
        const int MAX_FENCE_AMOUNT = 1000;

        // Accessable bool to let other HMP classes know that a fence placement is in progress.
        internal static bool FenceStarted = false;

        // Preview system to show where the fences will end up.
        PreviewFenceItemsSetup PreviewSetup = new PreviewFenceItemsSetup();
        
        // Counter for counting the number of previews shown.
        int counter = 0;

        /// <summary>
        /// Distance added or removed by hitting the change hotkey whilst the preview is showing.
        /// </summary>
        const float GAP_CHANGE_AMT = 0.1f;

        /// <summary>
        /// Shows a preview of each fence selected to be dropped on locations.
        /// </summary>
        private class PreviewFenceItemsSetup
        {
            // Stores the meshes and child meshes to be shown by the preview.
            internal List<FenceSubMesh> Meshes = new List<FenceSubMesh>();

            // Desired Begin and End positions of the fence. 
            internal Vector3 StartFencePosition, EndFencePosition;

            // List of preview items to display on gui.
            private List<PreviewFenceItem> previewFenceItems = new List<PreviewFenceItem>();

            // Has the preview object been populated?
            internal bool Showing = false;

            // Distance between the start and end positions of the fences.
            private float Distance = 0;

            // Distance between each item - driven by prefab group.
            internal float GapDistance = 1;

            // The terrain that this operation is being applied to. 
            Terrain t;

            /// <summary>
            ///  Refreshes the preview items
            /// </summary>
            internal void Refresh()
            {
                // Current width of object safeguard. 
                if (HelpMePlaceSystem.Instance.Selection.GetCurrentWidthOfObject() <= 0)
                    HelpMePlaceSystem.Instance.Selection.SetCurrentWidthOfObject(1f);

                // Useful debug in case of unexpected gap changes.
                // Debug.Log($"Refreshing {HelpMePlaceSystem.Instance.Selection.GetCurrentWidthOfObject()}; Distance {Distance}; GapDistance {GapDistance}");

                // Clear existing preview items.
                previewFenceItems.Clear();

                // Calculate normalized direction.
                var dir = (EndFencePosition - StartFencePosition).normalized;
                GapDistance = Mathf.Clamp(GapDistance, 0.1f, HelpMePlaceSystem.Instance.Selection.GetCurrentWidthOfObject() / 2);
                EndFencePosition += (dir * GapDistance);

                // Calculate how many fences can fit between start and end position.
                int numFences = (int)Mathf.Clamp(((int)Distance / HelpMePlaceSystem.Instance.Selection.GetCurrentWidthOfObject()), 0, MAX_FENCE_AMOUNT);

                Quaternion rot = Quaternion.identity;

                // Fix the look rotation to 90 degrees of the forward vector, so that they line up. (May make this configurable in future)
                if (dir != Vector3.zero)
                    rot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0, 90, 0);

                // Add the fence items to the previews.
                for (int i = 0; i < numFences; i++) {
                    Vector3 pos = StartFencePosition + dir * (i * HelpMePlaceSystem.Instance.Selection.GetCurrentWidthOfObject());
                    var h = t.SampleHeight(pos);
                    // Ensure sink amount is maintained.
                    pos.y = h + t.GetPosition().y + HelpMePlaceSystem.Instance.Selection.GetCurrentSinkAmount();
                    previewFenceItems.Add(new PreviewFenceItem(pos, rot));
                }

                // Rotate the blueprint preview to match the rotation of the end result. 
                HelpMePlaceSystem.Instance.Blueprint.SetRotationEuler(rot.eulerAngles);
            }

            /// <summary>
            /// Get all preview items for the current drawing previews.
            /// </summary>
            /// <returns>All current PreviewFenceItems</returns>
            internal List<PreviewFenceItem> GetPreviewItems() => previewFenceItems;

            /// <summary>
            /// Class item to hold the position and rotation of preview items & final fence placement.
            /// </summary>
            internal class PreviewFenceItem
            {
                private Vector3 pos;
                private Quaternion rot;

                internal PreviewFenceItem(Vector3 pos, Quaternion rot) {
                    this.pos = pos;
                    this.rot = rot;
                }

                internal Vector3 GetPos() => pos;
                internal Quaternion GetRot() => rot;
            }

            /// <summary>
            /// Gets the meshes from the selected prefab.
            /// </summary>
            /// <returns></returns>
            internal IEnumerable<FenceSubMesh> GetMeshes() => Meshes;

            /// <summary>
            /// Called when a preview is available to show
            /// </summary>
            internal void ReadyForPreview()
            {
                Showing = true;
                Distance = Vector3.Distance(StartFencePosition, EndFencePosition) + HelpMePlaceSystem.Instance.Selection.GetCurrentWidthOfObject();
                Refresh();
            }

            /// <summary>
            /// Places the prefabs in the desired areas.
            /// </summary>
            internal void Confirm()
            {
                // Get the desired parent or the default if selected.
                GameObject holder = new GameObject($"FenceHolder-{HelpMePlaceSystem.Instance.Selection.SelectedPrefab.name}");
                holder.transform.SetParent(HelpMePlaceSystem.Instance.GetParentForPlacedPrefab());

                // Instantiate each prefab at the preview position and rotations
                foreach (var po in previewFenceItems)
                {
                    GameObject go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(
                    HelpMePlaceSystem.Instance.Selection.SelectedPrefab, holder.transform);
                    
                    if (go == null)
                        go = HelpMePlaceSystem.Instantiate(HelpMePlaceSystem.Instance.Selection.SelectedPrefab, holder.transform);

                    go.transform.position = po.GetPos();
                    go.transform.rotation = po.GetRot();
                }

                // Register undo handler.
                UnityEditor.Undo.RegisterCreatedObjectUndo(holder, HMPConstants.HMP_UNDO_ITEM_PREFIX + holder.name);
            }

            // Resets the current preview items & clears any cached data.
            internal void Reset()
            {
                Showing = false;
                previewFenceItems.Clear();
                Meshes.Clear();
            }

            /// <summary>
            /// Gets the meshes contained within the selected prefab for preview items.
            /// </summary>
            /// <param name="go"></param>
            internal void SetMesh(GameObject go)
            {
                MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();

                Meshes.Clear();
                
                foreach (var mf in meshFilters)
                {
                    Meshes.Add(new FenceSubMesh()
                    {
                        Mesh = mf.sharedMesh,
                        // If this item is a child mesh - we cannot use the offset position.
                        IsChildObject = go.transform != mf.transform,
                        Offset = mf.transform.localPosition
                    });
                }
            }

            /// <summary>
            /// Sets the terrain to use to the current object.
            /// </summary>
            /// <param name="t">The terrain to place the items on.</param>
            internal void SetTerrain(Terrain t) => this.t = t;
        }

        /// <summary>
        /// Stores the mesh and offset so that they can be drawn in preview.
        /// </summary>
        private class FenceSubMesh
        {
            internal Mesh Mesh;
            internal bool IsChildObject;
            internal Vector3 Offset;
        }

        internal void OnDrawGizmos()
        {
            if (PreviewSetup.Showing)
            {
                // Stores the color of the gizmos before drawing.
                var origColor = Gizmos.color;

                // Gets the preview items to show
                var previewItems = PreviewSetup.GetPreviewItems();
                counter = 0;

                foreach (var pfi in previewItems)
                {
                    // Alternate colors, red if cancelling.
                    Gizmos.color = previewItems.Count() == 1 ? ErrorFenceColor :
                        counter++ % 2 == 1 ? FencePreviewColor : FencePreviewAlternativeColor;

                    // Draw the preview meshes as wire meshes.
                    foreach (FenceSubMesh m in PreviewSetup.GetMeshes()) {
                        Gizmos.DrawWireMesh(
                            m.Mesh,
                            0,
                            m.IsChildObject ? 
                            pfi.GetPos() + (pfi.GetRot() * m.Offset)
                            : pfi.GetPos()
                            , pfi.GetRot()
                        );;
                    }
                }
                // Reset colors
                Gizmos.color = origColor;
            }
        }

        internal void HandleInput(Event e)
        {
            HandleStartFence(e);
            
            if (FenceStarted)
            {
                PreviewSetup.EndFencePosition = HelpMePlaceSystem.Instance.Helpers.GetMouseOnTerrainPosition(e);
                PreviewSetup.ReadyForPreview();

                // Change width between each fence item.
                IncreaseWidth(e);
                DecreaseWidth(e);
            }

            HandleEndFence(e);
        }

        /// <summary>
        /// Increases width between each fence item
        /// </summary>
        /// <param name="e"></param>
        internal void IncreaseWidth(Event e)
        {
            if (PreviewSetup.Showing && ((e.type == EventType.KeyDown && e.keyCode == HMPKeyBinds.FenceModeIncreaseWidth) || (e.type == EventType.MouseDown && e.button == 0)))
            {
                HelpMePlaceSystem.Instance.Selection.SetCurrentWidthOfObject(GAP_CHANGE_AMT * (e.shift ? SHIFT_HELD_MULTIPLIER : 1));
                PreviewSetup.Refresh();
            }
        }

        /// <summary>
        /// Decreases width between each fence item
        /// </summary>
        /// <param name="e"></param>
        internal void DecreaseWidth(Event e)
        {
            if (PreviewSetup.Showing && ((e.type == EventType.KeyDown && e.keyCode == HMPKeyBinds.FenceModeDecreaseWidth) || (e.type == EventType.MouseDown && e.button == 1)))
            {
                if(HelpMePlaceSystem.Instance.Selection.GetCurrentWidthOfObject() - (GAP_CHANGE_AMT * (e.shift ? SHIFT_HELD_MULTIPLIER : 1)) <= 0)
                {
                    Debug.Log($"Cannot decrease width between objects to below zero. You may only increase the gap distance. If the Gap amount you require is smaller than {GAP_CHANGE_AMT * (e.shift ? 2 : 1)} - you can update GAP_CHANGE_AMOUNT in HMPFencePlacement.cs");
                    return;
                }

                HelpMePlaceSystem.Instance.Selection.SetCurrentWidthOfObject(-GAP_CHANGE_AMT * (e.shift ? SHIFT_HELD_MULTIPLIER : 1));
                PreviewSetup.Refresh();
            }
        }

        /// <summary>
        /// Initialises a new fence starting item point & begins the preview process.
        /// </summary>
        /// <param name="e"></param>
        internal void HandleStartFence(Event e)
        {
            if (!FenceStarted && (e.type == EventType.KeyDown && e.keyCode == HMPKeyBinds.FenceMode))
            {
                // Get terrain under mouse.
                var collidersHit = Physics.RaycastAll(Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight)), 1000f, HelpMePlaceSystem.Instance.Configuration.GroundLayer);

                if (!collidersHit.Any()) return;

                foreach (var c in collidersHit)
                {
                    var t = c.transform.GetComponent<Terrain>();
                    if (t != null)
                    {
                        // Begin the process if all is well.
                        FenceStarted = true;
                        PreviewSetup.StartFencePosition = HelpMePlaceSystem.Instance.Helpers.GetMouseOnTerrainPosition(e);
                        PreviewSetup.SetMesh(HelpMePlaceSystem.Instance.Blueprint.GetCurrentBlueprintGameObject());
                        PreviewSetup.SetTerrain(t);
                        return;
                    }
                }

                // Only fences on terrain are currently supported.
                Debug.Log("No terrain detected. - Fence mode cannot be activated.");
                return;
            }

            // If the button is tapped and the showing is on but no terrain exists, reset the process. 
            if (PreviewSetup.Showing && (e.type == EventType.KeyDown && e.keyCode == HMPKeyBinds.FenceMode))
                PreviewSetup.Showing = false;
        }

        /// <summary>
        /// Places the items selected if any exists, or cancels the placement if none are valid
        /// Removes the preview
        /// </summary>
        /// <param name="e"></param>
        internal void HandleEndFence(Event e)
        {
            if (e.type == EventType.KeyUp && e.keyCode == HMPKeyBinds.FenceMode)
            {
                FenceStarted = false;

                if (!(PreviewSetup.GetPreviewItems().Count() <= 1)) 
                    PreviewSetup.Confirm();

                PreviewSetup.Reset();
            }
        }
#endif
    }
}