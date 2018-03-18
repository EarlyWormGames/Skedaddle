using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class CreateMenuAddons
{
    private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
    private const string kBackgroundSpriteResourcePath = "UI/Skin/Background.psd";
    private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
    private const string kKnobPath = "UI/Skin/Knob.psd";
    private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";

    [MenuItem("GameObject/UI/TextMeshPro - Button")]
    public static void CreateTMProButton()
    {

        Transform selection = Selection.activeGameObject.transform;

        GameObject go = new GameObject();
        go.name = "Button";
        if (selection != null)
            go.transform.SetParent(selection);


        RectTransform rect = go.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;
        rect.sizeDelta = new Vector2(160, 30);

        Image img = go.AddComponent<Image>();
        img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
        img.type = Image.Type.Sliced;
        img.fillCenter = true;
        img.raycastTarget = true;
        img.color = Color.white;

        Button btn = go.AddComponent<Button>();
        btn.interactable = true;
        btn.targetGraphic = img;

        GameObject child = new GameObject();
        child.transform.SetParent(rect);
        child.name = "Text";

        RectTransform childRect = child.AddComponent<RectTransform>();
        childRect.localScale = Vector3.one;
        childRect.localPosition = Vector3.zero;
        childRect.anchorMin = new Vector2(0, 0);
        childRect.anchorMax = new Vector2(1, 1);
        childRect.anchoredPosition = Vector2.zero;
        childRect.pivot = new Vector2(0.5f, 0.5f);

        TextMeshProUGUI text = child.AddComponent<TextMeshProUGUI>();
        text.text = "Button";
        text.fontSize = 14;
        text.color = new Color(0.2f, 0.2f, 0.2f, 1);
        text.alignment = TextAlignmentOptions.Center;

        Undo.RegisterCreatedObjectUndo(go, "new button");
    }
}