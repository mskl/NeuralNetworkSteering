using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleInputEnabled : MonoBehaviour {

    public InputField inputFieldToToggle;

	public void ToggleEditable()
    {
        inputFieldToToggle.interactable = !inputFieldToToggle.interactable;
    }
}
