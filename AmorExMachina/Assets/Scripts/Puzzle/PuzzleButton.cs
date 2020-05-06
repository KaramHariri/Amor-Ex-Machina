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

    private Image background;

    private Sprite ButtonEmpty;

    private Sprite normalCircle;
    private Sprite normalCross;
    private Sprite normalUpArrow;
    private Sprite normalDownArrow;
    private Sprite normalRightArrow;
    private Sprite normalLeftArrow;
    private Sprite normalRotationUpArrow;
    private Sprite normalRotationDownArrow;
    private Sprite normalRotationRightArrow;
    private Sprite normalRotationLeftArrow;

    private Sprite selectedCircle;
    private Sprite selectedCross;
    private Sprite selectedUpArrow;
    private Sprite selectedDownArrow;
    private Sprite selectedRightArrow;
    private Sprite selectedLeftArrow;
    private Sprite selectedRotationUpArrow;
    private Sprite selectedRotationDownArrow;
    private Sprite selectedRotationRightArrow;
    private Sprite selectedRotationLeftArrow;


    public UnityEvent onButtonSelected;
    public UnityEvent onButtonDeselected;
    public UnityEvent onButtonHovered;

    AudioManager audioManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (type == PuzzleButton.ButtonType.Empty)
        //    return;

        //audioManager.Play("ActivateButton");

        //puzzle.OnButtonSelected(this);

        //SetSelectedImage();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (type == PuzzleButton.ButtonType.Empty)
        //    return;

        //audioManager.Play("SwitchButton");

        //puzzle.OnButtonEnter(this);

        //SetSelectedImage();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (type == PuzzleButton.ButtonType.Empty)
        //    return;

        //puzzle.OnButtonExit(this);

        //SetNormalImage();
    }

    private void Awake()
    {
        background = GetComponent<Image>();
        puzzle.Subscribe(this);

        LoadSprites();
        SetNormalImage();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void Select()
    {

        if (onButtonSelected != null)
        {
            onButtonSelected.Invoke();

            if(type == ButtonType.Empty)
            {
                buttonOn = !buttonOn;
                if(buttonOn)
                {
                    StartCoroutine(ChangeColorToGreen());
                }
                else
                {
                    StartCoroutine(ChangeColorToRed());
                }
                audioManager.Play("DoorPuzzleChangingColor");
            }
        }
    }

    public void Deselect()
    {
        if (onButtonDeselected != null)
        {
            onButtonDeselected.Invoke();
            SetNormalImage();
        }
    }

    public void Hovered()
    {
        if (onButtonHovered != null)
        {
            onButtonHovered.Invoke();
            SetSelectedImage();
            audioManager.Play("SwitchButton");
        }
    }

    public void RotateDirection()
    {
        if(type != ButtonType.RotatableArrow) { return; }

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

        SetSelectedImage();
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

    private void SetSelectedImage()
    {
        switch (type)
        {
            case ButtonType.Empty:
                background.sprite = ButtonEmpty;
                break;
            case ButtonType.Circle:
                background.sprite = selectedCircle;
                break;
            case ButtonType.Cross:
                background.sprite = selectedCross;
                break;
            case ButtonType.Arrow:
                {
                    switch (direction)
                    {
                        case ButtonDirection.North:
                            background.sprite = selectedUpArrow;
                            break;
                        case ButtonDirection.East:
                            background.sprite = selectedRightArrow;
                            break;
                        case ButtonDirection.West:
                            background.sprite = selectedLeftArrow;
                            break;
                        case ButtonDirection.South:
                            background.sprite = selectedDownArrow;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case ButtonType.RotatableArrow:
                {
                    switch (direction)
                    {
                        case ButtonDirection.North:
                            background.sprite = selectedRotationUpArrow;
                            break;
                        case ButtonDirection.East:
                            background.sprite = selectedRotationRightArrow;
                            break;
                        case ButtonDirection.West:
                            background.sprite = selectedRotationLeftArrow;
                            break;
                        case ButtonDirection.South:
                            background.sprite = selectedRotationDownArrow;
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    private void SetNormalImage()
    {
        switch (type)
        {
            case ButtonType.Empty:
                background.sprite = ButtonEmpty;
                break;
            case ButtonType.Circle:
                background.sprite = normalCircle;
                break;
            case ButtonType.Cross:
                background.sprite = normalCross;
                break;
            case ButtonType.Arrow:
                {
                    switch (direction)
                    {
                        case ButtonDirection.North:
                            background.sprite = normalUpArrow;
                            break;
                        case ButtonDirection.East:
                            background.sprite = normalRightArrow;
                            break;
                        case ButtonDirection.West:
                            background.sprite = normalLeftArrow;
                            break;
                        case ButtonDirection.South:
                            background.sprite = normalDownArrow;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case ButtonType.RotatableArrow:
                {
                    switch (direction)
                    {
                        case ButtonDirection.North:
                            background.sprite = normalRotationUpArrow;
                            break;
                        case ButtonDirection.East:
                            background.sprite = normalRotationRightArrow;
                            break;
                        case ButtonDirection.West:
                            background.sprite = normalRotationLeftArrow;
                            break;
                        case ButtonDirection.South:
                            background.sprite = normalRotationDownArrow;
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    private void LoadSprites()
    {
        ButtonEmpty = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/ButtonEmpty");

        normalCircle = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalCircle");
        normalCross = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalCross");
        normalUpArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalUpArrow");
        normalDownArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalDownArrow");
        normalRightArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalRightArrow");
        normalLeftArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalLeftArrow");
        normalRotationUpArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalRotationUpArrow");
        normalRotationDownArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalRotationDownArrow");
        normalRotationRightArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalRotationRightArrow");
        normalRotationLeftArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/NormalRotationLeftArrow");

        selectedCircle = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedCircle");
        selectedCross = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedCross");
        selectedUpArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedUpArrow");
        selectedDownArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedDownArrow");
        selectedRightArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedRightArrow");
        selectedLeftArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedLeftArrow");
        selectedRotationUpArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedRotationUpArrow");
        selectedRotationDownArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedRotationDownArrow");
        selectedRotationRightArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedRotationRightArrow");
        selectedRotationLeftArrow = Resources.Load<Sprite>("Graphics/PuzzleTester/TempIcons/SelectedRotationLeftArrow");
    }

}
