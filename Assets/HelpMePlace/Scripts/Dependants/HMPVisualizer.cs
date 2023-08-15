using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static HelpMePlace.HMPEnums;
using static HelpMePlace.HMPConstants;
using System.Linq;

namespace HelpMePlace
{
    [Serializable]
    public class HMPVisualizer
    {
#if UNITY_EDITOR
        [SerializeField]
        public int VisualiserShapeSize = HMP_VISUALIZER_SHAPE_SIZE_DEFAULT;

        [SerializeField]
        public VisualizerShape VisualizerShape = HMP_VISUALIZER_SHAPE_DEFAULT;

        [SerializeField]
        public bool AutoRefreshEnabled = false;

        [HideInInspector]
        public bool VisualizerEnabled = false;

        float timeToRefreshCache;
        private List<(int, string, List<Vector3>, Color)> listMap;

        void RefreshItemCache()
        {
            if (HelpMePlaceSystem.Instance == null)
            {
                Debug.Log("Instance null..");
                return;
            }

            timeToRefreshCache = Time.unscaledTime + 5f;
            int i = 0;
            listMap = new List<(int, string, List<Vector3>, Color)>();
            List<Vector3> cache;

            GameObject hmpHolder = GameObject.Find(HMP_HOLDER_DEFAULT_NAME);
            if (hmpHolder == null)
            {
                Debug.LogError("<color=blue>Help Me Place</color> Visualiser could not find any spawned prefabs. The visualiser can only show you prefabs that have been spawned into the default HMP holder. If you have overwritten any desired parents, these cannot be used by the visualiser.");
                VisualizerEnabled = false;
                return;
            }

            foreach (Transform t in hmpHolder.transform)
            {
                cache = new List<Vector3>();

                foreach (Transform t2 in t)
                    cache.Add(t2.position);

                var group = HelpMePlaceSystem.Instance.PrefabGroups.FirstOrDefault(o => o.GroupName == t.name.Replace(HMP_GROUP_HOLDER_DEFAULT_NAME_PREFIX, ""));

                Color c = group?.Color ?? UnityEngine.Random.ColorHSV();

                // Debug.Log($"Listmap Adding: index {i} name {group?.GroupName} numberOfitems:{cache} color {c}");
                listMap.Add((i, group?.GroupName ?? "NOT FOUND GROUP", cache, c));
                i++;
            }
            return;
            foreach (ValueTuple<int, string, List<Vector3>, Color> grp in listMap)
            {
                Debug.Log($"Cache contents: index {grp.Item1} name {grp.Item2} numberOfitems:{grp.Item3.Count()} color {grp.Item4}");
            }
        }

        internal void Draw()
        {
            if ((AutoRefreshEnabled && Time.unscaledTime > timeToRefreshCache) || (listMap == null || listMap.Count == 0))
                RefreshItemCache();

            if (listMap != null)
            {
                Color OrigonalGizmoColor = Gizmos.color;
                foreach (ValueTuple<int, string, List<Vector3>, Color> grp in listMap)
                {
                    foreach (Vector3 hpp in grp.Item3)
                    {
                        Gizmos.color = grp.Item4;

                        switch (VisualizerShape)
                        {
                            case VisualizerShape.Cube:
                                Gizmos.DrawCube(hpp, Vector3.one * VisualiserShapeSize);
                                break;
                            case VisualizerShape.Sphere:
                                Gizmos.DrawSphere(hpp, VisualiserShapeSize);
                                break;
                            case VisualizerShape.WireSphere:
                                Gizmos.DrawWireSphere(hpp, VisualiserShapeSize);
                                break;
                            case VisualizerShape.WireCube:
                                Gizmos.DrawWireCube(hpp, Vector3.one * VisualiserShapeSize);
                                break;
                            case VisualizerShape.SkyLine:
                                Gizmos.DrawLine(hpp, hpp + (Vector3.up * VisualiserShapeSize));
                                break;
                        }
                    }
                }
                Gizmos.color = OrigonalGizmoColor;
            }
        }
#endif
    }
}