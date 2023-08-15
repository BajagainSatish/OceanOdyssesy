using System.Collections.Generic;
using UnityEngine;
using static HelpMePlace.HMPEnums;

namespace HelpMePlace
{
    /// <summary>
    /// Handles all reporting of errors / validation issues in Help Me Place.
    /// </summary>
    public class HMPValidator
    {
#if UNITY_EDITOR
        // Map of all things that can go wrong with HelpMePlace
        readonly Dictionary<Validations, string> ValidationMap = new Dictionary<Validations, string>()
    {
        { Validations.SelectedPrefabIsNull, "No selected gameobject / prefab could be found. Please reselect a prefab and ensure your Help Me Place prefab list settings are correct." },
        { Validations.BlueprintDoesNotExist, "No blueprint has been created. Re-select a prefab to place." },
        { Validations.FailedToSpawn, "Prefab / Gameobject failed to spawn due to some unknown reason. Execution may not continue." },
        { Validations.NoGroupsAdded, "You cannot enable this asset unless you have specified at least one Prefab Group." },
        { Validations.AllPrefabsRemoved, "All selectable gameobjects / prefabs have been removed from this group. You cannot enable this asset unless you have specified at least one Prefab Group with Prefabs." },
        { Validations.ResetPreventedNoPrefabGroups, "No available prefab groups available, disabling Help Me Place. Please ensure you have Prefab Groups and Gameobjects within them to use this asset." },
    };

        // Error Colors for the Prefix
        readonly Dictionary<ValidationSeverity, string> ValidationMessageColorMap = new Dictionary<ValidationSeverity, string>() {
        { ValidationSeverity.Info, "blue" }, { ValidationSeverity.Warning, "yellow" }, { ValidationSeverity.Error, "red" },
    };

        /// <summary>
        /// Posts a message to the console. 
        /// </summary>
        /// <param name="v">Validation message to show</param>
        /// <param name="sev">Severity of the Message</param>
        private void PostValidationMessage(Validations v, ValidationSeverity sev)
        {
            // If its not an error, show a Debug.Log
            if (sev != ValidationSeverity.Error)
                Debug.Log(GetValidationMessagePrefix(sev) + ValidationMap[v]);
            else
                // Otherwise use LogError
                Debug.LogError(GetValidationMessagePrefix(sev) + ValidationMap[v]);
        }

        /// <summary>
        /// Formats the prefix so that all messages will always report that they are from HelpMePlace system.
        /// </summary>
        /// <param name="sev">Severity of the Message</param>
        /// <returns>A formatted string of the header with the name of the product, colored by the ValidationSeverity map</returns>
        private string GetValidationMessagePrefix(ValidationSeverity sev) => $"<color={ ValidationMessageColorMap[sev] }>Help Me Place: </color>";

        /// <summary>
        /// Shows an info message in the console
        /// </summary>
        /// <param name="v">Validation message to show</param>
        internal void Info(Validations v) => PostValidationMessage(v, ValidationSeverity.Info);

        /// <summary>
        /// Shows a warning message in the console
        /// </summary>
        /// <param name="v"></param>
        internal void Warn(Validations v) => PostValidationMessage(v, ValidationSeverity.Warning);

        /// <summary>
        /// Shows an error message in the console.
        /// </summary>
        /// <param name="v"></param>
        internal void Error(Validations v) => PostValidationMessage(v, ValidationSeverity.Error);
#endif
    }
}