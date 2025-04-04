using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CitizenVisualsData", menuName = "Orpheus/Citizen Visuals Data", order = 1)]
public class CitizenVisualsSO : ScriptableObject
{
    public List<CitizenVisualsData> CitizenVisualsData = new List<CitizenVisualsData>();
}

[Serializable]
public class CitizenVisualsData
{
    public CitizenBehaviour Prefab;
}
