using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayLevel : MonoBehaviour
{
    public InputField inputField;
    public int temp = 0;
    public Button myButton;
    public GameObject dataHolder;

    private void Start() {
        Button btn = myButton.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        dataHolder = GameObject.Find("DataObject");
    }

    private void OnClick() {
        StaticDataHolder.randomBlocks = temp;
        Application.LoadLevel(1);
    }

    public void setValue() {
        if (int.TryParse(inputField.text,out temp)) {
            temp = int.Parse(inputField.text);
        } else {
            temp = 0;
        }
    }
}
