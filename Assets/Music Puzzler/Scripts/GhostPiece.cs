using Unity.VisualScripting;
using UnityEngine;

public class GhostPiece : MonoBehaviour
{
    public GameObject ghostPiece;
    public Transform ghostTransform;

    private BackgroundColor backgroundColor;
    private MeshRenderer meshRenderer;

    void Start()
    {
        CreateGhost();
        backgroundColor = FindObjectOfType<BackgroundColor>();
        meshRenderer = ghostPiece.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        FollowActivePiece();
        UpdateGhostPosition();
        UpdateColor();
    }

    void UpdateColor()
    {
        foreach (Transform child in ghostTransform)
        {
            meshRenderer = child.GetComponent<MeshRenderer>();
            meshRenderer.material.color = backgroundColor.color;
        }
    }

    void CreateGhost()
    {
        // Create an empty GameObject to hold the ghost pieces
        ghostPiece = Instantiate(gameObject);
        ghostPiece.name = "Ghost " + name;
        Destroy(ghostPiece.GetComponent<Piece>());
        Destroy(ghostPiece.GetComponent<GhostPiece>());
        ghostTransform = ghostPiece.transform;

        // Set the ghost piece's color to be transparent
        foreach (Transform child in ghostTransform)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            renderer.material.color = new Color(1, 1, 1,1f);
        }
    }

    void FollowActivePiece()
    {
        // If piece is active, update ghost position and rotation, else destroy ghost
        if (GetComponent<Piece>().enabled)
        {
            ghostTransform.position = transform.position;
            ghostTransform.rotation = transform.rotation;
        }
        else
        {
            Destroy(ghostPiece);
            Destroy(this);
        }
    }

    void UpdateGhostPosition()
    {
        // Move ghost piece downwards until it finds the lowest valid position
        while (true)
        {
            ghostTransform.position += new Vector3(0, -1, 0);
            if (!IsValidPosition(ghostTransform))
            {
                ghostTransform.position -= new Vector3(0, -1, 0);
                break;
            }
        }
    }

    private bool IsValidPosition(Transform transform)
    {
        Board board = FindObjectOfType<Board>();
        return board.IsValidPosition(transform);
    }

    void OnDestroy()
    {
        // Clean up the ghost piece when the real piece is destroyed
        Destroy(ghostPiece);
    }
}
