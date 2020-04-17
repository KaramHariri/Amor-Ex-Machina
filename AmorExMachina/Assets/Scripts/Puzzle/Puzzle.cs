using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    //public List<PuzzleButton> buttons;
    public PuzzleButton[] buttons;
    private PuzzleButton selectedButton;
    private List<PuzzleButton> activateableButtons = new List<PuzzleButton>();
    private int selectedButtonIndex = 0;

    public FlexibleGridLayout layout;
    public int rows;
    public int columns;

    private List<PuzzleButton> flipButtons = new List<PuzzleButton>();

    float buttonSwitchingCooldown = 0f;
    float buttonSwitchingCooldownTime = 0.20f;
    float buttonRotateCooldown = 0f;
    //float buttonRotateCooldownTime = 0.2f;
    float buttonActivateCooldown = 0f;
    float timeBetweenFlips = 0.2f;

    public PuzzleActivator PA;

    private void Start()
    {
        foreach(PuzzleButton b in buttons)
        {
            if(b.type != PuzzleButton.ButtonType.Empty)
            {
                activateableButtons.Add(b);
            }
        }

        if(activateableButtons.Count == 0)
        {
            Debug.Log("All buttons are 'Empty'");
            return;
        }

        selectedButton = activateableButtons[0];
        SetHover(activateableButtons[0].transform.GetSiblingIndex());
    }

    private void Update()
    {
        DecreaseCooldowns();

        HandleMouseAndKeyboardInput();
        HandleControllerInput();
    }

    public void Subscribe(PuzzleButton button)
    {
        //if (buttons == null)
        //{
        //    buttons = new List<PuzzleButton>();
        //}

        //buttons.Add(button);
    }

    public void OnButtonEnter(PuzzleButton button)
    {
        ResetButtons();
        if (button.type != PuzzleButton.ButtonType.Empty)
        {
            SetHover(button.transform.GetSiblingIndex());
        }
    }

    public void OnButtonExit(PuzzleButton button)
    {
        ResetButtons();
    }

    public void OnButtonSelected(PuzzleButton button)
    {
        if (selectedButton != null)
        {
            selectedButton.Deselect();
        }

        selectedButton = button;

        selectedButton.Select();

        ResetButtons();
    }

    public void ResetButtons()
    {
        foreach (PuzzleButton button in buttons)
        {
            if (selectedButton != null && button == selectedButton) { continue; }
        }
    }

    public void SetActive(int index)
    {
        //Debug.Log("Set Active - Index " + index);
        selectedButton.Deselect();
        selectedButton = buttons[index];
        selectedButton.Select();
    }

    public void SetHover(int index)
    {
        //Debug.Log("Set Active - Index " + index);
        selectedButton.Deselect();
        selectedButton = buttons[index];
        selectedButton.Hovered();
    }

    public void GenerateFlipTileList()
    {
        if(selectedButton == null) { return; }

        flipButtons.Clear();

        switch (selectedButton.type)
        {
            case PuzzleButton.ButtonType.Circle:
                {
                    GenerateCircleFlip();
                }
                break;
            case PuzzleButton.ButtonType.Cross:
                {
                    GenerateCrossFlip();
                }
                break;
            case PuzzleButton.ButtonType.Arrow:
                {
                    GenerateArrowFlip();
                }
                break;
            case PuzzleButton.ButtonType.RotatableArrow:
                {
                    GenerateArrowFlip();
                }
                break;
            default:
                break;
        }

    }

    public void GenerateCircleFlip()
    {
        int index = selectedButton.transform.GetSiblingIndex();
        int yPos = index / columns;
        int xPos = index % columns;

        List<Vector2> temp = new List<Vector2>();

        //Origo is in the top left.
        temp.Add( new Vector2(yPos - 1, xPos)); //North
        temp.Add(new Vector2(yPos - 1, xPos + 1)); //NorthEast
        temp.Add( new Vector2(yPos, xPos + 1)); //East
        temp.Add(new Vector2(yPos + 1, xPos + 1)); //SouthEast
        temp.Add( new Vector2(yPos + 1, xPos)); //South
        temp.Add(new Vector2(yPos + 1, xPos - 1)); //SouthWest
        temp.Add( new Vector2(yPos, xPos - 1)); //West
        temp.Add(new Vector2(yPos - 1, xPos - 1)); //NorthWest



        foreach(Vector2 pos in temp)
        {
            if(pos.x < 0 || pos.y < 0 || pos.x > columns - 1 || pos.y > rows - 1)
            {
                continue;
            }

            int tileIndex = (int)((pos.x * columns) + pos.y);
            //Debug.Log("Added a button to flip at position (" + pos.x + ", " + pos.y + ")" + " it has index = " + tileIndex);
            if(buttons[tileIndex].type == PuzzleButton.ButtonType.Empty)
            {
                flipButtons.Add(buttons[tileIndex]);
            }
        
        }
    }

    public void GenerateCrossFlip()
    {
        int index = selectedButton.transform.GetSiblingIndex();
        int yPos = index / columns;
        int xPos = index % columns;

        List<Vector2> temp = new List<Vector2>();

        //Origo is in the top left.
        temp.Add(new Vector2(yPos - 1, xPos + 1)); //NorthEast
        temp.Add(new Vector2(yPos + 1, xPos + 1)); //SouthEast
        temp.Add(new Vector2(yPos + 1, xPos - 1)); //SouthWest
        temp.Add(new Vector2(yPos - 1, xPos - 1)); //NorthWest

        foreach (Vector2 pos in temp)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x > columns - 1 || pos.y > rows - 1)
            {
                continue;
            }

            int tileIndex = (int)((pos.x * columns) + pos.y);
            //Debug.Log("Added a button to flip at position (" + pos.x + ", " + pos.y + ")" + " it has index = " + tileIndex);
            if (buttons[tileIndex].type == PuzzleButton.ButtonType.Empty)
            {
                flipButtons.Add(buttons[tileIndex]);
            }
        }
    }

    public void GenerateArrowFlip()
    {
        int index = selectedButton.transform.GetSiblingIndex();
        int yPos = index / columns;
        int xPos = index % columns;
        //Debug.Log("Selected button at ( " + xPos + " , " + yPos + ")");

        List<Vector2> temp = new List<Vector2>();

        switch (selectedButton.direction)
        {
            case PuzzleButton.ButtonDirection.North:
                {
                    for (yPos = yPos - 1; yPos >= 0; yPos--)
                    {
                        //int tileIndex = (int)((xPos * columns) + yPos);
                        int tileIndex = (int)((yPos * columns) + xPos);
                        //Debug.Log("button at position (" + xPos + ", " + yPos + ")" + " it has index = " + tileIndex);
                        if (buttons[tileIndex].type == PuzzleButton.ButtonType.Empty)
                        {
                            flipButtons.Add(buttons[tileIndex]);
                        }
                    }
                }
                break;
            case PuzzleButton.ButtonDirection.East:
                {
                    for (xPos = xPos + 1; xPos <= columns - 1; xPos++)
                    {
                        //int tileIndex = (int)((xPos * columns) + yPos);
                        int tileIndex = (int)((yPos * columns) + xPos);
                        //Debug.Log("button at position (" + xPos + ", " + yPos + ")" + " it has index = " + tileIndex);
                        if (buttons[tileIndex].type == PuzzleButton.ButtonType.Empty)
                        {
                            flipButtons.Add(buttons[tileIndex]);
                        }
                    }
                }
                break;
            case PuzzleButton.ButtonDirection.West:
                {
                    for (xPos = xPos - 1; xPos >= 0; xPos--)
                    {
                        //int tileIndex = (int)((xPos * columns) + yPos);
                        int tileIndex = (int)((yPos * columns) + xPos);
                        //Debug.Log("button at position (" + xPos + ", " + yPos + ")" + " it has index = " + tileIndex);
                        if (buttons[tileIndex].type == PuzzleButton.ButtonType.Empty)
                        {
                            flipButtons.Add(buttons[tileIndex]);
                        }
                    }
                }
                break;
            case PuzzleButton.ButtonDirection.South:
                {
                    for (yPos = yPos + 1; yPos <= rows - 1; yPos++)
                    {
                        //int tileIndex = (int)((xPos * columns) + yPos);
                        int tileIndex = (int)((yPos * columns) + xPos);
                        //Debug.Log("button at position (" + xPos + ", " + yPos + ")" + " it has index = " + tileIndex);
                        if (buttons[tileIndex].type == PuzzleButton.ButtonType.Empty)
                        {
                            flipButtons.Add(buttons[tileIndex]);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    IEnumerator ActivateFlipping()
    {
        while (flipButtons.Count > 0)
        {
            yield return new WaitForSeconds(timeBetweenFlips);
            flipButtons[0].Select();
            flipButtons.RemoveAt(0);
        }
        CheckIfGameIsWon();
    }

    public bool CheckIfGameIsWon()
    {
        foreach (PuzzleButton button in buttons)
        {
            if(button.type == PuzzleButton.ButtonType.Empty)
            {
                if(button.buttonOn == false)
                {
                    return false;
                }

            }
        }
        Debug.Log("Puzzle completed!");

        if(PA != null)
        {
            PA.DeactivatePuzzle();
        }

        return true;
    }

    private void HandleMouseAndKeyboardInput()
    {
        if (selectedButton == null) { Debug.Log("selectedButton is null"); return; }

        //if (Input.GetButtonDown("X");


        if (Input.GetKeyDown(KeyCode.LeftArrow) && buttonSwitchingCooldown == 0.0f && selectedButtonIndex > 0)
        {
            //Debug.Log("L");
            buttonSwitchingCooldown = buttonSwitchingCooldownTime;
            selectedButtonIndex--;
            SetHover(activateableButtons[selectedButtonIndex].transform.GetSiblingIndex());
            return;
        }


        if (Input.GetKeyDown(KeyCode.RightArrow) && buttonSwitchingCooldown == 0.0f && selectedButtonIndex + 1 < activateableButtons.Count)
        {
            //Debug.Log("R");
            buttonSwitchingCooldown = buttonSwitchingCooldownTime;
            selectedButtonIndex++;
            SetHover(activateableButtons[selectedButtonIndex].transform.GetSiblingIndex());
            return;
        }


        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.F) ) && buttonRotateCooldown == 0.0f)
        {
            selectedButton.RotateDirection();
            return;
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) ) && buttonActivateCooldown == 0.0f)
        {
            selectedButton.Select();
            GenerateFlipTileList();
            buttonActivateCooldown = (flipButtons.Count * timeBetweenFlips) + 0.2f;
            StartCoroutine(ActivateFlipping());
        }
    }

    private void HandleControllerInput()
    {
        if (selectedButton == null) { Debug.Log("selectedButton is null"); return; }

        //if (Input.GetButtonDown("X");


        if (Input.GetAxis("Horizontal") < -0.6f && buttonSwitchingCooldown == 0.0f && selectedButtonIndex > 0)
        {
            //Debug.Log("L");
            buttonSwitchingCooldown = buttonSwitchingCooldownTime;
            selectedButtonIndex--;
            SetHover(activateableButtons[selectedButtonIndex].transform.GetSiblingIndex());
            return;
        }


        if (Input.GetAxis("Horizontal") > 0.6f && buttonSwitchingCooldown == 0.0f && selectedButtonIndex + 1 < activateableButtons.Count)
        {
            //Debug.Log("R");
            buttonSwitchingCooldown = buttonSwitchingCooldownTime;
            selectedButtonIndex++;
            SetHover(activateableButtons[selectedButtonIndex].transform.GetSiblingIndex());
            return;
        }


        if (Input.GetButtonDown("Triangle") && buttonRotateCooldown == 0.0f)
        {
            //if (selectedButton != null && selectedButton.type == PuzzleButton.ButtonType.RotatableArrow)
            //{
                selectedButton.RotateDirection();
                return;
            //}
        }

        if (Input.GetButtonDown("X") && buttonActivateCooldown == 0.0f)
        {
            selectedButton.Select();
            GenerateFlipTileList();
            buttonActivateCooldown = (flipButtons.Count * timeBetweenFlips) + 0.2f;
            StartCoroutine(ActivateFlipping());
        }
    }

    private void DecreaseCooldowns()
    {
        buttonSwitchingCooldown -= Time.deltaTime;
        if (buttonSwitchingCooldown < 0f) { buttonSwitchingCooldown = 0f; }
        buttonRotateCooldown -= Time.deltaTime;
        if (buttonRotateCooldown < 0f) { buttonRotateCooldown = 0f; }
        buttonActivateCooldown -= Time.deltaTime;
        if (buttonActivateCooldown < 0f) { buttonActivateCooldown = 0f; }
    }
}
