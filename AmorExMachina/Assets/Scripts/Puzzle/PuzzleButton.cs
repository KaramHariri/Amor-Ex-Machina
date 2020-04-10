using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class PuzzleButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonType
    {
        Empty,
        Circle,
        Cross,
        Arrow,
        RotatableArrow
    }

    public enum ButtonDirection
    {
        North,
        East,
        West,
        South
    }

    public Puzzle puzzle;
    public ButtonType type;
    public ButtonDirection direction;
    public bool buttonOn = false;

    public Image background;

    public Sprite ButtonIdle;
    public Sprite ButtonHover;
    public Sprite ButtonActive;


    public UnityEvent onButtonSelected;
    public UnityEvent onButtonDeselected;
    public UnityEvent onButtonHovered;

    public void OnPointerClick(PointerEventData eventData)
    {
        puzzle.OnButtonSelected(this);
        if (type != ButtonType.Empty)
        {
            background.sprite = ButtonActive;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        puzzle.OnButtonEnter(this);
        if(type != ButtonType.Empty)
        {
            background.sprite = ButtonHover;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        puzzle.OnButtonExit(this);
        if (type != ButtonType.Empty)
        {
            background.sprite = ButtonIdle;
        }
    }

    private void Start()
    {
        background = GetComponent<Image>();
        puzzle.Subscribe(this);
    }

    public void Select()
    {
        if (onButtonSelected != null)
        {
            onButtonSelected.Invoke();
            //background.sprite = ButtonActive;

            //StopCoroutine(ChangeColorToRed());
            //StopCoroutine(ChangeColorToGreen());
            if(type == ButtonType.Empty)
            {
                buttonOn = !buttonOn;
                if(buttonOn)
                {
                    //Debug.Log("Switching to green");
                    StartCoroutine(ChangeColorToGreen());
                }
                else
                {
                    //Debug.Log("Switching to red");
                    StartCoroutine(ChangeColorToRed());
                }
            }
        }
    }

    public void Deselect()
    {
        if (onButtonDeselected != null)
        {
            onButtonDeselected.Invoke();
            //background.sprite = ButtonIdle;
            //StopCoroutine(ChangeColorToRed());
            //StopCoroutine(ChangeColorToGreen());
            //StartCoroutine(ChangeColorToRed());
        }
    }

    public void Hovered()
    {
        if (onButtonHovered != null)
        {
            onButtonHovered.Invoke();
            //background.sprite = ButtonHover;
        }
    }

    public void RotateDirection()
    {
        switch (direction)
        {
            case ButtonDirection.North:
                direction = ButtonDirection.East;
                break;
            case ButtonDirection.East:
                direction = ButtonDirection.South;
                break;
            case ButtonDirection.West:
                direction = ButtonDirection.North;
                break;
            case ButtonDirection.South:
                direction = ButtonDirection.West;
                break;
            default:
                break;
        }
    }

    IEnumerator ChangeColorToRed()
    {
        float t = 0;
        float colorDuration = 1.0f;
        Color currentColor = background.color;
        while (t < colorDuration)
        {
            t += Time.deltaTime;
            background.color = Color.Lerp(currentColor, Color.red, t / colorDuration);
            yield return null;
        }
    }
    IEnumerator ChangeColorToGreen()
    {
        float t = 0;
        float colorDuration = 1.0f;
        Color currentColor = background.color;
        while (t < colorDuration)
        {
            t += Time.deltaTime;
            background.color = Color.Lerp(currentColor, Color.green, t / colorDuration);
            yield return null;
        }
    }

}
