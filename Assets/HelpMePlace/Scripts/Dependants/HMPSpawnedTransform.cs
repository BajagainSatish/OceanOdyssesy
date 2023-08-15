using UnityEngine;
using static HelpMePlace.HMPEnums;
using static HelpMePlace.HMPConstants;

namespace HelpMePlace
{
    /// <summary>
    /// All active transforms after they are spawned will be created using this structure. 
    /// </summary>
    internal class HMPSpawnedTransform
    {
#if UNITY_EDITOR
        // The state of the transform
        private SpawnedTransformState state;

        // The gameobject that was created
        internal GameObject SpawnedGo;

        // The position to end the animation at.
        private Vector3 EndPosition;

        // The rotation to end the animation at.
        private Quaternion EndRotation;

        // The scale of the item to end at.
        private Vector3 EndScale;

        /// <summary>
        /// Creates a spawned transform in the system that will be animated to its desired position.
        /// </summary>
        /// <param name="spawned">The prefab that was created.</param>
        /// <param name="endPosition">The end position of the animation</param>
        /// <param name="endRotation">The end rotation of the animation</param>
        /// <param name="endScale">The end scale of the animation</param>
        public HMPSpawnedTransform(GameObject spawned, Vector3 endPosition, Quaternion endRotation, Vector3 endScale)
        {
            // Cache the spawned item
            SpawnedGo = spawned;

            // Remove the word 'Clone' from the name
            SpawnedGo.name = SpawnedGo.name.Replace("(Clone)", "");

            if (HelpMePlaceSystem.Instance.HMPSystem.AnimatePlacement)
            {
                if (HelpMePlaceSystem.Instance.PrefabGroups[HelpMePlaceSystem.Instance.Selection.ColumnIndex].RotationToNormalEnabled)
                    SpawnedGo.transform.position = endPosition + (HelpMePlaceSystem.Instance.Blueprint.GetCurrentBlueprintGameObject().transform.up * (SPAWN_HEIGHT_FOR_ANIM_MODIFIER * 2));
                else
                    // Float the transform above the desired.
                    SpawnedGo.transform.position = endPosition + new Vector3(0, GetHeightOfAllGameobjectsWithinParent(SpawnedGo.transform) * SPAWN_HEIGHT_FOR_ANIM_MODIFIER, 0);

                // Store the end pos/rot.
                EndPosition = endPosition;
                EndRotation = endRotation;
                EndScale = endScale;
                // Initial state.
                state = SpawnedTransformState.Spawned;
            } else
            {
                // Float the transform above the desired.
                SpawnedGo.transform.position = endPosition;
                SpawnedGo.transform.rotation = endRotation;
                SpawnedGo.transform.localScale = endScale;

                // Initial state.
                state = SpawnedTransformState.Finished;
            }

            // Make the prefab static if desired by the user. 
            // Static is for batching at runtime, not in editor, so moving a static transform in the editor is totally fine. 
            SpawnedGo.isStatic = HelpMePlaceSystem.Instance.Configuration.MakePrefabStatic || SpawnedGo.isStatic;

            HelpMePlaceSystem.Instance.EventHooks.InvokePrefabPlaceStarted(SpawnedGo);
        }

        /// <summary>
        /// Gets bounds of all children renderers within a GameObject heirarchy.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        float GetHeightOfAllGameobjectsWithinParent(Transform parent)
        {
            Bounds bounds = new Bounds(parent.position, Vector3.one);
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
            
            // Encapsulate any renderers in the children to within the bounds of the initial. 
            foreach (Renderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            return bounds.size.y;
        }

        /// <summary>
        /// Sink the transform to its desired position.
        /// </summary>
        public void SinkToPos()
        {
            // If something has gone wrong at this stage, and we lost the cached item, error it out so that it is no longer used. 
            if (SpawnedGo == null)
            {
                HelpMePlaceSystem.Instance.EventHooks.InvokePrefabPlaceError();
                state = SpawnedTransformState.Error;
            }
            else
            {
                // Lerp the position / rotation of the spawned item.
                SpawnedGo.transform.position = Vector3.LerpUnclamped(SpawnedGo.transform.position, EndPosition, PREFAB_PLACED_SINK_SPEED);
                SpawnedGo.transform.rotation = Quaternion.LerpUnclamped(SpawnedGo.transform.rotation, EndRotation, PREFAB_PLACED_SINK_SPEED);
                SpawnedGo.transform.localScale = Vector3.LerpUnclamped(SpawnedGo.transform.localScale, EndScale, PREFAB_PLACED_SINK_SPEED);
            }
        }

        // Update loop for the spawned transform. 
        public void Update()
        {
            switch (state)
            {
                // Based on the state of the spawned transform, apply these rules to it.
                case SpawnedTransformState.Spawned:
                    // If just spawned, begin sinking.
                    state = SpawnedTransformState.Sinking;
                    break;
                case SpawnedTransformState.Sinking:
                    // Sink the transform to its desired end position.
                    SinkToPos();
                    if (SpawnedGo != null)
                    {
                        if (SpawnedGo.transform.position == EndPosition && SpawnedGo.transform.rotation == EndRotation)
                            // If the sinking is finished, transition to Settling.
                            state = SpawnedTransformState.Settling;
                    } 
                    else
                        // If undo is hit, the transform will now be nulled out. So set the object state to Unspawned for the core system to handle. 
                        state = SpawnedTransformState.Unspawned;
                    break;
                case SpawnedTransformState.Settling:
                    // The animation is finished, so ensure its static and flag it for removal.
                    if (SpawnedGo != null)
                    {
                        SpawnedGo.isStatic = HelpMePlaceSystem.Instance.Configuration.MakePrefabStatic || SpawnedGo.isStatic;
                        state = SpawnedTransformState.Finished;
                    } 
                    else 
                        state = SpawnedTransformState.Unspawned;
                    break;
                case SpawnedTransformState.Finished:
                    // Nothing to do now. Cleanup will be handled by the core.
                    return;
                case SpawnedTransformState.Error:
                    // Show an error to the user to notify that something has happened. 
                    // This is usually caused by undo / alt+tabbing / remote refresh / 3rd party software influence or incompatability with other assets. 
                    // Basically, the core system was trying to update this SpawnedTransform but it "lost" it after it was added to the list. 
                    // Generally speaking - nothing to worry about.
                    Debug.Log("<color=red>Error with spawned transform.</color>");
                    return;
                case SpawnedTransformState.Unspawned:
                    // Undo was pressed. Nothing to do here, clean up will be handled by core. 
                    return;
            }
        }

        /// <summary>
        /// Any Spawned transforms that should no longer by actioned by the core system will report True from this method. 
        /// </summary>
        /// <returns>True if the state of this Spawned transform shows that updates are no longer nescessary.</returns>
        internal bool ActionNotRequired() => 
            state == SpawnedTransformState.Finished || 
            state == SpawnedTransformState.Unspawned || 
            state == SpawnedTransformState.Error;
#endif
    }
}
