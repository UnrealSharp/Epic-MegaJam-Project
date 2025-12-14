using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Core.Attributes;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UTooltipWidget : UUserWidget
{
    [UProperty, BindWidget]
    public partial UTextBlock TooltipTextBlock { get; set; }
    
    public void SetTooltipText(FText text)
    {
        TooltipTextBlock.Text = text;
    }
}