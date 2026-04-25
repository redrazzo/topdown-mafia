using MafiaTopDown.Gameplay.Domain.Law;

namespace MafiaTopDown.Domain.Tests;

public sealed class PoliceResponsePlannerTests
{
    [TestCase(0, 0, "Clear")]
    [TestCase(12, 1, "Keep your head down")]
    [TestCase(35, 2, "Patrols are looking")]
    [TestCase(78, 3, "Citywide manhunt")]
    public void Maps_heat_intensity_to_police_response_profile(int heat, int responderCount, string status)
    {
        var response = PoliceResponsePlanner.Plan(HeatService.FromIntensity(heat));

        Assert.That(response.ResponderCount, Is.EqualTo(responderCount));
        Assert.That(response.StatusText, Is.EqualTo(status));
    }
}
