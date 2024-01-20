using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public Sprite keyboard;
    public Sprite gamepad;
    public Image img;

    void Start() {
        img.color = new Color(255f,1f,1f,0f);
    }

    public void ShowHint() {
        img.color = new Color(255f,1f,1f,0.5f);
        if (Controller.currentDevice == "Gamepad") {
            img.sprite = gamepad;
        }
        else if (Controller.currentDevice == "Keyboard") {
            img.sprite = keyboard;
        }
    }

    public void HideHint() {
        img.color = new Color(255f,1f,1f,0f);
    }
}