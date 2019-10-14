using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsValid(int[,] field)
    {
        if (field.GetLength(0) != field.GetLength(1) || (field.GetLength(0) != 3 && field.GetLength(0) != 9)) return false;
        if (field.GetLength(0) == 3)
        {
            // Check if small field is valid
            // Saves which Values already appeared in this field
            bool[] foundValues = new bool[9];
            // At the start, no values are visited; initialize the whole array with false
            for (int i = 0; i < foundValues.Length; i++)
            {
                foundValues[i] = false;
            }
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    // if foundValues of this position is already true, a pair was found, so the field isn't valid
                    if (foundValues[field[i, j]]) return false;
                    foundValues[field[i, j]] = true;
                }
            }
            // if no mistakes found, return true
            return true;
        }
        else
        {
            // Check if big field is valid
            // Check all small fields
            int[,] smallField = new int[3,3];
            for (int xOffset = 0; xOffset < 9; xOffset += 3)
            {
                for (int yOffset = 0; yOffset < 9; yOffset += 3)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            smallField[x, y] = field[x + xOffset, y + yOffset];
                        }
                    }
                    // return false if one of the small fields is not valid
                    if (!IsValid(smallField)) return false;
                }
            }

            // Check if all rows are IsValid
            // At the start, no values are visited; initialize the whole array with false
            bool[] foundValues = new bool[9];
            for (int i = 0; i < foundValues.Length; i++)
            {
                foundValues[i] = false;
            }
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if (foundValues[field[x, y]]) return false;
                    foundValues[field[x, y]] = true;
                }
                // initialize the foundValues with false again to start with a new row
                for (int i = 0; i < foundValues.Length; i++)
                {
                    foundValues[i] = false;
                }
            }

            // Check if all columns are IsValid
            // At the start, no values are visited; initialize the whole array with false
            for (int i = 0; i < foundValues.Length; i++)
            {
                foundValues[i] = false;
            }
            for (int y = 0; y < field.GetLength(1); y++)
            {
                for (int x = 0; x < field.GetLength(0); x++)
                {
                    if (foundValues[field[x, y]]) return false;
                    foundValues[field[x, y]] = true;
                }
                // initialize the foundValues with false again to start with a new row
                for (int i = 0; i < foundValues.Length; i++)
                {
                    foundValues[i] = false;
                }
            }
            // no mistakes found -> field is valid
            return true;
        }
    }
}
