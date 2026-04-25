using System;

namespace MafiaTopDown.Gameplay.Domain.Law
{
    public static class HeatService
    {
        public static HeatState FromIntensity(int intensity)
        {
            var clamped = Math.Clamp(intensity, 0, 100);
            return new HeatState(clamped, ResolveTier(clamped));
        }

        public static HeatState AddViolation(HeatState current, int intensityDelta)
        {
            var nextIntensity = Math.Clamp(current.Intensity + Math.Max(0, intensityDelta), 0, 100);
            return new HeatState(nextIntensity, ResolveTier(nextIntensity));
        }

        public static HeatState CoolDown(HeatState current, int intensityDelta)
        {
            var nextIntensity = Math.Clamp(current.Intensity - Math.Max(0, intensityDelta), 0, 100);
            return new HeatState(nextIntensity, ResolveTier(nextIntensity));
        }

        public static PoliceResponseTier ResolveTier(int intensity)
        {
            if (intensity <= 0)
            {
                return PoliceResponseTier.None;
            }

            if (intensity < 25)
            {
                return PoliceResponseTier.Watch;
            }

            if (intensity < 60)
            {
                return PoliceResponseTier.Patrol;
            }

            return PoliceResponseTier.Hunt;
        }
    }
}
