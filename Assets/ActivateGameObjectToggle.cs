using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateGameObjectToggle : MonoBehaviour
{

    [SerializeField] private GameObject obj;
    private Button btn;


    // Start is called before the first frame update
    void Awake()
    {
        btn = GetComponent<Button>();

        // Check if the button is assigned
        if (btn != null)
        {
            // Subscribe to the button's onClick event
            btn.onClick.AddListener(ToggleObj);
        }

    }

    public void ToggleObj()
    {
        obj.SetActive(!obj.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Optional: Unsubscribe from the event when the object is destroyed
    void OnDestroy()
    {
        if (btn != null)
        {
            btn.onClick.RemoveListener(ToggleObj);
        }
    }
}
