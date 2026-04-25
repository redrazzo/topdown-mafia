using MafiaTopDown.Gameplay.Domain.Law;

namespace MafiaTopDown.Domain.Tests;

public sealed class HeatServiceTests
{
    [Test]
    public void Adding_a_small_violation_enters_watch_response()
    {
        var updated = HeatService.AddViolation(HeatState.Cold, 15);

        Assert.That(updated.Intensity, Is.EqualTo(15));
        Assert.That(updated.ResponseTier, Is.EqualTo(PoliceResponseTier.Watch));
    }

    [Test]
    public void Repeated_violations_escalate_to_patrol_and_hunt()
    {
        var heat = HeatState.Cold;

        heat = HeatService.AddViolation(heat, 20);
        heat = HeatService.AddViolation(heat, 20);
        heat = HeatService.AddViolation(heat, 25);

        Assert.That(heat.Intensity, Is.EqualTo(65));
        Assert.That(heat.ResponseTier, Is.EqualTo(PoliceResponseTier.Hunt));
    }

    [Test]
    public void Cooling_down_reduces_intensity_and_can_clear_heat()
    {
        var hot = HeatService.AddViolation(HeatState.Cold, 65);

        var cooled = HeatService.CoolDown(hot, 25);
        var cleared = HeatService.CoolDown(cooled, 100);

        Assert.That(cooled.Intensity, Is.EqualTo(40));
        Assert.That(cooled.ResponseTier, Is.EqualTo(PoliceResponseTier.Patrol));
        Assert.That(cleared.Intensity, Is.EqualTo(0));
        Assert.That(cleared.ResponseTier, Is.EqualTo(PoliceResponseTier.None));
    }
}
