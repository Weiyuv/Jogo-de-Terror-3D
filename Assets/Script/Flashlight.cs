using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;
    private bool isOn = false;

    void Start()
    {
        flashlight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;  // alterna o estado
            flashlight.SetActive(isOn);
        }
    }
}
