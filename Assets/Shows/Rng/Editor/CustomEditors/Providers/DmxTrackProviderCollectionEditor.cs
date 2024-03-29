﻿using Plml.Dmx;
using Plml.Rng.Dmx;
using UnityEditor;

namespace Plml.Rng.Editor
{
    [CustomEditor(typeof(DmxTrackProviderCollection))]
    public class DmxTrackProviderCollectionEditor : ProviderCollectionEditor<DmxTrackProviderCollection, RngProvider<DmxTrack>>
    {

    }
}