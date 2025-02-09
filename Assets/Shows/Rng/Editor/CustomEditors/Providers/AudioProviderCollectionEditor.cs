﻿using Plml.Rng.Audio;
using UnityEditor;

namespace Plml.Rng.Editor
{
    [CustomEditor(typeof(AudioProviderCollection))]
    public class AudioProviderCollectionEditor : ProviderCollectionEditor<AudioProviderCollection, RngProvider<RngAudioData, float, float>>
    {

    }
}