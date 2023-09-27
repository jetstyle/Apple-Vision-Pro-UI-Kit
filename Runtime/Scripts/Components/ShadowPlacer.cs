using UnityEngine;

namespace JetXR.VisionUI
{
    public class ShadowPlacer : MonoBehaviour
    {
        [SerializeField] private Transform shadowTransform;
        [SerializeField] private Transform windowBottom;
        [SerializeField] private float floorOffset = 0.1f;
        [SerializeField] private float shadowVisibilityHeight = 3f;

        private CanvasGroup canvasGroup;

        void Start()
        {
            if (shadowTransform == null)
                shadowTransform = transform;

            if (windowBottom == null)
                windowBottom = transform.parent;

            if (!gameObject.TryGetComponent<CanvasGroup>(out canvasGroup))
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        void Update()
        {
            shadowTransform.position = new Vector3(shadowTransform.position.x, floorOffset, shadowTransform.position.z);
            canvasGroup.alpha = 1 - windowBottom.position.y / shadowVisibilityHeight;
        }
    }
}
