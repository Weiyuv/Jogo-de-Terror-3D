using UnityEngine;

public class LockMouse : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // trava o cursor no centro
        Cursor.visible = false;                   // esconde o cursor
    }
}
