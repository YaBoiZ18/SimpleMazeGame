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

    private Cell[,] grid; // Internal grid of cells
    
    private Stack<Vector2Int> stack = new Stack<Vector2Int>(); // Stack used for iterative DFS
    
    private System.Random rng = new System.Random(); // Random number generator for neighbor selection

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        GenerateMaze(new Vector2Int(0, 0));
        SpawnPlayerAtEdge();
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

    void SpawnPlayerAtEdge()
    {
        // Pick a random side: 0 = left, 1 = right, 2 = bottom, 3 = top
        int side = rng.Next(4);

        Vector2Int spawnCell = Vector2Int.zero;

        switch (side)
        {
            case 0: // Left edge
                spawnCell = new Vector2Int(0, rng.Next(height));
                break;

            case 1: // Right edge
                spawnCell = new Vector2Int(width - 1, rng.Next(height));
                break;

            case 2: // Bottom edge
                spawnCell = new Vector2Int(rng.Next(width), 0);
                break;

            case 3: // Top edge
                spawnCell = new Vector2Int(rng.Next(width), height - 1);
                break;
        }

        // Convert cell position to world position (center of cell)
        Vector3 worldPosition = new Vector3(
            spawnCell.x * cellSize,
            playerSpawnHeight,
            spawnCell.y * cellSize
        );

        GameObject spawnedPlayer = Instantiate(playerPrefab, worldPosition, Quaternion.identity);

        // Tell the UI which player to follow
        StaminaUI ui = FindObjectOfType<StaminaUI>();
        if (ui != null)
        {
            ui.SetPlayer(spawnedPlayer.GetComponent<PlayerController>());
        }
    } 
}
