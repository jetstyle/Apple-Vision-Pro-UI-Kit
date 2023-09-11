using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JetXR.VisionUI
{
    public static class VisionControls
    {
        #region Constants
        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
        private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
        #endregion

        #region Utilities
        private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components)
        {
            GameObject child = DefaultControls.factory.CreateGameObject(name, components);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components)
        {
            GameObject go = DefaultControls.factory.CreateGameObject(name, components);
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetDefaultColorTransitionValues(Selectable selectable)
        {
            ColorBlock colors = selectable.colors;

            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);

            selectable.colors = colors;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

#if UNITY_EDITOR
            Undo.SetTransformParent(child.transform, parent.transform, "");
#else
            child.transform.SetParent(parent.transform, false);
#endif
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        static void SetAnimatorController(Animator target, RuntimeAnimatorController controller)
        {
#if UNITY_EDITOR
            UnityEditor.Animations.AnimatorController.SetAnimatorController(target, controller as UnityEditor.Animations.AnimatorController);
#else
            target.runtimeAnimatorController = controller;
#endif
        }

        #endregion

        public struct Resources
        {
            public Material darkElementMaterial;
            public Material lightElementMaterial;
            public Material windowBlurredBackgroundMaterial;
            public Material windowBlurredOverlayMaterial;
            public Material windowOverlayMaterial;
            public Material toolbarBlurredOverlayMaterial;
            public Material tabbarBlurredOverlayMaterial;

            public TMP_FontAsset fontSemibold;
            public TMP_FontAsset fontBold;
            public TMP_FontAsset fontMedium;
            public TMP_FontAsset fontRegular;

            public RuntimeAnimatorController buttonAnimatorController;
            public RuntimeAnimatorController buttonNoPlatterAnimatorController;
            public RuntimeAnimatorController symbolAnimatorController;
            public RuntimeAnimatorController symbolNoPlatterAnimatorController;
            public RuntimeAnimatorController symbolTextButtonController;
            public RuntimeAnimatorController symbolTextButtonNoPlatterController;
            public RuntimeAnimatorController smallSliderAnimatorController;
            public RuntimeAnimatorController regularSliderAnimatorController;
            public RuntimeAnimatorController throbberAnimatorController;
            public RuntimeAnimatorController listElementAnimatorController;
            public RuntimeAnimatorController toggleAnimatorController;
            public RuntimeAnimatorController dropdownAnimatorController;
            public RuntimeAnimatorController dropdownItemAnimatorController;
            public RuntimeAnimatorController inputFieldAnimatorController;
            public RuntimeAnimatorController tabbarToggleController;
            public RuntimeAnimatorController closeButtonController;
            public RuntimeAnimatorController grabberController;

            public Sprite buttonBackground;
            public Sprite buttonHighlight;
            public Sprite symbolHighlight;
            public Sprite symbol;
            public Sprite roundedRectBackground;
            public Sprite roundedRectHighlight;

            public Sprite sliderKnob;
            public Sprite sliderHighlight;
            public Sprite smallSliderBackground;
            public Sprite smallSliderFill;
            public Sprite smallSliderShadow;
            public Sprite smallSliderGlow;
            public Sprite regularSliderBackground;
            public Sprite regularSliderFill;
            public Sprite regularSliderShadow;
            public Sprite regularSliderGlow;

            public Sprite toggleBGStateOff;
            public Sprite toggleBGStateOn;
            public Sprite toggleHighlight;
            public Sprite toggleShadow;
            public Sprite toggleKnob;

            public Sprite throbber;

            public Sprite listElementArrow;
            public Sprite listElementHighlight;
            public Sprite firstListElement;
            public Sprite middleListElement;
            public Sprite lastListElement;
            public Sprite singleListElement;

            public Sprite windowGlass;
            public Sprite windowGlassNoAlpha;
            public Sprite windowGlassSmallerSpecular;
            public Sprite windowShadow;
            public Sprite sidebar;

            public Sprite scrollbarHandle;
            public Sprite dropdownArrow;
            public Sprite dropdownHighlight;
            public Sprite dropdownShadow;
            public Sprite itemCheckmark;

            public Sprite tooltip;

            public Sprite inputFieldBackground;
            public Sprite inputFieldClearBackground;
            public Sprite inputFieldClearCross;
            public Sprite inputFieldHighlight;

            public Sprite toolbarBackground;

            public Sprite verticalSeparator;
            public Sprite horizontalSeparator;

            public Sprite appIcon;
            public Sprite crossIcon;

            public Sprite tabbarBackground;
            public Sprite tabbarToggleHighlight;
            public Sprite tabbarShadow;
        }

        // Button - Text (Platter)
        public static GameObject CreateTextButton(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Button - Text", new Vector2(86f, 44f), typeof(Image), typeof(Button), typeof(Animator));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject text = CreateUIObject("Text", root, typeof(TextMeshProUGUI));

            // Animation
            Button button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.buttonAnimatorController);

            // Image
            Image background = root.GetComponent<Image>();
            background.sprite = resources.buttonBackground;
            background.color = new Color(1, 1, 1, 0);
            background.material = resources.lightElementMaterial;
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 4;
            background.raycastPadding = new Vector4(0, -8, 0, -8);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = root.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.buttonHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);
            highlightImage.type = Image.Type.Sliced;
            highlightImage.pixelsPerUnitMultiplier = 4;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(0, 0f);
            highlightRect.anchorMax = new Vector2(1, 1f);
            highlightRect.sizeDelta = new Vector2(0, 0);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            textTMP.text = "Label";
            textTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            textTMP.font = resources.fontSemibold;
            textTMP.fontSize = 17;
            textTMP.overflowMode = TextOverflowModes.Ellipsis;
            textTMP.enableWordWrapping = false;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0f);
            textRect.anchorMax = new Vector2(1, 1f);
            textRect.sizeDelta = new Vector2(-22, 0);

            return root;
        }

        // Button - Text (No Platter)
        public static GameObject CreateTextButtonNoPlatter(Resources resources)
        {
            GameObject button = CreateTextButton(resources);
            GameObject highlight = button.transform.Find("Highlight").gameObject;
            button.name = "Button - Text (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.buttonNoPlatterAnimatorController);

            Image background = button.GetComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0f);

            CanvasRenderer canvasRenderer = button.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = true;

            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = new Color(1f, 1f, 1f, 0f);

            return button;
        }

        // Button - Text+Symbol (Platter)
        public static GameObject CreateTextSymbolButton(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Button - Text+Symbol", new Vector2(120f, 44f), typeof(Image), typeof(Button), typeof(Animator));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject text = CreateUIObject("Text", root, typeof(TextMeshProUGUI));
            GameObject symbol = CreateUIObject("Symbol", root, typeof(Image));

            // Animation
            Button button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.symbolTextButtonController);

            // Image
            Image background = root.GetComponent<Image>();
            background.sprite = resources.buttonBackground;
            background.color = new Color(1, 1, 1, 0);
            background.material = resources.lightElementMaterial;
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 4;
            background.raycastPadding = new Vector4(0, -8, 0, -8);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = root.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.buttonHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);
            highlightImage.type = Image.Type.Sliced;
            highlightImage.pixelsPerUnitMultiplier = 4;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(0, 0f);
            highlightRect.anchorMax = new Vector2(1, 1f);
            highlightRect.sizeDelta = new Vector2(0, 0);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            textTMP.text = "Label";
            textTMP.horizontalAlignment = HorizontalAlignmentOptions.Left;
            textTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            textTMP.font = resources.fontSemibold;
            textTMP.fontSize = 17;
            textTMP.overflowMode = TextOverflowModes.Ellipsis;
            textTMP.enableWordWrapping = false;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0f);
            textRect.anchorMax = new Vector2(1, 1f);
            textRect.anchoredPosition = new Vector2(16.5f, 0);
            textRect.sizeDelta = new Vector2(-55, 0);

            // Symbol
            Image symbolImage = symbol.GetComponent<Image>();
            symbolImage.sprite = resources.symbol;

            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            symbolRect.anchorMin = new Vector2(0, 0.5f);
            symbolRect.anchorMax = new Vector2(0, 0.5f);
            symbolRect.anchoredPosition = new Vector2(22, 0);
            symbolRect.sizeDelta = new Vector2(44f, 44f);

            return root;
        }

        // Button - Text+Symbol (No Platter)
        public static GameObject CreateTextSymbolButtonNoPlatter(Resources resources)
        {
            GameObject button = CreateTextSymbolButton(resources);
            GameObject highlight = button.transform.Find("Highlight").gameObject;
            button.name = "Button - Text+Symbol (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.symbolTextButtonNoPlatterController);

            Image background = button.GetComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0f);

            CanvasRenderer canvasRenderer = button.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = true;

            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = new Color(1f, 1f, 1f, 0f);

            return button;
        }

        // Button - Text Rounded Rect (Platter)
        public static GameObject CreateRoundedRectButton(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Button - Text Rounded Rect", new Vector2(86f, 44f), typeof(Image), typeof(Button), typeof(Animator));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject text = CreateUIObject("Text", root, typeof(TextMeshProUGUI));

            // Animation
            Button button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.buttonAnimatorController);

            // Image
            Image background = root.GetComponent<Image>();
            background.sprite = resources.roundedRectBackground;
            background.color = new Color(1, 1, 1, 0);
            background.material = resources.lightElementMaterial;
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 4;
            background.raycastPadding = new Vector4(0, -8, 0, -8);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = root.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.roundedRectHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);
            highlightImage.type = Image.Type.Sliced;
            highlightImage.pixelsPerUnitMultiplier = 4;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(0, 0f);
            highlightRect.anchorMax = new Vector2(1, 1f);
            highlightRect.sizeDelta = new Vector2(0, 0);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            textTMP.text = "Label";
            textTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            textTMP.font = resources.fontSemibold;
            textTMP.fontSize = 17;
            textTMP.overflowMode = TextOverflowModes.Ellipsis;
            textTMP.enableWordWrapping = false;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0f);
            textRect.anchorMax = new Vector2(1, 1f);
            textRect.sizeDelta = new Vector2(-22, 0);

            return root;
        }

        // Button - Text Rounded Rect (No Platter)
        public static GameObject CreateRoundedRectButtonNoPlatter(Resources resources)
        {
            GameObject button = CreateRoundedRectButton(resources);
            GameObject highlight = button.transform.Find("Highlight").gameObject;
            button.name = "Button - Text Rounded Rect (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.buttonNoPlatterAnimatorController);

            Image background = button.GetComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0f);

            CanvasRenderer canvasRenderer = button.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = true;

            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = new Color(1f, 1f, 1f, 0f);

            return button;
        }

        // Button - Symbol (Platter)
        public static GameObject CreateSymbolButton(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Button - Symbol", new Vector2(44f, 44f), typeof(Image), typeof(Button), typeof(Animator));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject symbol = CreateUIObject("Symbol", root, typeof(Image));

            // Animation
            Button button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.symbolAnimatorController);

            // Image
            Image background = root.GetComponent<Image>();
            background.sprite = resources.buttonBackground;
            background.color = new Color(1, 1, 1, 0);
            background.material = resources.lightElementMaterial;
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 4;
            background.raycastPadding = new Vector4(-8, -8, -8, -8);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = root.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.symbolHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;
            highlightRect.sizeDelta = new Vector2(0, 0);

            // Symbol
            Image symbolImage = symbol.GetComponent<Image>();
            symbolImage.sprite = resources.symbol;

            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            symbolRect.anchorMin = Vector2.zero;
            symbolRect.anchorMax = Vector2.one;
            symbolRect.sizeDelta = new Vector2(0, 0);

            return root;
        }

        // Button - Symbol (No Platter)
        public static GameObject CreateSymbolButtonNoPlatter(Resources resources)
        {
            GameObject button = CreateSymbolButton(resources);
            GameObject highlight = button.transform.Find("Highlight").gameObject;
            button.name = "Button - Symbol (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.symbolNoPlatterAnimatorController);

            Image background = button.GetComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0f);

            CanvasRenderer canvasRenderer = button.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = true;

            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = new Color(1f, 1f, 1f, 0f);

            return button;
        }

        // Small Slider 16px
        public static GameObject CreateSmallSlider(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Small Slider", new Vector2(288f, 16f), typeof(Slider), typeof(Animator));
     
            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject mask = CreateUIObject("Mask", root, typeof(Mask), typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", mask, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));
            GameObject shadow = CreateUIObject("Shadow", fill, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", fill, typeof(Image));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(Image));
            GameObject glow = CreateUIObject("Glow", handle, typeof(Image));

            // Background
            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = resources.smallSliderBackground;
            backgroundImage.material = resources.darkElementMaterial;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = new Color(1, 1, 1, 0);
            backgroundImage.pixelsPerUnitMultiplier = 4;
            backgroundImage.raycastPadding = new Vector4(0, -22, 0, -22);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0f);
            backgroundRect.anchorMax = new Vector2(1, 1f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = background.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Mask
            Image maskImage = mask.GetComponent<Image>();
            maskImage.sprite = resources.smallSliderBackground;
            maskImage.type = Image.Type.Sliced;
            maskImage.pixelsPerUnitMultiplier = 4;
            maskImage.raycastTarget = false;

            Mask maskMask = mask.GetComponent<Mask>();
            maskMask.showMaskGraphic = false;
     
            RectTransform maskRect = mask.GetComponent<RectTransform>();
            maskRect.anchorMin = new Vector2(0, 0f);
            maskRect.anchorMax = new Vector2(1, 1f);
            maskRect.sizeDelta = new Vector2(0, 0);
     
            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0f);
            fillAreaRect.anchorMax = new Vector2(1, 1f);
            fillAreaRect.anchoredPosition = new Vector2(0, 0);
            fillAreaRect.sizeDelta = new Vector2(-16, 0);
     
            // Fill
            Image fillImage = fill.GetComponent<Image>();
            fillImage.sprite = resources.smallSliderFill;
            fillImage.type = Image.Type.Sliced;
            fillImage.color = Color.white;
            fillImage.pixelsPerUnitMultiplier = 4;
     
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(16, 0);
     
            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            shadowImage.sprite = resources.smallSliderShadow;
            shadowImage.color = Color.white;
            shadowImage.raycastTarget = false;

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            shadowRect.anchorMin = new Vector2(1, 0.5f);
            shadowRect.anchorMax = new Vector2(1, 0.5f);
            shadowRect.anchoredPosition = new Vector2(-3.33f, 0);
            shadowRect.sizeDelta = new Vector2(25, 24);

            //Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.sliderHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);
            highlightImage.raycastTarget = false;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(1, 0.5f);
            highlightRect.anchorMax = new Vector2(1, 0.5f);
            highlightRect.sizeDelta = new Vector2(160, 40);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(0, 0f);
            handleAreaRect.anchorMax = new Vector2(1, 1f);
            handleAreaRect.anchoredPosition = new Vector2(0, 0);
            handleAreaRect.sizeDelta = new Vector2(-16, 0);
     
            // Handle
            Image handleImage = handle.GetComponent<Image>();
            handleImage.sprite = resources.sliderKnob;
            handleImage.color = new Color(1f, 1f, 1f, 0f);
     
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(12, -4);
     
            // Glow
            Image glowImage = glow.GetComponent<Image>();
            glowImage.sprite = resources.smallSliderGlow;
            glowImage.color = new Color(1f, 1f, 1f, 0f);
            glowImage.raycastTarget = false;

            RectTransform glowRect = glow.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(0.5f, 0.5f);
            glowRect.anchorMax = new Vector2(0.5f, 0.5f);
            glowRect.sizeDelta = new Vector2(32, 32);
     
            // Setup slider component
            Slider slider = root.GetComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);

            // Animation
            slider.transition = Selectable.Transition.Animation;

            Navigation navigation = slider.navigation;
            navigation.mode = Navigation.Mode.None;
            slider.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.smallSliderAnimatorController);


            return root;
        }

        // Regular Slider 28px
        public static GameObject CreateRegularSlider(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Regular Slider", new Vector2(288f, 28f), typeof(Slider), typeof(Animator));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject mask = CreateUIObject("Mask", root, typeof(Mask), typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", mask, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));
            GameObject shadow = CreateUIObject("Shadow", fill, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", fill, typeof(Image));
            GameObject symbol = CreateUIObject("Symbol", root, typeof(Image));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(Image));
            GameObject glow = CreateUIObject("Glow", handle, typeof(Image));

            // Background
            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = resources.regularSliderBackground;
            backgroundImage.material = resources.darkElementMaterial;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = new Color(1, 1, 1, 0);
            backgroundImage.pixelsPerUnitMultiplier = 4;
            backgroundImage.raycastPadding = new Vector4(0, -16, 0, -16);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = background.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Mask
            Image maskImage = mask.GetComponent<Image>();
            maskImage.sprite = resources.regularSliderFill;
            maskImage.type = Image.Type.Sliced;
            maskImage.pixelsPerUnitMultiplier = 4;
            maskImage.raycastTarget = false;

            Mask maskMask = mask.GetComponent<Mask>();
            maskMask.showMaskGraphic = false;

            RectTransform maskRect = mask.GetComponent<RectTransform>();
            maskRect.anchorMin = new Vector2(0, 0f);
            maskRect.anchorMax = new Vector2(1, 1f);
            maskRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0f);
            fillAreaRect.anchorMax = new Vector2(1, 1f);
            fillAreaRect.anchoredPosition = new Vector2(0, 0);
            fillAreaRect.sizeDelta = new Vector2(-28, 0);

            // Fill
            Image fillImage = fill.GetComponent<Image>();
            fillImage.sprite = resources.regularSliderFill;
            fillImage.type = Image.Type.Sliced;
            fillImage.color = Color.white;
            fillImage.pixelsPerUnitMultiplier = 4;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(28, 0);

            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            shadowImage.sprite = resources.regularSliderShadow;
            shadowImage.color = Color.white;
            shadowImage.pixelsPerUnitMultiplier = 4;
            shadowImage.raycastTarget = false;

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            shadowRect.anchorMin = new Vector2(1, 0.5f);
            shadowRect.anchorMax = new Vector2(1, 0.5f);
            shadowRect.anchoredPosition = new Vector2(-9.53f, 0);
            shadowRect.sizeDelta = new Vector2(37, 36);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.sliderHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);
            highlightImage.raycastTarget = false;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(1, 0.5f);
            highlightRect.anchorMax = new Vector2(1, 0.5f);
            highlightRect.sizeDelta = new Vector2(208, 52);

            // Symbol
            Image symbolImage = symbol.GetComponent<Image>();
            symbolImage.sprite = resources.symbol;
            symbolImage.color = Color.black;
            symbolImage.raycastTarget = false;

            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            symbolRect.anchorMin = new Vector2(0f, 0.5f);
            symbolRect.anchorMax = new Vector2(0f, 0.5f);
            symbolRect.anchoredPosition = new Vector2(14, 0);
            symbolRect.sizeDelta = new Vector2(28, 28);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(0, 0f);
            handleAreaRect.anchorMax = new Vector2(1, 1f);
            handleAreaRect.anchoredPosition = new Vector2(0, 0);
            handleAreaRect.sizeDelta = new Vector2(-28, 0);

            // Handle
            Image handleImage = handle.GetComponent<Image>();
            handleImage.sprite = resources.sliderKnob;
            handleImage.color = new Color(1f, 1f, 1f, 0f);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, -8);

            // Glow
            Image glowImage = glow.GetComponent<Image>();
            glowImage.sprite = resources.regularSliderGlow;
            glowImage.color = new Color(1f, 1f, 1f, 0f);
            glowImage.raycastTarget = false;

            RectTransform glowRect = glow.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(0.5f, 0.5f);
            glowRect.anchorMax = new Vector2(0.5f, 0.5f);
            glowRect.sizeDelta = new Vector2(52, 52);

            // Setup slider component
            Slider slider = root.GetComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);

            // Animation
            slider.transition = Selectable.Transition.Animation;

            Navigation navigation = slider.navigation;
            navigation.mode = Navigation.Mode.None;
            slider.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.regularSliderAnimatorController);

            return root;
        }

        // Throbber
        public static GameObject CreateThrobber(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Throbber", new Vector2(44f, 44f), typeof(Image), typeof(Animator));
            
            // Animation
            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.throbberAnimatorController);

            // Throbber
            Image throbberImage = root.GetComponent<Image>();
            throbberImage.sprite = resources.throbber;
            throbberImage.raycastTarget = false;

            return root;
        }

        // List Element
        public static GameObject CreateListElement(Resources resources)
        {
            GameObject root = CreateUIElementRoot("List Element", new Vector2(460f, 60f), typeof(Image), typeof(Button), typeof(Animator));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject text = CreateUIObject("Text", root, typeof(TextMeshProUGUI));
            GameObject arrow = CreateUIObject("Arrow", root, typeof(Image));

            // Animation
            Button button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            button.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.listElementAnimatorController);

            // Background
            Image background = root.GetComponent<Image>();
            background.sprite = resources.roundedRectBackground;
            background.material = resources.lightElementMaterial;
            background.color = new Color(1, 1, 1, 0);
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 4;

            // Background Sprite
            ListElement backgroundSprite = root.AddComponent<ListElement>();
            backgroundSprite.SetReferences(background, resources.singleListElement, resources.firstListElement, resources.middleListElement, resources.lastListElement);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.listElementHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(0, 0);
            highlightRect.anchorMax = new Vector2(1, 1);
            highlightRect.sizeDelta = new Vector2(0, 0);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            textTMP.text = "Title";
            textTMP.horizontalAlignment = HorizontalAlignmentOptions.Left;
            textTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            textTMP.font = resources.fontSemibold;
            textTMP.fontSize = 17;
            textTMP.overflowMode = TextOverflowModes.Ellipsis;
            textTMP.enableWordWrapping = false;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.anchoredPosition = new Vector2(-9.5f, 0);
            textRect.sizeDelta = new Vector2(-59f, 0);

            // Arrow
            Image arrowImage = arrow.GetComponent<Image>();
            arrowImage.sprite = resources.listElementArrow;
            arrowImage.color = new Color(1, 1, 1, 0.25f);

            RectTransform arrowRect = arrow.GetComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0.5f);
            arrowRect.anchorMax = new Vector2(1, 0.5f);
            arrowRect.anchoredPosition = new Vector2(-25.5f, 0);
            arrowRect.sizeDelta = new Vector2(11f, 44f);

            return root;
        }

        // Window
        public static GameObject CreateWindow(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Window", new Vector2(1306f, 734f), typeof(Image));

            GameObject shadow = CreateUIObject("Shadow", root, typeof(Image));

            // Background
            Image background = root.GetComponent<Image>();
            background.sprite = resources.windowGlassNoAlpha;
            background.material = resources.windowBlurredOverlayMaterial;
            background.color = Color.white;
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 2;

            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            shadowImage.sprite = resources.windowShadow;
            shadowImage.color = Color.white;
            shadowImage.type = Image.Type.Sliced;
            shadowImage.pixelsPerUnitMultiplier = 2;

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            shadowRect.anchorMin = Vector2.zero;
            shadowRect.anchorMax = Vector2.one;
            shadowRect.sizeDelta = new Vector2(16, 16);

            //Window Controls
            GameObject windowControls = CreateUIObject("Window Controls", root, typeof(RectTransform));
            GameObject closeButtonWindow = CreateUIObject("Close Button Window", windowControls, typeof(Image), typeof(Button), typeof(Animator));
            GameObject grabberObjectWindow = CreateUIObject("Grabber Window", windowControls, typeof(Image), typeof(Button), typeof(Animator));
            GameObject closeButton = CreateUIObject("Close Button", closeButtonWindow, typeof(Image));
            GameObject crossIcon = CreateUIObject("Cross Icon", closeButtonWindow, typeof(Image));
            GameObject grabberObject = CreateUIObject("Grabber", grabberObjectWindow, typeof(Image), typeof(BoxCollider), typeof(XRSimpleInteractable), typeof(Grabber));

            RectTransform windowControlsRect = windowControls.GetComponent<RectTransform>();
            windowControlsRect.anchorMin = new Vector2(0.5f, 0);
            windowControlsRect.anchorMax = new Vector2(0.5f, 0);
            windowControlsRect.pivot = new Vector2(0.5f, 1);
            windowControlsRect.sizeDelta = new Vector2(174, 14);
            windowControlsRect.anchoredPosition = new Vector2(0, -22);

            //Close Button Window
            RectTransform closeButtonWindowRect = closeButtonWindow.GetComponent<RectTransform>();
            closeButtonWindowRect.anchorMin = new Vector2(0, 0.5f);
            closeButtonWindowRect.anchorMax = new Vector2(0, 0.5f);
            closeButtonWindowRect.sizeDelta = new Vector2(14, 14);
            closeButtonWindowRect.anchoredPosition = new Vector2(7, 0);

            Button closeButtonWindowButton = closeButtonWindow.GetComponent<Button>();
            closeButtonWindowButton.transition = Selectable.Transition.Animation;

            Navigation closeButtonWindowNavigation = closeButtonWindowButton.navigation;
            closeButtonWindowNavigation.mode = Navigation.Mode.None;
            closeButtonWindowButton.navigation = closeButtonWindowNavigation;

            Animator closeButtonWindowAnimator = closeButtonWindow.GetComponent<Animator>();
            SetAnimatorController(closeButtonWindowAnimator, resources.closeButtonController);

            Image closeButtonWindowImage = closeButtonWindow.GetComponent<Image>();
            closeButtonWindowImage.sprite = resources.buttonBackground;
            closeButtonWindowImage.material = resources.toolbarBlurredOverlayMaterial;
            closeButtonWindowImage.type = Image.Type.Sliced;
            closeButtonWindowImage.pixelsPerUnitMultiplier = 1;
            closeButtonWindowImage.color = new Color(1, 1, 1, 1);
            closeButtonWindowImage.raycastPadding = new Vector4(-23, -23, -23, -23);

            //Grabber Window
            RectTransform grabberWindowRect = grabberObjectWindow.GetComponent<RectTransform>();
            grabberWindowRect.anchorMin = new Vector2(1, 0.5f);
            grabberWindowRect.anchorMax = new Vector2(1, 0.5f);
            grabberWindowRect.sizeDelta = new Vector2(136, 10);
            grabberWindowRect.anchoredPosition = new Vector2(-68, 0);

            Button grabberWindowButton = grabberObjectWindow.GetComponent<Button>();
            grabberWindowButton.transition = Selectable.Transition.Animation;

            Navigation grabberWindowNavigation = grabberWindowButton.navigation;
            grabberWindowNavigation.mode = Navigation.Mode.None;
            grabberWindowButton.navigation = grabberWindowNavigation;

            Animator grabberWindowAnimator = grabberObjectWindow.GetComponent<Animator>();
            SetAnimatorController(grabberWindowAnimator, resources.grabberController);

            Image grabberWindowImage = grabberObjectWindow.GetComponent<Image>();
            grabberWindowImage.sprite = resources.buttonBackground;
            grabberWindowImage.material = resources.toolbarBlurredOverlayMaterial;
            grabberWindowImage.type = Image.Type.Sliced;
            grabberWindowImage.pixelsPerUnitMultiplier = 17.6f;
            grabberWindowImage.color = new Color(1, 1, 1, 1);
            grabberWindowImage.raycastPadding = new Vector4(0, -25, 0, -25);

            //Close Button
            RectTransform closeButtonRect = closeButton.GetComponent<RectTransform>();
            closeButtonRect.anchorMin = new Vector2(0, 0);
            closeButtonRect.anchorMax = new Vector2(1, 1);
            closeButtonRect.sizeDelta = new Vector2(0, 0);
            closeButtonRect.anchoredPosition = new Vector2(0, 0);

            Image closeButtonImage = closeButton.GetComponent<Image>();
            closeButtonImage.sprite = resources.buttonBackground;
            closeButtonImage.material = resources.lightElementMaterial;
            closeButtonImage.type = Image.Type.Sliced;
            closeButtonImage.pixelsPerUnitMultiplier = 1;
            closeButtonImage.color = new Color(1, 1, 1, 0);

            CanvasRenderer closeButtonCanvasRenderer = closeButton.GetComponent<CanvasRenderer>();
            closeButtonCanvasRenderer.cullTransparentMesh = false;

            //Cross Icon
            RectTransform crossIconRect = crossIcon.GetComponent<RectTransform>();
            crossIconRect.anchorMin = new Vector2(0, 0);
            crossIconRect.anchorMax = new Vector2(1, 1);
            crossIconRect.sizeDelta = new Vector2(0, 0);
            crossIconRect.anchoredPosition = new Vector2(0, 0);

            Image crossIconImage = crossIcon.GetComponent<Image>();
            crossIconImage.sprite = resources.crossIcon;
            crossIconImage.color = new Color(0, 0, 0, 1);

            crossIcon.SetActive(false);

            //Grabber
            RectTransform grabberRect = grabberObject.GetComponent<RectTransform>();
            grabberRect.anchorMin = new Vector2(0, 0);
            grabberRect.anchorMax = new Vector2(1, 1);
            grabberRect.sizeDelta = new Vector2(0, 0);
            grabberRect.anchoredPosition = new Vector2(0, 0);

            Image grabberImage = grabberObject.GetComponent<Image>();
            grabberImage.sprite = resources.buttonBackground;
            grabberImage.material = resources.lightElementMaterial;
            grabberImage.type = Image.Type.Sliced;
            grabberImage.pixelsPerUnitMultiplier = 18;
            grabberImage.color = new Color(1, 1, 1, 0);

            CanvasRenderer grabberObjectCanvasRenderer = grabberObject.GetComponent<CanvasRenderer>();
            grabberObjectCanvasRenderer.cullTransparentMesh = false;

            BoxCollider grabberCollider = grabberObject.GetComponent<BoxCollider>();
            grabberCollider.size = new Vector3(136, 10, 1);

            Grabber grabber = grabberObject.GetComponent<Grabber>();
            grabber.SetReferences(root.transform);

            return root;
        }

        // Toggle
        public static GameObject CreateToggle(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Toggle", new Vector2(56f, 32f), typeof(Toggle), typeof(Animator), typeof(ToggleAnimation));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject foreground = CreateUIObject("Foreground", root, typeof(Image));
            GameObject mask = CreateUIObject("Mask", root, typeof(Image), typeof(Mask));
            GameObject knobArea = CreateUIObject("Knob Area", mask, typeof(RectTransform));
            GameObject highlight = CreateUIObject("Highlight", knobArea, typeof(Image));
            GameObject knob = CreateUIObject("Knob", knobArea, typeof(Image));
            GameObject shadow = CreateUIObject("Shadow", root, typeof(Image));

            // Background State OFF
            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = resources.toggleBGStateOff;
            backgroundImage.material = resources.darkElementMaterial;
            backgroundImage.color = new Color(1, 1, 1, 0);
            backgroundImage.raycastPadding = new Vector4(-2, -14, -2, -14);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = background.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Foreground State ON
            Image foregroundImage = foreground.GetComponent<Image>();
            foregroundImage.sprite = resources.toggleBGStateOn;
            foregroundImage.color = Color.white;

            RectTransform foregroundRect = foreground.GetComponent<RectTransform>();
            foregroundRect.anchorMin = new Vector2(0, 0);
            foregroundRect.anchorMax = new Vector2(1, 1);
            foregroundRect.sizeDelta = new Vector2(0, 0);

            // Mask
            Image maskImage = mask.GetComponent<Image>();
            maskImage.sprite = resources.toggleBGStateOff;
            maskImage.raycastTarget = false;

            Mask maskMask = mask.GetComponent<Mask>();
            maskMask.showMaskGraphic = false;

            RectTransform maskRect = mask.GetComponent<RectTransform>();
            maskRect.anchorMin = new Vector2(0, 0);
            maskRect.anchorMax = new Vector2(1, 1);
            maskRect.sizeDelta = new Vector2(-0.5f, -0.5f);

            // Knob Area
            RectTransform knobAreaRect = knobArea.GetComponent<RectTransform>();
            knobAreaRect.anchoredPosition = new Vector2(-11.75f, 0);
            knobAreaRect.sizeDelta = new Vector2(32, 32);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.toggleHighlight;
            highlightImage.color = new Color(1, 1, 1, 0);
            highlightImage.raycastTarget = false;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.sizeDelta = new Vector2(79, 32);

            // Knob
            Image knobImage = knob.GetComponent<Image>();
            knobImage.sprite = resources.toggleKnob;
            knobImage.color = Color.white;
            knobImage.raycastTarget = false;

            RectTransform knobRect = knob.GetComponent<RectTransform>();
            knobRect.sizeDelta = new Vector2(32, 32);

            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            shadowImage.sprite = resources.toggleShadow;
            shadowImage.color = Color.white;
            shadowImage.raycastTarget = false;

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            shadowRect.anchorMin = new Vector2(0, 0);
            shadowRect.anchorMax = new Vector2(1, 1);
            shadowRect.sizeDelta = new Vector2(0, 0);

            // Set up components
            Toggle toggle = root.GetComponent<Toggle>();
            toggle.isOn = false;
            toggle.graphic = foregroundImage;
            toggle.targetGraphic = backgroundImage;
            SetDefaultColorTransitionValues(toggle);

            // Animation
            toggle.transition = Selectable.Transition.Animation;

            Navigation navigation = toggle.navigation;
            navigation.mode = Navigation.Mode.None;
            toggle.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.toggleAnimatorController);

            return root;
        }

        // Scrollbar
        public static GameObject CreateScrollbar(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElementRoot("Scrollbar", new Vector2(6, 160), typeof(Scrollbar));

            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", sliderArea, typeof(Image));

            // Slider Area
            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            // Handle
            Image handleImage = handle.GetComponent<Image>();
            handleImage.sprite = resources.scrollbarHandle;
            handleImage.type = Image.Type.Sliced;
            handleImage.pixelsPerUnitMultiplier = 12;
            handleImage.color = new Color(0.2264151f, 0.2264151f, 0.2264151f, 0.5f);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            // Setup scrollbar component
            Scrollbar scrollbar = scrollbarRoot.GetComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

            return scrollbarRoot;
        }

        // Dropdown
        public static GameObject CreateDropdown(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Dropdown", new Vector2(120, 44), typeof(Image), typeof(TMP_Dropdown), typeof(Animator));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject shadow = CreateUIObject("Selected Shadow", root, typeof(Image));
            GameObject label = CreateUIObject("Label", root, typeof(TextMeshProUGUI));
            GameObject arrow = CreateUIObject("Arrow", root, typeof(Image));
            GameObject template = CreateUIObject("Template", root, typeof(ScrollRect));
            GameObject viewport = CreateUIObject("Viewport", template, typeof(Image), typeof(Mask));
            GameObject content = CreateUIObject("Content", viewport, typeof(RectTransform));
            GameObject item = CreateUIObject("Item", content, typeof(Toggle), typeof(Animator));
            GameObject itemBackground = CreateUIObject("Item Background", item, typeof(Image));
            GameObject itemHighlight = CreateUIObject("Item Highlight", item, typeof(Image));
            GameObject itemCheckmark = CreateUIObject("Item Checkmark", item, typeof(Image));
            GameObject itemLabel = CreateUIObject("Item Label", item, typeof(TextMeshProUGUI));
    
            // Scrollbar
            GameObject scrollbar = CreateScrollbar(resources);
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);
    
            Scrollbar scrollbarScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollbarScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);
    
            RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.anchoredPosition = new Vector2(6, 0);
            vScrollbarRT.sizeDelta = new Vector2(6, 0);

            // Setup item UI components.

            // Item
            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin = new Vector2(0, 0.5f);
            itemRT.anchorMax = new Vector2(1, 0.5f);
            itemRT.sizeDelta = new Vector2(0, 30);

            // Item Label
            TextMeshProUGUI itemLabelText = itemLabel.GetComponent<TextMeshProUGUI>();
            itemLabelText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            itemLabelText.verticalAlignment = VerticalAlignmentOptions.Middle;
            itemLabelText.color = Color.black;
            itemLabelText.font = resources.fontMedium;
            itemLabelText.fontSize = 17;
            itemLabelText.overflowMode = TextOverflowModes.Ellipsis;
            itemLabelText.enableWordWrapping = false;

            RectTransform itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin = Vector2.zero;
            itemLabelRT.anchorMax = Vector2.one;
            itemLabelRT.offsetMin = new Vector2(20, 1);
            itemLabelRT.offsetMax = new Vector2(-10, -2);

            // Item Background
            Image itemBackgroundImage = itemBackground.GetComponent<Image>();
            itemBackgroundImage.sprite = resources.roundedRectBackground;
            itemBackgroundImage.color = Color.white;
            itemBackgroundImage.type = Image.Type.Sliced;
            itemBackgroundImage.pixelsPerUnitMultiplier = 4;

            RectTransform itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin = Vector2.zero;
            itemBackgroundRT.anchorMax = Vector2.one;
            itemBackgroundRT.sizeDelta = Vector2.zero;

            // Item Background Sprite
            ListElement itemLE = item.AddComponent<ListElement>();
            itemLE.SetReferences(itemBackgroundImage, resources.singleListElement, resources.firstListElement, resources.middleListElement, resources.lastListElement);

            // Item Highlight
            Image itemHighlightImage = itemHighlight.GetComponent<Image>();
            itemHighlightImage.sprite = resources.listElementHighlight;
            itemHighlightImage.color = new Color(1, 1, 1, 0);

            RectTransform itemHighlightRT = itemHighlight.GetComponent<RectTransform>();
            itemHighlightRT.anchorMin = Vector2.zero;
            itemHighlightRT.anchorMax = Vector2.one;
            itemHighlightRT.sizeDelta = Vector2.zero;

            // Item Checkmark
            Image itemCheckmarkImage = itemCheckmark.GetComponent<Image>();
            itemCheckmarkImage.sprite = resources.itemCheckmark;
            itemCheckmarkImage.color = Color.black;

            RectTransform itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta = new Vector2(10, 10);
            itemCheckmarkRT.anchoredPosition = new Vector2(11, 0);

            // Item Toggle
            Toggle itemToggle = item.GetComponent<Toggle>();
            itemToggle.targetGraphic = itemBackgroundImage;
            itemToggle.graphic = itemCheckmarkImage;
            itemToggle.isOn = true;

            // Item Animation
            itemToggle.transition = Selectable.Transition.Animation;

            Navigation itemNavigation = itemToggle.navigation;
            itemNavigation.mode = Navigation.Mode.None;
            itemToggle.navigation = itemNavigation;

            Animator itemAnimator = item.GetComponent<Animator>();
            SetAnimatorController(itemAnimator, resources.dropdownItemAnimatorController);

            // Setup template UI components.

            //Template
            RectTransform templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin = new Vector2(0, 0);
            templateRT.anchorMax = new Vector2(1, 0);
            templateRT.pivot = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 0);
            templateRT.sizeDelta = new Vector2(0, 150);

            // Template ScrollRect
            ScrollRect templateScrollRect = template.GetComponent<ScrollRect>();
            templateScrollRect.content = content.GetComponent<RectTransform>();
            templateScrollRect.viewport = viewport.GetComponent<RectTransform>();
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;
            
            // Content
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1);
            contentRT.anchorMax = new Vector2(1f, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = new Vector2(0, 0);
            contentRT.sizeDelta = new Vector2(0, 28);

            // Viewport
            Image viewportImage = viewport.GetComponent<Image>();
            viewportImage.sprite = resources.roundedRectBackground;
            viewportImage.type = Image.Type.Sliced;
            viewportImage.pixelsPerUnitMultiplier = 4;

            Mask scrollRectMask = viewport.GetComponent<Mask>();
            scrollRectMask.showMaskGraphic = false;

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = new Vector2(0, 0);
            viewportRT.anchorMax = new Vector2(1, 1);
            viewportRT.sizeDelta = new Vector2(-18, 0);
            viewportRT.pivot = new Vector2(0, 1);

            template.SetActive(false);

            // Setup dropdown UI components.

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.dropdownHighlight;
            highlightImage.color = new Color(1, 1, 1, 0);

            RectTransform highlightRT = highlight.GetComponent<RectTransform>();
            highlightRT.anchorMin = Vector2.zero;
            highlightRT.anchorMax = Vector2.one;
            highlightRT.sizeDelta = Vector2.zero;

            // Shadow   
            Image shadowImage = shadow.GetComponent<Image>();
            shadowImage.sprite = resources.dropdownShadow;
            shadowImage.color = new Color(1, 1, 1, 0);

            RectTransform shadowRT = shadow.GetComponent<RectTransform>();
            shadowRT.anchorMin = Vector2.zero;
            shadowRT.anchorMax = Vector2.one;
            shadowRT.sizeDelta = new Vector2(9.47f, 7.31f);
            shadowRT.anchoredPosition = new Vector2(0, -3.65f);

            // Label
            TextMeshProUGUI labelText = label.GetComponent<TextMeshProUGUI>();
            labelText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            labelText.verticalAlignment = VerticalAlignmentOptions.Middle;
            labelText.color = Color.white;
            labelText.font = resources.fontBold;
            labelText.fontSize = 22;
            labelText.overflowMode = TextOverflowModes.Ellipsis;
            labelText.enableWordWrapping = false;

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = new Vector2(0, 0);
            labelRT.anchorMax = new Vector2(1, 1);
            labelRT.sizeDelta = new Vector2(-62, 0);
            labelRT.anchoredPosition = new Vector2(-13, 0);

            // Arrow
            Image arrowImage = arrow.GetComponent<Image>();
            arrowImage.sprite = resources.dropdownArrow;
            arrowImage.color = new Color(1, 1, 1, 0.23f);

            RectTransform arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(1, 0.5f);
            arrowRT.anchorMax = new Vector2(1, 0.5f);
            arrowRT.sizeDelta = new Vector2(17, 10.25f);
            arrowRT.anchoredPosition = new Vector2(-26.5f, 0);

            // Background
            Image backgroundImage = root.GetComponent<Image>();
            backgroundImage.sprite = resources.buttonBackground;
            backgroundImage.color = new Color(1, 1, 1, 0);
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.pixelsPerUnitMultiplier = 4;
            backgroundImage.raycastPadding = new Vector4(0, -8, 0, -8);

            TMP_Dropdown dropdown = root.GetComponent<TMP_Dropdown>();
            dropdown.targetGraphic = backgroundImage;
            SetDefaultColorTransitionValues(dropdown);
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = labelText;
            dropdown.itemText = itemLabelText;

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.options.Add(new TMP_Dropdown.OptionData { text = "Option A" });
            dropdown.options.Add(new TMP_Dropdown.OptionData { text = "Option B" });
            dropdown.options.Add(new TMP_Dropdown.OptionData { text = "Option C" });
            dropdown.RefreshShownValue();

            // Animation
            dropdown.transition = Selectable.Transition.Animation;

            Navigation navigation = dropdown.navigation;
            navigation.mode = Navigation.Mode.None;
            dropdown.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.dropdownAnimatorController);

            return root;
        }

        // Tooltip
        public static GameObject CreateTooltip(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Tooltip", new Vector2(64f, 24f), typeof(Image));

            GameObject label = CreateUIObject("Label", root, typeof(TextMeshProUGUI));

            // Background
            Image background = root.GetComponent<Image>();
            background.sprite = resources.tooltip;
            background.color = new Color(0.6705883f, 0.9411765f, 1f, 0.3019608f);
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 4;
            background.raycastTarget = false;

            RectTransform backgroundRect = root.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0.5f, 0);
            backgroundRect.anchorMax = new Vector2(0.5f, 0);
            backgroundRect.pivot = new Vector2(0.5f, 1);

            // Text
            TextMeshProUGUI text = label.GetComponent<TextMeshProUGUI>();
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            text.color = Color.white;
            text.font = resources.fontMedium;
            text.fontSize = 12;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.enableWordWrapping = false;
            text.text = "Tooltip";
            text.raycastTarget = false;

            return root;
        }

        // InputField
        public static GameObject CreateInputField(Resources resources)
        {
            GameObject root = CreateUIElementRoot("InputField", new Vector2(305, 44), typeof(Image), typeof(TMP_InputField), typeof(Animator));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject textArea = CreateUIObject("Text Area", root, typeof(RectTransform));
            GameObject childPlaceholder = CreateUIObject("Placeholder", textArea, typeof(TextMeshProUGUI));
            GameObject childText = CreateUIObject("Text", textArea, typeof(TextMeshProUGUI));
            GameObject clearButton = CreateUIObject("Clear Button", root, typeof(Button), typeof(Image));
            GameObject clearButtonCross = CreateUIObject("Cleaar Button Cross", clearButton, typeof(Image));

            // Background
            Image image = root.GetComponent<Image>();
            image.sprite = resources.inputFieldBackground;
            image.material = resources.darkElementMaterial;
            image.color = new Color(1, 1, 1, 0);
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 4;
            image.raycastPadding = new Vector4(0, -8, 0, -8);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = root.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // InputField
            TMP_InputField inputField = root.GetComponent<TMP_InputField>();
            SetDefaultColorTransitionValues(inputField);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.inputFieldHighlight;
            highlightImage.color = new Color(1, 1, 1, 0);
            highlightImage.raycastTarget = false;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;
            highlightRect.sizeDelta = Vector2.zero;

            // Text Area
            RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.sizeDelta = new Vector2(-40, 0);

            // Text
            TextMeshProUGUI text = childText.GetComponent<TextMeshProUGUI>();
            text.text = "";
            text.horizontalAlignment = HorizontalAlignmentOptions.Left;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            text.color = Color.white;
            text.font = resources.fontRegular;
            text.fontSize = 17;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.enableWordWrapping = false;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            // Placeholder
            TextMeshProUGUI placeholder = childPlaceholder.GetComponent<TextMeshProUGUI>();
            placeholder.text = "Enter text...";
            // Make placeholder color half as opaque as normal text color.
            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;
            placeholder.horizontalAlignment = HorizontalAlignmentOptions.Left;
            placeholder.verticalAlignment = VerticalAlignmentOptions.Middle;
            placeholder.font = resources.fontMedium;
            placeholder.fontSize = 17;
            placeholder.overflowMode = TextOverflowModes.Ellipsis;
            placeholder.enableWordWrapping = false;

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;

            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            // Clear Button
            Button cButton = clearButton.GetComponent<Button>();
            cButton.transition = Selectable.Transition.None;
            Navigation navigation = cButton.navigation;
            navigation.mode = Navigation.Mode.None;
            cButton.navigation = navigation;
            cButton.interactable = false;

            Image clearButtonImage = clearButton.GetComponent<Image>();
            clearButtonImage.sprite = resources.inputFieldClearBackground;
            clearButtonImage.material = resources.lightElementMaterial;
            clearButtonImage.color = new Color(1, 1, 1, 0);
            clearButtonImage.raycastPadding = new Vector4(-16, -16, -16, -16);

            // Canvas Renderer
            CanvasRenderer clearButtonCanvasRenderer = clearButton.GetComponent<CanvasRenderer>();
            clearButtonCanvasRenderer.cullTransparentMesh = false;

            RectTransform clearButtonRect = clearButton.GetComponent<RectTransform>();
            clearButtonRect.anchorMin = new Vector2(1, 0.5f);
            clearButtonRect.anchorMax = new Vector2(1, 0.5f);
            clearButtonRect.sizeDelta = new Vector2(28, 28);
            clearButtonRect.anchoredPosition = new Vector2(-22, 0);

            // Clear Button Cross
            Image clearCrossImage = clearButtonCross.GetComponent<Image>();
            clearCrossImage.sprite = resources.inputFieldClearCross;
            clearCrossImage.raycastTarget = false;

            RectTransform clearCrossRect = clearButtonCross.GetComponent<RectTransform>();
            clearCrossRect.sizeDelta = new Vector2(11, 11);

            clearButton.SetActive(false);

            // Animation
            inputField.transition = Selectable.Transition.Animation;

            Animator itemAnimator = root.GetComponent<Animator>();
            SetAnimatorController(itemAnimator, resources.inputFieldAnimatorController);

            return root;
        }

        // Vertical Separator
        public static GameObject CreateVerticalSeparator(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Vertical Separator", new Vector2(1, 30), typeof(Image));

            Image SeparatorImage = root.GetComponent<Image>();
            SeparatorImage.sprite = resources.verticalSeparator;
            SeparatorImage.type = Image.Type.Sliced;
            SeparatorImage.pixelsPerUnitMultiplier = 4;
            SeparatorImage.raycastTarget = false;

            return root;
        }

        // Horizontal Separator
        public static GameObject CreateHorizontalSeparator(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Horizontal Separator", new Vector2(30, 1), typeof(Image));

            Image SeparatorImage = root.GetComponent<Image>();
            SeparatorImage.sprite = resources.horizontalSeparator;
            SeparatorImage.type = Image.Type.Sliced;
            SeparatorImage.pixelsPerUnitMultiplier = 4;
            SeparatorImage.raycastTarget = false;

            return root;
        }

        // Toolbar
        public static GameObject CreateToolbar(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Toolbar", new Vector2(68, 68), typeof(Image), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));

            // Background
            Image backgroundImage = root.GetComponent<Image>();
            backgroundImage.sprite = resources.toolbarBackground;
            backgroundImage.material = resources.toolbarBlurredOverlayMaterial;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.pixelsPerUnitMultiplier = 4;

            HorizontalLayoutGroup layout = root.GetComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 16;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childScaleWidth = true;

            ContentSizeFitter fitter = root.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

            // Content

            // Button 1
            GameObject button1 = CreateSymbolButtonNoPlatter(resources);
            button1.name = "Button - Symbol";
            SetParentAndAlign(button1, root);

            // Button 2
            GameObject button2 = CreateSymbolButtonNoPlatter(resources);
            button2.name = "Button - Symbol";
            SetParentAndAlign(button2, root);

            // Separator
            GameObject separator = CreateVerticalSeparator(resources);
            separator.name = "Separator";
            SetParentAndAlign(separator, root);

            // Button 3
            GameObject button3 = CreateSymbolButtonNoPlatter(resources);
            button3.name = "Button - Symbol";
            SetParentAndAlign(button3, root);

            return root;
        }

        // Alert
        public static GameObject CreateAlert(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Alert", new Vector2(44, 44), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));

            GameObject appIcon = CreateUIObject("App Icon", root, typeof(Image));
            GameObject textBlock = CreateUIObject("Text Block", root, typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            GameObject title = CreateUIObject("Title", textBlock, typeof(TextMeshProUGUI));
            GameObject description = CreateUIObject("Description", textBlock, typeof(TextMeshProUGUI));

            // Background
            Image backgroundImage = root.GetComponent<Image>();
            backgroundImage.sprite = resources.windowGlassNoAlpha;
            backgroundImage.material = resources.windowBlurredOverlayMaterial;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.pixelsPerUnitMultiplier = 4;

            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.spacing = 8;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childScaleWidth = true;
            layout.childScaleHeight = true;

            ContentSizeFitter fitter = root.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            // Content

            // App Icon
            Image appIconImage = appIcon.GetComponent<Image>();
            appIconImage.sprite = resources.appIcon;

            RectTransform appIconRect = appIcon.GetComponent<RectTransform>();
            appIconRect.sizeDelta = new Vector2(52, 52);

            // Text Block
            VerticalLayoutGroup textLayout = textBlock.GetComponent<VerticalLayoutGroup>();
            textLayout.padding = new RectOffset(16, 16, 8, 8);
            textLayout.spacing = 2;
            textLayout.childAlignment = TextAnchor.MiddleCenter;
            textLayout.childControlWidth = true;
            textLayout.childForceExpandWidth = true;
            textLayout.childScaleHeight = true;

            ContentSizeFitter textfitter = textBlock.GetComponent<ContentSizeFitter>();
            textfitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            textfitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            RectTransform textBlockRect = textBlock.GetComponent<RectTransform>();
            textBlockRect.sizeDelta = new Vector2(270, 44);

            // Title
            TextMeshProUGUI titleTMP = title.GetComponent<TextMeshProUGUI>();
            titleTMP.text = "Title";
            titleTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
            titleTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            titleTMP.color = Color.white;
            titleTMP.font = resources.fontBold;
            titleTMP.fontSize = 19;
            titleTMP.overflowMode = TextOverflowModes.Ellipsis;
            titleTMP.enableWordWrapping = false;

            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(0, 24);

            // Description
            TextMeshProUGUI descriptionTMP = description.GetComponent<TextMeshProUGUI>();
            descriptionTMP.text = "Description text about this alert";
            descriptionTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
            descriptionTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            descriptionTMP.color = Color.white;
            descriptionTMP.font = resources.fontRegular;
            descriptionTMP.fontSize = 15;
            descriptionTMP.overflowMode = TextOverflowModes.Ellipsis;

            RectTransform descriptionRect = description.GetComponent<RectTransform>();
            descriptionRect.sizeDelta = new Vector2(0, 20);

            // Separator
            GameObject separator = CreateHorizontalSeparator(resources);
            separator.name = "Separator";
            SetParentAndAlign(separator, root);

            RectTransform separatorRect = separator.GetComponent<RectTransform>();
            separatorRect.sizeDelta = new Vector2(280, 1);

            // Action Button 1
            GameObject action1 = CreateRoundedRectButton(resources);
            GameObject highlight1 = action1.transform.Find("Highlight").gameObject;
            GameObject text1 = action1.transform.Find("Text").gameObject;
            action1.name = "Action Button";
            SetParentAndAlign(action1, root);

            RectTransform action1Rect = action1.GetComponent<RectTransform>();
            action1Rect.sizeDelta = new Vector2(280, 44);

            Animator action1Animator = action1.GetComponent<Animator>();
            SetAnimatorController(action1Animator, resources.buttonNoPlatterAnimatorController);
            
            Image action1Image = action1.GetComponent<Image>();
            action1Image.color = new Color(1f, 1f, 1f, 0f);
            
            Image highlight1Image = highlight1.GetComponent<Image>();
            highlight1Image.color = new Color(1f, 1f, 1f, 0f);

            TextMeshProUGUI text1TMP = text1.GetComponent<TextMeshProUGUI>();
            text1TMP.text = "Action";

            // Action Button 2
            GameObject action2 = CreateRoundedRectButton(resources);
            GameObject highlight2 = action2.transform.Find("Highlight").gameObject;
            GameObject text2 = action2.transform.Find("Text").gameObject;
            action2.name = "Action Button";
            SetParentAndAlign(action2, root);

            RectTransform action2Rect = action2.GetComponent<RectTransform>();
            action2Rect.sizeDelta = new Vector2(280, 44);

            Animator action2Animator = action2.GetComponent<Animator>();
            SetAnimatorController(action2Animator, resources.buttonNoPlatterAnimatorController);

            Image action2Image = action2.GetComponent<Image>();
            action2Image.color = new Color(1f, 1f, 1f, 0f);

            Image highlight2Image = highlight2.GetComponent<Image>();
            highlight2Image.color = new Color(1f, 1f, 1f, 0f);

            TextMeshProUGUI text2TMP = text2.GetComponent<TextMeshProUGUI>();
            text2TMP.text = "Action";

            return root;
        }

        // Tabbar Toggle
        public static GameObject CreateTabbarToggle(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Toggle", new Vector2(44f, 44f), typeof(Image), typeof(Toggle), typeof(Animator), typeof(ToggleAnimation));

            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject text = CreateUIObject("Text", root, typeof(TextMeshProUGUI));
            GameObject symbol = CreateUIObject("Symbol", root, typeof(Image));

            // Animation
            Toggle toggle = root.GetComponent<Toggle>();
            toggle.transition = Selectable.Transition.Animation;

            Navigation navigation = toggle.navigation;
            navigation.mode = Navigation.Mode.None;
            toggle.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.tabbarToggleController);

            // Image
            Image toggleImage = root.GetComponent<Image>();
            toggleImage.sprite = resources.buttonBackground;
            toggleImage.color = new Color(1, 1, 1, 0);
            toggleImage.material = resources.lightElementMaterial;
            toggleImage.type = Image.Type.Sliced;
            toggleImage.pixelsPerUnitMultiplier = 4;
            toggleImage.raycastPadding = new Vector4(-8, -8, -8, -8);

            // Canvas Renderer
            CanvasRenderer canvasRenderer = root.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.tabbarToggleHighlight;
            highlightImage.material = resources.lightElementMaterial;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);
            highlightImage.type = Image.Type.Sliced;
            highlightImage.pixelsPerUnitMultiplier = 4;
            highlightImage.raycastTarget = false;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(0, 0f);
            highlightRect.anchorMax = new Vector2(1, 1f);
            highlightRect.sizeDelta = new Vector2(0, 0);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            textTMP.text = "Label";
            textTMP.horizontalAlignment = HorizontalAlignmentOptions.Left;
            textTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            textTMP.font = resources.fontSemibold;
            textTMP.fontSize = 17;
            textTMP.overflowMode = TextOverflowModes.Ellipsis;
            textTMP.enableWordWrapping = false;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0f);
            textRect.anchorMax = new Vector2(1, 1f);
            textRect.anchoredPosition = new Vector2(16.5f, 0);
            textRect.sizeDelta = new Vector2(-55, 0);

            // Symbol
            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            symbolRect.anchorMin = new Vector2(0, 0.5f);
            symbolRect.anchorMax = new Vector2(0, 0.5f);
            symbolRect.anchoredPosition = new Vector2(22, 0);
            symbolRect.sizeDelta = new Vector2(44f, 44f);

            Image symbolImage = symbol.GetComponent<Image>();
            symbolImage.sprite = resources.symbol;

            // Set up components
            toggle.isOn = false;
            toggle.graphic = toggleImage;
            toggle.targetGraphic = symbolImage;

            return root;
        }

        //Tabbar
        public static GameObject CreateTabbar(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Tabbar", new Vector2(68, 120), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(LayoutElement),
                typeof(ToggleGroup), typeof(UpdateChildTogglesOnAwake), typeof(TabbarAnimation));

            GameObject shadow = CreateUIObject("Shadow", root, typeof(Image), typeof(LayoutElement));
            GameObject background = CreateUIObject("Background", root, typeof(Image), typeof(LayoutElement));

            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = resources.tabbarBackground;
            backgroundImage.material = resources.tabbarBlurredOverlayMaterial;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.pixelsPerUnitMultiplier = 4;

            LayoutElement backgroundLayoutElement = background.GetComponent<LayoutElement>();
            backgroundLayoutElement.ignoreLayout = true;

            Image shadowImage = shadow.GetComponent<Image>();
            shadowImage.sprite = resources.tabbarShadow;
            shadowImage.color = new Color(0, 0, 0, 0);
            shadowImage.type = Image.Type.Sliced;
            shadowImage.pixelsPerUnitMultiplier = 4;

            LayoutElement shadowLayoutElement = shadow.GetComponent<LayoutElement>();
            shadowLayoutElement.ignoreLayout = true;

            VerticalLayoutGroup tabbarLayout = root.GetComponent<VerticalLayoutGroup>();
            tabbarLayout.childAlignment = TextAnchor.UpperLeft;
            tabbarLayout.childForceExpandWidth = true;
            tabbarLayout.childForceExpandHeight = false;
            tabbarLayout.childControlWidth = true;
            tabbarLayout.spacing = 8;
            tabbarLayout.padding = new RectOffset(12, 12, 12, 12);

            ContentSizeFitter tabbarSizeFilter = root.GetComponent<ContentSizeFitter>();
            tabbarSizeFilter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            LayoutElement tabbarLayoutElement = root.GetComponent<LayoutElement>();
            tabbarLayoutElement.preferredWidth = 268;

            TabbarAnimation tabbarAnimation = root.GetComponent<TabbarAnimation>();
            tabbarAnimation.SetReferences(shadowImage);

            ToggleGroup tabbarToggleGroup = root.GetComponent<ToggleGroup>();

            //Toggles
            for (int i = 0; i < 2; i++)
            {
                GameObject toggleObject = CreateTabbarToggle(resources);
                SetParentAndAlign(toggleObject, root);
                Toggle toggle = toggleObject.GetComponent<Toggle>();
                toggle.group = tabbarToggleGroup;
                
                if (i == 0)
                    toggle.isOn = true;
            }

            return root;
        }

        //Window + Tabbar
        public static GameObject CreateWindowWithTabbar(Resources resources)
        {
            GameObject root = CreateWindow(resources);
            root.name = root.name + " + Tabbar";

            GameObject tabbar = CreateTabbar(resources);
            SetParentAndAlign(tabbar, root);

            var tabbarRect = tabbar.GetComponent<RectTransform>();
            tabbarRect.anchoredPosition = new Vector2(0, 0);

            var tabbarToggles = tabbar.GetComponentsInChildren<Toggle>();

            for (int i = 0; i < tabbarToggles.Length; i++)
            {
                GameObject tab = CreateUIObject($"Tab {i + 1}", root, typeof(RectTransform));
                GameObject label = CreateUIObject($"Label", tab, typeof(TextMeshProUGUI));

                RectTransform tabRect = tab.GetComponent<RectTransform>();
                tabRect.anchorMin = new Vector2(0, 0);
                tabRect.anchorMax = new Vector2(1, 1);

                TextMeshProUGUI labelText = label.GetComponent<TextMeshProUGUI>();
                labelText.text = $"Tab {i + 1}";
                labelText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            }
            
            return root;
        }

        //Progress Bar
        public static GameObject CreateProgressBar(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Progress Bar", new Vector2(500f, 10f), typeof(ProgressBar));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", root, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = resources.regularSliderBackground;
            backgroundImage.material = resources.darkElementMaterial;
            backgroundImage.color = new Color(1, 1, 1, 0);
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.pixelsPerUnitMultiplier = 11;

            // Canvas Renderer
            CanvasRenderer clearButtonCanvasRenderer = background.GetComponent<CanvasRenderer>();
            clearButtonCanvasRenderer.cullTransparentMesh = false;

            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0);
            fillAreaRect.anchorMax = new Vector2(1, 1);
            fillAreaRect.sizeDelta = new Vector2(-10, 0);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(0, 1);
            fillRect.sizeDelta = new Vector2(10, 0);

            Image fillImage = fill.GetComponent<Image>();
            fillImage.sprite = resources.regularSliderFill;
            fillImage.type = Image.Type.Sliced;
            fillImage.pixelsPerUnitMultiplier = 11;

            ProgressBar progressBar = root.GetComponent<ProgressBar>();
            progressBar.FillRect = fillRect;

            return root;
        }

        //Window + Toolbar
        public static GameObject CreateWindowWithToolbar(Resources resources)
        {
            GameObject root = CreateWindow(resources);
            root.name = root.name + " + Toolbar";

            GameObject toolbar = CreateToolbar(resources);
            SetParentAndAlign(toolbar, root);

            return root;
        }

        // Sidebar
        public static GameObject CreateSidebar(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Sidebar", new Vector2(362, -6), typeof(Image));

            Image Image = root.GetComponent<Image>();
            Image.sprite = resources.sidebar;
            Image.material = resources.darkElementMaterial;
            Image.color = new Color(1, 1, 1, 0);
            Image.type = Image.Type.Sliced;
            Image.pixelsPerUnitMultiplier = 2;
            Image.raycastTarget = false;

            // Canvas Renderer
            CanvasRenderer canvasRenderer = root.GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            return root;
        }
    }
}