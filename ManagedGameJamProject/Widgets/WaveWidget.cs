using ManagedGameJamProject.Spawn;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UWaveWidget : UUserWidget
{
    [UProperty, BindWidget]
    public partial UTextBlock WaveTextBlock { get; set; }
    
    [UProperty, BindWidget]
    public partial UTextBlock CountdownTextBlock { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation CountdownAnimation { get; set; }

    public override void Construct()
    {
        base.Construct();
        
        USpawnSubsystem spawnSubsystem = GetWorldSubsystem<USpawnSubsystem>();
        spawnSubsystem.OnTimeToNextRoundUpdated += OnTimeToNextRoundUpdated;
        spawnSubsystem.OnRoundStarted += OnRoundStarted;
        
        CountdownTextBlock.Visibility = ESlateVisibility.Hidden;
        WaveTextBlock.Visibility = ESlateVisibility.Hidden;
    }
    
    [UFunction]
    void OnTimeToNextRoundUpdated(int timeRemaining)
    {
        CountdownTextBlock.Text = timeRemaining.ToString();
        CountdownTextBlock.Visibility = ESlateVisibility.Visible;
        WaveTextBlock.Visibility = ESlateVisibility.Hidden;
        PlayAnimation(CountdownAnimation);
    }
    
    [UFunction]
    public void OnRoundStarted(int roundNumber)
    {
        WaveTextBlock.Text = $"Wave {roundNumber}";
        WaveTextBlock.Visibility = ESlateVisibility.Visible;
        CountdownTextBlock.Visibility = ESlateVisibility.Hidden;
    }
}