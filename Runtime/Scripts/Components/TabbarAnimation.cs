using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class TabbarAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverTimeout = 2f;
        [SerializeField] private float transitionTime = 0.25f;
        [SerializeField] private float prefferedWidth = 268f;
        [SerializeField] private Image shadowImage;

        private float time = 0;
        private bool hover;
        private RectTransform rectTransform;
        private Vector2 defaultSizeDelta;
        private Vector2 anchoredPosition;
        private Coroutine transitionCoroutine;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
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
                    if (transitionCoroutine != null)
                    {
                        StopCoroutine(transitionCoroutine);
                        transitionCoroutine = null;
                    }

                    StartCoroutine(TransitionRoutine(new Vector3(anchoredPosition.x, anchoredPosition.y, -20), new Vector2(prefferedWidth, defaultSizeDelta.y),
                        new Color(shadowImage.color.r, shadowImage.color.g, shadowImage.color.b, 1), transitionTime));

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

            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
                transitionCoroutine = null;
            }

            StartCoroutine(TransitionRoutine(anchoredPosition, defaultSizeDelta, new Color(shadowImage.color.r, shadowImage.color.g, shadowImage.color.b, 0), transitionTime));
        }

        public void SetReferences(Image shadowImage)
        {
            this.shadowImage = shadowImage;
        }

        private IEnumerator TransitionRoutine(Vector3 targetPosition, Vector2 targetSizeDelta, Color targetShadowColor, float duration)
        {
            float time = 0;
            Vector3 startAnchoredPosition3D = rectTransform.anchoredPosition3D;
            Vector3 startSizeDelta = rectTransform.sizeDelta;
            Color startShadowColor = shadowImage.color;

            while (time < duration)
            {
                rectTransform.anchoredPosition3D = Vector3.Lerp(startAnchoredPosition3D, targetPosition, time / duration);
                rectTransform.sizeDelta = Vector2.Lerp(startSizeDelta, targetSizeDelta, time / duration);
                shadowImage.color = Color.Lerp(startShadowColor, targetShadowColor, time / duration);

                time += Time.deltaTime;
                yield return null;
            }

            rectTransform.anchoredPosition3D = targetPosition;
            rectTransform.sizeDelta = targetSizeDelta;
            shadowImage.color = targetShadowColor;

            transitionCoroutine = null;
        }
    }
}
