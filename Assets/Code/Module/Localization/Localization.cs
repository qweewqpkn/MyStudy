using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Localization : MonoBehaviour
{
    public int mID;
    private TextMeshProUGUI mText;
    void Awake()
    {
        Debug.Log("Awake");
        mText = GetComponent<TextMeshProUGUI>();
        mText.text = "";
    }

    void Start()
    {
        Debug.Log("Start");
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }
}
