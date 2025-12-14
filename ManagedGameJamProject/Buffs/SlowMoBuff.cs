using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Buffs;

[UClass]
public partial class USlowMoBuff : UBuff
{
    private readonly float _slowFactor = 250.0f;
    
    public USlowMoBuff()
    {
        Duration = 5.0f;
    }
    
    protected override void OnApplyBuff(AActor target)
    {
        ACharacter targetCharacter = (ACharacter)target;
        targetCharacter.CharacterMovement.MaxWalkSpeed -= _slowFactor;
    }
    
    protected override void OnRemoveBuff()
    {
        ACharacter targetCharacter = (ACharacter) TargetActor;
        targetCharacter.CharacterMovement.MaxWalkSpeed = 600.0f;
    }
}