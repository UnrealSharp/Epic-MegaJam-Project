using ManagedGameJamProject.Core;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UMainMenuWidget : UUserWidget
{
    [UProperty, BindWidget]
    public partial UBKButton StartGameButton { get; set;  }
    
    [UProperty, BindWidget]
    public partial UBKButton QuitGameButton { get; set;  }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial TSoftObjectPtr<UWorld> MainLevel { get; set;  }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial FText TutorialPromptText { get; set;  }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial FText SkipTutorialText { get; set;  }
    
    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial FText ConfirmTutorialText { get; set;  }

    public override void Construct()
    {
        base.Construct();
        WidgetLibrary.SetInputModeUIOnly(OwningPlayer, this);
        OwningPlayerController.ShowMouseCursor = true;
        
        StartGameButton.Button.OnClicked += OnStartGameClicked;
        QuitGameButton.Button.OnClicked += OnQuitGameClicked;
    }
    
    [UFunction]
    void OnStartGameClicked()
    {
        Action onChooseTutorial = () =>
        {
            OpenMainMenu(true);
        };
        
        Action onChooseSkipTutorial = () =>
        {
            OpenMainMenu(false);
        };
            
        UConfirmationWidget.ShowConfirmation(TutorialPromptText, new FConfirmationOptions(onChooseTutorial, ConfirmTutorialText), new FConfirmationOptions(onChooseSkipTutorial, SkipTutorialText));
    }
    
    void OpenMainMenu(bool runTutorial)
    {
        UBKGameInstance gameInstance = World.GameInstanceAs<UBKGameInstance>();
        gameInstance.RunTutorial = runTutorial;
        UGameplayStatics.OpenLevelBySoftObjectPtr(MainLevel);
    }
    
    [UFunction]
    void OnQuitGameClicked()
    {
        SystemLibrary.QuitGame(OwningPlayerController, EQuitPreference.Quit, false);
    }
}