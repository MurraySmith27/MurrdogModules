using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIPopup
{
    [Tooltip("The id of the popup")]
    public string id;
    [Tooltip("The popup prefab, must have a UIPopupComponent attached to the prefab root")]
    public GameObject prefab;
    [Tooltip("The name of the input action that shows this popup")]
    public string showPopupActionName;
    [Tooltip("The name of the input action that hides this popup")]
    public string hidePopupActionName;
    [Tooltip("Popup will not show up in any of the scenes with names provided here")]
    public List<string> excludedSceneNames;
}
