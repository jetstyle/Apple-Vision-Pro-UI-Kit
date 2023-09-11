using UnityEngine;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class ToggleAnimation : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Animator animator;

        private readonly int isOnID = Animator.StringToHash("IsOn");

        private void OnEnable()
        {
            CheckReferences();
            OnToggleValueChanged(toggle.isOn);

            if (toggle != null)
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDisable()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void Reset()
        {
            CheckReferences();
        }

        private void CheckReferences()
        {
            if (toggle == null)
                toggle = GetComponent<Toggle>();

            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void OnToggleValueChanged(bool newValue)
        {
            if (animator == null)
                return;

            animator.SetBool(isOnID, newValue);
        }
    }
}