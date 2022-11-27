using System;

namespace Plml.Dmx
{
    internal class DmxMissingFeatureException : Exception
    {
        public DmxFeature Feature { get; }

        public DmxMissingFeatureException(DmxFeature feature) : base($"Missing feature: '{feature}'") => Feature = feature;
    }
}