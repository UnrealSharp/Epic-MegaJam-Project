using UnrealSharp.Attributes;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Item;

[UClass(ClassFlags.DefaultToInstanced | ClassFlags.EditInlineNew)]
public partial class UConsumable : UObject
{
    public virtual void ConsumeItem(APawn pawn)
    {
        
    }
}