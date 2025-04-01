using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    [SerializeField] private List<RelicIcon> relicIcons;
    [SerializeField] private List<Transform> relicPreviewTransforms;
    [SerializeField] private List<Rect> relicRenderTextureUVs;

    [SerializeField] private RelicVisualsSO relicVisuals;


    [Header("UI Elements")] 
    [SerializeField] private Button refreshButton;
    
    private int _numRelicRefreshes = 0;

    private List<Relic3DVisual> _instantiatedRelicVisuals = new();

    private void Start()
    {
        RoundState.Instance.OnRoundEnd -= OnRoundEnd;
        RoundState.Instance.OnRoundEnd += OnRoundEnd;

        RoundState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        RoundState.Instance.OnGoldValueChanged += OnGoldValueChanged;
    }
    
    private void OnDestroy()
    {
        if (RoundState.IsAvailable)
        {
            RoundState.Instance.OnRoundEnd -= OnRoundEnd;
            RoundState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        }
    }
    
    private void OnEnable()
    {
        PopulateShop();
    }

    private void OnRoundEnd(int roundNum)
    {
        _numRelicRefreshes = 0;
    }

    private void OnGoldValueChanged(long newValue)
    {
        long refreshGoldCost = GameConstants.SHOP_REFRESH_GOLD_INITIAL_COST +
                               GameConstants.SHOP_REFRESH_GOLD_INCREASE * _numRelicRefreshes;
        refreshButton.interactable = (newValue >= refreshGoldCost);
    }

    public void OnRefreshButtonClicked()
    {
        long refreshGoldCost = GameConstants.SHOP_REFRESH_GOLD_INITIAL_COST +
                               GameConstants.SHOP_REFRESH_GOLD_INCREASE * _numRelicRefreshes;
        if (RoundState.Instance.CurrentGold >= refreshGoldCost)
        {
            RoundState.Instance.ChangeCurrentGold(-refreshGoldCost);
            _numRelicRefreshes++;
            PopulateShop();
        }
    }

    public void PopulateShop()
    {
        Clear();
        
        List<RelicTypes> currentRelicTypes = RandomChanceSystem.Instance.GenerateRelicTypesInShop(relicIcons.Count, _numRelicRefreshes);

        for (int i = 0; i < currentRelicTypes.Count; i++)
        {
            GameObject prefab = relicVisuals.GetVisualsPrefabForRelic(currentRelicTypes[i]);

            GameObject instance = Instantiate(prefab, relicPreviewTransforms[i]);
            
            _instantiatedRelicVisuals.Add(instance.GetComponentInChildren<Relic3DVisual>());
            
            relicIcons[i].Populate(relicRenderTextureUVs[i], currentRelicTypes[i]);
        }
    }

    private void Clear()
    {
        foreach (Relic3DVisual visual in _instantiatedRelicVisuals)
        {
            Destroy(visual);
        }
        
        _instantiatedRelicVisuals.Clear();
    }

    public void OnBackButtonClicked()
    {
        UIPopupSystem.Instance.HidePopup("ShopPopup");
    }
}
