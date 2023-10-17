using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadingText : MonoBehaviour
{
    public TextMeshProUGUI subjectName;
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public float displayTime = 2f;
    public bool condition;

    private float alpha = 0f;
    private float timer = 0f;
    private bool fadingIn = true;
    private bool fadingOut = false;

    //private Animator fadeAnimation;

    private void Start()
    {
        //fadeAnimation = GetComponent<Animator>();
    }
    private void Update()
    {
        if (condition)
        {
            //    fadeAnimation.StartPlayback();
            //}
            //else
            //{
            //    fadeAnimation.enabled = false;
            //}
            if (fadingIn)
            {
                alpha += Time.deltaTime / fadeInTime;
                subjectName.color = new Color(subjectName.color.r, subjectName.color.g, subjectName.color.b, alpha);
                if (alpha >= 1f)
                {
                    fadingIn = false;
                    timer = displayTime;
                }
            }
            else if (timer > 0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    fadingOut = true;
                }
            }
            else if (fadingOut)
            {
                alpha -= Time.deltaTime / fadeOutTime;
                subjectName.color = new Color(subjectName.color.r, subjectName.color.g, subjectName.color.b, alpha);
                if (alpha <= 0f)
                {
                    fadingOut = false;
                }
            }
        }
        else
        {
            alpha = 0f;
            timer = 0f;
            fadingIn = true;
            fadingOut = false;
            subjectName.color = new Color(subjectName.color.r, subjectName.color.g, subjectName.color.b, alpha);
        }
    }
}