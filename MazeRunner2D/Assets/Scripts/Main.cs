using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject distance;
    public GameObject breadCrumb;
    public GameObject maze;
    public GameObject cell;
    public GameObject cellWallEast;
    public GameObject cellWallSouth;
    public GameObject cellWallNorth;
    public GameObject cellWallWest;
    public GameObject goal;
    private GameObject currentMaze;
    private float playerUpdateTimer = Mathf.Epsilon;
    private float currentPlayerUpdateTimer = 0.0f;
    private Dictionary<Tuple<int, int>, List<string>> world;
    private Dictionary<Vector3, GameObject> breadcrumbs = new Dictionary<Vector3, GameObject>();
    private Player player;
    private MyGrid grid;
    private DataManager dataManager = new DataManager();
    private Debugger debugger;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        debugger = GameObject.Find("Debugger").GetComponent<Debugger>();
        player.LoadData();
        // grid = new MyGrid(25, 25);
        // RecursiveBacktracker recursiveBacktracker = new RecursiveBacktracker();

        // recursiveBacktracker.On(ref grid);
        // Serializer.WriteToBinaryFile<MyGrid>("Assets/Resources/Grid.txt", grid);
        grid = Serializer.ReadFromBinaryFile<MyGrid>("Assets/Resources/Grid.txt");
        Initialize();
    }
    // Start is called before the first frame update
    void Initialize()
    {
        dataManager.Reset();
        debugger.Reset();

        currentMaze = Instantiate(maze, Vector3.zero, Quaternion.identity);
        
        int startX = UnityEngine.Random.Range(0, grid.columns);
        int startY = UnityEngine.Random.Range(0, grid.rows);
        dataManager.SetStartValues(startX, startY);
        // int startX = 0;
        // int startY = 0;
        Cell start = grid.CellAt(startX, startY);
        player.transform.position = new Vector3(start.column, start.row, 0.0f);

        Distances distances = start.Distances;
        grid.distances = distances.PathToGoal(grid.CellAt(grid.rows - 1, grid.columns - 1));
        
        GameObject newGoal = Instantiate(goal, new Vector3(grid.CellAt(grid.rows - 1, grid.columns - 1).column, grid.CellAt(grid.rows - 1, grid.columns - 1).row, 0.0f), Quaternion.identity);
        newGoal.transform.parent = currentMaze.transform;

        BuildMaze(grid);
        player.SetCurrentState(world);
    }

    private void BuildMaze(MyGrid grid)
    {
        world = new Dictionary<Tuple<int, int>, List<string>>();
        // breadcrumbs = new Dictionary<Vector3, GameObject>();
        for(int i = 0; i < grid.rows; i++)
        {
            for(int j = 0; j < grid.columns; j++)
            {
                List<string> cellState = new List<string>();
                Cell currentCell = grid.CellAt(i, j);
                Vector3 position = new Vector3(currentCell.column, currentCell.row, 0.0f);
                AddCellToParentMaze(position);
                Dictionary<CellLocation, Cell> unlinkedCells = currentCell.GetUnlinks();
                foreach(CellLocation cellLocation in unlinkedCells.Keys)
                {
                    // print(cellLocation);
                    if(cellLocation == CellLocation.NORTH)
                    {
                        Vector3 tmpPosition = position;
                        tmpPosition.y += 0.451f;
                        AddWallToParentMaze(cellWallNorth, tmpPosition);
                        cellState.Add("NorthBlocked");
                    }
                    else if(cellLocation == CellLocation.SOUTH)
                    {
                        Vector3 tmpPosition = position;
                        tmpPosition.y -= 0.451f;
                        AddWallToParentMaze(cellWallSouth, tmpPosition);
                        cellState.Add("SouthBlocked");
                    }
                    else if(cellLocation == CellLocation.EAST)
                    {
                        Vector3 tmpPosition = position;
                        tmpPosition.x += 0.451f;
                        AddWallToParentMaze(cellWallEast, tmpPosition);
                        cellState.Add("EastBlocked");
                    }
                    else if(cellLocation == CellLocation.WEST)
                    {
                        Vector3 tmpPosition = position;
                        tmpPosition.x -= 0.451f;
                        AddWallToParentMaze(cellWallWest, tmpPosition);
                        cellState.Add("WestBlocked");
                    }
                }
                world.Add(Tuple.Create(currentCell.column, currentCell.row), cellState);
            }
        }
        // foreach(Cell cell in grid.distances.Cells())
        // {
        //     GameObject newDistance = Instantiate(distance, new Vector3(cell.column, cell.row, 0.0f), Quaternion.identity);
        //     newDistance.transform.parent = currentMaze.transform;
        // }
    }

    private void AddCellToParentMaze(Vector3 position)
    {
        GameObject newCell = Instantiate(cell, position, Quaternion.identity);
        newCell.transform.parent = currentMaze.transform;
    }
    private void AddWallToParentMaze(GameObject wall, Vector3 position)
    {
        GameObject newWall = Instantiate(wall, position, Quaternion.identity);
        newWall.transform.parent = currentMaze.transform;
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerUpdateTimer += Time.deltaTime;
        if(currentPlayerUpdateTimer > playerUpdateTimer)
        {
            player.SelectAction();
            float currentQ = player.GetQ();
            
            dataManager.UpdateTimer(currentPlayerUpdateTimer);
            dataManager.SetQValue(currentQ);
            dataManager.SetMinMaxQValues(currentQ);

            debugger.SetColors(currentQ, dataManager.GetMinQValue(), dataManager.GetMaxQValue());
            debugger.SpawnMove(player, grid);

            player.Move(world);
            player.SetNextState(world);

            // player.UpdateAllTau();

            if(player.hasFoundGoal)
            {
                print("Found goal");
                player.hasFoundGoal = false;
                player.QUpdate(100.0f, true);
                // player.UpdateModel(100.0f);
                player.SaveData();
                dataManager.RegisterObservation();
                Destroy(currentMaze);
                Initialize();
            }
            else if(!player.hasMoved)
            {
                dataManager.CollidedWithWalls();
                player.QUpdate(-1.0f, false);
                // player.UpdateModel(-1.0f);
            }
            else
            {
                player.QUpdate(-1.0f, false);
                // player.UpdateModel(-1.0f);
            }

            // for(int i = 0; i < 100; i++)
            // {
            //     player.RunSimulation();
            // }

            currentPlayerUpdateTimer = 0.0f;
        }
    }
}
