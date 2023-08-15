using UnityEngine;
using UnityEngine.Events;

namespace HelpMePlace
{
    /// <summary>
    /// Editor event can be listened to for tracking and using with your custom code.
    /// </summary>
    [System.Serializable]
    public class HMPEditorEventHooks
    {
        #region Activation
        /// <summary> Delegate that fires when the on enable of the HelpMePlace has finished. </summary>
        public delegate void OnHMPSystemActivated();

        /// <summary> This event fires when the on enable of the system has finished. </summary>
        public event OnHMPSystemActivated HMPSystemActivated;

        /// <summary> This event fires when the on enable of the system has finished. </summary>
        public UnityEvent HMPSystemActivatedUE;

        /// <summary> Internal use firing of the event. </summary>
        internal void InvokeHMPSystemActivated()
        {
            HMPSystemActivated?.Invoke();
            HMPSystemActivatedUE?.Invoke();
        }

        /// <summary> Delegate that fires when the on disable of the HelpMePlace has finished. </summary>
        public delegate void OnHMPSystemDeactivated();

        /// <summary> This event fires when the on disable of the system has finished. </summary>
        public event OnHMPSystemDeactivated HMPSystemDeactivated;

        /// <summary> This event fires when the on disable of the system has finished. </summary>
        public UnityEvent HMPSystemDeactivatedUE;

        /// <summary> Internal use firing of the event. </summary>
        internal void InvokeHMPSystemDeactivated()
        {
            HMPSystemDeactivated?.Invoke();
            HMPSystemDeactivatedUE?.Invoke();
        }
        #endregion

        #region Prefab lifecycle events
        /// <summary> Delegate that fires when the spawning item has been created. </summary>
        public delegate void OnStartPrefabPlacing(GameObject go);

        /// <summary> Event that fires when the spawning item has been created. </summary>
        public event OnStartPrefabPlacing PrefabPlaceStarted;
        /// <summary> Event that fires when the spawning item has been created. </summary>
        public UnityEvent PrefabPlaceStartedUE;

        /// <summary> Internal use firing of the event. </summary>
        internal void InvokePrefabPlaceStarted(GameObject go)
        {
            PrefabPlaceStarted?.Invoke(go);
            PrefabPlaceStartedUE?.Invoke();
        }
        
        /// <summary> Delegate that fires once the prefab has reached its desired location. </summary>
        public delegate void OnEndPrefabPlacing(GameObject go);

        /// <summary> Event that fires once the prefab has reached its desired location. </summary>
        public event OnEndPrefabPlacing PrefabPlaceFinished;
        /// <summary> Event that fires once the prefab has reached its desired location. </summary>
        public UnityEvent PrefabPlaceFinishedUE;

        /// <summary> Internal use firing of the event. </summary>
        internal void InvokePrefabPlaceFinished(GameObject go)
        {
            PrefabPlaceFinished?.Invoke(go);
            PrefabPlaceFinishedUE?.Invoke();
        }

        /// <summary> Delegate that fires once the prefab has had a problem during placement. </summary>
        public delegate void OnErrorPrefabPlacing();

        /// <summary> Event that fires once the prefab has had a problem during placement. </summary>
        public event OnErrorPrefabPlacing PrefabPlaceError;
        /// <summary> Event that fires once the prefab has had a problem during placement. </summary>
        public UnityEvent PrefabPlaceErrorUE;

        /// <summary> Internal use firing of the event. </summary>
        internal void InvokePrefabPlaceError()
        {
            PrefabPlaceError?.Invoke();
            PrefabPlaceErrorUE?.Invoke();
        }
        #endregion

        #region Selection
        /// <summary> Delegate that fires whenever the selected prefab changes. </summary>
        public delegate void OnCurrentPrefabChanged();

        /// <summary> Event that fires whenever the selected prefab changes. </summary>
        public event OnCurrentPrefabChanged PrefabChange;
        /// <summary> Event that fires whenever the selected prefab changes. </summary>
        public UnityEvent PrefabChangeUE;

        /// <summary> Internal use firing of the event. </summary>
        internal void InvokePrefabChange()
        {
            PrefabChange?.Invoke();
            PrefabChangeUE?.Invoke();
        }

        /// <summary> Delegate that fires whenever the selected prefab group changes. </summary>
        public delegate void OnCurrentPrefabGroupChanged();

        /// <summary> Event that fires whenever the selected prefab group changes. </summary>
        public event OnCurrentPrefabGroupChanged PrefabGroupChange;
        /// <summary> Event that fires whenever the selected prefab group changes. </summary>
        public UnityEvent PrefabGroupChangeUE;

        /// <summary> Internal use firing of the event. </summary>
        internal void InvokePrefabGroupChange()
        {
            PrefabGroupChange?.Invoke();
            PrefabGroupChangeUE?.Invoke();
        }
        #endregion
    }
}
