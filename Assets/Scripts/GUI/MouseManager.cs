using UnityEngine;

namespace Assets.Scripts.GUI
{
    public class MouseManager : MonoBehaviour
    {
        private bool _lockCursor = true;

        public void ToggleMouse()
        {
            _lockCursor = !_lockCursor;

            Cursor.lockState = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_lockCursor;
        }
    }
}
