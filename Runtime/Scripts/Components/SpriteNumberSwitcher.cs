using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class SpriteNumberSwitcher : UIBehaviour
    {
        public Sprite[] Sprites;
        public float MaxValue = 1.0f;
        public Image Target;

        public void SetValue(float value)
        {
            int index = Mathf.Clamp(Mathf.CeilToInt((Sprites.Length - 1) * Mathf.Clamp01(value / MaxValue)), 0, Sprites.Length - 1);
            if (Target != null)
                Target.sprite = Sprites[index];
        }

    }
}
