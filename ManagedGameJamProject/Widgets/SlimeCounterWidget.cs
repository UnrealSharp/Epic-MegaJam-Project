using ManagedGameJamProject.Subsystems;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class USlimeCounterWidget : UUserWidget
{
    [UProperty, BindWidget]
    public partial UTextBlock SlimeCountText { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation SlimeCountChangeAnim { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation SlimeCountLoss { get; set; }
    
    [UProperty]
    public partial UStoredGooManager StoredGooManager { get; set; }
    
    private bool _isPlayingAnimation;

    public override void Construct()
    {
        base.Construct();
        
        StoredGooManager = GetWorldSubsystem<UStoredGooManager>();
        StoredGooManager.OnGooStorageInventoryChanged += OnSlimeCountChanged;
        
        int totalSlime = (int) StoredGooManager.GetTotalStoredGooAmount();
        OnSlimeCountChanged(totalSlime, totalSlime);
    }
    
    [UFunction]
    public void OnSlimeCountChanged(float newValue, float oldValue)
    {
        int totalSlime = (int) StoredGooManager.GetTotalStoredGooAmount(); 
        SlimeCountText.Text = $"Collected Slime: {totalSlime}+";
        
        if (_isPlayingAnimation)
        {
            return;
        }
        
        if (newValue < oldValue)
        {
            PlayAnimation(SlimeCountLoss);
        }
        else
        {
            PlayAnimation(SlimeCountChangeAnim);
        }
        
        _isPlayingAnimation = true;
    }

    public override void OnAnimationFinished(UWidgetAnimation animation)
    {
        base.OnAnimationFinished(animation);
        
        if (Equals(animation, SlimeCountChangeAnim) || Equals(animation, SlimeCountLoss))
        {
            _isPlayingAnimation = false;
        }
    }
}