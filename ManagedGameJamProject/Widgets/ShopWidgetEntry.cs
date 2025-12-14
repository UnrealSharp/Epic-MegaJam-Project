using ManagedGameJamProject.Shop;
using ManagedGameJamProject.Subsystems;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.SlateCore;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UShopWidgetEntry : UUserWidget
{
    [UProperty, BindWidget]
    public partial UImage ThumbnailImage { get; set; }
    
    [UProperty, BindWidget]
    public partial UTextBlock ItemNameText { get; set; }
    
    [UProperty, BindWidget]
    public partial UTextBlock QuantityText { get; set; }
    
    [UProperty, BindWidget]
    public partial UTextBlock CurrentGooText { get; set; }
    
    [UProperty, BindWidget]
    public partial UTextBlock PriceText { get; set; }
    
    [UProperty, BindWidget]
    public partial UButton PurchaseButton { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial FSlateColor NotAffordableColor { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial FSlateColor AffordableColor { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<UTooltipWidget> ToolTipWidgetClass { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial USoundBase PurchaseSound { get; set; }
    
    [UProperty]
    private partial UShopItemAsset Asset { get; set; }
    
    public void InitializeFrom(UShopItemAsset asset)
    {
        Asset = asset;
        
        ThumbnailImage.BrushResourceObject = asset.Icon;
        ItemNameText.Text = asset.ItemName;
        
        UStoredGooManager storedGooManager = GetWorldSubsystem<UStoredGooManager>();
        storedGooManager.OnGooStorageInventoryChanged += OnTotalGooAmountChanged;
        
        float playerGooAmount = storedGooManager.GetTotalStoredGooAmount();
        OnTotalGooAmountChanged(playerGooAmount, playerGooAmount);
        
        PurchaseButton.OnClicked += OnPurchaseButtonClicked;
    }

    [UFunction(FunctionFlags.BlueprintCallable | FunctionFlags.BlueprintPure)]
    UWidget GetToolTipWidget()
    {
        UTooltipWidget tooltipWidget = CreateWidget(ToolTipWidgetClass);
        tooltipWidget.SetTooltipText(Asset.Description);
        return tooltipWidget;
    }
    
    [UFunction]
    void OnPurchaseButtonClicked()
    {
        UShopManager shopManager = GetWorldSubsystem<UShopManager>();
        
        if (!shopManager.PurchaseItem(Asset))
        {
            return;
        }
        
        Asset.Consumable.ConsumeItem(OwningPlayerPawn);
        UGameplayStatics.PlaySound2D(PurchaseSound);
    }
    
    [UFunction]
    void OnTotalGooAmountChanged(float newAmount, float oldAmount)
    {
        float displayAmount = (float) Math.Round(newAmount, 2);
        CurrentGooText.Text = $"{displayAmount}";
        PriceText.Text = $"{Asset.Price} Slime";
        
        if (newAmount >= Asset.Price)
        {
            CurrentGooText.ColorAndOpacity = AffordableColor;
        }
        else
        {
            CurrentGooText.ColorAndOpacity = NotAffordableColor;
        }
        
        UShopManager shopManager = GetWorldSubsystem<UShopManager>();
        
        if (Asset.HasQuantity && shopManager.GetItemQuantity(Asset, out int quantity))
        {
            QuantityText.Text = $"x{quantity}";
            
            if (quantity <= 0)
            {
                QuantityText.ColorAndOpacity = NotAffordableColor;
            }
        }
        else
        {
            QuantityText.Text = string.Empty;
        }
    }
}