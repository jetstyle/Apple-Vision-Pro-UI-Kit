using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JetXR.VisionUI
{
    [ExecuteInEditMode]
    public class ListElement : UIBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite singleElementSprite;
        [SerializeField] private Sprite firstElementSprite;
        [SerializeField] private Sprite middleElementSprite;
        [SerializeField] private Sprite lastElementSprite;

        protected override void OnEnable()
        {
            UpdateElementSprite();
        }

        protected override void Start()
        {
            UpdateElementSprite();
        }

        private void UpdateElementSprite()
        {
            if (image == null)
                return;

            Transform parent = transform.parent;

            if (parent == null)
                return;

            int currentSiblingIndex = transform.GetSiblingIndex();
            Transform previousChild = null;
            Transform nextChild = null;

            for (int i = currentSiblingIndex - 1; i >= 0; i--)
            {
                if (parent.GetChild(i) != null && parent.GetChild(i).gameObject.activeSelf)
                {
                    previousChild = parent.GetChild(i);
                    break;
                }
            }

            for (int i = currentSiblingIndex + 1; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) != null && parent.GetChild(i).gameObject.activeSelf)
                {
                    nextChild = parent.GetChild(i);
                    break;
                }
            }

            bool previousChildIsListElement = previousChild != null && previousChild.TryGetComponent<ListElement>(out _);
            bool nextChildIsListElement = nextChild != null && nextChild.TryGetComponent<ListElement>(out _);

            if (previousChildIsListElement)
            {
                if (nextChildIsListElement)
                {
                    image.sprite = middleElementSprite;
                }
                else
                {
                    image.sprite = lastElementSprite;
                }
            }
            else
            {
                if (nextChildIsListElement)
                {
                    image.sprite = firstElementSprite;
                }
                else
                {
                    image.sprite = singleElementSprite;
                }
            }
        }

        public void SetReferences(Image newImage, Sprite newSingleElementSprite, Sprite newFirstElementSprite, Sprite newMiddleElementSprite, Sprite newLastElementSprite)
        {
            image = newImage;
            singleElementSprite = newSingleElementSprite;
            firstElementSprite = newFirstElementSprite;
            middleElementSprite = newMiddleElementSprite;
            lastElementSprite = newLastElementSprite;
        }
    }
}