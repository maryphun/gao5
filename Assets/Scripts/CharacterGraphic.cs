using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterFacing
{
    Top,
    Bottom,
    Left,
    Right,
    Max
}

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Animator))]
public class CharacterGraphic : MonoBehaviour
{
    SpriteRenderer renderer;
    Animator animator;
    CharacterFacing characterDirection;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void ChangeFacing(CharacterFacing direction)
    {
        characterDirection = direction;

        if (characterDirection == CharacterFacing.Left)
        {
            SetAnimatorFacing("Left");
        }
        else if (characterDirection == CharacterFacing.Right)
        {
            SetAnimatorFacing("Right");
        }
        else if (characterDirection == CharacterFacing.Top)
        {
            SetAnimatorFacing("Top");
        }
        else if (characterDirection == CharacterFacing.Bottom)
        {
            SetAnimatorFacing("Bottom");
        }
    }

    public CharacterFacing GetCurrentFacing()
    {
        return characterDirection;
    }

    public void Run(bool boolean)
    {
        animator.SetBool("Run", boolean);
    }

    private void SetAnimatorFacing(string parameterName)
    {
        string[] parameters = new string[(int)CharacterFacing.Max];
        parameters[0] = "Left";
        parameters[1] = "Right";
        parameters[2] = "Top";
        parameters[3] = "Bottom";

        for (int i = 0; i < (int)CharacterFacing.Max; i++)
        {
            animator.SetBool(parameters[i], parameters[i] == parameterName);
        }
    }
}
