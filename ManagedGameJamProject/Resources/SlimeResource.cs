using ManagedGameJamProject.Components;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;

namespace ManagedGameJamProject.Resources;

public struct FVanishParameters
{
    public Action<UBKSlimeInventory> OnVanishComplete;
    public readonly USceneComponent? TargetComponent;
    public FName? SocketName;

    public FVanishParameters(Action<UBKSlimeInventory> onVanishComplete, USceneComponent? targetComponent = null,
        FName? socketName = null)
    {
        OnVanishComplete = onVanishComplete;
        TargetComponent = targetComponent;
        SocketName = socketName;
    }

    public FVector GetTargetLocation()
    {
        if (TargetComponent != null)
        {
            if (SocketName != null)
            {
                return TargetComponent.GetSocketLocation(SocketName.Value);
            }
            else
            {
                return TargetComponent.WorldLocation;
            }
        }
        
        return FVector.Zero;
    }
}

[UClass]
public partial class ASlimeResource : AActor
{
    [UProperty(DefaultComponent = true, RootComponent = true)]
    public partial USlimeMeshComponent Mesh { get; set; }

    [UProperty(DefaultComponent = true)] 
    public partial UTimelineComponent VanishTimeline { get; set; }

    [UProperty(PropertyFlags.EditDefaultsOnly)]
    public partial UCurveFloat VanishCurve { get; set; }

    [UProperty(DefaultComponent = true)]
    public partial UBKSlimeInventory SlimeInventory { get; set; }

    private FVanishParameters? _vanishParameters;

    public override void BeginPlay()
    {
        base.BeginPlay();
        SlimeInventory.SetMaxValue(1);
        SlimeInventory.SetCurrentValue(1);
    }

    public void StartVanish(FVanishParameters parameters)
    {
        if (_vanishParameters != null)
        {
            return;
        }
        
        Mesh.SimulatePhysics = false;

        _vanishParameters = parameters;
        TDelegate<FOnTimelineFloat> onReceiveTimelineValue = new TDelegate<FOnTimelineFloat>();
        onReceiveTimelineValue.BindUFunction(this, nameof(OnVanishUpdate));
        VanishTimeline.AddInterpFloat(VanishCurve, onReceiveTimelineValue);

        TDelegate<FOnTimelineEvent> onTimelineFinished = new TDelegate<FOnTimelineEvent>();
        onTimelineFinished.BindUFunction(this, nameof(OnVanishFinished));
        VanishTimeline.TimelineFinishedFunc = onTimelineFinished;

        VanishTimeline.PlayFromStart();
    }

    public void StartVanish(Action<UBKSlimeInventory> onVanishComplete, USceneComponent targetComponent)
    {
        StartVanish(new FVanishParameters(onVanishComplete, targetComponent));
    }

    [UFunction]
    void OnVanishUpdate(float value)
    {
        if (_vanishParameters == null)
        {
            return;
        }

        FVector targetLocation = _vanishParameters.Value.GetTargetLocation();
        FVector currentLocation = ActorLocation;
        FVector newLocation = MathLibrary.VInterpTo(currentLocation, targetLocation, UGameplayStatics.WorldDeltaSeconds.ToFloat(), 5.0f);
        SetActorLocation(newLocation);

        double scaleValue = MathLibrary.Lerp(1.0f, 0.0f, value);
        
        if (scaleValue <= 0.01)
        {
            scaleValue = 0.01;
        }
        
        ActorScale3D = new FVector(scaleValue);
    }

    [UFunction]
    public void OnVanishFinished()
    {
        if (_vanishParameters != null)
        {
            _vanishParameters.Value.OnVanishComplete(SlimeInventory);
        }

        DestroyActor();
    }
}