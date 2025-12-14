using ManagedGameJamProject.Shop;
using ManagedGameJamProject.Subsystems;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UShopWidget : UUserWidget
{
    [UProperty, BindWidget]
    public partial UGridPanel ShopGrid { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSubclassOf<UShopWidgetEntry> ShopItemWidgetClass { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial int EditorItemCount { get; set; }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial int ItemsPerRow { get; set; }
    
    [UProperty, BindWidget]
    public partial UBKButton CloseButton { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation OpenAnimation { get; set; }
    
    public override void Construct()
    {
        base.Construct();
        PopulateShopItems();
        
        PlayAnimation(OpenAnimation);
        
        UGameplayStatics.SetGamePaused(true);
        WidgetLibrary.SetInputModeUIOnly(OwningPlayer, this);
        
        CloseButton.Button.OnClicked += OnCloseButtonClicked;
    }

    public override void Destruct()
    {
        base.Destruct();
        UGameplayStatics.SetGamePaused(false);
        WidgetLibrary.SetInputMode_GameOnly(OwningPlayer);
    }

    public override void PreConstruct(bool isDesignTime)
    {
        base.PreConstruct(isDesignTime);
        
        if (isDesignTime && EditorItemCount > 0)
        {
            for (int i = 0; i < EditorItemCount; i++)
            {
                UShopWidgetEntry shopItemWidget = CreateWidget(ShopItemWidgetClass);
                ShopGrid.AddChildToGrid(shopItemWidget, i / ItemsPerRow, i % ItemsPerRow);
            }
        }
    }

    void PopulateShopItems()
    {
        UShopManager shopManager = GetWorldSubsystem<UShopManager>();
        
        ShopGrid.ClearChildren();
        
        foreach (UShopItemAsset shopItem in shopManager.AvailableShopItems)
        {
            UShopWidgetEntry shopItemWidget = CreateWidget(ShopItemWidgetClass);
            shopItemWidget.InitializeFrom(shopItem);

            int index = ShopGrid.ChildrenCount;
            ShopGrid.AddChildToGrid(shopItemWidget, index / ItemsPerRow, index % ItemsPerRow);
        }
    }
    
    [UFunction]
    void OnCloseButtonClicked()
    {
        PlayAnimationReverse(OpenAnimation);
        
        TDelegate<FWidgetAnimationDynamicEvent> onAnimationFinished = new TDelegate<FWidgetAnimationDynamicEvent>();
        onAnimationFinished.BindUFunction(this, nameof(OnCloseAnimationFinished));
        
        OpenAnimation.BindToAnimationFinished(this, onAnimationFinished);
    }
    
    [UFunction]
    public void OnCloseAnimationFinished()
    {
        RemoveFromParent();
        UGameplayStatics.SetGamePaused(false);
        WidgetLibrary.SetInputModeGameAndUI(OwningPlayer, null, EMouseLockMode.DoNotLock, false);
    }
}