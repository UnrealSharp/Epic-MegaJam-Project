using ManagedGameJamProject.Shop;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.UnrealSharpCore;

namespace ManagedGameJamProject.Subsystems;

[UClass]
public partial class UShopManager : UCSWorldSubsystem
{
    public override bool DoesSupportWorldType(ECSWorldType worldType)
    {
        return worldType == ECSWorldType.Game || worldType == ECSWorldType.PIE;
    }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    public partial IList<UShopItemAsset> AvailableShopItems { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly)]
    partial IDictionary<UShopItemAsset, int> QuantityByItem { get; set; }
    
    [UProperty]
    partial UStoredGooManager StoredGooManager { get; set; }

    public override void PostInitialize()
    {
        base.PostInitialize();
        LoadShopItems();
        
        StoredGooManager = GetWorldSubsystem<UStoredGooManager>();
    }

    async void LoadShopItems()
    {
        try
        {
            UAssetManager assetManager = UAssetManager.Get();
            IList<UShopItemAsset> shopItems = await assetManager.LoadPrimaryAssets<UShopItemAsset>(AssetTypes.ShopItem.PrimaryAssetList);
        
            for (int i = 0; i < shopItems.Count; i++)
            {
                AvailableShopItems.Add(shopItems[i]);
            
                if (shopItems[i].HasQuantity)
                {
                    QuantityByItem[shopItems[i]] = shopItems[i].Quantity;
                }
            }
        }
        catch (Exception e)
        {
            LogBK.LogError($"Failed to load shop items: {e.Message}");
        }
    }
    
    public bool GetItemQuantity(UShopItemAsset item, out int quantity)
    {
        if (QuantityByItem.TryGetValue(item, out int value))
        {
            quantity = value;
            return true;
        }

        quantity = -1;
        return false;
    }
    
    public bool PurchaseItem(UShopItemAsset item)
    {
        if (!CanPurchaseItem(item))
        {
            return false;
        }
        
        if (item.HasQuantity)
        {
            if (QuantityByItem.TryGetValue(item, out int quantity))
            {
                QuantityByItem[item] = quantity - 1;
            }
        }
        
        StoredGooManager.DeductSlime(item.Price);
        return true;
    }
    
    public bool CanPurchaseItem(UShopItemAsset item)
    {
        if (item.HasQuantity)
        {
            if (QuantityByItem.TryGetValue(item, out int quantity) && quantity <= 0)
            {
                return false;
            }
        }
        
        float totalGoo = StoredGooManager.GetTotalStoredGooAmount();
        return totalGoo >= item.Price;
    }
    
}