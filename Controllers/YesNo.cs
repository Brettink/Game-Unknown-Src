using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YesNo : MonoBehaviour
{
    public Text titleText;
    public static GameObject toSend, parent;
    public static YesNo self;
    public void Start() {
        self = this;
        gameObject.SetActive(false);
    }
    public static void set(GameObject parentG, GameObject toSendG, string title) {
        toSend = toSendG;
        parent = parentG;
        self.titleText.text = title;
    }
}
