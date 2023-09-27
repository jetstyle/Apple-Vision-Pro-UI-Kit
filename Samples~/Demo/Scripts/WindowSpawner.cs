using JetXR.VisionUI;
using UnityEngine;
using UnityEngine.UI;

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
