using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class WindowsStacker : MonoBehaviour
    {
        [SerializeField] private  GameObject activeWindow;
        [SerializeField] private Transform windowControls;
        [SerializeField] private float distanceBetweenWindows = 45;
        [SerializeField] private float maxVisibleWindows = 5;
        [SerializeField] private float transitionDuration = 0.5f;

        private Stack<GameObject> stack = new Stack<GameObject>();
        private GameObject inactiveWindows;

        private void Awake()
        {
            inactiveWindows = new GameObject("[Inactive Windows]", typeof(RectTransform));
            inactiveWindows.transform.SetParent(transform);

            RectTransform inactiveWindowsRect = inactiveWindows.GetComponent<RectTransform>();
            inactiveWindowsRect.localScale = Vector3.one;
            inactiveWindowsRect.localRotation = Quaternion.identity;
            inactiveWindowsRect.anchorMin = Vector2.zero;
            inactiveWindowsRect.anchorMax = Vector2.one;
            inactiveWindowsRect.sizeDelta = Vector2.zero;
            inactiveWindowsRect.anchoredPosition3D = Vector3.zero;

            if (activeWindow != null)
            {
                stack.Push(activeWindow);
            }

            if (windowControls != null)
                windowControls.SetAsLastSibling();
        }

        public void OpenWindowFromPrefab(GameObject prefab)
        {
            var newWindow = GameObject.Instantiate(prefab, transform);

            if (windowControls != null)
                windowControls.SetAsLastSibling();

            OpenWindow(newWindow);
        }

        public void OpenWindow(GameObject window)
        {
            if (activeWindow != null)
            {
                activeWindow.transform.SetParent(inactiveWindows.transform, true);
                StartCoroutine(PositionTransitionRoutine(inactiveWindows.transform, inactiveWindows.transform.localPosition + Vector3.forward * distanceBetweenWindows, transitionDuration));
            }

            stack.Push(window);
            activeWindow = window;

            if (!activeWindow.TryGetComponent<CanvasGroup>(out var canvasGroup))
                canvasGroup = activeWindow.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;

            activeWindow.transform.localPosition -= Vector3.forward * distanceBetweenWindows;
            StartCoroutine(PositionTransitionRoutine(activeWindow.transform, activeWindow.transform.localPosition + Vector3.forward * distanceBetweenWindows, transitionDuration));

            UpdateWindowsVisibility();
        }

        public void CloseWindow()                                                                             
        {
            if (stack.TryPop(out var windowToClose))
            {
                StartCoroutine(PositionTransitionRoutine(windowToClose.transform, windowToClose.transform.localPosition - Vector3.forward * distanceBetweenWindows, transitionDuration));
                StartCoroutine(PositionTransitionRoutine(inactiveWindows.transform, inactiveWindows.transform.localPosition - Vector3.forward * distanceBetweenWindows, transitionDuration));

                if (!windowToClose.TryGetComponent<CanvasGroup>(out var canvasGroup))
                    canvasGroup = windowToClose.AddComponent<CanvasGroup>();

                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;

                StartCoroutine(VisibilityTransitionRoutine(windowToClose, 0, new Color(0, 0, 0, 0), () => {
                    GameObject.Destroy(windowToClose);
                    if (stack.TryPeek(out activeWindow))
                    {
                        activeWindow.transform.SetParent(transform);
                        activeWindow.transform.SetAsLastSibling();

                        if (windowControls != null)
                            windowControls.SetAsLastSibling();
                    }
                }));
            }

            UpdateWindowsVisibility();
        }

        private void UpdateWindowsVisibility()
        {
            foreach (GameObject window in stack)
            {
                if (!window.TryGetComponent<CanvasGroup>(out var canvasGroup))
                    canvasGroup = window.AddComponent<CanvasGroup>();

                bool isActiveWindow = window == stack.Peek();

                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = isActiveWindow;

                var inversedSiblingIndex = window.transform.parent.childCount - window.transform.GetSiblingIndex();
                float targetAlpha = isActiveWindow ? 1 : 1 - Mathf.Clamp(inversedSiblingIndex, 0, maxVisibleWindows) / maxVisibleWindows;
                Color targetColor = new Color(0, 0, 0, isActiveWindow ? 0 : 0.33f);

                StartCoroutine(VisibilityTransitionRoutine(window, targetAlpha, targetColor));
            }
        }

        private IEnumerator PositionTransitionRoutine(Transform targetTransform, Vector3 targetPosition, float duration)
        {
            float time = 0;
            Vector3 startPosition = targetTransform.localPosition;

            while (time < duration)
            {
                targetTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            targetTransform.localPosition = targetPosition;
        }

        private IEnumerator VisibilityTransitionRoutine(GameObject window, float targetAlpha, Color targetColor, Action onLerpDone = null)
        {
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
            Image targetImage = GetInactiveTintObject(window).GetComponent<Image>();

            float time = 0;
            float startAlpha = canvasGroup.alpha;
            Color startColor = targetImage.color;

            while (time < transitionDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / transitionDuration);
                targetImage.color = Color.Lerp(startColor, targetColor, time / transitionDuration);

                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            targetImage.color = targetColor;

            onLerpDone?.Invoke();
        }

        private GameObject GetInactiveTintObject(GameObject parent)
        {
            if (parent.transform.Find("[Inactive Tint]") != null)
            {
                return parent.transform.Find("[Inactive Tint]").gameObject;
            }

            var inactiveTintObject = new GameObject("[Inactive Tint]", typeof(Image), typeof(LayoutElement));
            inactiveTintObject.transform.SetParent(parent.transform);

            LayoutElement inactiveTintLayoutElement = inactiveTintObject.GetComponent<LayoutElement>();
            inactiveTintLayoutElement.ignoreLayout = true;

            Image parentImage = parent.GetComponent<Image>();

            Image inactiveTintImage = inactiveTintObject.GetComponent<Image>();
            inactiveTintImage.sprite = parentImage.sprite;
            inactiveTintImage.color = new Color(0, 0, 0, 0);
            inactiveTintImage.type = parentImage.type;
            inactiveTintImage.fillCenter = parentImage.fillCenter;
            inactiveTintImage.pixelsPerUnitMultiplier = parentImage.pixelsPerUnitMultiplier;
            inactiveTintImage.raycastTarget = false;

            RectTransform inactiveRect = inactiveTintObject.GetComponent<RectTransform>();
            inactiveRect.localScale = Vector3.one;
            inactiveRect.localRotation = Quaternion.identity;
            inactiveRect.anchorMin = Vector2.zero;
            inactiveRect.anchorMax = Vector2.one;
            inactiveRect.sizeDelta = Vector2.zero;
            inactiveRect.anchoredPosition3D = Vector3.zero;

            return inactiveTintObject;
        }

        public void SetReferences(GameObject activeWindow, Transform windowControls)
        {
            this.activeWindow = activeWindow;
            this.windowControls = windowControls;
        }
    }
}