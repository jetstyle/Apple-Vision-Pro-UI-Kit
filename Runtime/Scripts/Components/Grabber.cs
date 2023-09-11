using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace JetXR.VisionUI
{
    public class Grabber : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Transform windowTransform;

        private XRBaseInteractable interactable;
        private XRBaseInteractor interactor;
        private float radius;
        private bool xrEnabled;

        private void OnEnable()
        {
            xrEnabled = XRSettings.enabled;

            if (!xrEnabled)
                return;

            interactable = GetComponent<XRBaseInteractable>();
            interactable.selectEntered.AddListener(OnSelectEntered);
        }

        private void OnDisable()
        {
            if (!xrEnabled)
                return;

            interactable.selectEntered.RemoveListener(OnSelectEntered);
        }

        public void OnSelectEntered(SelectEnterEventArgs eventData)
        {
            if (!xrEnabled)
                return;

            interactor = eventData.interactorObject.transform.GetComponent<XRBaseInteractor>();

            radius = Vector3.Distance(new Vector3(windowTransform.position.x, 0, windowTransform.position.z), new Vector3(interactor.transform.position.x, 0, interactor.transform.position.z));
        }

        private void Update()
        {
            if (!xrEnabled || interactable == null || !interactable.isSelected)
                return;

            //position
            var interactorPosition = interactor.transform.position;
            interactorPosition.y = 0;

            var targetPositon = interactor.transform.forward;
            targetPositon.y = 0;
            targetPositon.Normalize();
            targetPositon *= radius;
            targetPositon += interactorPosition;

            //height
            windowTransform.rotation = Quaternion.LookRotation(targetPositon - interactorPosition);
            var height = windowTransform.position.y;
            var plane = new Plane(windowTransform.forward, windowTransform.position);
            var ray = new Ray(interactor.transform.position, interactor.transform.forward);

            if (plane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                height = hitPoint.y + windowTransform.position.y - transform.position.y;
            }

            targetPositon.y = height;
            windowTransform.position = targetPositon;

            //rotation
            //TODO
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (xrEnabled)
                return;
            
            windowTransform.localPosition += (Vector3)eventData.delta;
        }

        public void SetReferences(Transform windowTransform)
        {
            this.windowTransform = windowTransform;
        }
    }
}