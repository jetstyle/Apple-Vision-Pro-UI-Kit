using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class TabbarAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverTimeout = 2f;
        [SerializeField] private Image shadowImage;

        private float time = 0;
        private bool hover;
        private RectTransform rectTransform;
        private ContentSizeFitter sizeFilter;
        private Vector2 defaultSizeDelta;
        private Vector2 anchoredPosition;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            sizeFilter = GetComponent<ContentSizeFitter>();
            defaultSizeDelta = rectTransform.sizeDelta;
            anchoredPosition = rectTransform.anchoredPosition;
        }

        private void Update()
        {
            if (hover)
            {
                time += Time.deltaTime;

                if (time > hoverTimeout)
                {
                    sizeFilter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    rectTransform.anchoredPosition3D = new Vector3(anchoredPosition.x, anchoredPosition.y, -20);
                    shadowImage.color = new Color(shadowImage.color.r, shadowImage.color.g, shadowImage.color.b, 1);
                    hover = false;
                    time = 0;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hover = false;
            time = 0;
            sizeFilter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            rectTransform.sizeDelta = defaultSizeDelta;
            rectTransform.anchoredPosition3D = anchoredPosition;
            shadowImage.color = new Color(shadowImage.color.r, shadowImage.color.g, shadowImage.color.b, 0);
        }

        public void SetReferences(Image shadowImage)
        {
            this.shadowImage = shadowImage;
        }
    }
}
