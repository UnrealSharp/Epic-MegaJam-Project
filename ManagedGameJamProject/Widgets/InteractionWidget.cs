using ManagedGameJamProject.Interaction;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core.Attributes;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UInteractionWidget : UUserWidget
{
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial TSubclassOf<UInputActionWidget> InputActionWidgetClass { get; set; }
    
    [UProperty, BindWidget]
    public partial UVerticalBox InteractionWidgetBox { get; set; }
    
    [UProperty]
    private partial UInteractionComponent InteractionComponent { get; set; }

    public void InitializeFrom(UInteractionComponent interactionComponent)
    {
        InteractionWidgetBox.ClearChildren();

        UInteractionInfo? interactionInfo = interactionComponent.InteractionInfo;
        
        if (interactionInfo == null)
        {
            LogBK.LogWarning("InteractionWidget: InteractionComponent has no InteractionInfo set!");
            return;
        }
        
        UInputActionWidget inputActionWidget = CreateWidget(InputActionWidgetClass);
        inputActionWidget.InitializeFrom(interactionInfo.InputAction, interactionInfo.InteractionName);
        InteractionWidgetBox.AddChildToVerticalBox(inputActionWidget);
    }
}