using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace JetXR.VisionUI
{
    public class Grabber : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Transform windowTransform;
        [SerializeField] private float translateSpeed = 2f;
        [SerializeField] private float rotateSpeed = 50f;

        private XRBaseInteractable interactable;
        private XRBaseInteractor interactor;
        private float radius;
        private float windowAngleOnSelect;
        private float additiveRotation;
        private bool xrEnabled;
        private ActionBasedController actionBasedController;

        private void OnEnable()
        {
            xrEnabled = XRSettings.enabled;

            if (!xrEnabled)
                return;

            interactable = GetComponent<XRBaseInteractable>();
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }

        private void OnDisable()
        {
            if (!xrEnabled)
                return;

            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }

        private void TranslateAction_performed(InputAction.CallbackContext obj)
        {
            var value = obj.action.ReadValue<Vector2>();
            radius += value.y * Time.deltaTime * translateSpeed;
        }

        private void RotateAction_performed(InputAction.CallbackContext obj)
        {
            var value = obj.action.ReadValue<Vector2>();
            additiveRotation += value.x * Time.deltaTime * rotateSpeed;
        }

        public void OnSelectEntered(SelectEnterEventArgs eventData)
        {
            if (!xrEnabled)
                return;

            interactor = eventData.interactorObject.transform.GetComponent<XRBaseInteractor>();

            actionBasedController = (interactor as XRBaseControllerInteractor).xrController as ActionBasedController;

            if (actionBasedController != null)
            {
                actionBasedController.translateAnchorAction.action.performed += TranslateAction_performed;
                actionBasedController.rotateAnchorAction.action.performed += RotateAction_performed;
            }

            radius = Vector3.Distance(new Vector3(windowTransform.position.x, 0, windowTransform.position.z), new Vector3(interactor.transform.position.x, 0, interactor.transform.position.z));
            additiveRotation = 0;

            var interactorPosition = interactor.transform.position;
            interactorPosition.y = 0;

            var targetPositon = interactor.transform.forward;
            targetPositon.y = 0;
            targetPositon.Normalize();
            targetPositon *= radius;
            targetPositon += interactorPosition;

            var windowRot = windowTransform.rotation;
            windowRot.eulerAngles = new Vector3(0, windowRot.eulerAngles.y, 0);

            windowAngleOnSelect = windowTransform.rotation.eulerAngles.y - Quaternion.LookRotation(targetPositon - interactorPosition).eulerAngles.y;
        }

        public void OnSelectExited(SelectExitEventArgs eventData)
        {
            if (actionBasedController != null)
            {
                actionBasedController.translateAnchorAction.action.performed -= TranslateAction_performed;
                actionBasedController.rotateAnchorAction.action.performed -= RotateAction_performed;
                actionBasedController = null;
            }
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

            windowTransform.Rotate(Vector3.up * (windowAngleOnSelect + additiveRotation));
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