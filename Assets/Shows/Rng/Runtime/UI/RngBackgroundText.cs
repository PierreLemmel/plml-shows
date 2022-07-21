using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using URandom = UnityEngine.Random;

namespace Plml.Rng.UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class RngBackgroundText : MonoBehaviour
    {
        [Range(10f, 150f)]
        public float cellWidth = 50f;

        [Range(10f, 150f)]
        public float cellHeight = 50f;

        [Range(0f, 20f)]
        public float hSpacing = 5f;

        [Range(0f, 20f)]
        public float vSpacing = 5f;

        [RangeBounds01]
        public FloatRange intensityRange = new(0f, 1f);
        public Color textColor = Color.white;

        private Vector2 perlinPosition = new();

        public Vector2 animationSpeed = new(0.3f, 0.05f);
        public Vector2 animationScale = new(1f, 1f);

        private ISpy spy = Spy.CreateNew();
        private GridLayoutGroup grid;

        private bool[] bits = Array.Empty<bool>();
        private TMP_Text[] texts = Array.Empty<TMP_Text>();

        public TMP_Text textPrefab;

        [RangeBounds(1, 12)]
        public IntRange textChanges = new(2, 5);

        [RangeBounds(1f/60f, 1f)]
        public FloatRange changeInterval = new(0.05f, 0.25f);

        private void Awake()
        {
            grid = GetComponent<GridLayoutGroup>();
            
            spy.When(this)
                .HasChangesOn(
                    th => Screen.width,
                    th => Screen.height,
                    th => cellWidth, 
                    th => cellHeight,
                    th => hSpacing,
                    th => vSpacing
                )
                .Do(SetupGrid);
        }

        private void Start() => StartCoroutine(TextChangeCoroutine());

        private IEnumerator TextChangeCoroutine()
        {
            while (true)
            {
                float nextDuration = MoreRandom.Range(changeInterval);
                yield return new WaitForSeconds(nextDuration);

                int changes = MoreRandom.Range(textChanges);

                int size = rows * cols;

                for (int i = 0; i < changes; i++)
                {
                    int nextIndex = URandom.Range(0, size);

                    bool newVal = !bits[nextIndex];
                    bits[nextIndex] = newVal;
                    texts[nextIndex].text = newVal ? "1" : "0";
                }
            }
        }

        private void Update()
        {
            spy.DetectChanges();

            perlinPosition += Time.deltaTime * animationSpeed;

            int size = rows * cols;

            float fRows = rows;
            float fCols = cols;

            float px0 = perlinPosition.x;
            float py0 = perlinPosition.y;

            float pw = animationScale.x;
            float ph = animationScale.y;

            float iMin = intensityRange.min;
            float iRange = intensityRange.range;

            for (int i = 0; i < size; i++)
            {
                int row = i / rows;
                int col = i % cols;

                float px = px0 + (col / fCols) * pw;
                float py = py0 + (row / fRows) * ph;

                float intensity = iMin + Mathf.PerlinNoise(px, py) * iRange;

                texts[i].color = textColor * intensity;
            }
        }

        private int cols;
        private int rows;
        public void SetupGrid()
        {
            int sw = Screen.width;
            int sh = Screen.height;

            grid.cellSize = new(cellWidth, cellHeight);

            int newCols = Mathf.FloorToInt(sw / cellWidth);
            int newRows = Mathf.FloorToInt(sh / cellHeight);

            if (rows != newRows || cols != newCols)
            {
                rows = newRows;
                cols = newCols;

                int size = rows * cols;

                grid.ClearChildren();
                bits = Arrays.Create(size, () => MoreRandom.Boolean);
                texts = new TMP_Text[size];

                for (int i = 0; i < size; i++)
                {
                    int row = i / rows;
                    int col = i % cols;

                    GameObject child = gameObject.AddChild($"Text ({row}, {col})", textPrefab);
                    TMP_Text text = child.GetComponent<TMP_Text>();
                    text.text = bits[i] ? "1" : "0";

                    texts[i] = text;
                }
            }

            float extraHSpacing = sw % cellWidth / (cols - 1);
            float extraVSpacing = sh % cellHeight / (rows - 1);
            grid.spacing = new(hSpacing + extraHSpacing, vSpacing + extraVSpacing);
        }
    }
}