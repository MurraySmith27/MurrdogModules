using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TextController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool adjustTextSize = true;

    [SerializeField] private bool createTooltips = true;

    [SerializeField] private bool isTextTooltip = false;

    [SerializeField] private RectTransform textRectTransform;

    [SerializeField] private Camera overrideCamera;
    
    private TMP_Text _textMesh;

    private bool _mouseCurrentlyInText;

    private Camera _camera;
    
    private int _currentlyDisplayingTooltipId = -1;
    
    void Awake()
    {
        _textMesh = GetComponent<TMP_Text>();

        if (overrideCamera != null)
        {
            _camera = overrideCamera;
        }
        else _camera = Camera.main;
        
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseCurrentlyInText = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseCurrentlyInText = false;
    }

    private void Update()
    {
        Debug.LogError($"currently in text: {_mouseCurrentlyInText}");
        if (_mouseCurrentlyInText && UIMouseData.Instance.IsMouseOverRectTransform(textRectTransform))
        {
            Debug.LogError($"2");
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMesh, Input.mousePosition, _camera);
            if (_currentlyDisplayingTooltipId == -1)
            {
                Debug.LogError($"3");
                if (linkIndex != -1)
                {
                    TMP_LinkInfo linkInfo = _textMesh.textInfo.linkInfo[linkIndex];
                    Debug.LogError($"link: {linkInfo.GetLinkID()}");

                    _currentlyDisplayingTooltipId = TooltipManager.Instance.ShowTooltipById(
                        Input.mousePosition,
                        linkInfo.GetLinkID(),
                        () => { _currentlyDisplayingTooltipId = -1; },
                        isTextTooltip);
                }
            }
            else if (linkIndex == -1)
            {
                TooltipManager.Instance.HideTooltipIfMouseOff(_currentlyDisplayingTooltipId);
            }
        }
    }
}
