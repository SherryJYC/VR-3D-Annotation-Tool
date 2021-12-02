using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class VRButton : MonoBehaviour
{
    public Transform mapTransform;
    private Transform playerCameraTransform;
    private BoxCollider boxCollider;
    private RectTransform rectTransform;
    private Button locationButton; 
    private Text buttonText;
    

    private void OnEnable()
    {
        ValidateCollider();
        playerCameraTransform = GameObject.FindGameObjectWithTag("Player").transform;
        buttonText = GetComponentInChildren<Text>();
        locationButton = GetComponent<Button>();
        locationButton.onClick.AddListener(OnClick);

    }

    private void OnValidate()
    {
        ValidateCollider();
    }

    void OnClick()
    {
        playerCameraTransform.position = mapTransform.position;
    }
    private void ValidateCollider()
    {
        rectTransform = GetComponent<RectTransform>();

        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        boxCollider.size = rectTransform.sizeDelta;
    }
}

