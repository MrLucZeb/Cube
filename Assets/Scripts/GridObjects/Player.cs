using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    bool doubleMoveToggle = false;

    protected override bool MoveToCell(Vector2Int position)
    {
        if (!CanMoveToCell(position)) { return false; }

        return base.MoveToCell(position);
    }

    protected override bool CanMoveToCell(Vector2Int position)
    {
        if (!base.CanMoveToCell(position)) return false;
        if (doubleMoveToggle && gridManager.HasEntity(position)) return false;

        return true;
    }

    public override ObjectType GetObjectType()
    {
        return ObjectType.PLAYER;
    }

    private Vector2Int GetInputDirection()
    {
        int x = (Input.GetKeyDown(KeyCode.D)) ? 1 : (Input.GetKeyDown(KeyCode.A)) ? -1 : 0;
        int y = (Input.GetKeyDown(KeyCode.W)) ? 1 : (Input.GetKeyDown(KeyCode.S)) ? -1 : 0;

        return new Vector2Int(x, y);
    }

    public override bool ExecuteTurn() // Rename to request turn
    {
        Vector2Int direction = GetInputDirection();

        if (Input.GetKeyDown(KeyCode.LeftShift)) doubleMoveToggle = !doubleMoveToggle;

        if (direction == Vector2Int.zero) return false;
        if (blockedDirections.Contains(direction) || !moveDirections.Contains(direction)) return false;

        if (doubleMoveToggle) direction *= 2;

        if (Move(direction))
        {
            blockedDirections.Clear();
            doubleMoveToggle = false;

            return true;
        }

        return false;
    }
}
