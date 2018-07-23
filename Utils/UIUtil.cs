using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class UIUtil
    {

        public static Button CreateTemplateButton(Button sourceButton, string label, float sizeOffsetX, float sizeOffsetY, float offsetFromSourceX, float offsetFromSourceY)
        {
            Button button = UnityEngine.Object.Instantiate<Button>(sourceButton, sourceButton.transform.parent);
            button.transform.localPosition = new Vector3(sourceButton.transform.localPosition.x + offsetFromSourceX, sourceButton.transform.localPosition.y + offsetFromSourceY, sourceButton.transform.localPosition.z);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(button.GetComponent<RectTransform>().sizeDelta.x + sizeOffsetX, button.GetComponent<RectTransform>().sizeDelta.y + sizeOffsetY);
            Button internalButton = button.GetComponent<Button>();
            button.colors = sourceButton.colors;
            internalButton.transition = sourceButton.transition;
            button.GetComponentInChildren<Text>().text = label;
            button.onClick = new Button.ButtonClickedEvent();
            button.interactable = true;
            return button;
        }


        public static InputField CreateInput(InputField sourceInput, Button refButton, string placeholder, float sizeOffsetX, float sizeOffsetY, float offsetFromSourceX, float offsetFromSourceY)
        {
            InputField inputField = UnityEngine.Object.Instantiate<InputField>(sourceInput, refButton.transform.parent);
            inputField.GetComponent<RectTransform>().anchorMax = refButton.GetComponent<RectTransform>().anchorMax;
            inputField.GetComponent<RectTransform>().anchorMin = refButton.GetComponent<RectTransform>().anchorMin;
            inputField.transform.localPosition = new Vector3(refButton.transform.localPosition.x + offsetFromSourceX, refButton.transform.localPosition.y + offsetFromSourceY, refButton.transform.localPosition.z);
            inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(refButton.GetComponent<RectTransform>().sizeDelta.x + sizeOffsetX, refButton.GetComponent<RectTransform>().sizeDelta.y + sizeOffsetY);
            inputField.gameObject.GetComponentInChildren<Text>().text = "Template name";
            inputField.onValueChanged.RemoveAllListeners();
            return inputField;
        }

    }
}
