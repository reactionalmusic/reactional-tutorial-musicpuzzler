using System.Collections;
using System.Collections.Generic;
using Reactional.Playback;
using UnityEngine;

public class Board : MonoBehaviour
{
    private int width = 10;
    private int height = 20;
    public Transform[,] grid;

    void Start()
    {
        grid = new Transform[width, height];
    }

    public bool IsValidPosition(Transform pieceTransform)
    {
        foreach (Transform child in pieceTransform)
        {
            Vector2 pos = Vector2Int.RoundToInt(child.position);

            if (!IsWithinBoard((int)pos.x, (int)pos.y))
                return false;

            if (IsCellOccupied((int)pos.x, (int)pos.y))
                return false;
        }

        return true;
    }

    private bool IsWithinBoard(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private bool IsCellOccupied(int x, int y)
    {
        return grid[x, y] != null;
    }

    public void StorePiece(Transform pieceTransform)
    {
        foreach (Transform child in pieceTransform)
        {
            Vector2 pos = Vector2Int.RoundToInt(child.position);
            if(child.position.y >= height)
            {                
                GameManager.Instance.GameOver();
                return;
            }
            grid[(int)pos.x, (int)pos.y] = child;
        }

        CheckForLines();
    }

void CheckForLines()
{
    bool clearedLine = false;
    for (int y = height - 1; y >= 0; y--)
    {
        if (IsLineFull(y))
        {
            clearedLine = true;
            ClearLine(y);
            MoveLinesDown(y + 1);
            y++; // Recheck the current line after moving the lines down
        }
    }
    if (clearedLine)
    {
        StartCoroutine(GameManager.Instance.ScreenShake(0.5f));

        // PLAY LINE CLEAR STINGER
        Theme.SetControl(GameManager.Instance.lineClearStinger, 0.25f);
    }
}

    public bool CheckForGameOver()
    {
        // Check every position in the spawn point to see if it is already occupied
        foreach (Transform child in GameManager.Instance.currentPiece.transform)
        {
            Vector2 pos = Vector2Int.RoundToInt(child.position);
            if (IsCellOccupied((int)pos.x, (int)pos.y))
                return true;
        }
        return false;
    }

    bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
                return false;
        }
        return true;
    }

    void ClearLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            ExplodeClearedObject(grid[x, y].gameObject);
            grid[x, y] = null;
            GameManager.Instance.UpdateScore(10);
        }
    }

void MoveLinesDown(int fromRow)
{
    for (int y = fromRow; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                // Move this block one position down
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Update the position of the block
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
}
    public void ExplodeClearedObject(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material.color = Color.white;
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -5);
        if(obj.GetComponent<Rigidbody>() == null)
            obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().AddForce(direction * 2, ForceMode.Impulse);
        Destroy(obj, 1f);
    }

}