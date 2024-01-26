using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace JetXR.VisionUI
{
    public class Resizer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private GameObject objectToTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private string hoveredBool = "RHovered";

        private bool isDrag;
        private bool isHover;
        private bool xrEnabled;
        private Vector3 defaultColliderSize;

        private XRBaseInteractable interactable;
        private XRBaseInteractor interactor;
        private BoxCollider boxCollider;

        private Vector3 scaleOnSelectEntered;
        private float distanceOnSelectEntered;

        private void OnEnable()
        {
            xrEnabled = XRSettings.enabled;

            if (animator == null)
                animator = GetComponent<Animator>();

            if (!xrEnabled)
                return;

            boxCollider = GetComponent<BoxCollider>();
            defaultColliderSize = boxCollider.size;
            interactable = GetComponent<XRBaseInteractable>();
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isHover)
                return;

            isHover = true;

            if (!isDrag)
                animator.SetBool(hoveredBool, isHover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isHover)
                return;

            isHover = false;

            if (!isDrag)
                animator.SetBool(hoveredBool, isHover);
        }

        #region Non XR
        public void OnPointerDown(PointerEventData eventData)
        {
            if (xrEnabled)
                return;

            scaleOnSelectEntered = objectToTransform.transform.localScale;
            distanceOnSelectEntered = Vector3.Distance(objectToTransform.transform.position, eventData.position);

            isDrag = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (xrEnabled)
                return;

            isDrag = false;

            if (!isHover)
                animator.SetBool(hoveredBool, isHover);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (xrEnabled)
                return;

            objectToTransform.transform.localScale = scaleOnSelectEntered * Vector3.Distance(objectToTransform.transform.position, eventData.position) / distanceOnSelectEntered;
        }
        #endregion

        #region XR
        public void OnSelectEntered(SelectEnterEventArgs eventData)
        {
            if (!xrEnabled)
                return;

            interactor = eventData.interactorObject.transform.GetComponent<XRBaseInteractor>();

            scaleOnSelectEntered = objectToTransform.transform.localScale;

            if ((interactor as XRRayInteractor).TryGetCurrent3DRaycastHit(out var hit))
            {
                distanceOnSelectEntered = Vector3.Distance(objectToTransform.transform.position, hit.point);
            }

            boxCollider.size = new Vector3(1000, 1000, 1);

            isDrag = true;
        }

        public void OnSelectExited(SelectExitEventArgs eventData)
        {
            if (!xrEnabled)
                return;

            boxCollider.size = defaultColliderSize;

            if (!isHover)
                animator.SetBool(hoveredBool, isHover);

            isDrag = false;
        }

        private void Update()
        {
            if (!xrEnabled || interactable == null || !interactable.isSelected)
                return;

            if ((interactor as XRRayInteractor).TryGetCurrent3DRaycastHit(out var hit))
            {
                objectToTransform.transform.localScale = scaleOnSelectEntered * Vector3.Distance(objectToTransform.transform.position, hit.point) / distanceOnSelectEntered;
            }
        }
        #endregion

        public void SetReferences(GameObject objectToTransform, Animator animator, string hoveredBool)
        {
            this.objectToTransform = objectToTransform;
            this.animator = animator;
            this.hoveredBool = hoveredBool;
        }
    }
}