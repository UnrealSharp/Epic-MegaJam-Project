using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Core.Attributes;
using UnrealSharp.DeveloperSettings;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

public struct FConfirmationOptions
{
    public readonly Action ResultAction;
    public FText ButtonText;

    public FConfirmationOptions(Action inResultAction, FText inButtonText)
    {
        ResultAction = inResultAction;
        ButtonText = inButtonText;
    }
}

[UClass(ClassFlags.Config | ClassFlags.DefaultConfig)]
public partial class UConfirmationWidgetSettings : UDeveloperSettings
{
    [UProperty(PropertyFlags.Config | PropertyFlags.EditAnywhere)]
    public partial TSoftClassPtr<UConfirmationWidget> ConfirmationWidgetClass { get; set; }
}

[UClass]
public partial class UConfirmationWidget : UUserWidget
{
    private Action? _onconfirmation;
    private Action? _oncancellation;
    
    [UProperty, BindWidget]
    public partial UTextBlock PromptTextBlock { get; set; }
    
    [UProperty, BindWidget]
    public partial UBKButton ConfirmButton { get; set; }
    
    [UProperty, BindWidget]
    public partial UBKButton CancelButton { get; set; }
    
    [UProperty, BindWidget]
    public partial UBKButton OkButton { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation FadeIn { get; set; }

    public override void Construct()
    {
        base.Construct();
        ConfirmButton.Button.OnClicked += OnConfirmButtonClicked;
        CancelButton.Button.OnClicked += OnCancelButtonClicked;
        OkButton.Button.OnClicked += OnOkButtonClicked;
        
        PlayAnimation(FadeIn); 
    }
    
    [UFunction]
    void OnConfirmButtonClicked()
    {
        _onconfirmation!();
        RemoveFromParent();
    }

    [UFunction]
    void OnCancelButtonClicked()
    {
        _oncancellation!();
        RemoveFromParent();
    }

    [UFunction]
    public void OnOkButtonClicked()
    {
        RemoveFromParent();
    }
    
    void InitializeFrom(FText promptText, FConfirmationOptions? onConfirmed, FConfirmationOptions? onCancelled)
    {
        PromptTextBlock.Text = promptText;
        
        if (onCancelled != null)
        {
            CancelButton.SetText(onCancelled.Value.ButtonText);
            _oncancellation = onCancelled.Value.ResultAction;
        }
        else
        {
            CancelButton.Visibility = ESlateVisibility.Collapsed;
        }
        
        if (onConfirmed != null)
        {
            ConfirmButton.SetText(onConfirmed.Value.ButtonText);
            _onconfirmation = onConfirmed.Value.ResultAction;
        }
        else
        {
            ConfirmButton.Visibility = ESlateVisibility.Collapsed;
        }
        
        if (onConfirmed == null && onCancelled == null)
        {
            OkButton.Visibility = ESlateVisibility.Visible;
        }
        else
        {
            OkButton.Visibility = ESlateVisibility.Collapsed;
        }
    }
    
    public static void ShowConfirmation(FText promptText, FConfirmationOptions? onConfirmed, FConfirmationOptions? onCancelled)
    {
        UConfirmationWidgetSettings settings = GetDefault<UConfirmationWidgetSettings>();
        TSubclassOf<UConfirmationWidget> widget = settings.ConfirmationWidgetClass.LoadSynchronous();
        
        UConfirmationWidget confirmationWidget = CreateWidget(widget);
        confirmationWidget.InitializeFrom(promptText, onConfirmed, onCancelled);
        confirmationWidget.AddToViewport();
    }
    
}