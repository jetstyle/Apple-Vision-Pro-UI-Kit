using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using UnityEditor;
using TMPro;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Reflection;

namespace JetXR.VisionUI.Editor
{
    static internal class VisionMenuOptions
    {
        #region Utilities
        // Copied from UnityEditor.UI.MenuOptions

        private const string kUILayerName = "UI";

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            bool explicitParentChoice = true;
            if (parent == null)
            {
                parent = GetOrCreateCanvasGameObject();
                explicitParentChoice = false;

                // If in Prefab Mode, Canvas has to be part of Prefab contents,
                // otherwise use Prefab root instead.
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
                    parent = prefabStage.prefabContentsRoot;
            }
            if (parent.GetComponentsInParent<Canvas>(true).Length == 0)
            {
                // Create canvas under context GameObject,
                // and make that be the parent which UI element is added under.
                GameObject canvas = VisionMenuOptions.CreateNewUI();
                Undo.SetTransformParent(canvas.transform, parent.transform, "");
                parent = canvas;
            }

            GameObjectUtility.EnsureUniqueNameForSibling(element);

            SetParentAndAlign(element, parent);
            if (!explicitParentChoice) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

            // This call ensure any change made to created Objects after they where registered will be part of the Undo.
            Undo.RegisterFullObjectHierarchyUndo(parent == null ? element : parent, "");

            // We have to fix up the undo name since the name of the object was only known after reparenting it.
            Undo.SetCurrentGroupName("Create " + element.name);

            Selection.activeGameObject = element;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            Undo.SetTransformParent(child.transform, parent.transform, "");

            RectTransform rectTransform = child.transform as RectTransform;
            if (rectTransform)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                Vector3 localPosition = rectTransform.localPosition;
                localPosition.z = 0;
                rectTransform.localPosition = localPosition;
            }
            else
            {
                child.transform.localPosition = Vector3.zero;
            }
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        static public GameObject CreateNewUI()
        {
            // Root for the UI
            var root = ObjectFactory.CreateGameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.layer = LayerMask.NameToLayer(kUILayerName);
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Works for all stages.
            StageUtility.PlaceGameObjectInCurrentStage(root);
            bool customScene = false;
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                Undo.SetTransformParent(root.transform, prefabStage.prefabContentsRoot.transform, "");
                customScene = true;
            }

            Undo.SetCurrentGroupName("Create " + root.name);

