using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManualPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject static_manual;
    public GameObject dynamic_manual;
    public GameObject indication_text;
    public GameObject SwitchButton;

    public bool showStatic = true;
    
   public void switch_mode()
    {
        showStatic = !showStatic;
        if(showStatic)
        {
            indication_text.GetComponent<TextMeshProUGUI>().text = "User-static Mode Manual";
            SwitchButton.GetComponent<TextMeshProUGUI>().text = "see user-dynamic mode";
            static_manual.SetActive(true);
            dynamic_manual.SetActive(false);
        }
        else
        {
            indication_text.GetComponent<TextMeshProUGUI>().text = "User-dynamic Mode Manual";
            SwitchButton.GetComponent<TextMeshProUGUI>().text = "see user-static mode";
            static_manual.SetActive(false);
            dynamic_manual.SetActive(true);

        }

    }
        
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
