using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    //public List<PuzzleButton> buttons;
    public PuzzleButton[] buttons;
    public PuzzleButton selectedButton;

    public FlexibleGridLayout layout;
    public int rows;
    public int columns;

    public List<PuzzleButton> flipButtons = new List<PuzzleButton>();

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(selectedButton != null && selectedButton.transform.GetSiblingIndex() > 0)
            {
                //Debug.Log("Current index = " +selectedButton.transform.GetSiblingIndex());
                SetHover(selectedButton.transform.GetSiblingIndex() - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedButton != null && selectedButton.transform.GetSiblingIndex() < (layout.transform.childCount - 1))
            {
                //Debug.Log("Current index = " + selectedButton.transform.GetSiblingIndex());
                SetHover(selectedButton.transform.GetSiblingIndex() + 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedButton != null && (selectedButton.transform.GetSiblingIndex() - columns >= 0))
            {
                //Debug.Log("up Current index = " + selectedButton.transform.GetSiblingIndex());
                SetHover(selectedButton.transform.GetSiblingIndex() - columns);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedButton != null && (selectedButton.transform.GetSiblingIndex() + columns) < (layout.transform.childCount))
            {
                //Debug.Log("down Current index = " + selectedButton.transform.GetSiblingIndex());
                //Debug.Log("layout transform childcount = " + layout.transform.childCount);
                SetHover(selectedButton.transform.GetSiblingIndex() + columns);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(selectedButton != null)
            {
                selectedButton.Select();
                GenerateFlipTileList();
                StartCoroutine(ActivateFlipping());
                //CheckIfGameIsWon();
            }

        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            if (selectedButton != null && selectedButton.type == PuzzleButton.ButtonType.RotatableArrow)
            {
                selectedButton.RotateDirection();
            }

        }


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
        if (selectedButton == null || button != selectedButton)
        {
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
        float waitTime = 0.35f;

        while (flipButtons.Count > 0)
        {
            yield return new WaitForSeconds(waitTime);
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
        return true;
    }
}
