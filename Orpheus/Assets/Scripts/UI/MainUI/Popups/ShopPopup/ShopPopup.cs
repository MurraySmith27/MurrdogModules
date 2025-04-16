using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    [Header("Relic Icons")]
    [SerializeField] private List<RelicIcon> relicIcons;
    [SerializeField] private RelicVisualsSO relicVisuals;
    
    [Header("Item Icons")]
    [SerializeField] private List<ItemIcon> itemIcons;
    [SerializeField] private ItemVisualsSO itemVisuals;
    [SerializeField] private List<Image> itemSoldOutBanners;
    
    //TODO: Generate 3d previews for booster packs
    [Header("Booster Pack Icons")]
    [SerializeField] private List<Image> boosterPackSoldOutBanners;

    
    [Header("UI Elements")] 
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Text refreshButtonCostText;
    [SerializeField] private List<TMP_Text> shopRelicCostTexts;
    [SerializeField] private List<Button> shopRelicCostButtons;
    [SerializeField] private List<Image> soldOutBanners;

    [SerializeField] private TMP_Text citizenCostText;
    [SerializeField] private TMP_Text boosterPackCostText;
    
    private int _numRelicRefreshes = 0;

    private List<RelicTypes> _currentRelics = new();

    private List<Preview3DController.PreviewTransform> _currentPreviewTransforms = new();

    private List<Icon3DVisual> _instantiatedRelicVisuals = new();

    private List<Icon3DVisual> _instantiatedItemVisuals = new();

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

            Preview3DController.PreviewTransform previewTransform;
            if (!Preview3DController.Instance.GetPreviewTransform(out previewTransform))
            {
                Debug.LogError("Tried to request a 3d preview transform, but none are available!");
                return;
            }
            
            _currentPreviewTransforms.Add(previewTransform);
            
            GameObject instance = Instantiate(prefab, previewTransform.Transform);
            
            _instantiatedRelicVisuals.Add(instance.GetComponent<Icon3DVisual>());

            relicIcons[i].Populate(previewTransform.UVRect, _currentRelics[i]);

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

        GameObject itemPrefabs = itemVisuals.GetVisualsPrefabForItem(ItemTypes.BONUS_CITIZEN);
        
        Preview3DController.PreviewTransform bonusCitizenPreviewTransform;
        if (!Preview3DController.Instance.GetPreviewTransform(out bonusCitizenPreviewTransform))
        {
            Debug.LogError("Tried to request a 3d preview transform, but none are available!");
            return;
        }
        
        _currentPreviewTransforms.Add(bonusCitizenPreviewTransform);
        
        GameObject itemInstance = Instantiate(itemPrefabs, bonusCitizenPreviewTransform.Transform);
        
        _instantiatedItemVisuals.Add(itemInstance.GetComponent<Icon3DVisual>());

        itemIcons[0].Populate(bonusCitizenPreviewTransform.UVRect, ItemTypes.BONUS_CITIZEN);
        
        citizenCostText.SetText($"<sprite index=0>{ShopUtils.GetCostOfItem(ItemTypes.BONUS_CITIZEN)}");
        
        citizenCostText.SetText($"<sprite index=0>{ShopUtils.GetCostOfBoosterPack(BoosterPackTypes.BASIC_TILE_BOOSTER)}");
        
        shopRelicCostButtons[0].interactable = true;

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

    public void OnBonusCitizenPurchaseButtonClicked()
    {
        if (!ItemSystem.Instance.HasItem(ItemTypes.BONUS_CITIZEN))
        {
            long bonusCitizenCost = ShopUtils.GetCostOfItem(ItemTypes.BONUS_CITIZEN);

            if (PersistentState.Instance.CurrentGold >= bonusCitizenCost)
            {
                ItemSystem.Instance.AddItem(ItemTypes.BONUS_CITIZEN);
                PersistentState.Instance.ChangeCurrentGold(-bonusCitizenCost);

                itemSoldOutBanners[0].gameObject.SetActive(true);
            }
        }
    }

    public void OnBoosterPackPurchaseButtonClicked()
    {
        long boosterPackCost = ShopUtils.GetCostOfBoosterPack(BoosterPackTypes.BASIC_TILE_BOOSTER);
        
        if (PersistentState.Instance.CurrentGold >= boosterPackCost)
        {
            BoosterPackSystem.Instance.OpenBoosterPack(BoosterPackTypes.BASIC_TILE_BOOSTER);
            PersistentState.Instance.ChangeCurrentGold(-boosterPackCost);
            
            boosterPackSoldOutBanners[0].gameObject.SetActive(true);
        }
        
    }

    private void Clear()
    {
        foreach (Icon3DVisual visual in _instantiatedRelicVisuals)
        {
            Destroy(visual.gameObject);
        }
        
        _instantiatedRelicVisuals.Clear();

        foreach (Icon3DVisual visual in _instantiatedItemVisuals)
        {
            Destroy(visual.gameObject);
        }
        
        _instantiatedItemVisuals.Clear();

        foreach (Preview3DController.PreviewTransform previewTransform in _currentPreviewTransforms)
        {
            Preview3DController.Instance.FreePreviewTransform(previewTransform);
        }
        
        _currentPreviewTransforms.Clear();
    }

    public void OnBackButtonClicked()
    {
        Close();
    }

    public void Close()
    {
        UIPopupSystem.Instance.HidePopup("ShopPopup");
    }
}
