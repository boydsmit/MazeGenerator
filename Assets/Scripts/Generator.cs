using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int columns;
    [SerializeField] private int rows;

    private enum State
    {
        Possible,
        Filled,
        Current
    };

    private State[,] _currentStates;
    private GameObject[,] _grid;

    private int _curColumn;
    private int _curRow;
    private int _prevColumn;
    private int _prevRow;

    private void Start()
    {
        StartGeneration();
    }

    private void StartGeneration()
    {
        _grid = new GameObject[columns, rows];
        _currentStates = new State[columns, rows];
        var randomCol = Random.Range(0, columns);
        var randomRow = Random.Range(0, rows);
        LoadRandomPrefab(randomCol, randomRow);
    }

    private void LoadRandomPrefab(int curCol, int curRow)
    {
        var random = Random.Range(0, prefabs.Length);
        _currentStates[curCol, curRow] = State.Current;
        _grid[curCol, curRow] = 
            Instantiate(prefabs[random], new Vector3(curCol * 3, 0, curRow * 3), Quaternion.identity);

        (_curColumn, _curRow) = FindStateLocation(State.Current);
        CheckStates();

        _currentStates[curCol, curRow] = State.Filled;
        CorrectShapeRotation();
        SelectNewCurrent();
    }

    private void CheckStates()
    {
        CheckNeighbours(_curColumn + 1, _curRow);
        CheckNeighbours(_curColumn - 1, _curRow);
        CheckNeighbours(_curColumn, _curRow + 1);
        CheckNeighbours(_curColumn, _curRow - 1);
    }
    

    private (int, int) FindStateLocation(State state)
    {    
        for (var i = 0; i < columns; i++)
        {
            for (var j = 0; j < rows; j++)
            {
                if (_currentStates[i, j] == state)
                {
                    return (i, j);
                }
            }
        }

        return(404,404);
    }

    private void CheckNeighbours(int x, int z)
    {
        if (Physics.CheckSphere(new Vector3(x * 3, 0, z * 3), 0.25f) && _currentStates[x, z] != State.Filled)
        {
            _currentStates[x, z] = State.Possible;
        }
    }

    private void SelectNewCurrent()
    {
        for (var i = 0; i < columns; i++)
        {
            for (var j = 0; j < rows; j++)
            {
                if (_currentStates[i, j] == State.Possible)
                { 
                    LoadRandomPrefab(i, j);
                    _prevColumn = _curColumn;
                    _prevRow = _curRow;
                    _curColumn = i;
                    _curRow = j;
                }
            }
        }
    }

    private void CorrectShapeRotation()
    {
        switch (_grid[_curColumn, _curRow].tag)
        {
            case "Hall":
                if (_curColumn != _prevColumn)
                {
                    RandomRotation(_grid[_curColumn,_curRow], 90);
                }
                break;

            case "LShape":
                if (_curColumn > _prevColumn)
                {
                    RandomRotation(_grid[_curColumn,_curRow], 180, true, 270);
                }

                if (_curColumn < _prevColumn)
                {
                    RandomRotation(_grid[_curColumn,_curRow],0,true, 90);
                }

                if (_curRow > _prevRow)
                {
                   RandomRotation(_grid[_curColumn,_curRow], 0, true, 270);
                }

                if (_curRow < _prevRow)
                {
                    RandomRotation(_grid[_curColumn,_curRow], 90, true, 180);
                }
                break;

            case "TShape":
                if (_curColumn < _prevColumn)
                {
                    RandomRotation(_grid[_curColumn, _curRow], 90, true, 270);
                }

                if (_curColumn > _prevColumn)
                {
                    RandomRotation(_grid[_curColumn, _curRow], 270, true, 180);
                }

                if (_curRow > _prevRow)
                {    
                    RandomRotation(_grid[_curColumn, _curRow], 90, true, 180);    
                }

                if (_curRow < _prevRow)
                {
                    RandomRotation(_grid[_curColumn,_curRow],180, true,270);
                }
                break;

            case "End":
                if (_curColumn > _prevColumn)
                {
                    RandomRotation(_grid[_curColumn, _curRow], 90);
                }

                if (_curRow < _prevRow)
                {
                    RandomRotation(_grid[_curColumn, _curRow], 180);
                }

                if (_curRow > _prevRow)
                {
                    RandomRotation(_grid[_curColumn, _curRow], 270);
                }
                break;

            default:
                print("Error at finding possible cell");
                break;
        }
    }

    private void RandomRotation(GameObject obj, int rotation1,bool useRandomizer = false, int rotation2 = 0 )
    {
        if (!useRandomizer)
        {
            obj.transform.Rotate(0,rotation1,0);
        }
        else
        {    
            if (Random.Range(0,2) == 0)
            {
                obj.transform.Rotate(0,rotation1,0);
            }
            else
            {
                obj.transform.Rotate(0,rotation2,0);
            }
        }
    }
}
