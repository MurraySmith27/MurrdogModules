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
    [Tooltip("(optional), if prefab is null, The instance of the object in the scene, must have a UIPopupComponent attached to the prefab root")]
    public GameObject gameObjectInstance;
    [Tooltip("The name of the inputs that show this popup")]
    public List<UIInputType> showPopupInputs;
    [Tooltip("The name of the inputs that hide this popup")]
    public List<UIInputType> hidePopupInputs;
    [Tooltip("Popup will not show up in any of the scenes with names provided here")]
    public List<string> excludedSceneNames;
}
