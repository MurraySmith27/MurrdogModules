using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicIcon : MonoBehaviour
{
    [SerializeField] private Image relicImage;

    [SerializeField] private RelicVisualsSO relicVisuals;

    public void SetIcon(RelicTypes relicType)
    {
        relicImage.sprite = relicVisuals.GetIconForRelic(relicType);
    }
}
