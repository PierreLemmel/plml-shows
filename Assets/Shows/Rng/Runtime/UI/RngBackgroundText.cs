using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using URandom = UnityEngine.Random;
using System.Text;

namespace Plml.Rng.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class RngBackgroundText : MonoBehaviour
    {
        [RangeBounds01]
        public FloatRange intensityRange = new(0f, 1f);
        public Color textColor = Color.white;

        public Vector2 animationSpeed = new(0.3f, 0.05f);
        public Vector2 animationScale = new(1f, 1f);

        private ISpy spy = Spy.CreateNew();

        private bool[] bits = Array.Empty<bool>();

        [RangeBounds(6, 50)]
        public IntRange textChanges = new(10, 25);

        [RangeBounds(1f/60f, 1f)]
        public FloatRange changeInterval = new(0.05f, 0.25f);

        [Range(100, 1000)]
        public int nbOfBits = 1000;

        private TMP_Text text;

        private Material fontMaterial;
        private Texture2D perlinTexture;

        private const int textureSize = 512;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();

            fontMaterial = text.fontMaterial;
            perlinTexture = new(textureSize, textureSize);

            for (int i = 0; i < textureSize; i++)
            {
                for (int j = 0; j < textureSize; j++)
                {
                    float x = j;
                    float y = i;

                    float p = Mathf.PerlinNoise(x, y);
                    perlinTexture.SetPixel(i, j, new(p, p, p));
                }
            }
            perlinTexture.Apply();
            fontMaterial.mainTexture = perlinTexture;

            spy.When(this)
                .HasChangesOn(
                    th => nbOfBits
                )
                .Do(SetupBits);
        }

        private void Start() => StartCoroutine(TextChangeCoroutine());

        private StringBuilder sb;
        private void SetupBits()
        {
            bits = Arrays.Create(nbOfBits, () => MoreRandom.Boolean);
            sb = new(2 * nbOfBits);
        }

        private void UpdateText()
        {
            sb.Clear();
            sb.AppendJoin(" ", bits.Select(b => b ? "1" : "0"));
            text.text = sb.ToString();
        }

        private IEnumerator TextChangeCoroutine()
        {
            while (true)
            {
                float nextDuration = MoreRandom.Range(changeInterval);
                yield return new WaitForSeconds(nextDuration);

                int changes = MoreRandom.Range(textChanges);

                for (int i = 0; i < changes; i++)
                {
                    int nextIndex = URandom.Range(0, nbOfBits);

                    bool newVal = !bits[nextIndex];
                    bits[nextIndex] = newVal;
                }

                UpdateText();
            }
        }

        private void Update()
        {
            spy.DetectChanges();
        }

        
    }
}