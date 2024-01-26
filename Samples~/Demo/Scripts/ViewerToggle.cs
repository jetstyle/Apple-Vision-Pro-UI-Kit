using UnityEngine;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class ViewerToggle : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private Image cardImage;

        public void ChangeTargetSprite(bool isOn)
        {
            if (isOn)
            {
                targetImage.sprite = cardImage.sprite;
            }
        }
    }
}