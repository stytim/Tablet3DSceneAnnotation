using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SubmitInputField : TMP_InputField
{

    [Serializable]
    public class KeyboardDoneEvent : UnityEvent { }

    [SerializeField]
    private KeyboardDoneEvent m_keyboardDone = new KeyboardDoneEvent();

    public KeyboardDoneEvent onKeyboardDone
    {
        get { return m_keyboardDone; }
        set { m_keyboardDone = value; }
    }

    void Update()
    {
        if (m_SoftKeyboard != null && m_SoftKeyboard.status == TouchScreenKeyboard.Status.Done && m_SoftKeyboard.status != TouchScreenKeyboard.Status.Canceled)
        {
            m_keyboardDone.Invoke();
        }
    }

}
