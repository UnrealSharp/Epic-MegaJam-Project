using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Core.Attributes;
using UnrealSharp.UMG;

namespace ManagedGameJamProject.Widgets;

[UClass]
public partial class UBKButton : UUserWidget
{
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UButton Button { get; set; }
    
    [UProperty(PropertyFlags.BlueprintReadOnly), BindWidget]
    public partial UTextBlock ButtonText { get; set; }
    
    [UProperty(PropertyFlags.EditAnywhere)]
    public partial FText Text { get; set;  }

    public override void PreConstruct(bool isDesignTime)
    {
        base.PreConstruct(isDesignTime);
        ButtonText.Text = Text;
    }
    
    public void SetText(FText newText)
    {
        Text = newText;
        ButtonText.Text = newText;
    }
}