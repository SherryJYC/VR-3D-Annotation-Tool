using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RectTransform))]
public class HomeDynamicButton : MonoBehaviour
{

    private BoxCollider boxCollider;
    public Button myButton;
    private RectTransform rectTransform;


    private void OnEnable()
    {
        ValidateCollider();
        myButton.onClick.AddListener(OnClick);

    }

    private void OnValidate()
    {
        ValidateCollider();
    }

    void OnClick()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "MiniMapScene")
        {
            Debug.Log("MiniMapScene OK");
            SceneManager.LoadScene("UserDynamicScene", LoadSceneMode.Single);

        }
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

