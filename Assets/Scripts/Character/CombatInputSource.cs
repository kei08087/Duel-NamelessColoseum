using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public readonly struct CombatCommand
{
    public readonly string Slot;
    public readonly Vector3 Direction;

    public CombatCommand(string slot, Vector3 direction)
    {
        Slot = slot;
        Direction = direction;
    }
}

public class CombatInputSource : MonoBehaviour
{
    public bool readDesktopInput = true;
    private readonly Queue<CombatCommand> commands = new();
    private Vector2 joystick;
    private bool joystickActive;

    public Vector3 MoveDirection
    {
        get
        {
            Vector2 axis = joystickActive ? joystick : readDesktopInput ? ReadDesktopMovement() : Vector2.zero;
            return new Vector3(axis.x, 0f, axis.y).normalized;
        }
    }

    public void SetJoystick(Vector2 direction)
    {
        joystick = Vector2.ClampMagnitude(direction, 1f);
        joystickActive = true;
    }

    public void ReleaseJoystick()
    {
        joystick = Vector2.zero;
        joystickActive = false;
    }

    public void Press(string slot, Vector3 direction)
    {
        commands.Enqueue(new CombatCommand(slot, direction));
    }

    public bool TryDequeue(out CombatCommand command)
    {
        if (commands.Count > 0)
        {
            command = commands.Dequeue();
            return true;
        }

        command = default;
        return false;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!readDesktopInput) return;
        if (Input.GetMouseButtonDown(0) &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
            Press("LClick", Vector3.zero);
        if (Input.GetMouseButtonDown(1)) Press("RClick", Vector3.zero);
        if (Input.GetKeyDown(KeyCode.Q)) Press("Q", Vector3.zero);
        if (Input.GetKeyDown(KeyCode.E)) Press("E", Vector3.zero);
        if (Input.GetKeyDown(KeyCode.LeftShift)) Press("LShift", Vector3.zero);
        if (Input.GetKeyDown(KeyCode.Space)) Press("Space", Vector3.zero);
        if (Input.GetKeyDown(KeyCode.LeftControl)) Press("LCtrl", Vector3.zero);
#endif
    }

    private static Vector2 ReadDesktopMovement()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#else
        return Vector2.zero;
#endif
    }
}
