using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TextController : MonoBehaviour
{
    [SerializeField] private bool adjustTextSize = true;

    [SerializeField] private bool createTooltips = true;

    [SerializeField] private bool isTextTooltip = false;

    [SerializeField] private RectTransform textRectTransform;

    private TMP_Text _textMesh;
    
    private int _currentlyDisplayingTooltipId = -1;
    
    void Awake()
    {
        _textMesh = GetComponent<TMP_Text>();
        
        if (adjustTextSize)
        {

            if (_textMesh == null)
            {
                Debug.LogError(
                    $"Text mesh is not available on an object with a TextSizeController! object: {GameObjectUtils.GetHeirarchyPath(gameObject)}");
                return;
            }
            else
            {
                _textMesh.fontSize *= GlobalSettings.TextSizeMultiplier;
            }
        }
    }

    private void Update()
    {
        if (createTooltips)
        {
            if (UIMouseData.Instance.IsMouseOverRectTransform(textRectTransform))
            {
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMesh, Input.mousePosition, null);
                if (_currentlyDisplayingTooltipId == -1)
                {
                    if (linkIndex != -1)
                    {
                        TMP_LinkInfo linkInfo = _textMesh.textInfo.linkInfo[linkIndex];

                        if (isTextTooltip)
                        {
                            _currentlyDisplayingTooltipId = TooltipManager.Instance.ShowTooltipChildByTooltipId(
                                Input.mousePosition,
                                linkInfo.GetLinkID(),
                                () =>
                                {
                                    _currentlyDisplayingTooltipId = -1;
                                });
                        }
                        else
                        {
                            _currentlyDisplayingTooltipId = TooltipManager.Instance.ShowTooltipById(
                                Input.mousePosition,
                                linkInfo.GetLinkID(),
                                () =>
                                {
                                    _currentlyDisplayingTooltipId = -1;
                                });
                        }
                    }
                }
            }
            else if (_currentlyDisplayingTooltipId != -1)
            {
                TooltipManager.Instance.HideTooltipIfMouseOff(_currentlyDisplayingTooltipId);
            }
        }
    }
}