            // If there is no event system add one...
            // No need to place event system in custom scene as these are temporary anyway.
            // It can be argued for or against placing it in the user scenes,
            // but let's not modify scene user is not currently looking at.
            if (!customScene)
                CreateEventSystem(false);
            return root;
        }

        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            StageHandle stage = parent == null ? StageUtility.GetCurrentStageHandle() : StageUtility.GetStageHandle(parent);
            var esys = stage.FindComponentOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = ObjectFactory.CreateGameObject("EventSystem");
                if (parent == null)
                    StageUtility.PlaceGameObjectInCurrentStage(eventSystem);
                else
                    SetParentAndAlign(eventSystem, parent);
                esys = ObjectFactory.AddComponent<EventSystem>(eventSystem);
                ObjectFactory.AddComponent<StandaloneInputModule>(eventSystem);

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (IsValidCanvas(canvas))
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use any valid canvas.
            // We have to find all loaded Canvases, not just the ones in main scenes.
            Canvas[] canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();
            for (int i = 0; i < canvasArray.Length; i++)
                if (IsValidCanvas(canvasArray[i]))
                    return canvasArray[i].gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        static bool IsValidCanvas(Canvas canvas)
        {
            if (canvas == null || !canvas.gameObject.activeInHierarchy)
                return false;

            // It's important that the non-editable canvas from a prefab scene won't be rejected,
            // but canvases not visible in the Hierarchy at all do. Don't check for HideAndDontSave.
            if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
                return false;

            return StageUtility.GetStageHandle(canvas.gameObject) == StageUtility.GetCurrentStageHandle();
        }

        private class DefaultEditorFactory : DefaultControls.IFactoryControls
        {
            public static DefaultEditorFactory Default = new DefaultEditorFactory();

            public GameObject CreateGameObject(string name, params Type[] components)
            {
                return ObjectFactory.CreateGameObject(name, components);
            }
        }

        private class FactorySwapToEditor : IDisposable
        {
            DefaultControls.IFactoryControls factory;

            public FactorySwapToEditor()
            {
                factory = DefaultControls.factory;
                DefaultControls.factory =  DefaultEditorFactory.Default;
            }

            public void Dispose()
            {
                DefaultControls.factory = factory;
            }
        }

        static private VisionControls.Resources visionUIResources = new VisionControls.Resources();

        static private VisionControls.Resources GetStandardResources()
        {
            CheckResources();
            return visionUIResources;
        }

        static void CheckResource<TRes>(ref TRes resource, string pathInPackage) where TRes : UnityEngine.Object
        {
            if (resource == null)
                resource = AssetDatabase.LoadAssetAtPath<TRes>($"Packages/com.jetxr.visionui/{pathInPackage}");
        }

        static void CheckProjectResource<TRes>(ref TRes resource, string pathInResources) where TRes : UnityEngine.Object
        {
            if (resource == null)
                resource = Resources.Load<TRes>(pathInResources);
        }
        #endregion

        static void CheckResources()
        {
            CheckResource(ref visionUIResources.darkElementMaterial, "Runtime/Materials/DarkElementBackground.mat");
            CheckResource(ref visionUIResources.lightElementMaterial, "Runtime/Materials/LightElementBackground.mat");
            CheckResource(ref visionUIResources.lightElementWithFrameMaterial, "Runtime/Materials/LightElementBackgroundWithFrame.mat");
            CheckResource(ref visionUIResources.windowBlurredBackgroundMaterial, "Runtime/Materials/WindowBlurredBackground.mat");
            CheckResource(ref visionUIResources.windowBlurredOverlayMaterial, "Runtime/Materials/WindowBlurredOverlayBackground.mat");
            CheckResource(ref visionUIResources.windowOverlayMaterial, "Runtime/Materials/WindowOverlayBackground.mat");
            CheckResource(ref visionUIResources.toolbarBlurredOverlayMaterial, "Runtime/Materials/ToolbarBlurredOverlayBackground.mat");
            CheckResource(ref visionUIResources.tabbarBlurredOverlayMaterial, "Runtime/Materials/TabbarBlurredOverlayBackground.mat");
            CheckResource(ref visionUIResources.alphaBackgroundMaterial, "Runtime/Materials/AlphaBackground.mat");

            CheckProjectResource(ref visionUIResources.fontSemibold, "Inter-SemiBold SDF");
            CheckProjectResource(ref visionUIResources.fontBold, "Inter-Bold SDF");
            CheckProjectResource(ref visionUIResources.fontMedium, "Inter-Medium SDF");
            CheckProjectResource(ref visionUIResources.fontRegular, "Inter-Regular SDF");

            CheckResource(ref visionUIResources.buttonAnimatorController, "Runtime/Animators/ButtonController.controller");
            CheckResource(ref visionUIResources.buttonNoPlatterAnimatorController, "Runtime/Animators/ButtonNoPlatterController.controller");
            CheckResource(ref visionUIResources.symbolAnimatorController, "Runtime/Animators/SymbolController.controller");
            CheckResource(ref visionUIResources.symbolNoPlatterAnimatorController, "Runtime/Animators/SymbolNoPlatterController.controller");
            CheckResource(ref visionUIResources.symbolTextButtonController, "Runtime/Animators/SymbolTextButtonController.controller");
            CheckResource(ref visionUIResources.symbolTextButtonNoPlatterController, "Runtime/Animators/SymbolTextButtonNoPlatterController.controller");
            CheckResource(ref visionUIResources.miniSliderAnimatorController, "Runtime/Animators/MiniSliderController.controller");
            CheckResource(ref visionUIResources.smallSliderAnimatorController, "Runtime/Animators/SmallSliderController.controller");
            CheckResource(ref visionUIResources.regularSliderAnimatorController, "Runtime/Animators/RegularSliderController.controller");
            CheckResource(ref visionUIResources.largeSliderAnimatorController, "Runtime/Animators/LargeSliderController.controller");
            CheckResource(ref visionUIResources.throbberAnimatorController, "Runtime/Animators/ThrobberController.controller");
            CheckResource(ref visionUIResources.listElementAnimatorController, "Runtime/Animators/ListElementController.controller");
            CheckResource(ref visionUIResources.toggleAnimatorController, "Runtime/Animators/ToggleController.controller");
            CheckResource(ref visionUIResources.dropdownAnimatorController, "Runtime/Animators/DropdownController.controller");
            CheckResource(ref visionUIResources.dropdownItemAnimatorController, "Runtime/Animators/DropdownItemController.controller");
            CheckResource(ref visionUIResources.inputFieldAnimatorController, "Runtime/Animators/InputFieldController.controller");
            CheckResource(ref visionUIResources.tabbarToggleController, "Runtime/Animators/TabbarToggleController.controller");
            CheckResource(ref visionUIResources.closeButtonController, "Runtime/Animators/CloseButtonController.controller");
            CheckResource(ref visionUIResources.grabberController, "Runtime/Animators/GrabberController.controller");
            CheckResource(ref visionUIResources.volumeController, "Runtime/Animators/VolumeButton.controller");
            CheckResource(ref visionUIResources.resizerController, "Runtime/Animators/Window Resizer.controller");
            
            CheckResource(ref visionUIResources.buttonBackground, "Runtime/Sprites/Buttons/Background.png");
            CheckResource(ref visionUIResources.buttonHighlight, "Runtime/Sprites/Buttons/TextHighlight.png");
            CheckResource(ref visionUIResources.symbolHighlight, "Runtime/Sprites/Buttons/SymbolHighlight.png");
            CheckResource(ref visionUIResources.symbol, "Runtime/Sprites/Buttons/Symbol.png");
            CheckResource(ref visionUIResources.roundedRectBackground, "Runtime/Sprites/Buttons/RoundedRectBackground.png");
            CheckResource(ref visionUIResources.roundedRectHighlight, "Runtime/Sprites/Buttons/RoundedRectHighlight.png");

            CheckResource(ref visionUIResources.sliderElement, "Runtime/Sprites/Sliders/SliderElement.png");
            CheckResource(ref visionUIResources.sliderHighlight, "Runtime/Sprites/Sliders/Highlight.png");

            CheckResource(ref visionUIResources.miniSliderShadow, "Runtime/Sprites/Sliders/MiniShadow.png");
            CheckResource(ref visionUIResources.miniSliderGlow, "Runtime/Sprites/Sliders/MiniGlow.png");
            CheckResource(ref visionUIResources.miniSliderEmboss, "Runtime/Sprites/Sliders/MiniEmboss.png");

            CheckResource(ref visionUIResources.smallSliderShadow, "Runtime/Sprites/Sliders/SmallShadow.png");
            CheckResource(ref visionUIResources.smallSliderGlow, "Runtime/Sprites/Sliders/SmallGlow.png");
            CheckResource(ref visionUIResources.smallSliderEmboss, "Runtime/Sprites/Sliders/SmallEmboss.png");

            CheckResource(ref visionUIResources.regularSliderShadow, "Runtime/Sprites/Sliders/RegularShadow.png");
            CheckResource(ref visionUIResources.regularSliderGlow, "Runtime/Sprites/Sliders/RegularGlow.png");
            CheckResource(ref visionUIResources.regularSliderEmboss, "Runtime/Sprites/Sliders/RegularEmboss.png");

            CheckResource(ref visionUIResources.largeSliderShadow, "Runtime/Sprites/Sliders/LargeShadow.png");
            CheckResource(ref visionUIResources.largeSliderGlow, "Runtime/Sprites/Sliders/LargeGlow.png");
            CheckResource(ref visionUIResources.largeSliderEmboss, "Runtime/Sprites/Sliders/LargeEmboss.png");


            CheckResource(ref visionUIResources.regularSliderBackground, "Runtime/Sprites/Sliders/RegularBackground.png");
            CheckResource(ref visionUIResources.regularSliderFill, "Runtime/Sprites/Sliders/RegularFill.png");


            CheckResource(ref visionUIResources.toggleBGStateOff, "Runtime/Sprites/Toggle/BackgroundStateOff.png");
            CheckResource(ref visionUIResources.toggleBGStateOn, "Runtime/Sprites/Toggle/BackgroundStateOn.png");
            CheckResource(ref visionUIResources.toggleHighlight, "Runtime/Sprites/Toggle/Highlight.png");
            CheckResource(ref visionUIResources.toggleShadow, "Runtime/Sprites/Toggle/Shadow.png");
            CheckResource(ref visionUIResources.toggleKnob, "Runtime/Sprites/Toggle/Knob.png");

            CheckResource(ref visionUIResources.throbber, "Runtime/Sprites/Throbber/Throbber.png");

            CheckResource(ref visionUIResources.listElementArrow, "Runtime/Sprites/ListElements/Arrow.png");
            CheckResource(ref visionUIResources.listElementHighlight, "Runtime/Sprites/ListElements/ListElementHighlight.png");
            CheckResource(ref visionUIResources.firstListElement, "Runtime/Sprites/ListElements/ListElementFirst.png");
            CheckResource(ref visionUIResources.middleListElement, "Runtime/Sprites/ListElements/ListElementMiddle.png");
            CheckResource(ref visionUIResources.lastListElement, "Runtime/Sprites/ListElements/ListElementLast.png");
            CheckResource(ref visionUIResources.singleListElement, "Runtime/Sprites/ListElements/ListElementSingle.png");

            CheckResource(ref visionUIResources.windowGlass, "Runtime/Sprites/Windows/WindowGlass.png");
            CheckResource(ref visionUIResources.windowGlassNoAlpha, "Runtime/Sprites/Windows/WindowGlassNoAlpha.png");
            CheckResource(ref visionUIResources.windowGlassSmallerSpecular, "Runtime/Sprites/Windows/WindowGlassSmallerSpecular.png");
            CheckResource(ref visionUIResources.windowShadow, "Runtime/Sprites/Windows/WindowShadow.png");
            CheckResource(ref visionUIResources.windowFloorShadow, "Runtime/Sprites/Windows/WindowFloorShadow.png");
            CheckResource(ref visionUIResources.sidebar, "Runtime/Sprites/Windows/Sidebar.png");

            CheckResource(ref visionUIResources.scrollbarHandle, "Runtime/Sprites/Dropdown/ScrollbarHandle.png");
            CheckResource(ref visionUIResources.dropdownArrow, "Runtime/Sprites/Dropdown/Arrow.png");
            CheckResource(ref visionUIResources.dropdownHighlight, "Runtime/Sprites/Dropdown/Highlight.png");
            CheckResource(ref visionUIResources.dropdownShadow, "Runtime/Sprites/Dropdown/Shadow.png");
            CheckResource(ref visionUIResources.itemCheckmark, "Runtime/Sprites/Dropdown/ItemCheckmark.png");

            CheckResource(ref visionUIResources.tooltip, "Runtime/Sprites/Tooltip/Tooltip.png");

            CheckResource(ref visionUIResources.inputFieldBackground, "Runtime/Sprites/InputField/Background.png");
            CheckResource(ref visionUIResources.inputFieldClearBackground, "Runtime/Sprites/InputField/Clear Button.png");
            CheckResource(ref visionUIResources.inputFieldClearCross, "Runtime/Sprites/InputField/Clear Cross.png");
            CheckResource(ref visionUIResources.inputFieldHighlight, "Runtime/Sprites/InputField/Highlight.png");

            CheckResource(ref visionUIResources.toolbarBackground, "Runtime/Sprites/Toolbar/ToolbarBackground.png");

            CheckResource(ref visionUIResources.verticalSeparator, "Runtime/Sprites/Separator/Vertical Separator.png");
            CheckResource(ref visionUIResources.horizontalSeparator, "Runtime/Sprites/Separator/Horizontal Separator.png");

            CheckResource(ref visionUIResources.appIcon, "Runtime/Sprites/Icons/App Icon.png");
            CheckResource(ref visionUIResources.crossIcon, "Runtime/Sprites/Icons/Cross Icon.png");

            CheckResource(ref visionUIResources.tabbarBackground, "Runtime/Sprites/Tabbar/TabbarBackground.png");
            CheckResource(ref visionUIResources.tabbarShadow, "Runtime/Sprites/Tabbar/TabbarShadow.png");

            CheckResource(ref visionUIResources.segmentedControlHighlight, "Runtime/Sprites/SegmentedControl/SegmentedControlHighlight.png");

            CheckResource(ref visionUIResources.speakerSlash, "Runtime/Sprites/Volume/speakerSlash.png");
            CheckResource(ref visionUIResources.speaker1, "Runtime/Sprites/Volume/speaker1.png");
            CheckResource(ref visionUIResources.speaker2, "Runtime/Sprites/Volume/speaker2.png");
            CheckResource(ref visionUIResources.speaker3, "Runtime/Sprites/Volume/speaker3.png");

            CheckResource(ref visionUIResources.trailing, "Runtime/Sprites/Resizer/Trailing.png");
            
        }

        #region Buttons

        #region Button - Text

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text/Small 32px", false, 10)]
        static public void AddTextButtonSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextButton(GetStandardResources(), 65f, 32f, 15f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text/Standard 44px", false, 10)]
        static public void AddTextButtonStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextButton(GetStandardResources(), 86f, 44f, 17f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text/Large 52px", false, 10)]
        static public void AddTextButtonLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextButton(GetStandardResources(), 101f, 52f, 19f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Button - Text (No Platter)

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text (No Platter)/Small 32px", false, 10)]
        static public void AddTextButtonNoPlatterSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextButtonNoPlatter(GetStandardResources(), 65f, 32f, 15f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text (No Platter)/Standard 44px", false, 10)]
        static public void AddTextButtonNoPlatterStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextButtonNoPlatter(GetStandardResources(), 86f, 44f, 17f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text (No Platter)/Large 52px", false, 10)]
        static public void AddTextButtonNoPlatterLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextButtonNoPlatter(GetStandardResources(), 101f, 52f, 19f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Button - Text+Symbol

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text+Symbol/Small 32px", false, 10)]
        static public void AddTextSymbolButtonSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextSymbolButton(GetStandardResources(), 91f, 32f, 15f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text+Symbol/Standard 44px", false, 10)]
        static public void AddTextSymbolButtonStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextSymbolButton(GetStandardResources(), 120f, 44f, 17f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text+Symbol/Large 52px", false, 10)]
        static public void AddTextSymbolButtonLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextSymbolButton(GetStandardResources(), 139f, 52f, 19f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Button - Text+Symbol (No Platter)

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text+Symbol (No Platter)/Small 32px", false, 10)]
        static public void AddTextSymbolButtonNoPlatterSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextSymbolButtonNoPlatter(GetStandardResources(), 91f, 32f, 15f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text+Symbol (No Platter)/Standard 44px", false, 10)]
        static public void AddTextSymbolButtonNoPlatterStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextSymbolButtonNoPlatter(GetStandardResources(), 120f, 44f, 17f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text+Symbol (No Platter)/Large 52px", false, 10)]
        static public void AddTextSymbolButtonNoPlatterLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTextSymbolButtonNoPlatter(GetStandardResources(), 139f, 52f, 19f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Button - Symbol

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol/Mini 28px", false, 10)]
        static public void AddSymbolButtonMini(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButton(GetStandardResources(), 28f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol/Small 32px", false, 10)]
        static public void AddSymbolButtonSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButton(GetStandardResources(), 32f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol/Standard 44px", false, 10)]
        static public void AddSymbolButtonStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButton(GetStandardResources(), 44f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol/Large 52px", false, 10)]
        static public void AddSymbolButtonLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButton(GetStandardResources(), 52f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol/Extra Large 64px", false, 10)]
        static public void AddSymbolButtonExtraLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButton(GetStandardResources(), 64f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Button - Symbol (No Platter)

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol (No Platter)/Mini 28px", false, 10)]
        static public void AddSymbolButtonNoPlatterMini(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButtonNoPlatter(GetStandardResources(), 28f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol (No Platter)/Small 32px", false, 10)]
        static public void AddSymbolButtonNoPlatterSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButtonNoPlatter(GetStandardResources(), 32f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol (No Platter)/Standard 44px", false, 10)]
        static public void AddSymbolButtonNoPlatterStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButtonNoPlatter(GetStandardResources(), 44f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol (No Platter)/Large 52px", false, 10)]
        static public void AddSymbolButtonNoPlatterLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButtonNoPlatter(GetStandardResources(), 52f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Symbol (No Platter)/Extra Large 64px", false, 10)]
        static public void AddSymbolButtonNoPlatterExtraLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSymbolButtonNoPlatter(GetStandardResources(), 64f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Button - Text Rounded Rect

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text Rounded Rect/Small 32px", false, 10)]
        static public void AddRoundedRectButtonSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateRoundedRectButton(GetStandardResources(), 65f, 32f, 15f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text Rounded Rect/Standard 44px", false, 10)]
        static public void AddRoundedRectButtonStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateRoundedRectButton(GetStandardResources(), 86f, 44f, 17f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text Rounded Rect/Large 52px", false, 10)]
        static public void AddRoundedRectButtonLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateRoundedRectButton(GetStandardResources(), 101f, 52f, 19f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Button - Button - Text Rounded Rect (No Platter)

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text Rounded Rect (No Platter)/Small 32px", false, 10)]
        static public void AddRoundedRectButtonNoPlatterSmall(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateRoundedRectButtonNoPlatter(GetStandardResources(), 65f, 32f, 15f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text Rounded Rect (No Platter)/Standard 44px", false, 10)]
        static public void AddRoundedRectButtonNoPlatterStandard(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateRoundedRectButtonNoPlatter(GetStandardResources(), 86f, 44f, 17f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Buttons/Button - Text Rounded Rect (No Platter)/Large 52px", false, 10)]
        static public void AddRoundedRectButtonNoPlatterLarge(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateRoundedRectButtonNoPlatter(GetStandardResources(), 101f, 52f, 19f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #endregion

        #region Sliders

       [MenuItem("GameObject/Vision UI/Sliders/Mini Slider 12px", false, 10)]
       static public void AddMiniSlider(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateMiniSlider(GetStandardResources(), 12f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Sliders/Small Slider 16px", false, 10)]
        static public void AddSmallSlider(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSmallSlider(GetStandardResources(), 16f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Sliders/Regular Slider 28px", false, 10)]
        static public void AddRegularSlider(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateRegularSlider(GetStandardResources(), 28f);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Sliders/Large Slider 44px", false, 10)]
        static public void AddLargeSlider(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateLargeSlider(GetStandardResources(), 44f);
            PlaceUIElementRoot(go, menuCommand);
        }

        #endregion

        #region Progress Indicators
        [MenuItem("GameObject/Vision UI/Progress Indicators/Throbber", false, 10)]
        static public void AddThrobber(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateThrobber(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Progress Indicators/Progress Bar", false, 10)]
        static public void AddProgressBar(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateProgressBar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
        #endregion

        #region Windows
        [MenuItem("GameObject/Vision UI/Windows/Window", false, 10)]
        static public void AddWindow(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateWindow(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Windows/Window + Tabbar", false, 10)]
        static public void AddWindowWithTabbar(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateWindowWithTabbar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var tabbarObject = go.transform.Find("Tabbar");
            RectTransform tabbarRect = tabbarObject.GetComponent<RectTransform>();
            tabbarRect.anchorMin = new Vector2(0, 0.5f);
            tabbarRect.anchorMax = new Vector2(0, 0.5f);
            tabbarRect.pivot = new Vector2(0, 0.5f);
            tabbarRect.anchoredPosition = new Vector2(-96, 0);

            var shadowObject = tabbarObject.transform.Find("Shadow");
            RectTransform shadowRect = shadowObject.GetComponent<RectTransform>();
            shadowRect.anchorMin = new Vector2(0, 0);
            shadowRect.anchorMax = new Vector2(1, 1);
            shadowRect.sizeDelta = new Vector2(0, 0);
            shadowRect.anchoredPosition = new Vector2(25, -25);

            var backgroundObject = tabbarObject.transform.Find("Background");
            RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);
            backgroundRect.anchoredPosition = new Vector2(0, 0);

            var tabbarToggles = tabbarObject.GetComponentsInChildren<Toggle>();

            for (int i = 0; i < tabbarToggles.Length; i++)
            {
                GameObject tab = go.transform.Find($"Tab {i + 1}").gameObject;

                UnityEventTools.AddPersistentListener(tabbarToggles[i].onValueChanged, tab.SetActive);
            }
        }

        [MenuItem("GameObject/Vision UI/Windows/Window + Toolbar", false, 10)]
        static public void AddWindowWithToolbar(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateWindowWithToolbar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform toolbarRect = go.transform.Find("Toolbar").GetComponent<RectTransform>();
            toolbarRect.anchorMin = new Vector2(0.5f, 0);
            toolbarRect.anchorMax = new Vector2(0.5f, 0);
            toolbarRect.pivot = new Vector2(0.5f, 1f);
            toolbarRect.anchoredPosition3D = new Vector3(0, 20, -20);
        }

        [MenuItem("GameObject/Vision UI/Windows/Alert", false, 10)]
        static public void AddAlert(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateAlert(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Windows/Windows Stacker", false, 10)]
        static public void AddWindowsStacker(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateWindowsStacker(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var windowControls = go.transform.Find($"Window Controls");
            var closeButtonWindow = windowControls.Find($"Close Button Window");

            UnityEventTools.AddPersistentListener(closeButtonWindow.GetComponent<Button>().onClick, go.GetComponent<WindowsStacker>().CloseWindow);

            Transform windowRightResizer = windowControls.Find("Window Right Resizer");
            Transform interactableR = windowRightResizer.Find("Interactable");
            
            WindowsStacker stackerR = interactableR.GetComponentInParent<WindowsStacker>();
            GameObject objectToTransformR = stackerR == null ? go.transform.parent.gameObject : stackerR.gameObject;
            
            interactableR.GetComponent<Resizer>().SetReferences(objectToTransformR, windowControls.GetComponent<Animator>(), "RHovered");
            
            Transform windowLeftResizer = windowControls.Find("Window Left Resizer");
            Transform interactableL = windowLeftResizer.Find("Interactable");
            
            WindowsStacker stackerL = interactableL.GetComponentInParent<WindowsStacker>();
            GameObject objectToTransformL = stackerL == null ? go.transform.parent.gameObject : stackerL.gameObject;
            
            interactableL.GetComponent<Resizer>().SetReferences(objectToTransformL, windowControls.GetComponent<Animator>(), "LHovered");
        }

        #endregion

        #region Window Add-ons
        [MenuItem("GameObject/Vision UI/Windows/Add-ons/Toolbar", false, 10)]
        static public void AddToolbar(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateToolbar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform toolbarRect = go.GetComponent<RectTransform>();
            toolbarRect.anchorMin = new Vector2(0.5f, 0);
            toolbarRect.anchorMax = new Vector2(0.5f, 0);
            toolbarRect.pivot = new Vector2(0.5f, 1f);
            toolbarRect.anchoredPosition3D = new Vector3(0, 20, -20);
        }

        [MenuItem("GameObject/Vision UI/Windows/Add-ons/Tabbar", false, 10)]
        static public void AddTabbar(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTabbar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform tabbarRect = go.GetComponent<RectTransform>();
            tabbarRect.anchorMin = new Vector2(0, 0.5f);
            tabbarRect.anchorMax = new Vector2(0, 0.5f);
            tabbarRect.pivot = new Vector2(0, 0.5f);
            tabbarRect.anchoredPosition = new Vector2(-96, 0);

            var shadowObject = go.transform.Find("Shadow");
            RectTransform shadowRect = shadowObject.GetComponent<RectTransform>();
            shadowRect.anchorMin = new Vector2(0, 0);
            shadowRect.anchorMax = new Vector2(1, 1);
            shadowRect.sizeDelta = new Vector2(0, 0);
            shadowRect.anchoredPosition = new Vector2(25, -25);

            var backgroundObject = go.transform.Find("Background");
            RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);
            backgroundRect.anchoredPosition = new Vector2(0, 0);
        }

        [MenuItem("GameObject/Vision UI/Windows/Add-ons/Sidebar", false, 10)]
        static public void AddSidebar(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSidebar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform sidebarRect = go.GetComponent<RectTransform>();
            sidebarRect.anchorMin = Vector2.zero;
            sidebarRect.anchorMax = new Vector2(0, 1);
            sidebarRect.pivot = new Vector2(0, 0.5f);
            sidebarRect.anchoredPosition = new Vector2(1.6f, 0);
        }

        [MenuItem("GameObject/Vision UI/Windows/Add-ons/Window Controls", false, 10)]
        static public void AddWindowControls(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateWindowControls(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform windowControlsRect = go.GetComponent<RectTransform>();
            windowControlsRect.anchorMin = new Vector2(0, 0);
            windowControlsRect.anchorMax = new Vector2(1, 0);
            windowControlsRect.pivot = new Vector2(0.5f, 1);
            windowControlsRect.sizeDelta = new Vector2(0, 14);
            windowControlsRect.anchoredPosition = new Vector2(0, -22);

            Grabber grabber = go.GetComponentInChildren<Grabber>();
            grabber.SetReferences((menuCommand.context as GameObject).transform);

            Transform windowRightResizer = go.transform.Find("Window Right Resizer");
            Transform interactableR = windowRightResizer.transform.Find("Interactable");

            WindowsStacker stackerR = interactableR.GetComponentInParent<WindowsStacker>();
            GameObject objectToTransformR = stackerR == null ? go.transform.parent.gameObject : stackerR.gameObject;

            interactableR.GetComponent<Resizer>().SetReferences(objectToTransformR, go.GetComponent<Animator>(), "RHovered");

            Transform windowLeftResizer = go.transform.Find("Window Left Resizer");
            Transform interactableL = windowLeftResizer.transform.Find("Interactable");

            WindowsStacker stackerL = interactableL.GetComponentInParent<WindowsStacker>();
            GameObject objectToTransformL = stackerL == null ? go.transform.parent.gameObject : stackerL.gameObject;

            interactableL.GetComponent<Resizer>().SetReferences(objectToTransformL, go.GetComponent<Animator>(), "LHovered");
        }

        [MenuItem("GameObject/Vision UI/Windows/Add-ons/Window Resizer", false, 10)]
        static public void AddWindowResizer(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateWindowResizer(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform windowResizerRect = go.GetComponent<RectTransform>();
            windowResizerRect.pivot = new Vector2(0, 0);
            windowResizerRect.anchorMin = new Vector2(1, 0);
            windowResizerRect.anchorMax = new Vector2(1, 0);
            windowResizerRect.sizeDelta = new Vector2(75, 75);

            Transform interactable = go.transform.Find("Interactable");

            Resizer interactableResizer = interactable.GetComponent<Resizer>();

            WindowsStacker stacker = interactable.GetComponentInParent<WindowsStacker>();
            GameObject objectToTransform = stacker == null ? go.transform.parent.gameObject : stacker.gameObject;

            interactableResizer.SetReferences(objectToTransform, null, "RHovered");
        }

        #endregion

        #region List
        [MenuItem("GameObject/Vision UI/List/Completed List", false, 10)]
        static public void AddCompletedList(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateCompletedList(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform completedListRect = go.GetComponent<RectTransform>();
            VisionControls.SetupRect(completedListRect, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 298), Vector2.zero);
        }

        [MenuItem("GameObject/Vision UI/List/List Element", false, 10)]
        static public void AddListElement(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateListElement(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/List/List Element (No Platter)", false, 10)]
        static public void AddListElementNoPlatter(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateListElementNoPlatter(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
        #endregion

        [MenuItem("GameObject/Vision UI/Toggle", false, 10)]
        static public void AddToggle(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateToggle(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Tooltip", false, 10)]
        static public void AddTooltip(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateTooltip(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            RectTransform toolTipRect = go.GetComponent<RectTransform>();
            toolTipRect.anchoredPosition = new Vector2(0, -4);

            // Tooltip Display 
            EventTrigger eventTrigger = go.transform.parent.gameObject.GetComponent<EventTrigger>();

            if (eventTrigger == null)
                eventTrigger = go.transform.parent.gameObject.AddComponent<EventTrigger>();

            var pointerEnterTrigger = new EventTrigger.Entry();
            pointerEnterTrigger.eventID = EventTriggerType.PointerEnter;

            var pointerExitTrigger = new EventTrigger.Entry();
            pointerExitTrigger.eventID = EventTriggerType.PointerExit;

            UnityAction<bool> action = go.SetActive;
            UnityEventTools.AddBoolPersistentListener(pointerEnterTrigger.callback, action, true);
            UnityEventTools.AddBoolPersistentListener(pointerExitTrigger.callback, action, false);

            eventTrigger.triggers.Clear();
            eventTrigger.triggers.Add(pointerEnterTrigger);
            eventTrigger.triggers.Add(pointerExitTrigger);

            go.SetActive(false);
        }

        //[MenuItem("GameObject/Vision UI/Dropdown", false, 10)]
        static public void AddDropdown(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateDropdown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Input Field", false, 10)]
        static public void AddInputField(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            //Logic
            TMP_InputField inputField = go.GetComponent<TMP_InputField>();
            Button cButton = go.transform.Find("Clear Button").GetComponent<Button>();

            MethodInfo targetMethod = inputField.GetType().GetProperty("text").GetSetMethod();
            var targetAction = Delegate.CreateDelegate(typeof(UnityAction<string>), inputField, targetMethod);
            UnityEventTools.AddStringPersistentListener(cButton.onClick, (UnityAction<string>)targetAction, "");

            UnityEventTools.AddVoidPersistentListener(cButton.onClick, inputField.Select);
        }

        [MenuItem("GameObject/Vision UI/Segmented Control", false, 10)]
        static public void AddSegmentedControl(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateSegmentedControl(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/Vision UI/Volume", false, 10)]
        static public void AddVolume(MenuCommand menuCommand)
        {
            GameObject go;
            using (new FactorySwapToEditor())
                go = VisionControls.CreateVolume(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            GameObject sliderObject = go.transform.Find("Slider").gameObject;
            Slider slider = sliderObject.GetComponent<Slider>();

            SpriteNumberSwitcher switcher = go.GetComponent<SpriteNumberSwitcher>();

            UnityEventTools.AddPersistentListener(slider.onValueChanged, switcher.SetValue);
        }
    }
}