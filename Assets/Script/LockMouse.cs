using UnityEngine;

public class LockMouse : MonoBehaviour
{
    private bool isLocked = true;

    void Start()
    {
        Lock();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Unlock();
        }
    }

    void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isLocked = true;
    }

    void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isLocked = false;
    }
}
