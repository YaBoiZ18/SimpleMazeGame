using UnityEngine;
using System.Collections.Generic;

public class Cell
{
    // Whether the maze generation algorithm has visited this cell
    public bool visited = false;

    // References to instantiated wall GameObjects for this cell
    public GameObject topWall;
    public GameObject bottomWall;
    public GameObject leftWall;
    public GameObject rightWall;
}

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]  
    public int width = 7; // Number of cells horizontally 
    public int height = 7; // Number of cells vertically
    public float cellSize = 4f; // World size of each cell (distance between cell centers)

    [Header("Wall Prefab")]  
    public GameObject wallPrefab; // Prefab used to create walls (should be a simple cube or plane)

    [Header("Player")]
    public GameObject playerPrefab; // Drag your Player prefab here    
    public float playerSpawnHeight = 1f; // Prevent spawning inside floor

    [Header("Exit")]    
    public GameObject exitPrefab; // Prefab for the exit/goal object    
    public float exitSpawnHeight = 0.5f; // Height to place the exit above the ground

    [Header("Key")]
    public GameObject keyPrefab; // Prefab for the key object that unlocks the exit
    public float keySpawnHeight = 0.5f; // Height to place the key above the ground

    [Header("Maze Colors")]
    public Color topLeftColor = new Color(0.6f, 0.8f, 1f); // Light blue
    public Color topRightColor = new Color(1f, 0.7f, 0.7f); // Light pink
    public Color bottomLeftColor = new Color(0.7f, 1f, 0.7f); // Light green
    public Color bottomRightColor = new Color(1f, 0.9f, 0.6f); // Light orange

    private MazeExit mazeExit; // Cached reference to the MazeExit component for unlocking logic

    private int playerSpawnSide; // Tracks which side the player was spawned on (0..3)
    
    private Cell[,] grid; // Internal grid of cells
  
    private Stack<Vector2Int> stack = new Stack<Vector2Int>(); // Stack used for iterative DFS
    
    private System.Random rng = new System.Random(); // Random number generator for neighbor selection

    private Vector2Int exitCellPosition; // Stores the grid coordinates of the exit for key placement logic
    private Vector2Int playerSpawnCell; // Stores the grid coordinates of the player's spawn cell for key placement logic

    void Start()
    {
        GenerateGrid();
        GenerateMaze(new Vector2Int(0, 0));
        SpawnPlayerAtEdge();
        ObjectiveUI.Instance.SetObjective("Find the key hidden in the maze");
    }

    // Create grid and instantiate walls for each cell
    void GenerateGrid()
    {
        grid = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                Cell cell = new Cell();

                Color wallColor = GetCellColor(x, y);

                // Instantiate top wall at +Z edge of cell
                cell.topWall = Instantiate(wallPrefab,
                    position + new Vector3(0, 0, cellSize / 2),
                    Quaternion.identity,
                    transform);
                cell.topWall.transform.localScale = new Vector3(cellSize, 2f, 0.3f);

                // Instantiate bottom wall at -Z edge of cell
                cell.bottomWall = Instantiate(wallPrefab,
                    position + new Vector3(0, 0, -cellSize / 2),
                    Quaternion.identity,
                    transform);
                cell.bottomWall.transform.localScale = new Vector3(cellSize, 2f, 0.3f);

                // Instantiate left wall at -X edge of cell (rotated 90 degrees)
                cell.leftWall = Instantiate(wallPrefab,
                    position + new Vector3(-cellSize / 2, 0, 0),
                    Quaternion.Euler(0, 90, 0),
                    transform);
                cell.leftWall.transform.localScale = new Vector3(cellSize, 2f, 0.3f);

                // Instantiate right wall at +X edge of cell (rotated 90 degrees)
                cell.rightWall = Instantiate(wallPrefab,
                    position + new Vector3(cellSize / 2, 0, 0),
                    Quaternion.Euler(0, 90, 0),
                    transform);
                cell.rightWall.transform.localScale = new Vector3(cellSize, 2f, 0.3f);

                cell.topWall.GetComponent<Renderer>().material.color = wallColor;
                cell.bottomWall.GetComponent<Renderer>().material.color = wallColor;
                cell.leftWall.GetComponent<Renderer>().material.color = wallColor;
                cell.rightWall.GetComponent<Renderer>().material.color = wallColor;

                grid[x, y] = cell;
            }
        }
    }

    // Generate the maze using iterative DFS starting from startPos
    void GenerateMaze(Vector2Int startPos)
    {
        grid[startPos.x, startPos.y].visited = true;
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                // There are unvisited neighbors: push current back and process a random neighbor
                stack.Push(current);

                Vector2Int next = neighbors[rng.Next(neighbors.Count)];

                // Remove the shared wall between current and next
                RemoveWall(current, next);

                // Mark neighbor visited and continue
                grid[next.x, next.y].visited = true;
                stack.Push(next);
            }
            // If no neighbors, loop continues and backtracking happens by popping previous cells
        }
    }

    // Return a list of orthogonal neighbors that haven't been visited yet
    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (pos.x > 0 && !grid[pos.x - 1, pos.y].visited)
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y));

        if (pos.x < width - 1 && !grid[pos.x + 1, pos.y].visited)
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y));

        if (pos.y > 0 && !grid[pos.x, pos.y - 1].visited)
            neighbors.Add(new Vector2Int(pos.x, pos.y - 1));

        if (pos.y < height - 1 && !grid[pos.x, pos.y + 1].visited)
            neighbors.Add(new Vector2Int(pos.x, pos.y + 1));

        return neighbors;
    }

    // Destroy the wall GameObjects between two adjacent cells a and b
    void RemoveWall(Vector2Int a, Vector2Int b)
    {
        Vector2Int diff = b - a;

        // Neighbor is to the right
        if (diff.x == 1)
        {
            Destroy(grid[a.x, a.y].rightWall);
            Destroy(grid[b.x, b.y].leftWall);
        }
        // Neighbor is to the left
        else if (diff.x == -1)
        {
            Destroy(grid[a.x, a.y].leftWall);
            Destroy(grid[b.x, b.y].rightWall);
        }
        // Neighbor is above (higher Y)
        else if (diff.y == 1)
        {
            Destroy(grid[a.x, a.y].topWall);
            Destroy(grid[b.x, b.y].bottomWall);
        }
        // Neighbor is below (lower Y)
        else if (diff.y == -1)
        {
            Destroy(grid[a.x, a.y].bottomWall);
            Destroy(grid[b.x, b.y].topWall);
        }
    }

    // Choose a random cell along one edge to spawn the player
    void SpawnPlayerAtEdge()
    {
        // Randomly pick one of the four sides (0 = left, 1 = right, 2 = bottom, 3 = top)
        playerSpawnSide = rng.Next(4);
        Vector2Int spawnCell = Vector2Int.zero;

        switch (playerSpawnSide)
        {
            case 0: spawnCell = new Vector2Int(0, rng.Next(height)); break;
            case 1: spawnCell = new Vector2Int(width - 1, rng.Next(height)); break;
            case 2: spawnCell = new Vector2Int(rng.Next(width), 0); break;
            case 3: spawnCell = new Vector2Int(rng.Next(width), height - 1); break;
        }

        playerSpawnCell = spawnCell;

        // Compute world-space position at the center of the chosen cell
        Vector3 worldPosition = new Vector3(
            spawnCell.x * cellSize,
            playerSpawnHeight,
            spawnCell.y * cellSize
        );

        // Instantiate the player prefab at the calculated position
        GameObject spawnedPlayer = Instantiate(playerPrefab, worldPosition, Quaternion.identity);

        // If there's a UI that needs the player reference, set it
        StaminaUI ui = FindObjectOfType<StaminaUI>();
        if (ui != null)
            ui.SetPlayer(spawnedPlayer.GetComponent<PlayerController>());

        // Spawn the exit opposite the player's side
        SpawnExit();
        SpawnKey();
    }

    // Place the exit on the opposite edge from the player's spawn side
    void SpawnExit()
    {
        // Determine which side of the maze the exit should appear on
        int exitSide = (playerSpawnSide + 2) % 4;

        Vector2Int exitCell = Vector2Int.zero;

        // Pick a random cell along the chosen edge
        switch (exitSide)
        {
            case 0: exitCell = new Vector2Int(0, rng.Next(height)); break;              // Left edge
            case 1: exitCell = new Vector2Int(width - 1, rng.Next(height)); break;      // Right edge
            case 2: exitCell = new Vector2Int(rng.Next(width), 0); break;               // Bottom edge
            case 3: exitCell = new Vector2Int(rng.Next(width), height - 1); break;      // Top edge
        }

        // Store the chosen exit cell
        exitCellPosition = exitCell;

        // Convert grid coordinates to world-space position
        Vector3 worldPosition = new Vector3(
            exitCell.x * cellSize,
            exitSpawnHeight,
            exitCell.y * cellSize
        );

        // Spawn the exit object in the world
        GameObject exitObj = Instantiate(exitPrefab, worldPosition, Quaternion.identity);

        // Cache the MazeExit component for later use
        mazeExit = exitObj.GetComponent<MazeExit>();
    }

    void SpawnKey()
    {
        // Define all four corners
        List<Vector2Int> corners = new List<Vector2Int>()
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, height - 1),
        new Vector2Int(width - 1, 0),
        new Vector2Int(width - 1, height - 1)
    };

        // Remove the corner closest to the player spawn
        corners.RemoveAll(c => c == GetClosestCorner(playerSpawnCell));

        // Remove the corner closest to the exit (avoid easy key near exit)
        corners.RemoveAll(c => c == GetClosestCorner(exitCellPosition));

        // Randomly pick from remaining valid corners
        Vector2Int keyCell = corners[rng.Next(corners.Count)];

        Vector3 worldPosition = new Vector3(
            keyCell.x * cellSize,
            keySpawnHeight,
            keyCell.y * cellSize
        );

        GameObject keyObj = Instantiate(keyPrefab, worldPosition, Quaternion.identity);

        MazeKey keyScript = keyObj.GetComponent<MazeKey>();
        keyScript.Initialize(mazeExit);
    }

    Vector2Int GetClosestCorner(Vector2Int position)
    {
        List<Vector2Int> corners = new List<Vector2Int>()
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, height - 1),
        new Vector2Int(width - 1, 0),
        new Vector2Int(width - 1, height - 1)
    };

        Vector2Int closest = corners[0];
        float minDistance = Vector2Int.Distance(position, closest);

        foreach (var corner in corners)
        {
            float distance = Vector2Int.Distance(position, corner);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = corner;
            }
        }

        return closest;
    }

    Color GetCellColor(int x, int y)
    {
        float xPercent = (float)x / (width - 1);
        float yPercent = (float)y / (height - 1);

        Color topBlend = Color.Lerp(topLeftColor, topRightColor, xPercent);
        Color bottomBlend = Color.Lerp(bottomLeftColor, bottomRightColor, xPercent);

        return Color.Lerp(bottomBlend, topBlend, yPercent);
    }
}
