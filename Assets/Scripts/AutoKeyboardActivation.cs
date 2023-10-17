using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoKeyboardActivation : MonoBehaviour
{
    public TMP_InputField InputField;
    // Start is called before the first frame update
    void Start()
    {
        ;
    }


    private void OnEnable()
    {
        InputField.ActivateInputField();
    }

    public void SetActivateInputField()
    {
        InputField.ActivateInputField();
    }
}
