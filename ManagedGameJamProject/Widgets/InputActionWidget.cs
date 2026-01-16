using ManagedGameJamProject.Input;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Core.Attributes;
using UnrealSharp.Engine;
using UnrealSharp.EnhancedInput;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UInputActionWidget : UUserWidget
{
    [UProperty, BindWidget] 
    public partial UImage KeyImage { get; set; }
    
    [UProperty, BindWidget]
    public partial UTextBlock LeftActionText { get; set; }
    
    [UProperty, BindWidget]
    public partial UTextBlock RightActionText { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere), Category("Input Defaults")]
    public partial UInputAction? DefaultInputAction { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere), Category("Input Defaults")]
    public partial FText DefaultActionText { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere), Category("Input Defaults")]
    public partial EInputActionDescriptionLocation DefaultDescriptionLocation { get; set; }
    
    [UProperty(PropertyFlags.Transient), BindWidgetAnim]
    public partial UWidgetAnimation PressAnimation { get; set; }

    private UInputAction? _inputAction;
    private UInputActionVisuals? _inputActionVisuals;

    public override void Construct()
    {
        base.Construct();

        if (DefaultInputAction != null)
        {
            InitializeFrom(DefaultInputAction, DefaultActionText, DefaultDescriptionLocation);
        }
    }

    public async void InitializeFrom(UInputAction inputAction, FText actionText, EInputActionDescriptionLocation descriptionLocation = EInputActionDescriptionLocation.Left)
    {
        Visibility = ESlateVisibility.Hidden;
        
        UInputWorldSubsystem inputWorldSubsystem = GetWorldSubsystem<UInputWorldSubsystem>();
        UInputActionVisuals? visuals = await inputWorldSubsystem.GetInputVisuals(inputAction).ConfigureWithUnrealContext();
        
        if (visuals == null)
        {
            LogBK.LogWarning("InputActionWidget: No visuals found for input action " + inputAction.Name);
            return;
        }
        
        Visibility = ESlateVisibility.Visible;
        
        _inputAction = inputAction;
        _inputActionVisuals = visuals;
        
        KeyImage.SetBrushFromTexture(_inputActionVisuals.Icon);

        if (descriptionLocation == EInputActionDescriptionLocation.Left)
        {
            LeftActionText.Text = actionText;
            HideTextBlock(RightActionText);
        }
        else
        {
            RightActionText.Text = actionText;
            HideTextBlock(LeftActionText);
        }

        ACharacter playerCharacter = OwningPlayerPawnAs<ACharacter>();
        UEnhancedInputComponent enhancedInputComponent = (UEnhancedInputComponent)playerCharacter.InputComponent;

        enhancedInputComponent.BindAction(_inputAction, ETriggerEvent.Started, Started);
        enhancedInputComponent.BindAction(_inputAction, ETriggerEvent.Completed, Release);
        enhancedInputComponent.BindAction(_inputAction, ETriggerEvent.Canceled, Release);
    }

    [UFunction()]
    void Started(FInputActionValue value, float arg2, float arg3, UInputAction action)
    {
        PlayAnimation(PressAnimation, 0.0f, 1, EUMGSequencePlayMode.Forward);
    }

    [UFunction()]
    void Release(FInputActionValue value, float arg2, float arg3, UInputAction action)
    {
        PlayAnimation(PressAnimation, 0.0f, 1, EUMGSequencePlayMode.Reverse);
    }
    
    void HideTextBlock(UTextBlock textBlock)
    {
        textBlock.Visibility = ESlateVisibility.Collapsed;
    }
}