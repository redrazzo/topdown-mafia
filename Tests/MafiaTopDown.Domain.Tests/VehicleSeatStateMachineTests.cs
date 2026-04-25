using MafiaTopDown.Gameplay.Domain.Vehicles;

namespace MafiaTopDown.Domain.Tests;

public sealed class VehicleSeatStateMachineTests
{
    [Test]
    public void Transitions_from_on_foot_to_driving_when_enter_sequence_completes()
    {
        var machine = new VehicleSeatStateMachine();

        machine.BeginEnter();
        machine.ConfirmEntered();

        Assert.That(machine.State, Is.EqualTo(VehicleSeatState.Driving));
    }

    [Test]
    public void Transitions_back_to_on_foot_when_exit_sequence_completes()
    {
        var machine = new VehicleSeatStateMachine();

        machine.BeginEnter();
        machine.ConfirmEntered();
        machine.BeginExit();
        machine.ConfirmExited();

        Assert.That(machine.State, Is.EqualTo(VehicleSeatState.OnFoot));
    }

    [Test]
    public void Ignores_invalid_exit_requests_when_not_driving()
    {
        var machine = new VehicleSeatStateMachine();

        machine.BeginExit();

        Assert.That(machine.State, Is.EqualTo(VehicleSeatState.OnFoot));
    }
}
