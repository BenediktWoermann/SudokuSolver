using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Solver
{
    private static bool lastStep;

    public static int[,] Solve(int[,] input) {
        int counttest = 0;
        int[,] field = input;
        int count = 0;
        for(int i = 0; i<9; i++) { 
            for(int j = 0; j<9; j++) { 
                if(field[i,j] == 0) {
                    count++;
                }
            }
        }
        int[] solution = new int[count];
        for(int i = 0; i<solution.Length; i++) {
            solution[i] = 1;
        }

        while (true) 
        {
            counttest++;
            field = (int[,])input.Clone();
            count = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (field[i, j] == 0)
                    {
                        field[i, j] = solution[count];
                        count++;
                    }
                }
            }
            if (IsValid(field))
            {
                return field;
            }
            // find new possible solution
            if (solution[0] < 9) solution[0]++;
            else
            {
                if (solution[0] == 9)
                {
                    solution[0] = 1;
                    solution[1]++;
                }
            }
            for (int i = 1; i < solution.Length - 1; i++)
            {
                if (solution[i] == 10)
                {
                    solution[i] = 1;
                    solution[i + 1]++;
                }
            }
            //Debug.Log(solution[0] + "  " + solution[1] + "  " + solution[2] + "  " + solution[3]);
            if (lastStep)
            {
                //Debug.Log(counttest);
                return null;
            }
            lastStep = true;
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] != 9)
                {
                    lastStep = false;
                }
            }
        } 
    }

    public static bool IsValid(int[,] field)
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
                    if (foundValues[field[i, j]-1]) return false;
                    foundValues[field[i, j]-1] = true;
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
                            //Debug.Log(smallField[x,y]);
                        }
                    }
                    // return false if one of the small fields is not valid
                    if (!IsValid(smallField)) return false;
                    Debug.Log("small ok");
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
                    if (foundValues[field[x, y]-1]) return false;
                    foundValues[field[x, y]-1] = true;
                }
                // initialize the foundValues with false again to start with a new row
                for (int i = 0; i < foundValues.Length; i++)
                {
                    foundValues[i] = false;
                }
            }
            Debug.Log("rows ok");

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
                    if (foundValues[field[x, y]-1]) return false;
                    foundValues[field[x, y]-1] = true;
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
