using ManagedGameJamProject.Subsystems;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UEndGameWidget : UUserWidget
{
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UTextBlock EndGameText { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UTextBlock ScoreText { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UBKButton RestartButton { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UBKButton ResumeButton { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UBKButton QuitToMenuButton { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UBKButton QuitButton { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial TSoftObjectPtr<UWorld> MainLevelSoftPtr { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial TSoftObjectPtr<UWorld> MainMenuLevelSoftPtr { get; set; }
    
    public void InitializeFrom(bool isPausing)
    {
        RestartButton.Button.OnClicked += OnRestartClicked;
        QuitButton.Button.OnClicked += OnQuitClicked;
        QuitToMenuButton.Button.OnClicked += OnQuitToMenuClicked;
        ResumeButton.Button.OnClicked += OnResumeClicked;
        
        UGameplayStatics.SetGamePaused(true);

        if (isPausing)
        {
            EndGameText.Text = "Game Paused";
            ResumeButton.Visibility = ESlateVisibility.Visible;
        }
        else
        {
            UStoredGooManager storedGooManager = GetWorldSubsystem<UStoredGooManager>();
            int totalScore = (int) storedGooManager.GetTotalStoredGooAmount();
            ScoreText.Text = $"Total Slime Collected: {totalScore}";
            ScoreText.Visibility = ESlateVisibility.Visible;
            EndGameText.Text = "You Died!";
            RestartButton.Visibility = ESlateVisibility.Visible;
        }
    }
    
    [UFunction]
    void OnRestartClicked()
    {
        UGameplayStatics.OpenLevelBySoftObjectPtr(MainLevelSoftPtr);
    }

    [UFunction]
    void OnQuitClicked()
    {
        Action onChooseTutorial = () =>
        {
            SystemLibrary.QuitGame(OwningPlayer, EQuitPreference.Quit, true);
        };
        
        Action onChooseSkipTutorial = () =>
        {
            
        };
            
        UConfirmationWidget.ShowConfirmation("Quit To Desktop?", new FConfirmationOptions(onChooseTutorial, "Quit"), new FConfirmationOptions(onChooseSkipTutorial, "Cancel"));
        
    }
    
    [UFunction]
    void OnQuitToMenuClicked()
    {
        Action onChooseTutorial = () =>
        {
            UGameplayStatics.OpenLevelBySoftObjectPtr(MainMenuLevelSoftPtr);
        };
        
        Action onChooseSkipTutorial = () =>
        {
            
        };
            
        UConfirmationWidget.ShowConfirmation("Quit To Main Menu?", new FConfirmationOptions(onChooseTutorial, "Quit"), new FConfirmationOptions(onChooseSkipTutorial, "Cancel"));
    }

    [UFunction]
    void OnResumeClicked()
    {
        UGameplayStatics.SetGamePaused(false);
        RemoveFromParent();
    }
}