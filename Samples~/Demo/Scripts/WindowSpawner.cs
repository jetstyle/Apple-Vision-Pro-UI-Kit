using UnityEngine;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    public class WindowSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject windowPrefab;
        [SerializeField] private WindowsStacker windowsStacker;

        void Start()
        {
            if (windowsStacker == null)
                windowsStacker = GetComponentInParent<WindowsStacker>();

            GetComponent<Button>().onClick.AddListener(OpenNewWindow);
        }

        void OpenNewWindow()
        {
            windowsStacker.OpenWindowFromPrefab(windowPrefab);
        }
    }
}