using UnityEngine;
using UnityEngine.SceneManagement;

public class LockMouse : MonoBehaviour
{
    [Tooltip("Se vazio, usa a cena atual onde o objeto está. Se preencher, só fará lock nessa cena (nome exato).")]
    public string targetSceneName = "";

    private bool isLocked = false;

    void Start()
    {
        // Se não foi especificado, usa o nome da cena atual (facilita uso).
        if (string.IsNullOrEmpty(targetSceneName))
            targetSceneName = SceneManager.GetActiveScene().name;

        // Se estivermos na cena alvo, trava o cursor.
        if (SceneManager.GetActiveScene().name == targetSceneName)
            SetLock(true);
        else
            SetLock(false);
    }

    void Update()
    {
        // Se a cena mudou e não estamos mais na cena alvo, garante que o cursor seja liberado.
        if (SceneManager.GetActiveScene().name != targetSceneName && isLocked)
        {
            SetLock(false);
            enabled = false; // desliga o script, já que não precisa mais rodar nesta cena
            return;
        }

        // Só processa o toggle se estivermos na cena alvo.
        if (SceneManager.GetActiveScene().name == targetSceneName)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetLock(!isLocked);
            }
        }
    }

    void OnDisable()
    {
        // Se o script for desativado, certifica de liberar o cursor
        Unlock();
    }

    void OnDestroy()
    {
        // Se o GameObject for destruído (troca de cena, por exemplo), libera o cursor.
        Unlock();
    }

    private void SetLock(bool lockCursor)
    {
        if (lockCursor)
            Lock();
        else
            Unlock();
    }

    private void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isLocked = true;
    }

    private void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isLocked = false;
    }
}
