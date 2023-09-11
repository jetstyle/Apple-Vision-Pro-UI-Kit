using UnityEngine;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class UpdateChildTogglesOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            foreach (var toggle in GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.Invoke(toggle.isOn);
            }
        }
    }
}
