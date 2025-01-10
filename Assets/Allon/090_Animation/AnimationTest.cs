using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationTest : MonoBehaviour
{
    public Slider slider;
    public Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(sliderValueChanged);
    }

    private void sliderValueChanged(float value)
    {
        anim.SetFloat("speed", value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
