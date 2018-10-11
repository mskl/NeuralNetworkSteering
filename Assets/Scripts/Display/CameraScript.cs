using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	void Update () {

        if (Input.GetMouseButton(1))    // Po stisknutí pravého tlačítka
        {
            Vector2 middleScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 translateVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - middleScreen;
            transform.Translate(new Vector2(translateVector.x / middleScreen.x, translateVector.y / middleScreen.y));
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //print(Input.GetAxis("Mouse ScrollWheel"));
            float minSize = 10F;
            float maxSize = 100F;
            float sensitivity = 10F;

            float size = Camera.main.orthographicSize;
            size -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            size = Mathf.Clamp(size, minSize, maxSize);
            Camera.main.orthographicSize = size;
        }
    }

}
