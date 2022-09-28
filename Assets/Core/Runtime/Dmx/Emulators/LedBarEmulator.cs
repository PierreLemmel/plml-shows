using System;
using UnityEngine;
using UnityEngine.UI;

namespace Plml.Dmx.Emulators
{
    public class LedBarEmulator : MonoBehaviour
    {
        public DmxTrackElement ledBar;

        [Range(0f, 10f)]
        public float spacing = 1f;

        private Image[] cells;
        private HorizontalLayoutGroup group;

        private ISpy spy;
        private Color24[] colors;

        private void Awake()
        {
            colors = ledBar.GetColors();
            int count = colors.Length;

            group = this.AddComponent<HorizontalLayoutGroup>(hlg =>
            {
                hlg.childControlHeight = true;
                hlg.childAlignment = TextAnchor.MiddleCenter;
            });

            this.ClearChildren();
            this.AddChildren(count,
                i => $"Cell {i}",
                cellGo => cellGo.AddComponent<Image>(img => img.transform.rotation = transform.rotation)
            );

            cells = GetComponentsInChildren<Image>();

            spy = Spy.CreateNew();


            spy.When(() => spacing)
                .HasChanged()
                .Do(() => group.spacing = spacing);
        }

        private void Update()
        {
            spy.DetectChanges();

            ledBar.GetColors(colors);
            colors.ForEach((color, i) => cells[i].color = color);
        }
    }
}