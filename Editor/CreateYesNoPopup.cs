using System;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CreateYesNoPopup
{
    [MenuItem("GameObject/UI/MurrdogModules/Yes No Popup", false, 10)]
    public static void Create()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
        }

        // Root popup object
        GameObject popupRoot = new GameObject("YesNoPopup");
        popupRoot.transform.SetParent(canvas.transform, false);
        RectTransform popupRT = popupRoot.AddComponent<RectTransform>();
        StretchToFill(popupRT);
        popupRoot.AddComponent<CanvasGroup>();
        popupRoot.AddComponent<UIPopupComponent>();
        UIYesNoPopupComponent yesNoComponent = popupRoot.AddComponent<UIYesNoPopupComponent>();

        // Background overlay
        GameObject background = CreateUIElement("Background", popupRoot.transform);
        StretchToFill(background.GetComponent<RectTransform>());
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.5f);

        // Center panel
        GameObject panel = CreateUIElement("Panel", popupRoot.transform);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(500f, 250f);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        VerticalLayoutGroup panelLayout = panel.AddComponent<VerticalLayoutGroup>();
        panelLayout.padding = new RectOffset(30, 30, 30, 30);
        panelLayout.spacing = 20f;
        panelLayout.childAlignment = TextAnchor.MiddleCenter;
        panelLayout.childControlWidth = true;
        panelLayout.childControlHeight = true;
        panelLayout.childForceExpandWidth = true;
        panelLayout.childForceExpandHeight = true;

        // Prompt text
        GameObject promptGO = CreateUIElement("PromptText", panel.transform);
        TextMeshProUGUI promptText = promptGO.AddComponent<TextMeshProUGUI>();
        promptText.text = "Are you sure?";
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.fontSize = 24f;
        promptText.color = Color.white;

        // Button container
        GameObject buttonContainer = CreateUIElement("ButtonContainer", panel.transform);
        HorizontalLayoutGroup hLayout = buttonContainer.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 20f;
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = false;
        LayoutElement containerLE = buttonContainer.AddComponent<LayoutElement>();
        containerLE.preferredHeight = 50f;

        // Yes button
        GameObject yesButtonGO = CreateButton("YesButton", "Yes", buttonContainer.transform);
        Button yesButton = yesButtonGO.GetComponent<Button>();

        // No button
        GameObject noButtonGO = CreateButton("NoButton", "No", buttonContainer.transform);
        Button noButton = noButtonGO.GetComponent<Button>();

        // Wire button OnClick -> UIYesNoPopupComponent
        UnityEventTools.AddVoidPersistentListener(yesButton.onClick, new UnityAction(yesNoComponent.OnYesPressed));
        UnityEventTools.AddVoidPersistentListener(noButton.onClick, new UnityAction(yesNoComponent.OnNoPressed));

        // Wire _onPromptTextChanged -> TMP_Text.SetText
        FieldInfo field = typeof(UIYesNoPopupComponent).GetField("_onPromptTextChanged",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            object promptEvent = field.GetValue(yesNoComponent);
            if (promptEvent == null)
            {
                promptEvent = new UIYesNoPopupComponent.PromptTextChangedEvent();
                field.SetValue(yesNoComponent, promptEvent);
            }

            // UnityEventTools.AddPersistentListener(
            //     (UnityEvent<string>)promptEvent,
            //     promptText.SetText);
        }

        popupRoot.SetActive(false);
        Undo.RegisterCreatedObjectUndo(popupRoot, "Create Yes/No Popup");
        Selection.activeGameObject = popupRoot;
    }

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    private static void StretchToFill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static GameObject CreateButton(string name, string label, Transform parent)
    {
        GameObject buttonGO = CreateUIElement(name, parent);
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        buttonGO.AddComponent<Button>();

        GameObject textGO = CreateUIElement("Text", buttonGO.transform);
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        StretchToFill(textRT);
        TextMeshProUGUI buttonText = textGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = label;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontSize = 20f;
        buttonText.color = Color.white;

        return buttonGO;
    }
}
