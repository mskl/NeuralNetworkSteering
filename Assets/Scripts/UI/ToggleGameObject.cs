using UnityEngine;
using System.Collections;

public class ToggleGameObject : MonoBehaviour {

	public void ToggleVisibility()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
