using System.Collections;
using System.Collections.Generic;
using Reactional.Playback;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] pieces;  // Array to hold the different prefabs
    public List<GameObject> spawnedPieces = new List<GameObject>();
    public Transform pieceSpawnPoint;  // The point in the scene where new pieces are spawned
    public int score = 0;  // The player's score
    public TMPro.TextMeshProUGUI scoreText;  // Reference to the UI text element that displays the score
    public GameObject currentPiece;  // Currently active piece
    public Board board;
    private Camera mainCamera;
    private Vector3 originalCamPos;
    bool gameOver = false;

    [Header("Audio")]
    public AudioSource drop;

    [Header("Stinger Assignment")]
    public string dropStinger;
    public string lineClearStinger;
    public string moveStinger;

    public static GameManager Instance;  // Singleton instance for easy access

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        mainCamera = Camera.main;
        originalCamPos = mainCamera.transform.position;
        board = FindObjectOfType<Board>();
        SpawnPiece();

        // Start a recurring coroutine to destroy cleared pieces
        StartCoroutine(DestroyClearedPiecesRoutine());
    }

    void Update()
    {
        if (gameOver)
        {
            StartCoroutine(RestartGameRoutine());
        }
    }

    IEnumerator DestroyClearedPiecesRoutine()
    {
        while (true)
        {
            DestroyClearedPieces();
            yield return new WaitForSeconds(3f);
        }
    }
    void DestroyClearedPieces()
    {
        List<GameObject> toRemove = new List<GameObject>();
    
        // Collect objects to remove
        foreach (GameObject child in spawnedPieces)
        {
            if (child != null && child.transform.childCount == 0)
            {
                toRemove.Add(child);
            }
        }
    
        // Remove and destroy collected objects
        foreach (GameObject child in toRemove)
        {
            if (child != null)
            {
                spawnedPieces.Remove(child);
                Destroy(child);
            }
        }
    }

    public void GameOver()
    {
        Theme.SetControl(dropStinger, 0.5f);

        gameOver = true;
        Destroy(currentPiece);
        StartCoroutine(ScreenShake(2f));
        
        for (int i = 0; i < board.grid.GetLength(0); i++)
        {
            for (int j = 0; j < board.grid.GetLength(1); j++)
            {
                if (board.grid[i, j] != null)
                {
                    board.ExplodeClearedObject(board.grid[i, j].gameObject);
                }
            }
        }
    }

    void SpawnPiece()
    {
        if (gameOver)
        {
            return;
        }
        int randomPiece = Random.Range(0, pieces.Length);
        currentPiece = Instantiate(pieces[randomPiece], pieceSpawnPoint.position, Quaternion.identity);
        spawnedPieces.Add(currentPiece);
    }

    public void PieceSettled()
    {
        Reactional.Playback.MusicSystem.ScheduleAudio(drop, 0.5f);
        currentPiece = null;  // Clear the current piece reference
        SpawnPiece();  // Spawn a new piece
    }

    private IEnumerator RestartGameRoutine()
    {
        gameOver = false;
        yield return new WaitForSeconds(2f);
        RestartGame();
    }
    public void RestartGame()
    {
        score = 0;
        spawnedPieces.ForEach(Destroy);
        spawnedPieces.Clear();
        gameOver = false;
        SpawnPiece();
        Playlist.Random();
    }

    public IEnumerator ScreenShake(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= 0.2f * damper;
            y *= 0.2f * damper;
            Camera.main.transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);
            yield return null;
        }
        Camera.main.transform.position = originalCamPos;
    }

    public IEnumerator DropCamBounce()
    {
        float elapsed = 0.0f;
        while (elapsed < 0.25f)
        {
            elapsed += Time.deltaTime;
            float percentComplete = elapsed / 0.25f;
            float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0f, 1.0f);
            Camera.main.transform.position = new Vector3(originalCamPos.x, originalCamPos.y - 0.25f * -damper, originalCamPos.z);
            yield return null;
        }
        Camera.main.transform.position = originalCamPos;
    }

    public void UpdateScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();
    }
}
