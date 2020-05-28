using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
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
    private Material backgroundMaterial;

    private Sprite ButtonEmpty;

    private Sprite circle;
    private Sprite cross;
    private Sprite upArrow;
    private Sprite downArrow;
    private Sprite rightArrow;
    private Sprite leftArrow;
    private Sprite rotationUpArrow;
    private Sprite rotationDownArrow;
    private Sprite rotationRightArrow;
    private Sprite rotationLeftArrow;


    public UnityEvent onButtonSelected;
    public UnityEvent onButtonDeselected;
    public UnityEvent onButtonHovered;
    private float lerpDuration = 0.7f;

    AudioManager audioManager;

    //DEFAULT color #324558 opacity 66%
    //HOVER color #2898b8 opacity 44%
    //GREEN color #94b828 opacity 44%
    //RED color #b82828 opacity 44%

    //[SerializeField] private Color defaultColor = new Color(50, 69, 88);
    //[SerializeField] private Color hoverColor = new Color(40, 152, 184);
    //[SerializeField] private Color greenColor = new Color(148, 184, 40);
    //[SerializeField] private Color redColor = new Color(184, 40, 40);
    public Color defaultColor;
    public Color hoverColor;
    public Color greenColor;
    public Color redColor;

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
        background = transform.Find("ButtonIcon").GetComponent<Image>();
        backgroundMaterial = transform.Find("ButtonBackground").GetComponent<Image>().material;
        puzzle = transform.parent.transform.parent.GetComponent<Puzzle>();
        puzzle.Subscribe(this);

        LoadSprites();
        SetNormalImage();
        SetInitialMaterialColor();
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
            backgroundMaterial.SetColor("_Color", defaultColor);
        }
    }

    public void Hovered()
    {
        if (onButtonHovered != null)
        {
            onButtonHovered.Invoke();
            backgroundMaterial.SetColor("_Color", hoverColor);
            audioManager.Play("SwitchButton");
        }
    }

    // Added 20-05-18
    public void SetInitialHovered()
    {
        if (onButtonHovered != null)
        {
            onButtonHovered.Invoke();
            //SetSelectedImage();
            SetNormalImage();
            backgroundMaterial.SetColor("_Color", hoverColor);
        }
    }
  
    public void SetInitialMaterialColor()
    {
        switch (type)
        {
            case ButtonType.Empty:
                {
                    if(buttonOn)
                    {
                        backgroundMaterial.SetColor("_Color", greenColor);
                    }
                    else
                    {
                        backgroundMaterial.SetColor("_Color", redColor);
                    }
                }
                break;
            case ButtonType.Circle:
                {
                    backgroundMaterial.SetColor("_Color", defaultColor);
                }
                break;
            case ButtonType.Cross:
                {
                    backgroundMaterial.SetColor("_Color", defaultColor);
                }
                break;
            case ButtonType.Arrow:
                {
                    backgroundMaterial.SetColor("_Color", defaultColor);
                }
                break;
            case ButtonType.RotatableArrow:
                {
                    backgroundMaterial.SetColor("_Color", defaultColor);
                }
                break;
            default:
                backgroundMaterial.SetColor("_Color", defaultColor);
                break;
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

        SetNormalImage();

        //SetSelectedImage();
    }

    IEnumerator ChangeColorToRed()
    {
        float t = 0;
        Color currentColor = Color.white;
        //Color currentColor = backgroundMaterial.color;
        currentColor.a = 0.7f;
        while (t < lerpDuration)
        {
            t += Time.deltaTime;
            backgroundMaterial.SetColor("_Color", Color.Lerp(currentColor, redColor, t / lerpDuration));
            yield return null;
        }
    }
    IEnumerator ChangeColorToGreen()
    {
        float t = 0;
        Color currentColor = Color.white;
        //Color currentColor = backgroundMaterial.color;
        currentColor.a = 0.7f;
        while (t < lerpDuration)
        {
            t += Time.deltaTime;
            backgroundMaterial.SetColor("_Color", Color.Lerp(currentColor, greenColor, t / lerpDuration));
            yield return null;
        }
    }

    private void SetSelectedImage()
    {
        //switch (type)
        //{
        //    case ButtonType.Empty:
        //        background.sprite = ButtonEmpty;
        //        break;
        //    case ButtonType.Circle:
        //        background.sprite = selectedCircle;
        //        break;
        //    case ButtonType.Cross:
        //        background.sprite = selectedCross;
        //        break;
        //    case ButtonType.Arrow:
        //        {
        //            switch (direction)
        //            {
        //                case ButtonDirection.North:
        //                    background.sprite = selectedUpArrow;
        //                    break;
        //                case ButtonDirection.East:
        //                    background.sprite = selectedRightArrow;
        //                    break;
        //                case ButtonDirection.West:
        //                    background.sprite = selectedLeftArrow;
        //                    break;
        //                case ButtonDirection.South:
        //                    background.sprite = selectedDownArrow;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        break;
        //    case ButtonType.RotatableArrow:
        //        {
        //            switch (direction)
        //            {
        //                case ButtonDirection.North:
        //                    background.sprite = selectedRotationUpArrow;
        //                    break;
        //                case ButtonDirection.East:
        //                    background.sprite = selectedRotationRightArrow;
        //                    break;
        //                case ButtonDirection.West:
        //                    background.sprite = selectedRotationLeftArrow;
        //                    break;
        //                case ButtonDirection.South:
        //                    background.sprite = selectedRotationDownArrow;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        break;
        //    default:
        //        break;
        //}
    }

    private void SetNormalImage()
    {
        switch (type)
        {
            case ButtonType.Empty:
                background.sprite = ButtonEmpty;
                break;
            case ButtonType.Circle:
                background.sprite = circle;
                break;
            case ButtonType.Cross:
                background.sprite = cross;
                break;
            case ButtonType.Arrow:
                {
                    switch (direction)
                    {
                        case ButtonDirection.North:
                            background.sprite = upArrow;
                            break;
                        case ButtonDirection.East:
                            background.sprite = rightArrow;
                            break;
                        case ButtonDirection.West:
                            background.sprite = leftArrow;
                            break;
                        case ButtonDirection.South:
                            background.sprite = downArrow;
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
                            background.sprite = rotationUpArrow;
                            break;
                        case ButtonDirection.East:
                            background.sprite = rotationRightArrow;
                            break;
                        case ButtonDirection.West:
                            background.sprite = rotationLeftArrow;
                            break;
                        case ButtonDirection.South:
                            background.sprite = rotationDownArrow;
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
        ButtonEmpty = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_empty");

        circle = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_circle");
        cross = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_cross");
        upArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_arrow-up");
        downArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_arrow-down");
        rightArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_arrow-right");
        leftArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_arrow-left");
        rotationUpArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_rotate-up");
        rotationDownArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_rotate-down");
        rotationRightArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_rotate-right");
        rotationLeftArrow = Resources.Load<Sprite>("Graphics/Puzzle/puzzle_rotate-left");
    }

}
