using System;

namespace Core.Extensions
{
    public static class AngleExtension
    {
        public static float ToRadians(this float degreeAngle)
        {
            return (float)(degreeAngle * Math.PI) / 180;
        }
    }
}