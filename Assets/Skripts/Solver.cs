using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SolverComplicated
{
    private static bool lastStep;

    // Variables for SolveBackTrackingStep
    public static int[,] inputfield, editingfield;
    public static int[] missingNrs, possibState;
    public static int[,][] possib;
    public static Vector2Int[] emptyCoordinates;
    public static int currentTileNr;
    public static bool backInNextStep;

    public static int[,] SolveObvious(int[,] input) {
        int[,] field;
        int[,][] possibilities;
        int count;


        while (true)
        {

            field = (int[,])input.Clone();
            possibilities = new int[9, 9][];

            // FIND ALL NUMBER-POSSIBILITIES FOR EACH TILE IN FIELD

            count = 0;
            bool noChange = true;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    //foreach tile:
                    if (input[x, y] == 0)
                    {
                        count++;
                        int[] possib = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        // check for non possible numbers due to rows and columns
                        for (int i = 0; i < 9; i++)
                        {
                            if (input[x, i] != 0) possib[input[x, i] - 1] = 0;
                            if (input[i, y] != 0) possib[input[i, y] - 1] = 0;
                        }
                        // check for non possible numbers due to small field
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (input[x - x % 3 + i, y - y % 3 + j] != 0) possib[input[x - x % 3 + i, y - y % 3 + j] - 1] = 0;
                            }
                        }
                        // copy possib to new array with no 0
                        int cnt = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            if (possib[i] != 0) cnt++;
                        }
                        possibilities[x, y] = new int[cnt];
                        cnt = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            if (possib[i] != 0)
                            {
                                possibilities[x, y][cnt] = possib[i];
                                cnt++;
                            }
                        }
                        // if only one possibility, fill it permanently in the corresponding tile
                        if (possibilities[x, y].Length == 1)
                        {
                            input[x, y] = possibilities[x, y][0];
                            noChange = false;
                        }
                        // if at least one tile was filled, start whole function again with filed field

                    }
                }
            }
            if (noChange) break;

        }

        return field;
    }

    //Finds all combinations of numbers to fit into the field
    public static int[,][] FindPossibilitiesForField(int[,] field) {
        int[,][] output = new int[9,9][];
        for(int x = 0; x<9; x++) { 
            for(int y = 0; y<9; y++) { 
                if(field[x,y] == 0) {
                    output[x, y] = FindPossibilitiesForEmptyTile(field, new Vector2Int(x, y));
                }
            }
        }
        return output;
    }

    //Finds all numbers which fit in a specific tile of an field
    public static int[] FindPossibilitiesForEmptyTile(int[,]field, Vector2Int coordinates) {
        if (field[coordinates.x, coordinates.y] != 0) return null;

        int x = coordinates.x;
        int y = coordinates.y;

        int[] allpossib = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };


        // check for non possible numbers due to rows and columns
        for (int i = 0; i < 9; i++)
        {
            if (field[x, i] != 0) allpossib[field[x, i] - 1] = 0;
            if (field[i, y] != 0) allpossib[field[i, y] - 1] = 0;
        }


        // check for non possible numbers due to small field
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (field[x - x % 3 + i, y - y % 3 + j] != 0) allpossib[field[x - x % 3 + i, y - y % 3 + j] - 1] = 0;
            }
        }


        // copy possib to new array with no zeros
        int cnt = 0;
        for (int i = 0; i < 9; i++)
        {
            if (allpossib[i] != 0) cnt++;
        }
        int[] output = new int[cnt];
        cnt = 0;
        for (int i = 0; i < 9; i++)
        {
            if (allpossib[i] != 0)
            {
                output[cnt] = allpossib[i];
                cnt++;
            }
        }


        return output;
    }

    //Finds all numbers in an unsolved sudoku field and writes it in an int array with length 10, where every slot is a count for the number "index", and the "0" slot displays the amount of empty tiles.
    public static int[] FindMissingNumbersInField(int[,] field)
    {
        int[] output = new int[10];
        output[0] = 0;
        for(int i = 1; i<10; i++) {
            output[i] = 9;
        }
        foreach(int i in field) {
            if (i != 0) output[i]--;
            else output[0]++;
        }

        return output;
    }

    public static Vector2Int[] FindMissingCoordinates(int[,] field) {
        Vector2Int[] output;
        int count = 0;
        for(int i = 0; i<field.GetLength(0); i++) { 
            for(int j = 0; j<field.GetLength(1); j++) {
                if (field[i, j] == 0) count++;
            }
        }
        output = new Vector2Int[count];
        count = 0;
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                if (field[i, j] == 0)
                {
                    output[count] = new Vector2Int(i, j);
                    count++;
                }
            }
        }
        return output;
    }

    // return if algorithm has ended
    public static bool SolveBackTrackingStep(int[,] input, bool firstIteration = false) {


        if (firstIteration) {
            possib = FindPossibilitiesForField(input);
            editingfield = input;
            missingNrs = FindMissingNumbersInField(input);
            if (missingNrs[0] == 0) return true;
            possibState = new int[missingNrs[0]];
            for(int i = 1; i<possibState.Length; i++) {
                possibState[i] = -1;
            }
            possibState[0] = 0;
            emptyCoordinates = FindMissingCoordinates(input);
            editingfield[emptyCoordinates[0].x, emptyCoordinates[0].y] = possib[emptyCoordinates[0].x, emptyCoordinates[0].y][0];
            missingNrs[possib[emptyCoordinates[0].x, emptyCoordinates[0].y][0]]--;
            currentTileNr = 0;
            backInNextStep = false;
            if (missingNrs[0] == 0) return true;
            return false;
        }


        else {
            //Try -> Error -> Backtracking
            //start with: first empty tile, first possibility, leave all others empty

                if (IsValid(editingfield, false) && !backInNextStep)
                {
                    // step is ok for now, continue with next tile, or, if field is full, return field
                    Debug.Log("next tile");
                    // current tile is the last one and field is valid -> retrun field
                    if (currentTileNr + 1 >= possibState.Length) return true;

                    // current tile isn't the last one, edit successor
                    currentTileNr++;
                    Debug.Log("coo-len: " + emptyCoordinates.Length + "  curr-tile: " + currentTileNr + "  state-len: " + possibState.Length);
                    Vector2Int coo = emptyCoordinates[currentTileNr];
    FindNextStep:
                    if (possibState[currentTileNr] >= 0 && possib[coo.x, coo.y][possibState[currentTileNr]] > 0)
                    {
                        missingNrs[possib[coo.x, coo.y][possibState[currentTileNr]]]++;
                    }
                    if(possibState[currentTileNr] >= possib[coo.x, coo.y].Length-1) {
                        backInNextStep = true;
                        return false;
                    }
                    possibState[currentTileNr]++;
                    missingNrs[possib[coo.x, coo.y][possibState[currentTileNr]]]--;
                    if (missingNrs[possib[coo.x, coo.y][possibState[currentTileNr]]] < 0)
                    {
                        goto FindNextStep;
                    }
                    editingfield[coo.x, coo.y] = possib[coo.x, coo.y][possibState[currentTileNr]];

                backInNextStep = false;
                return false;
                }
                else
                {
                    // step isn't valid, try next possible number for this tile or, if none left, continue with the previous tile

                    if (currentTileNr == -1)
                    {
                        Debug.Log("no solution found!");
                        return true;
                    }
                    Vector2Int coo = emptyCoordinates[currentTileNr];
                    // try next possible number in current tile if there is a next one, else go one tile back, try next value (in the next iteration) and reset the current tile's state
                    if (possibState[currentTileNr] + 1 < possib[coo.x, coo.y].Length)
                    {
                        Debug.Log("same tile");
    FindNextStep2:
                        if (possibState[currentTileNr] >= 0 && possib[coo.x, coo.y][possibState[currentTileNr]] > 0)
                        {
                            missingNrs[possib[coo.x, coo.y][possibState[currentTileNr]]]++;
                        }
                        if(possibState[currentTileNr] >= possib[coo.x, coo.y].Length - 1) {
                            goto StepBack;
                        }
                        possibState[currentTileNr]++;
                        missingNrs[possib[coo.x,coo.y][possibState[currentTileNr]]]--;
                        if (missingNrs[possib[coo.x, coo.y][possibState[currentTileNr]]] < 0)
                        {
                            goto FindNextStep2;
                        }
                        editingfield[coo.x, coo.y] = possib[coo.x, coo.y][possibState[currentTileNr]];
                        backInNextStep = false;
                        return false;
                    }
    StepBack:
                        Debug.Log("last tile");
                        missingNrs[possib[coo.x, coo.y][possibState[currentTileNr]]]++;
                        possibState[currentTileNr] = -1;
                        editingfield[coo.x, coo.y] = 0;
                        currentTileNr--;
                        backInNextStep = true;

                    return false;
                }

        }


    }

    // do as many steps as needed till a error appears, go one step back and try another possibility
    public static int[,] SolveBackTracking(int[,] input){
        int[,] field = (int[,])input.Clone();
        int[,][] possibilities = new int[9,9][];;
        int count = 0;

            // FIND ALL NUMBER-POSSIBILITIES FOR EACH TILE IN FIELD
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
                        // copy possib to new array with no zeros
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
                    }
                }
            }


        int[] state = new int[count];
        for(int i = 0; i<count; i++) {
            state[i] = -1;
        }
        state[0] = 0;

        // ITERATE THROUGH THOSE POSSIBILITIES

        Vector2Int[] coordinates = new Vector2Int[count];
        int tileNumber = 0;
        for(int x = 0; x<9; x++) {
            for(int y = 0; y<9; y++) {
                if(input[x,y] == 0) {
                    coordinates[tileNumber] = new Vector2Int(x,y);
                    tileNumber++;
                }
            }
        }
        if (count == 0)
        {
            Debug.Log("no empty tile!");
            return input;
        }

        //Try -> Error -> Backtracking
        //start with: first empty tile, first possibility, leave all others empty
        field[coordinates[0].x, coordinates[0].y] = possibilities[coordinates[0].x, coordinates[0].y][0];
        int currentTile = 0;
        bool stepBack = false;
        while(true){

            if(IsValid(field, false) && !stepBack){
                // step is ok for now, continue with next tile, or, if field is full, return field
                Debug.Log("next tile");
                // current tile is the last one and field is valid -> retrun field
                if(currentTile+1 >= state.Length) return field;

                // current tile isn't the last one, edit successor
                currentTile++;
                Debug.Log("coo-len: " + coordinates.Length + "  curr-tile: " + currentTile + "  state-len: " + state.Length);
                Vector2Int coo = coordinates[currentTile];
                state[currentTile]++;
                field[coo.x, coo.y] = possibilities[coo.x,coo.y][state[currentTile]];
                stepBack = false;

            }else{
                // step isn't valid, try next possible number for this tile or, if none left, continue with the previous tile

                if (currentTile == -1)
                {
                    Debug.Log("no solution found!");
                    return field;
                }
                Vector2Int coo = coordinates[currentTile];
                // try next possible number in current tile if there is a next one, else go one tile back, try next value (in the next iteration) and reset the current tile's state
                if(state[currentTile]+1 < possibilities[coo.x,coo.y].Length){
                    Debug.Log("same tile");
                    state[currentTile]++;
                    field[coo.x,coo.y] = possibilities[coo.x,coo.y][state[currentTile]];
                    stepBack = false;
                }else{
                    Debug.Log("last tile");
                    state[currentTile] = -1;
                    field[coo.x,coo.y] = 0;
                    currentTile--;
                    stepBack = true;
                }
            }



        }
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

    public static bool IsValid(int[,] field, bool filled = true)
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
                    if (field[i, j] == 0 && filled) return false;
                    if(field[i,j] == 0) break;
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
                    if (!IsValid(smallField, filled)) return false;
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
                    if(field[x,y] == 0 && filled) return false;
                    if(field[x,y] == 0) break;
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
                    if(field[x,y] == 0 && filled) return false;
                    if(field[x,y] == 0) break;
                    if (foundValues[field[x, y]-1]) return false;
                    foundValues[field[x, y]-1] = true;
                }
                // initialize the foundValues with false again to start with a new row
                for (int i = 0; i < foundValues.Length; i++)
                {
                    foundValues[i] = false;
                }
            }

            if (!filled) {
                int[] numberCount = new int[9];
                for(int i = 0; i<9; i++) {
                    numberCount[i] = 0;
                }
                for(int i = 0; i<9; i++) { 
                    for(int j = 0; j<9; j++) { 
                        if(field[i,j] != 0) {
                            numberCount[field[i, j]-1]++;
                        }
                    }
                }
                for(int i = 0; i<9; i++) {
                    if (numberCount[i] > 9) return false;
                }
            }

            // no mistakes found -> field is valid
            return true;
        }
    }
}
