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
        private static Color s_TextColorPrimary = new Color(1, 1, 1, 1);
        private static Color s_TextColorSecondary = new Color(1, 1, 1, 0.25f);
        private static Color s_TextColorTertiary = new Color(1, 1, 1, 0.1f);
        private static Color s_WindowElementColor = new Color(1, 1, 1, 1);
        private static Color s_WhiteTransparent = new Color(1, 1, 1, 0);
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
            public Material lightElementWithFrameMaterial;
            public Material windowBlurredBackgroundMaterial;
            public Material windowBlurredOverlayMaterial;
            public Material windowOverlayMaterial;
            public Material toolbarBlurredOverlayMaterial;
            public Material tabbarBlurredOverlayMaterial;
            public Material alphaBackgroundMaterial;

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
            public RuntimeAnimatorController miniSliderAnimatorController;
            public RuntimeAnimatorController smallSliderAnimatorController;
            public RuntimeAnimatorController regularSliderAnimatorController;
            public RuntimeAnimatorController largeSliderAnimatorController;
            public RuntimeAnimatorController throbberAnimatorController;
            public RuntimeAnimatorController listElementAnimatorController;
            public RuntimeAnimatorController toggleAnimatorController;
            public RuntimeAnimatorController dropdownAnimatorController;
            public RuntimeAnimatorController dropdownItemAnimatorController;
            public RuntimeAnimatorController inputFieldAnimatorController;
            public RuntimeAnimatorController tabbarToggleController;
            public RuntimeAnimatorController closeButtonController;
            public RuntimeAnimatorController grabberController;
            public RuntimeAnimatorController volumeController;
            public RuntimeAnimatorController resizerController;

            public Sprite buttonBackground;
            public Sprite buttonHighlight;
            public Sprite symbolHighlight;
            public Sprite symbol;
            public Sprite roundedRectBackground;
            public Sprite roundedRectHighlight;

            public Sprite sliderElement;
            public Sprite sliderHighlight;
            public Sprite miniSliderShadow;
            public Sprite miniSliderGlow;
            public Sprite miniSliderEmboss;
            public Sprite smallSliderShadow;
            public Sprite smallSliderGlow;
            public Sprite smallSliderEmboss;
            public Sprite regularSliderBackground;
            public Sprite regularSliderFill;
            public Sprite regularSliderShadow;
            public Sprite regularSliderGlow;
            public Sprite regularSliderEmboss;
            public Sprite largeSliderShadow;
            public Sprite largeSliderGlow;
            public Sprite largeSliderEmboss;

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
            public Sprite windowFloorShadow;
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
            public Sprite segmentedControlHighlight;
            public Sprite tabbarShadow;

            public Sprite speakerSlash;
            public Sprite speaker1;
            public Sprite speaker2;
            public Sprite speaker3;

            public Sprite trailing;
        }

        // Button - Text (Platter)
        public static GameObject CreateTextButton(Resources resources, float width, float height, float fontSize)
        //float multiplier
        {
            GameObject root = CreateUIElementRoot("Button - Text", new Vector2(width, height), typeof(Image), typeof(Button), typeof(Animator));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
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

            float horizontalPadding = width > 60 ? 0 : (width - 60) / 2;
            float verticalPadding = height > 60 ? 0 : (height - 60) / 2;
            Vector4 padding = new Vector4(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);

            SetupImage(background, resources.buttonBackground, s_WindowElementColor, resources.lightElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.buttonBackground.texture.height / height);

            SetupWindowElement(root, alphaBackground, resources);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.buttonHighlight, s_WhiteTransparent, null, false, Vector4.zero, true, Image.Type.Sliced, true, resources.buttonHighlight.texture.height / height);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            SetupTextMeshProUGUI(textTMP, "Label", resources.fontSemibold, fontSize, Color.white, HorizontalAlignmentOptions.Center, VerticalAlignmentOptions.Middle, false, TextOverflowModes.Ellipsis);

            RectTransform textRect = text.GetComponent<RectTransform>();
            SetupRect(textRect, Vector2.zero, Vector2.one, new Vector2(-22, 0), Vector2.zero);

            return root;
        }

        // Button - Text (No Platter)
        public static GameObject CreateTextButtonNoPlatter(Resources resources, float width, float height, float fontSize)
        {
            GameObject button = CreateTextButton(resources, width, height, fontSize);
            button.name = "Button - Text (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.buttonNoPlatterAnimatorController);

            Image background = button.GetComponent<Image>();
            background.color = s_WhiteTransparent;

            GameObject highlight = button.transform.Find("Highlight").gameObject;
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = s_WhiteTransparent;

            GameObject alphaBackground = button.transform.Find("Alpha Background").gameObject;
            Image alphaBackgroundImage = alphaBackground.GetComponent<Image>();
            alphaBackgroundImage.color = s_WhiteTransparent;

            return button;
        }

        // Button - Text+Symbol (Platter)
        public static GameObject CreateTextSymbolButton(Resources resources, float width, float height, float fontSize)
        {
            GameObject root = CreateUIElementRoot("Button - Text+Symbol", new Vector2(width, height), typeof(Image), typeof(Button), typeof(Animator));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
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

            float horizontalPadding = width > 60 ? 0 : (width - 60) / 2;
            float verticalPadding = height > 60 ? 0 : (height - 60) / 2;
            Vector4 padding = new Vector4(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);

            SetupImage(background, resources.buttonBackground, s_WindowElementColor, resources.lightElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.buttonBackground.texture.height / height);

            SetupWindowElement(root, alphaBackground, resources);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.buttonHighlight, s_WhiteTransparent, null, false, Vector4.zero, true, Image.Type.Sliced, true, resources.buttonHighlight.texture.height / height);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            SetupTextMeshProUGUI(textTMP, "Label", resources.fontSemibold, fontSize, Color.white, HorizontalAlignmentOptions.Left, VerticalAlignmentOptions.Middle, false, TextOverflowModes.Ellipsis);

            RectTransform textRect = text.GetComponent<RectTransform>();

            float sizeDeltaX = -(height + 11);
            float positionX = ((width - (height + 11)) / 2 + height) - width / 2;

            SetupRect(textRect, Vector2.zero, Vector2.one, new Vector2(sizeDeltaX, 0), new Vector2(positionX, 0));

            // Symbol
            Image symbolImage = symbol.GetComponent<Image>();
            symbolImage.sprite = resources.symbol;

            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            SetupRect(symbolRect, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(height, height), new Vector2(height / 2, 0));

            return root;
        }

        // Button - Text+Symbol (No Platter)
        public static GameObject CreateTextSymbolButtonNoPlatter(Resources resources, float width, float height, float fontSize)
        {
            GameObject button = CreateTextSymbolButton(resources, width, height, fontSize);
            button.name = "Button - Text+Symbol (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.symbolTextButtonNoPlatterController);

            Image background = button.GetComponent<Image>();
            background.color = s_WhiteTransparent;

            GameObject highlight = button.transform.Find("Highlight").gameObject;
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = s_WhiteTransparent;

            GameObject alphaBackground = button.transform.Find("Alpha Background").gameObject;
            Image alphaBackgroundImage = alphaBackground.GetComponent<Image>();
            alphaBackgroundImage.color = s_WhiteTransparent;

            return button;
        }

        // Button - Text Rounded Rect (Platter)
        public static GameObject CreateRoundedRectButton(Resources resources, float width, float height, float fontSize)
        {
            GameObject root = CreateUIElementRoot("Button - Text Rounded Rect", new Vector2(width, height), typeof(Image), typeof(Button), typeof(Animator));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
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

            float horizontalPadding = width > 60 ? 0 : (width - 60) / 2;
            float verticalPadding = height > 60 ? 0 : (height - 60) / 2;
            Vector4 padding = new Vector4(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);

            SetupImage(background, resources.roundedRectBackground, s_WindowElementColor, resources.lightElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.roundedRectBackground.texture.height / height);

            SetupWindowElement(root, alphaBackground, resources);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.roundedRectHighlight, s_WhiteTransparent, null, false, Vector4.zero, true, Image.Type.Sliced, true, resources.roundedRectHighlight.texture.height / height);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            SetupTextMeshProUGUI(textTMP, "Label", resources.fontSemibold, fontSize, Color.white, HorizontalAlignmentOptions.Center, VerticalAlignmentOptions.Middle, false, TextOverflowModes.Ellipsis);

            RectTransform textRect = text.GetComponent<RectTransform>();
            SetupRect(textRect, Vector2.zero, Vector2.one, new Vector2(-22, 0), Vector2.zero);

            return root;
        }

        // Button - Text Rounded Rect (No Platter)
        public static GameObject CreateRoundedRectButtonNoPlatter(Resources resources, float width, float height, float fontSize)
        {
            GameObject button = CreateRoundedRectButton(resources, width, height, fontSize);
            button.name = "Button - Text Rounded Rect (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.buttonNoPlatterAnimatorController);

            Image background = button.GetComponent<Image>();
            background.color = s_WhiteTransparent;

            GameObject highlight = button.transform.Find("Highlight").gameObject;
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = s_WhiteTransparent;

            GameObject alphaBackground = button.transform.Find("Alpha Background").gameObject;
            Image alphaBackgroundImage = alphaBackground.GetComponent<Image>();
            alphaBackgroundImage.color = s_WhiteTransparent;

            return button;
        }

        // Button - Symbol (Platter)
        public static GameObject CreateSymbolButton(Resources resources, float size)
        {
            GameObject root = CreateUIElementRoot("Button - Symbol", new Vector2(size, size), typeof(Image), typeof(Button), typeof(Animator));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
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

            float buttonPadding = size > 60 ? 0 : (size - 60) / 2;
            Vector4 padding = new Vector4(buttonPadding, buttonPadding, buttonPadding, buttonPadding);

            SetupImage(background, resources.buttonBackground, s_WindowElementColor, resources.lightElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.buttonBackground.texture.height / size);

            SetupWindowElement(root, alphaBackground, resources);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.symbolHighlight, s_WhiteTransparent, null, false, Vector4.zero, true, Image.Type.Sliced, true, resources.symbolHighlight.texture.height / size);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Symbol
            Image symbolImage = symbol.GetComponent<Image>();
            symbolImage.sprite = resources.symbol;

            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            SetupRect(symbolRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            return root;
        }

        // Button - Symbol (No Platter)
        public static GameObject CreateSymbolButtonNoPlatter(Resources resources, float size)
        {
            GameObject button = CreateSymbolButton(resources, size);
            button.name = "Button - Symbol (No Platter)";

            Animator buttonAnimator = button.GetComponent<Animator>();
            SetAnimatorController(buttonAnimator, resources.symbolNoPlatterAnimatorController);

            Image background = button.GetComponent<Image>();
            background.color = s_WhiteTransparent;

            GameObject highlight = button.transform.Find("Highlight").gameObject;
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = s_WhiteTransparent;

            GameObject alphaBackground = button.transform.Find("Alpha Background").gameObject;
            Image alphaBackgroundImage = alphaBackground.GetComponent<Image>();
            alphaBackgroundImage.color = s_WhiteTransparent;

            return button;
        }

        // Mini Slider 12px
        public static GameObject CreateMiniSlider(Resources resources, float height)
        {
            GameObject root = CreateUIElementRoot("Mini Slider", new Vector2(288f, height), typeof(Slider), typeof(Animator));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject mask = CreateUIObject("Mask", root, typeof(Mask), typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", mask, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));
            GameObject shadow = CreateUIObject("Shadow", fill, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", fill, typeof(Image));
            GameObject emboss = CreateUIObject("Emboss", root, typeof(Image));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(Image));
            GameObject glow = CreateUIObject("Glow", handle, typeof(Image));

            // Background
            Image backgroundImage = background.GetComponent<Image>();

            float verticalPadding = height > 60 ? 0 : (height - 60) / 2;
            Vector4 padding = new Vector4(0, verticalPadding, 0, verticalPadding);

            SetupImage(backgroundImage, resources.sliderElement, s_WindowElementColor, resources.darkElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            SetupRect(backgroundRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Mask
            Image maskImage = mask.GetComponent<Image>();
            SetupImage(maskImage, resources.sliderElement, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            Mask maskMask = mask.GetComponent<Mask>();
            maskMask.showMaskGraphic = false;

            RectTransform maskRect = mask.GetComponent<RectTransform>();
            SetupRect(maskRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            SetupRect(fillAreaRect, Vector2.zero, Vector2.one, new Vector2(-height, 0), Vector2.zero);

            // Fill
            Image fillImage = fill.GetComponent<Image>();
            SetupImage(fillImage, resources.sliderElement, new Vector4(1, 1, 1, 0.6f), null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(height, 0);

            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            SetupImage(shadowImage, resources.miniSliderShadow, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            SetupRect(shadowRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(21, 20), new Vector2(-1.55f, 0));

            //Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.sliderHighlight, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(96, 24), Vector2.zero);

            // Emboss
            Image embossImage = emboss.GetComponent<Image>();
            SetupImage(embossImage, resources.miniSliderEmboss, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.miniSliderEmboss.texture.height / height);

            RectTransform embossRect = emboss.GetComponent<RectTransform>();
            SetupRect(embossRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            SetupRect(handleAreaRect, Vector2.zero, Vector2.one, new Vector2(-height, 0), Vector2.zero);

            // Handle
            Image handleImage = handle.GetComponent<Image>();
            SetupImage(handleImage, resources.sliderElement, s_WhiteTransparent, null, true, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(8, -4);

            // Glow
            Image glowImage = glow.GetComponent<Image>();
            SetupImage(glowImage, resources.miniSliderGlow, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform glowRect = glow.GetComponent<RectTransform>();
            SetupRect(glowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(28, 28), Vector2.zero);

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
            SetAnimatorController(animator, resources.miniSliderAnimatorController);

            return root;
        }

        // Small Slider 16px
        public static GameObject CreateSmallSlider(Resources resources, float height)
        {
            GameObject root = CreateUIElementRoot("Small Slider", new Vector2(288f, height), typeof(Slider), typeof(Animator));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject mask = CreateUIObject("Mask", root, typeof(Mask), typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", mask, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));
            GameObject shadow = CreateUIObject("Shadow", fill, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", fill, typeof(Image));
            GameObject emboss = CreateUIObject("Emboss", root, typeof(Image));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(Image));
            GameObject glow = CreateUIObject("Glow", handle, typeof(Image));

            // Background
            Image backgroundImage = background.GetComponent<Image>();

            float verticalPadding = height > 60 ? 0 : (height - 60) / 2;
            Vector4 padding = new Vector4(0, verticalPadding, 0, verticalPadding);

            SetupImage(backgroundImage, resources.sliderElement, s_WindowElementColor, resources.darkElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            SetupRect(backgroundRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Mask
            Image maskImage = mask.GetComponent<Image>();
            SetupImage(maskImage, resources.sliderElement, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            Mask maskMask = mask.GetComponent<Mask>();
            maskMask.showMaskGraphic = false;

            RectTransform maskRect = mask.GetComponent<RectTransform>();
            SetupRect(maskRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            SetupRect(fillAreaRect, Vector2.zero, Vector2.one, new Vector2(-16, 0), Vector2.zero);

            // Fill
            Image fillImage = fill.GetComponent<Image>();
            SetupImage(fillImage, resources.sliderElement, new Vector4(1, 1, 1, 0.6f), null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(16, 0);

            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            SetupImage(shadowImage, resources.smallSliderShadow, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            SetupRect(shadowRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(25, 24), new Vector2(-3.33f, 0));

            //Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.sliderHighlight, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(160, 40), Vector2.zero);

            // Emboss
            Image embossImage = emboss.GetComponent<Image>();
            SetupImage(embossImage, resources.smallSliderEmboss, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.smallSliderEmboss.texture.height / height);

            RectTransform embossRect = emboss.GetComponent<RectTransform>();
            SetupRect(embossRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            SetupRect(handleAreaRect, Vector2.zero, Vector2.one, new Vector2(-16, 0), Vector2.zero);

            // Handle
            Image handleImage = handle.GetComponent<Image>();
            SetupImage(handleImage, resources.sliderElement, s_WhiteTransparent, null, true, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(12, -4);

            // Glow
            Image glowImage = glow.GetComponent<Image>();
            SetupImage(glowImage, resources.smallSliderGlow, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform glowRect = glow.GetComponent<RectTransform>();
            SetupRect(glowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(32, 32), Vector2.zero);

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
        public static GameObject CreateRegularSlider(Resources resources, float height)
        {
            GameObject root = CreateUIElementRoot("Regular Slider", new Vector2(288f, height), typeof(Slider), typeof(Animator));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject mask = CreateUIObject("Mask", root, typeof(Mask), typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", mask, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));
            GameObject shadow = CreateUIObject("Shadow", fill, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", fill, typeof(Image));
            GameObject symbol = CreateUIObject("Symbol", root, typeof(Image));
            GameObject emboss = CreateUIObject("Emboss", root, typeof(Image));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(Image));
            GameObject glow = CreateUIObject("Glow", handle, typeof(Image));

            // Background
            Image backgroundImage = background.GetComponent<Image>();
            float verticalPadding = height > 60 ? 0 : (height - 60) / 2;
            Vector4 padding = new Vector4(0, verticalPadding, 0, verticalPadding);

            SetupImage(backgroundImage, resources.sliderElement, s_WindowElementColor, resources.darkElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            SetupRect(backgroundRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Mask
            Image maskImage = mask.GetComponent<Image>();
            SetupImage(maskImage, resources.sliderElement, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            Mask maskMask = mask.GetComponent<Mask>();
            maskMask.showMaskGraphic = false;

            RectTransform maskRect = mask.GetComponent<RectTransform>();
            SetupRect(maskRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            SetupRect(fillAreaRect, Vector2.zero, Vector2.one, new Vector2(-height, 0), Vector2.zero);

            // Fill
            Image fillImage = fill.GetComponent<Image>();
            SetupImage(fillImage, resources.sliderElement, new Vector4(1, 1, 1, 0.6f), null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(height, 0);

            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            SetupImage(shadowImage, resources.regularSliderShadow, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            SetupRect(shadowRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(37, 36), new Vector2(-9.53f, 0));

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.sliderHighlight, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(208, 52), Vector2.zero);

            // Symbol
            Image symbolImage = symbol.GetComponent<Image>();
            SetupImage(symbolImage, resources.symbol, Color.black, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            SetupRect(symbolRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(height, height), new Vector2(height / 2, 0));

            // Emboss
            Image embossImage = emboss.GetComponent<Image>();
            SetupImage(embossImage, resources.regularSliderEmboss, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.regularSliderEmboss.texture.height / height);

            RectTransform embossRect = emboss.GetComponent<RectTransform>();
            SetupRect(embossRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            SetupRect(handleAreaRect, Vector2.zero, Vector2.one, new Vector2(-height, 0), Vector2.zero);

            // Handle
            Image handleImage = handle.GetComponent<Image>();
            SetupImage(handleImage, resources.sliderElement, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            SetupRect(handleRect, Vector2.zero, Vector2.one, new Vector2(20, -8), Vector2.zero);

            // Glow
            Image glowImage = glow.GetComponent<Image>();
            SetupImage(glowImage, resources.regularSliderGlow, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform glowRect = glow.GetComponent<RectTransform>();
            SetupRect(glowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(52, 52), Vector2.zero);

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

        // Large Slider 44px
        public static GameObject CreateLargeSlider(Resources resources, float height)
        {
            GameObject root = CreateUIElementRoot("Large Slider", new Vector2(288f, height), typeof(Slider), typeof(Animator));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject mask = CreateUIObject("Mask", root, typeof(Mask), typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", mask, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));
            GameObject shadow = CreateUIObject("Shadow", fill, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", fill, typeof(Image));
            GameObject symbol = CreateUIObject("Symbol", root, typeof(Image));
            GameObject emboss = CreateUIObject("Emboss", root, typeof(Image));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(Image));
            GameObject glow = CreateUIObject("Glow", handle, typeof(Image));

            // Background
            Image backgroundImage = background.GetComponent<Image>();
            float verticalPadding = height > 60 ? 0 : (height - 60) / 2;
            Vector4 padding = new Vector4(0, verticalPadding, 0, verticalPadding);

            SetupImage(backgroundImage, resources.sliderElement, s_WindowElementColor, resources.darkElementMaterial, true, padding, true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            SetupRect(backgroundRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Mask
            Image maskImage = mask.GetComponent<Image>();
            SetupImage(maskImage, resources.sliderElement, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            Mask maskMask = mask.GetComponent<Mask>();
            maskMask.showMaskGraphic = false;

            RectTransform maskRect = mask.GetComponent<RectTransform>();
            SetupRect(maskRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            SetupRect(fillAreaRect, Vector2.zero, Vector2.one, new Vector2(-height, 0), Vector2.zero);

            // Fill
            Image fillImage = fill.GetComponent<Image>();
            SetupImage(fillImage, resources.sliderElement, new Vector4(1, 1, 1, 0.6f), null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.sliderElement.texture.height / height);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(height, 0);

            // Shadow
            Image shadowImage = shadow.GetComponent<Image>();
            SetupImage(shadowImage, resources.largeSliderShadow, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            SetupRect(shadowRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(53, 52), new Vector2(-17.5f, 0));

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            SetupImage(highlightImage, resources.sliderHighlight, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(352, 88), Vector2.zero);

            // Symbol
            Image symbolImage = symbol.GetComponent<Image>();
            SetupImage(symbolImage, resources.symbol, Color.black, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform symbolRect = symbol.GetComponent<RectTransform>();
            SetupRect(symbolRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(height, height), new Vector2(height / 2, 0));

            // Emboss
            Image embossImage = emboss.GetComponent<Image>();
            SetupImage(embossImage, resources.largeSliderEmboss, s_WindowElementColor, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Sliced, true, resources.largeSliderEmboss.texture.height / height);

            RectTransform embossRect = emboss.GetComponent<RectTransform>();
            SetupRect(embossRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            SetupRect(handleAreaRect, Vector2.zero, Vector2.one, new Vector2(-height, 0), Vector2.zero);

            // Handle
            Image handleImage = handle.GetComponent<Image>();
            SetupImage(handleImage, resources.sliderElement, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            SetupRect(handleRect, Vector2.zero, Vector2.one, new Vector2(32, -12), Vector2.zero);

            // Glow
            Image glowImage = glow.GetComponent<Image>();
            SetupImage(glowImage, resources.largeSliderGlow, s_WhiteTransparent, null, false, new Vector4(0, 0, 0, 0), true, Image.Type.Simple, true, 0);

            RectTransform glowRect = glow.GetComponent<RectTransform>();
            SetupRect(glowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(76, 76), Vector2.zero);

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
            SetAnimatorController(animator, resources.largeSliderAnimatorController);

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

        // Window
        public static GameObject CreateWindow(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Window", new Vector2(1306f, 734f), typeof(Image));

            GameObject shadow = CreateUIObject("Shadow", root, typeof(Image));
            GameObject windowBottom = CreateUIObject("Window Bottom", root, typeof(RectTransform));
            GameObject floorShadow = CreateUIObject("Floor Shadow", windowBottom, typeof(Image), typeof(ShadowPlacer));

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
            shadowImage.pixelsPerUnitMultiplier = 6.1f;

            RectTransform shadowRect = shadow.GetComponent<RectTransform>();
            shadowRect.anchorMin = Vector2.zero;
            shadowRect.anchorMax = Vector2.one;
            shadowRect.sizeDelta = new Vector2(24, 24);

            // Window Bottom
            RectTransform windowBottomRect = windowBottom.GetComponent<RectTransform>();
            SetupRect(windowBottomRect, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 0));

            // Floor Shadow
            Image floorShadowImage = floorShadow.GetComponent<Image>();
            floorShadowImage.sprite = resources.windowFloorShadow;
            floorShadowImage.color = Color.black;
            floorShadowImage.type = Image.Type.Sliced;
            floorShadowImage.pixelsPerUnitMultiplier = 12;
            floorShadowImage.raycastTarget = false;

            RectTransform floorShadowRect = floorShadow.GetComponent<RectTransform>();
            SetupRect(floorShadowRect, Vector2.zero, new Vector2(1, 0), new Vector2(0, 100), new Vector2(0, -114));
            floorShadowRect.eulerAngles = new Vector3(90, -0, 0);

            return root;
        }

        // Toggle
        public static GameObject CreateToggle(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Toggle", new Vector2(56f, 32f), typeof(Toggle), typeof(Animator), typeof(ToggleAnimation));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject alphaBackground = CreateUIObject("Alpha Background", background, typeof(Image));
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
            backgroundImage.color = s_WindowElementColor;
            backgroundImage.raycastPadding = new Vector4(-2, -14, -2, -14);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            SetupWindowElement(background, alphaBackground, resources);

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
            background.color = Color.white;
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

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject textArea = CreateUIObject("Text Area", root, typeof(RectTransform));
            GameObject childPlaceholder = CreateUIObject("Placeholder", textArea, typeof(TextMeshProUGUI));
            GameObject childText = CreateUIObject("Text", textArea, typeof(TextMeshProUGUI));
            GameObject clearButton = CreateUIObject("Clear Button", root, typeof(Button), typeof(Image));
            GameObject clearButtonAlphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
            GameObject clearButtonCross = CreateUIObject("Cleaar Button Cross", clearButton, typeof(Image));

            // Background
            Image image = root.GetComponent<Image>();
            image.sprite = resources.inputFieldBackground;
            image.material = resources.darkElementMaterial;
            image.color = s_WindowElementColor;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 4;
            image.raycastPadding = new Vector4(0, -8, 0, -8);

            SetupWindowElement(root, alphaBackground, resources);

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
            clearButtonImage.color = s_WindowElementColor;
            clearButtonImage.raycastPadding = new Vector4(-16, -16, -16, -16);

            RectTransform clearButtonRect = clearButton.GetComponent<RectTransform>();
            clearButtonRect.anchorMin = new Vector2(1, 0.5f);
            clearButtonRect.anchorMax = new Vector2(1, 0.5f);
            clearButtonRect.sizeDelta = new Vector2(28, 28);
            clearButtonRect.anchoredPosition = new Vector2(-22, 0);

            SetupWindowElement(clearButton, clearButtonAlphaBackground, resources);

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
            GameObject button1 = CreateSymbolButtonNoPlatter(resources, 44);
            SetParentAndAlign(button1, root);

            // Button 2
            GameObject button2 = CreateSymbolButtonNoPlatter(resources, 44);
            SetParentAndAlign(button2, root);

            // Separator
            GameObject separator = CreateVerticalSeparator(resources);
            SetParentAndAlign(separator, root);

            // Button 3
            GameObject button3 = CreateSymbolButtonNoPlatter(resources, 44);
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

            // Action Buttons
            for (int i = 0; i < 2; i++)
            {
                GameObject button = CreateRoundedRectButton(resources, 86, 44, 17);
                GameObject highlight = button.transform.Find("Highlight").gameObject;
                GameObject text = button.transform.Find("Text").gameObject;
                button.name = "Action Button";
                SetParentAndAlign(button, root);

                RectTransform action2Rect = button.GetComponent<RectTransform>();
                action2Rect.sizeDelta = new Vector2(280, 44);

                Animator action2Animator = button.GetComponent<Animator>();
                SetAnimatorController(action2Animator, resources.buttonNoPlatterAnimatorController);

                Image actionImage = button.GetComponent<Image>();
                actionImage.color = new Color(1f, 1f, 1f, 0f);

                Image highlightImage = highlight.GetComponent<Image>();
                highlightImage.color = new Color(1f, 1f, 1f, 0f);

                TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
                textTMP.text = "Action";

                GameObject alphaBackground = button.transform.Find("Alpha Background").gameObject;
                Image alphaBackgroundImage = alphaBackground.GetComponent<Image>();
                alphaBackgroundImage.color = new Color(1, 1, 1, 0);
            }

            return root;
        }

        // Tabbar Toggle
        public static GameObject CreateTabbarToggle(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Toggle", new Vector2(44f, 44f), typeof(Image), typeof(Toggle), typeof(Animator), typeof(ToggleAnimation));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject highlightAlphaBackground = CreateUIObject("Alpha Background", highlight, typeof(Image));
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
            toggleImage.color = s_WindowElementColor;
            toggleImage.material = resources.lightElementMaterial;
            toggleImage.type = Image.Type.Sliced;
            toggleImage.pixelsPerUnitMultiplier = 4;
            toggleImage.raycastPadding = new Vector4(-8, -8, -8, -8);

            SetupWindowElement(root, alphaBackground, resources);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.symbolHighlight;
            highlightImage.material = resources.lightElementMaterial;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);
            highlightImage.type = Image.Type.Sliced;
            highlightImage.pixelsPerUnitMultiplier = 4;
            highlightImage.raycastTarget = false;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(0, 0f);
            highlightRect.anchorMax = new Vector2(1, 1f);
            highlightRect.sizeDelta = new Vector2(0, 0);

            SetupWindowElement(highlight, highlightAlphaBackground, resources);

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
            GameObject root = CreateUIElementRoot("Tabbar", new Vector2(68, 120), typeof(VerticalLayoutGroup), typeof(ToggleGroup), typeof(UpdateChildTogglesOnAwake), typeof(TabbarAnimation));

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
            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", root, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = resources.regularSliderBackground;
            backgroundImage.material = resources.darkElementMaterial;
            backgroundImage.color = s_WindowElementColor;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.pixelsPerUnitMultiplier = 11;

            SetupWindowElement(background, alphaBackground, resources);

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
            GameObject root = CreateUIElementRoot("Sidebar", new Vector2(362, -3.2f), typeof(Image));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));

            Image Image = root.GetComponent<Image>();
            Image.sprite = resources.sidebar;
            Image.material = resources.darkElementMaterial;
            Image.color = s_WindowElementColor;
            Image.type = Image.Type.Sliced;
            Image.pixelsPerUnitMultiplier = 2;
            Image.raycastTarget = false;

            SetupWindowElement(root, alphaBackground, resources);

            return root;
        }

        public static GameObject CreateWindowsStacker(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Windows Stacker", new Vector2(1306f, 734f), typeof(RectTransform), typeof(WindowsStacker));

            var windowControls = CreateWindowControls(resources);
            SetParentAndAlign(windowControls, root);

            RectTransform windowControlsRect = windowControls.GetComponent<RectTransform>();
            windowControlsRect.anchorMin = new Vector2(0, 0);
            windowControlsRect.anchorMax = new Vector2(1, 0);
            windowControlsRect.pivot = new Vector2(0.5f, 1);
            windowControlsRect.sizeDelta = new Vector2(0, 14);
            windowControlsRect.anchoredPosition = new Vector2(0, -22);

            Grabber grabber = windowControls.GetComponentInChildren<Grabber>();
            grabber.SetReferences(root.transform);

            GameObject resizerR = windowControls.transform.Find("Window Right Resizer").gameObject;
            RectTransform resizerRRect = resizerR.GetComponent<RectTransform>();
            resizerRRect.anchoredPosition = new Vector2(-32, -10.5f);

            GameObject resizerL = windowControls.transform.Find("Window Left Resizer").gameObject;
            RectTransform resizerLRect = resizerL.GetComponent<RectTransform>();
            resizerLRect.anchoredPosition = new Vector2(32, -10.5f);

            GameObject window = CreateWindow(resources);
            SetParentAndAlign(window, root);

            WindowsStacker windowsStacker = root.GetComponent<WindowsStacker>();
            windowsStacker.SetReferences(window, windowControls.transform);

            return root;
        }

        public static GameObject CreateWindowControls(Resources resources)
        {
            //Window Controls
            GameObject root = CreateUIElementRoot("Window Controls", new Vector2(174, 14), typeof(RectTransform), typeof(Animator));
            GameObject closeButtonWindow = CreateUIObject("Close Button Window", root, typeof(Image), typeof(Button), typeof(Animator));
            GameObject grabberObjectWindow = CreateUIObject("Grabber Window", root, typeof(Image), typeof(Button), typeof(Animator));
            GameObject closeButton = CreateUIObject("Close Button", closeButtonWindow, typeof(Image));
            GameObject closeButtonAlphaBackground = CreateUIObject("Alpha Background", closeButtonWindow, typeof(Image));
            GameObject crossIcon = CreateUIObject("Cross Icon", closeButtonWindow, typeof(Image));
            GameObject grabberObject = CreateUIObject("Grabber", grabberObjectWindow, typeof(Image), typeof(BoxCollider), typeof(XRSimpleInteractable), typeof(Grabber));
            GameObject grabberAlphaBackground = CreateUIObject("Alpha Background", grabberObjectWindow, typeof(Image));

            // Animation
            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.resizerController);

            //Close Button Window
            RectTransform closeButtonWindowRect = closeButtonWindow.GetComponent<RectTransform>();
            closeButtonWindowRect.anchorMin = new Vector2(0.5f, 0);
            closeButtonWindowRect.anchorMax = new Vector2(0.5f, 0);
            closeButtonWindowRect.sizeDelta = new Vector2(14, 14);
            closeButtonWindowRect.anchoredPosition = new Vector2(-80, 7);

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
            grabberWindowRect.anchorMin = new Vector2(0.5f, 0);
            grabberWindowRect.anchorMax = new Vector2(0.5f, 0);
            grabberWindowRect.sizeDelta = new Vector2(136, 10);
            grabberWindowRect.anchoredPosition = new Vector2(19, 7);

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
            closeButtonImage.color = s_WindowElementColor;
            closeButtonImage.raycastPadding = new Vector4(-23, -23, -23, -23);

            SetupWindowElement(closeButton, closeButtonAlphaBackground, resources);

            //Cross Icon
            RectTransform crossIconRect = crossIcon.GetComponent<RectTransform>();
            crossIconRect.anchorMin = new Vector2(0, 0);
            crossIconRect.anchorMax = new Vector2(1, 1);
            crossIconRect.sizeDelta = new Vector2(0, 0);
            crossIconRect.anchoredPosition = new Vector2(0, 0);

            Image crossIconImage = crossIcon.GetComponent<Image>();
            crossIconImage.sprite = resources.crossIcon;
            crossIconImage.color = new Color(0, 0, 0, 1);
            crossIconImage.raycastTarget = false;

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
            grabberImage.color = s_WindowElementColor;
            grabberImage.raycastPadding = new Vector4(0, -25, 0, -25);

            BoxCollider grabberCollider = grabberObject.GetComponent<BoxCollider>();
            grabberCollider.size = new Vector3(136, 60, 1);

            SetupWindowElement(grabberObject, grabberAlphaBackground, resources);

            //Right Resizer
            var rightResizer = CreateWindowResizer(resources);
            SetParentAndAlign(rightResizer, root);
            rightResizer.name = "Window Right Resizer";

            RectTransform windowRightResizerRect = rightResizer.GetComponent<RectTransform>();
            windowRightResizerRect.pivot = new Vector2(0, 0);
            windowRightResizerRect.anchorMin = new Vector2(1, 0);
            windowRightResizerRect.anchorMax = new Vector2(1, 0);
            windowRightResizerRect.sizeDelta = new Vector2(75, 75);
            windowRightResizerRect.anchoredPosition = new Vector2(-32, 37.5f);

            //Left Resizer
            var leftResizer = CreateWindowResizer(resources);
            SetParentAndAlign(leftResizer, root);
            leftResizer.name = "Window Left Resizer";

            RectTransform windowLeftResizerRect = leftResizer.GetComponent<RectTransform>();
            windowLeftResizerRect.pivot = new Vector2(0, 0);
            windowLeftResizerRect.anchorMin = Vector2.zero;
            windowLeftResizerRect.anchorMax = Vector2.zero;
            windowLeftResizerRect.sizeDelta = new Vector2(75, 75);
            windowLeftResizerRect.anchoredPosition = new Vector2(32, 37.5f);
            windowLeftResizerRect.localScale = new Vector3(-1, 1, 1);

            return root;
        }

        // Segmented Control
        public static GameObject CreateSegmentedControl(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Segmented Control", new Vector2(188, 44), typeof(Image), typeof(HorizontalLayoutGroup), typeof(ToggleGroup));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));

            Image segmentedControlImage = root.GetComponent<Image>();
            SetupImage(segmentedControlImage, resources.buttonBackground, s_WindowElementColor, resources.darkElementMaterial, false, Vector4.zero, true, Image.Type.Sliced, true, 4);

            HorizontalLayoutGroup segmentedControlHorizontalLayoutGroup = root.GetComponent<HorizontalLayoutGroup>();
            SetupLayoutGroup(segmentedControlHorizontalLayoutGroup, new RectOffset(4, 4, 4, 4), 4, TextAnchor.MiddleCenter, true, true, false, false, true, true);

            ToggleGroup segmentedControlToggleGroup = root.GetComponent<ToggleGroup>();

            SetupWindowElement(root, alphaBackground, resources);

            for (int i = 0; i < 2; i++)
            {
                GameObject toggleObject = CreateUIObject("Toggle", root, typeof(Image), typeof(Toggle), typeof(Animator), typeof(ToggleAnimation));
                GameObject toggleAlphaBackground = CreateUIObject("Alpha Background", toggleObject, typeof(Image));
                GameObject highlight = CreateUIObject("Highlight", toggleObject, typeof(Image));
                GameObject title = CreateUIObject("Title", toggleObject, typeof(HorizontalLayoutGroup));
                GameObject text = CreateUIObject("Text", title, typeof(TextMeshProUGUI));

                // Toggle

                Image toggleImage = toggleObject.GetComponent<Image>();
                SetupImage(toggleImage, resources.buttonBackground, s_WindowElementColor, resources.lightElementWithFrameMaterial, true, Vector4.zero, true, Image.Type.Sliced, true, 4.888889f);

                Toggle toggle = toggleObject.GetComponent<Toggle>();
                SetupToggle(toggle, Selectable.Transition.Animation, Navigation.Mode.None, i == 0, toggleImage, segmentedControlToggleGroup);

                Animator animator = toggleObject.GetComponent<Animator>();
                SetAnimatorController(animator, resources.tabbarToggleController);

                SetupWindowElement(toggleObject, toggleAlphaBackground, resources);

                // Highlight
                RectTransform highlightRect = highlight.GetComponent<RectTransform>();
                SetupRect(highlightRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

                Image highlightImage = highlight.GetComponent<Image>();
                SetupImage(highlightImage, resources.segmentedControlHighlight, new Color(1, 1, 1, 0), null, true, Vector4.zero, true, Image.Type.Sliced, true, 4f);

                // Title
                RectTransform titleRect = title.GetComponent<RectTransform>();
                SetupRect(titleRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

                HorizontalLayoutGroup titleHorizontalLayoutGroup = title.GetComponent<HorizontalLayoutGroup>();
                SetupLayoutGroup(titleHorizontalLayoutGroup, new RectOffset(0, 0, 0, 0), 0, TextAnchor.MiddleCenter, false, false, false, false, false, false);

                // Text
                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.sizeDelta = new Vector2(41, 36);

                TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
                SetupTextMeshProUGUI(textTMP, "Label", resources.fontSemibold, 15, s_TextColorPrimary, HorizontalAlignmentOptions.Center, VerticalAlignmentOptions.Middle, false, TextOverflowModes.Ellipsis);
            }

            return root;
        }

        #region List
        //Completed List
        public static GameObject CreateCompletedList(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Completed List", new Vector2(0f, 298f), typeof(VerticalLayoutGroup));

            VerticalLayoutGroup completedListRectVerticalLayoutGroup = root.GetComponent<VerticalLayoutGroup>();
            SetupLayoutGroup(completedListRectVerticalLayoutGroup, new RectOffset(12, 12, 0, 0), 0, TextAnchor.UpperLeft, true, false, false, false, true, true);

            GameObject headerText = CreateUIObject("Header Text", root, typeof(TextMeshProUGUI));

            RectTransform headerTextRect = headerText.GetComponent<RectTransform>();
            headerTextRect.sizeDelta = new Vector2(0, 32);

            TextMeshProUGUI headerTextTMP = headerText.GetComponent<TextMeshProUGUI>();
            SetupTextMeshProUGUI(headerTextTMP, "Header Text", resources.fontBold, 19, s_TextColorPrimary, HorizontalAlignmentOptions.Left, VerticalAlignmentOptions.Top, false, TextOverflowModes.Ellipsis);
            headerTextTMP.margin = new Vector4(20, 0, 20, 8);

            for (int i = 0; i < 4; i++)
            {
                GameObject listElement = CreateListElement(resources);
                SetParentAndAlign(listElement, root);
            }

            GameObject footerText = CreateUIObject("Footer Text", root, typeof(TextMeshProUGUI));

            RectTransform footerTextRect = footerText.GetComponent<RectTransform>();
            footerTextRect.sizeDelta = new Vector2(0, 26);

            TextMeshProUGUI footerTextTMP = footerText.GetComponent<TextMeshProUGUI>();
            SetupTextMeshProUGUI(footerTextTMP, "Footer Text", resources.fontMedium, 13, s_TextColorSecondary, HorizontalAlignmentOptions.Left, VerticalAlignmentOptions.Top, false, TextOverflowModes.Ellipsis);
            footerTextTMP.margin = new Vector4(20, 8, 20, 0);

            return root;
        }

        // List Element
        public static GameObject CreateListElement(Resources resources)
        {
            GameObject root = CreateUIElementRoot("List Element", new Vector2(460f, 60f), typeof(Image), typeof(Button), typeof(Animator));

            GameObject alphaBackground = CreateUIObject("Alpha Background", root, typeof(Image));
            GameObject highlight = CreateUIObject("Highlight", root, typeof(Image));
            GameObject text = CreateUIObject("Text", root, typeof(TextMeshProUGUI));
            GameObject arrow = CreateUIObject("Arrow", root, typeof(Image));

            Button button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            button.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.listElementAnimatorController);

            Image background = root.GetComponent<Image>();
            SetupImage(background, resources.roundedRectBackground, s_WindowElementColor, resources.darkElementMaterial, true, Vector4.zero, true, Image.Type.Sliced, true, 4);

            ListElement listElement = root.AddComponent<ListElement>();
            listElement.SetReferences(background, resources.singleListElement, resources.firstListElement, resources.middleListElement, resources.lastListElement,
                new List<Image> {
                    alphaBackground.GetComponent<Image>()
                });

            SetupWindowElement(root, alphaBackground, resources);

            // Highlight
            Image highlightImage = highlight.GetComponent<Image>();
            highlightImage.sprite = resources.listElementHighlight;
            highlightImage.color = new Color(1f, 1f, 1f, 0f);

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            SetupRect(highlightRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Text
            TextMeshProUGUI textTMP = text.GetComponent<TextMeshProUGUI>();
            SetupTextMeshProUGUI(textTMP, "Title", resources.fontMedium, 17, s_TextColorPrimary, HorizontalAlignmentOptions.Left, VerticalAlignmentOptions.Middle, false, TextOverflowModes.Ellipsis);

            RectTransform textRect = text.GetComponent<RectTransform>();
            SetupRect(textRect, Vector2.zero, Vector2.one, new Vector2(-59f, 44f), new Vector2(-9.5f, 0));

            // Arrow
            Image arrowImage = arrow.GetComponent<Image>();
            arrowImage.sprite = resources.listElementArrow;
            arrowImage.color = new Color(1, 1, 1, 0.25f);

            RectTransform arrowRect = arrow.GetComponent<RectTransform>();
            SetupRect(arrowRect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(11f, 44f), new Vector2(-25.5f, 0));

            return root;
        }

        public static GameObject CreateListElementNoPlatter(Resources resources)
        {
            GameObject listElement = CreateListElement(resources);
            listElement.name = "List Element (No Platter)";

            Image background = listElement.GetComponent<Image>();
            background.material = resources.lightElementMaterial;
            background.color = new Color(1, 1, 1, 0);

            return listElement;
        }
        #endregion

        //Volume
        public static GameObject CreateVolume(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Volume", new Vector2(64, 64), typeof(Image), typeof(Button), typeof(Animator), typeof(SpriteNumberSwitcher));

            GameObject slider = CreateLargeSlider(resources, 44);

            //Slider
            slider.name = "Slider";
            SetParentAndAlign(slider, root);

            Slider sliderSlider = slider.GetComponent<Slider>();
            sliderSlider.value = 0.5f;

            GameObject sliderBackground = slider.transform.Find("Background").gameObject;
            Image sliderBackgroundImage = sliderBackground.GetComponent<Image>();
            sliderBackgroundImage.material = resources.windowBlurredBackgroundMaterial;

            GameObject symbol = slider.transform.Find("Symbol").gameObject;
            symbol.SetActive(false);

            RectTransform sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.localRotation = Quaternion.Euler(0, 0, 90);
            sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = Vector2.zero;

            slider.SetActive(false);

            // Animation
            Button button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            Animator animator = root.GetComponent<Animator>();
            SetAnimatorController(animator, resources.volumeController);

            // Image
            Image symbolImage = root.GetComponent<Image>();
            symbolImage.raycastPadding = new Vector4(0, -8, 0, -8);

            // Sprite Switcher
            SpriteNumberSwitcher symbolSwitcher = root.GetComponent<SpriteNumberSwitcher>();
            symbolSwitcher.Sprites = new Sprite[] { resources.speakerSlash, resources.speaker1, resources.speaker2, resources.speaker3 };
            symbolSwitcher.Target = symbolImage;
            symbolSwitcher.SetValue(sliderSlider.value);

            return root;
        }

        //Window Resizer
        public static GameObject CreateWindowResizer(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Window Resizer", new Vector2(75, 75), typeof(RectTransform));

            GameObject positionPivot = CreateUIObject("Position Pivot", root, typeof(RectTransform));
            GameObject trailing = CreateUIObject("Trailing", positionPivot, typeof(Image));
            GameObject line = CreateUIObject("Line", root, typeof(Image));
            GameObject interactable = CreateUIObject("Interactable", root, typeof(Image), typeof(Resizer), typeof(XRSimpleInteractable), typeof(BoxCollider));

            // Position Pivot
            RectTransform positionPivotRect = positionPivot.GetComponent<RectTransform>();
            SetupRect(positionPivotRect, new Vector2(1, 0), new Vector2(1, 0), new Vector2(75, 75), new Vector2(-75, 75));
            positionPivotRect.pivot = new Vector2(0, 1);

            // Trailing
            Image trailingImage = trailing.GetComponent<Image>();
            trailingImage.sprite = resources.trailing;
            trailingImage.color = Color.white;

            RectTransform trailingRect = trailing.GetComponent<RectTransform>();
            SetupRect(trailingRect, Vector2.zero, Vector2.zero, new Vector2(62.5f, 62.5f), Vector2.zero);
            trailingRect.pivot = Vector2.zero;
            trailingRect.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            // Line
            Image lineImage = line.GetComponent<Image>();
            lineImage.sprite = resources.buttonBackground;
            lineImage.color = s_WhiteTransparent;
            lineImage.type = Image.Type.Sliced;
            lineImage.pixelsPerUnitMultiplier = 18;

            RectTransform lineRect = line.GetComponent<RectTransform>();
            SetupRect(lineRect, Vector2.zero, Vector2.zero, new Vector2(20, 10), new Vector2(-200, 5));

            // Interactable
            Image interactableImage = interactable.GetComponent<Image>();
            interactableImage.color = new Color(1, 1, 1, 0);

            BoxCollider interactableCollider = interactable.GetComponent<BoxCollider>();
            interactableCollider.size = new Vector3(100, 100, 1);

            return root;
        }

        #region Setup Methods
        private static void SetupWindowElement(GameObject background, GameObject alphaBackground, Resources resources)
        {
            if (!alphaBackground.TryGetComponent<LayoutElement>(out var layoutElement))
                layoutElement = alphaBackground.AddComponent<LayoutElement>();

            layoutElement.ignoreLayout = true;

            RectTransform alphaBackgroundRect = alphaBackground.GetComponent<RectTransform>();
            SetupRect(alphaBackgroundRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Image alphaBackgroundImage = alphaBackground.GetComponent<Image>();
            Image backgroundImage = background.GetComponent<Image>();

            SetupImage(alphaBackgroundImage, backgroundImage.sprite, new Color(1, 1, 1, 0), resources.alphaBackgroundMaterial, false, backgroundImage.raycastPadding,
                backgroundImage.maskable, backgroundImage.type, backgroundImage.fillCenter, backgroundImage.pixelsPerUnitMultiplier);
        }

        public static void SetupRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        public static void SetupImage(Image image, Sprite sprite, Color color, Material material, bool raycastTarget, Vector4 raycastPadding, bool maskable,
            Image.Type type, bool fillCenter, float pixelsPerUnitMultiplier)
        {
            image.sprite = sprite;
            image.color = color;
            image.material = material;
            image.raycastTarget = raycastTarget;
            image.raycastPadding = raycastPadding;
            image.maskable = maskable;
            image.type = type;
            image.fillCenter = fillCenter;
            image.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
        }

        public static void SetupTextMeshProUGUI(TextMeshProUGUI textMeshProUGUI, string text, TMP_FontAsset font, float fontSize, Color color, HorizontalAlignmentOptions horizontalAlignment,
            VerticalAlignmentOptions verticalAlignment, bool enableWordWrapping, TextOverflowModes overflowMode)
        {
            textMeshProUGUI.text = text;
            textMeshProUGUI.font = font;
            textMeshProUGUI.fontSize = fontSize;
            textMeshProUGUI.color = color;
            textMeshProUGUI.horizontalAlignment = horizontalAlignment;
            textMeshProUGUI.verticalAlignment = verticalAlignment;
            textMeshProUGUI.enableWordWrapping = enableWordWrapping;
            textMeshProUGUI.overflowMode = overflowMode;
        }

        public static void SetupToggle(Toggle toggle, Selectable.Transition transition, Navigation.Mode mode, bool isOn, Image graphic, ToggleGroup toggleGroup)
        {
            toggle.transition = transition;
            Navigation navigation = toggle.navigation;
            navigation.mode = mode;
            toggle.navigation = navigation;
            toggle.isOn = isOn;
            toggle.graphic = graphic;
            toggle.group = toggleGroup;
        }

        public static void SetupLayoutGroup(HorizontalOrVerticalLayoutGroup horizontalLayoutGroup, RectOffset padding, float spacing, TextAnchor childAligment,
            bool childControlWidth, bool childControlHeight, bool childScaleWidth, bool childScaleHeight, bool childForceExpandWidth, bool childForceExpandHeight)
        {
            horizontalLayoutGroup.padding = padding;
            horizontalLayoutGroup.spacing = spacing;
            horizontalLayoutGroup.childAlignment = childAligment;
            horizontalLayoutGroup.childControlWidth = childControlWidth;
            horizontalLayoutGroup.childControlHeight = childControlHeight;
            horizontalLayoutGroup.childScaleWidth = childScaleWidth;
            horizontalLayoutGroup.childScaleHeight = childScaleHeight;
            horizontalLayoutGroup.childForceExpandWidth = childForceExpandWidth;
            horizontalLayoutGroup.childForceExpandHeight = childForceExpandHeight;
        }
        #endregion
    }
}