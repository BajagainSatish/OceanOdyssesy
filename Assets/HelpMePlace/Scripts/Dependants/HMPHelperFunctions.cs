using System;
using UnityEngine;
using UnityEditor;

namespace HelpMePlace
{
    internal class HMPHelperFunctions
    {
#if UNITY_EDITOR
        /// <summary>
        /// Utility to get mouse contact point on the chosen colliders layer.
        /// </summary>
        /// <param name="e">The current event of user</param>
        /// <returns>Vector3.zero if there is no contact point, otherwise, the contact point against the chosen colliders layer.</returns>
        internal Vector3 GetMouseOnTerrainPosition(Event e)
        {
            //  Ensure the camera has been initialised.
            if (Camera.current == null) 
                return Vector3.zero;

            if (Physics.Raycast(
                    Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x * EditorGUIUtility.pixelsPerPoint, 
                    -e.mousePosition.y * EditorGUIUtility.pixelsPerPoint + Camera.current.pixelHeight)),
                    out RaycastHit screenPointPos,
                    1000f,
                    HelpMePlaceSystem.Instance.Configuration.GroundLayer
                )) {

                Vector3 newOrigin = screenPointPos.point + (screenPointPos.normal * 1.5f);
                Vector3 direction = newOrigin - screenPointPos.point;

                if (Physics.Raycast(
                        newOrigin,
                        // new Vector3(screenPointPos.point.x, screenPointPos.point.y + 1, screenPointPos.point.z),
                        -direction,
                        out RaycastHit hitToGround,
                        1000f,
                        HelpMePlaceSystem.Instance.Configuration.GroundLayer
                    )) {

                    // Debug.DrawLine(Vector3.zero, hitToGround.point, Color.cyan, 0.5f);

                    return hitToGround.point;
                }
            }
            
            return Vector3.zero;
        }

        internal void DrawSkyline(Vector3 v, Color c) => Debug.DrawLine(v, v + (Vector3.up * 100), c, 1);

        /// <summary>
        /// Is the shift button being held
        /// </summary>
        /// <param name="e">The current event (Key press / etc)</param>
        /// <returns>True if the Shift button is held.</returns>
        internal bool IsShiftHeld(Event e) => e.shift;

        /// <summary>
        /// Used by the blueprint system to update sink amount. Gets a vector on the terrain from above the blueprint position.
        /// </summary>
        /// <param name="pos">The blueprint current position.</param>
        /// <returns>The position if found, else, vector3.zero.</returns>
        internal Vector3 GetPositionOnTerrainFromAbove(Vector3 pos)
        {
            if (Physics.Raycast(
                       pos + new Vector3(0, pos.y + 100, 0),
                       Vector3.down,
                       out RaycastHit rch,
                       150f,
                       HelpMePlaceSystem.Instance.Configuration.GroundLayer
            ))
                return rch.point;
            else
                return Vector3.zero;
        }

        internal Vector3 GetNormalOfGroundAtPosition(Event e)
        {
            //  Ensure the camera has been initialised.
            if (Camera.current == null)
                return Vector3.zero;

            if (Physics.Raycast(
                    Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x * EditorGUIUtility.pixelsPerPoint, -e.mousePosition.y * 
                    EditorGUIUtility.pixelsPerPoint + Camera.current.pixelHeight)),
                    out RaycastHit result,
                    1000,
                    HelpMePlaceSystem.Instance.Configuration.GroundLayer
                )
            )
                return result.normal;
            else
                return Vector3.zero;
        }


        internal Vector3 GetNormalOfGroundAtPosition(Vector3 e)
        {
            //  Ensure the camera has been initialised.
            if (Camera.current == null)
                return Vector3.zero;

            // Does not support spherical
            if (Physics.Raycast(
                    e + (Vector3.up * 100), 
                    Vector3.down,
                    out RaycastHit result,
                    120,
                    HelpMePlaceSystem.Instance.Configuration.GroundLayer
                )
            )
                return result.normal;
            else
                return Vector3.zero;
        }
#endif
    }
}