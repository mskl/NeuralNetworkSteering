using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderToInputField : MonoBehaviour {

    public InputField textToChange;

	public void ChangeText()
    {
        var slider = GetComponent<Slider>();
        textToChange.text = slider.value.ToString();
    }
}
