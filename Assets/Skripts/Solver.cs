using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Solver
{
    private static bool lastStep;

    public static int[,] SolveObvious(int[,] input) {
        int[,] field = input;

        while (true)
        {
            bool noChange = true;
            // small fields
            int count = 0;
            int sum = 0;
            Vector2Int empty = new Vector2Int(0, 0);
            for (int xOffset = 0; xOffset < 7; xOffset += 3)
            {
                for (int yOffset = 0; yOffset < 7; yOffset += 3)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            if (field[xOffset + x, yOffset + y] == 0)
                            {
                                count++;
                                empty = new Vector2Int(xOffset + x, yOffset + y);
                            }
                            sum += field[xOffset + x, yOffset + y];
                        }
                    }
                    if (count == 1)
                    {
                        field[empty.x, empty.y] = 45 - sum;
                        noChange = false;
                    }
                    sum = 0;
                    count = 0;
                }
            }

            // columns
            for (int x = 0; x < 9; x++)
            {
                count = 0;
                sum = 0;
                for (int y = 0; y < 9; y++)
                {
                    if (field[x, y] == 0)
                    {
                        count++;
                        empty = new Vector2Int(x, y);
                    }
                    sum += field[x, y];
                }
                if (count == 1)
                {
                    field[empty.x, empty.y] = 45 - sum;
                    noChange = false;
                }
            }

            // rows
            for (int y = 0; y < 9; y++)
            {
                count = 0;
                sum = 0;
                for (int x = 0; x < 9; x++)
                {
                    if (field[x, y] == 0)
                    {
                        count++;
                        empty = new Vector2Int(x, y);
                    }
                    sum += field[x, y];
                }
                if (count == 1)
                {
                    field[empty.x, empty.y] = 45 - sum;
                    noChange = false;
                }
            }

            if (noChange) break;
        }

        return field;
    }

    public static int[,] SolveFast(int[,] input) {
        int[,] field;
        int[,][] possibilities;
        int count;


        while(true){

            field = (int[,])input.Clone();
            possibilities = new int[9,9][];

            // FIND ALL NUMBER-POSSIBILITIES FOR EACH TILE IN FIELD

            count = 0;
            bool noChange = true;
            for(int x = 0; x < 9; x++) {
                for(int y = 0; y<9; y++) {
                    //foreach tile:
                    if (input[x, y] == 0) {
                        count++;
                        int[] possib = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        // check for non possible numbers due to rows and columns
                        for (int i = 0; i < 9; i++) {
                            if (input[x, i] != 0) possib[input[x, i] - 1] = 0;
                            if (input[i, y] != 0) possib[input[i, y] - 1] = 0;
                        }
                        // check for non possible numbers due to small field
                        for (int i = 0; i < 3; i++) {
                            for (int j = 0; j < 3; j++) {
                                if (input[x - x % 3 + i, y - y % 3 + j] != 0) possib[input[x - x % 3 + i, y - y % 3 + j] - 1] = 0;
                            }
                        }
                        // copy possib to new array with no 0
                        int cnt = 0;
                        for (int i = 0; i < 9; i++) {
                            if (possib[i] != 0) cnt++;
                        }
                        possibilities[x, y] = new int[cnt];
                        cnt = 0;
                        for (int i = 0; i < 9; i++) {
                            if (possib[i] != 0) {
                                possibilities[x, y][cnt] = possib[i];
                                cnt++;
                            }
                        }
                        // if only one possibility, fill it permanently in the corresponding tile
                        if(possibilities[x,y].Length == 1){
                            input[x,y] = possibilities[x,y][0];
                            noChange = false;
                        }
                        // if at least one tile was filled, start whole function again with filed field

                    }
                }
            }
            if(noChange) break;

        }

        // fill first possibility
        for(int x = 0; x < 9; x++) {
            for(int y = 0; y<9; y++) {
                //foreach tile:
                if (input[x, y] == 0) {
                    if (possibilities[x, y].Length == 0) {
                        Debug.LogError("not solvable");
                        Debug.Log("x: " + x + "     y: " + y);
                        GameObject.Find("FieldSpawner").GetComponent<FieldSpawner>().PrintSolution(field);
                    }
                    field[x, y] = possibilities[x, y][0];
                }
            }
        }


        // check first field
        if (IsValid(field))
        {
            Debug.Log("Solution found! (first one)");
            return field;
        }

        int[] state = new int[count];
        for(int i = 0; i<count; i++) {
            state[i] = 0;
        }

        // ITERATE THROUGH THOSE POSSIBILITIES

        Vector2Int firstEmpty = new Vector2Int(-1,-1);
        for(int x = 0; x<9; x++) {
            for(int y = 0; y<9; y++) {
                if(input[x,y] == 0) {
                    firstEmpty = new Vector2Int(x, y);
                    goto FirstFound;
                }
            }
        }
    FirstFound:
        if (firstEmpty.x == -1 || firstEmpty.y == -1)
        {
            Debug.Log("no empty tile!");
            return input;
        }

        while (true)
        {
            count = 0;

            string st = "state ";
            for(int i = 0; i<state.Length; i++) {
                st = st + " " + (state[i]+1).ToString();
            }
            Debug.Log(st);

            st = "possi ";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (possibilities[i, j] != null)
                    {
                        st = st + " " + possibilities[i,j].Length.ToString();
                    }
                }
            }
            Debug.Log(st);

            // if the field isnt filled with the last possibility...
            if (field[firstEmpty.x, firstEmpty.y] != possibilities[firstEmpty.x, firstEmpty.y][possibilities[firstEmpty.x, firstEmpty.y].Length - 1])
            {
                state[count]++;
                field[firstEmpty.x, firstEmpty.y] = possibilities[firstEmpty.x, firstEmpty.y][state[count]];
            }
            else
            {
                state[count] = 0;
                field[firstEmpty.x, firstEmpty.y] = possibilities[firstEmpty.x, firstEmpty.y][state[count]];
                state[count + 1]++;
            }

            count = 0;

            // Iterate over other empty fields to check if idx is out of the array bounds
            for (int x = firstEmpty.x; x < 9; x++)
            {
                for (int y = firstEmpty.y+1; y < 9; y++)
                {
                    if (input[x, y] == 0)
                    {
                        count++;
                        if((count == state.Length-1 && state[count] >= possibilities[x, y].Length) || count>state.Length-1) {
                            // tried each combination but none worked
                            Debug.LogError("no combination worked!");
                            return input;
                        }
                        if (state[count] >= possibilities[x, y].Length)
                        {
                            state[count] = 0;
                            field[x, y] = possibilities[x, y][state[count]];
                            state[count + 1]++;
                        }
                        else
                        {
                            field[x, y] = possibilities[x, y][state[count]];
                        }
                    }
                }
            }


            if (IsValid(field))
            {
                Debug.Log("Solution found!");
                return field;
            }

        }

    }

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
