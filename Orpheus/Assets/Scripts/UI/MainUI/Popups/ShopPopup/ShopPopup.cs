using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TMP_Text refreshButtonCostText;
    [SerializeField] private List<TMP_Text> shopRelicCostTexts;
    [SerializeField] private List<Button> shopRelicCostButtons;
    [SerializeField] private List<Image> soldOutBanners;
    
    private int _numRelicRefreshes = 0;

    private List<RelicTypes> _currentRelics = new();

    private List<Relic3DVisual> _instantiatedRelicVisuals = new();

    private void Start()
    {
        PersistentState.Instance.OnRoundEnd -= OnRoundEnd;
        PersistentState.Instance.OnRoundEnd += OnRoundEnd;

        PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
        PersistentState.Instance.OnGoldValueChanged += OnGoldValueChanged;

        for (int i = 0; i < shopRelicCostButtons.Count; i++)
        {
            int temp = i;
            shopRelicCostButtons[i].onClick.AddListener(() =>
            {
                OnRelicPurchaseButtonClicked(temp);
            });
        }
    }
    
    private void OnDestroy()
    {
        if (PersistentState.IsAvailable)
        {
            PersistentState.Instance.OnRoundEnd -= OnRoundEnd;
            PersistentState.Instance.OnGoldValueChanged -= OnGoldValueChanged;
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
        refreshButtonCostText.SetText($"<sprite index=0>{refreshGoldCost}");
    }

    public void OnRefreshButtonClicked()
    {
        long refreshGoldCost = GameConstants.SHOP_REFRESH_GOLD_INITIAL_COST +
                               GameConstants.SHOP_REFRESH_GOLD_INCREASE * _numRelicRefreshes;
        if (PersistentState.Instance.CurrentGold >= refreshGoldCost)
        {
            PersistentState.Instance.ChangeCurrentGold(-refreshGoldCost);
            _numRelicRefreshes++;
            _currentRelics.Clear();
            PopulateShop();
        }
    }

    public void PopulateShop()
    {
        Clear();
        
        if (_currentRelics.Count == 0)
        {
            _currentRelics = RandomChanceSystem.Instance.GenerateRelicTypesInShop(relicIcons.Count, _numRelicRefreshes);
        }

        for (int i = 0; i < _currentRelics.Count; i++)
        {
            GameObject prefab = relicVisuals.GetVisualsPrefabForRelic(_currentRelics[i]);
            
            GameObject instance = Instantiate(prefab, relicPreviewTransforms[i]);
            
            _instantiatedRelicVisuals.Add(instance.GetComponent<Relic3DVisual>());

            relicIcons[i].Populate(relicRenderTextureUVs[i], _currentRelics[i]);

            bool ownsRelic = RelicSystem.Instance.HasRelic(_currentRelics[i]);
            soldOutBanners[i].gameObject.SetActive(ownsRelic);

            if (!ownsRelic)
            {
                shopRelicCostTexts[i].SetText($"<sprite index=0>{GameConstants.SHOP_RELIC_COST}");
                shopRelicCostButtons[i].interactable = true;
            }
            else
            {
                shopRelicCostTexts[i].SetText($"SOLD");
                shopRelicCostButtons[i].interactable = false;
            }
        }

        for (int i = _currentRelics.Count; i < relicIcons.Count; i++)
        {
            soldOutBanners[i].gameObject.SetActive(true);
        }

        OnGoldValueChanged(PersistentState.Instance.CurrentGold);
    }

    public void OnRelicPurchaseButtonClicked(int relicIndex)
    {
        long relicGoldCost = GameConstants.SHOP_RELIC_COST;

        if (PersistentState.Instance.CurrentGold >= relicGoldCost && !RelicSystem.Instance.HasRelic(_currentRelics[relicIndex]))
        {
            RelicSystem.Instance.AddRelic(_currentRelics[relicIndex]);
            PersistentState.Instance.ChangeCurrentGold(-relicGoldCost);
            shopRelicCostTexts[relicIndex].SetText($"SOLD");
            shopRelicCostButtons[relicIndex].interactable = false;
            soldOutBanners[relicIndex].gameObject.SetActive(true);
        }
    }

    private void Clear()
    {
        foreach (Relic3DVisual visual in _instantiatedRelicVisuals)
        {
            Destroy(visual.gameObject);
        }
        
        _instantiatedRelicVisuals.Clear();
    }

    public void OnBackButtonClicked()
    {
        UIPopupSystem.Instance.HidePopup("ShopPopup");
    }
}
