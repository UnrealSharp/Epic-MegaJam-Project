using ManagedGameJamProject.Components;
using UnrealSharp.Attributes;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Core;

[UClass]
public partial class ABKCharacterBase : ACharacter
{
    [UProperty(DefaultComponent = true)] 
    public partial UBKHealthComponent HealthComponent { get; set; }
    
    [UProperty(DefaultComponent = true)]
    public partial UHurtComponent HurtComponent { get; set; }

    public override void BeginPlay()
    {
        base.BeginPlay();
        HealthComponent.OnDeathDelegate += OnDeath;
    }

    [UFunction]
    public virtual void OnDeath(UBKHealthComponent healthComp)
    {
        
    }
}