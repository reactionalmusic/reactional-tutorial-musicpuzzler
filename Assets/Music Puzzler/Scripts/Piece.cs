using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reactional.Playback;
public class Piece : MonoBehaviour
{
    private Board board;
    public Vector3 rotationPoint;
    float previousBeat = 0;
    float heldBeat = 0;

    void Start()
    {
        board = GameManager.Instance.board;
        gameObject.transform.parent = board.transform;
    }

    void Update()
    {
        // Move the piece down by one unit every beat
        if (MusicSystem.GetCurrentBeat() > previousBeat)
        {
            transform.position += new Vector3(0, -1, 0);
            if (!board.IsValidPosition(transform))
            {
                transform.position -= new Vector3(0, -1, 0);

                StartCoroutine(WaitForBeatToSettle());
                this.enabled = false; // Disable the script to stop further updates after the piece is placed.
            }
            previousBeat = MusicSystem.GetNextBeat(1);
        }

        // Move the piece left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                heldBeat = 0;

            if (MusicSystem.GetCurrentBeat() > heldBeat)
            {
                transform.position += new Vector3(-1, 0, 0);
                if (!board.IsValidPosition(transform))
                {
                    transform.position -= new Vector3(-1, 0, 0);
                }
                float waitTime = MusicSystem.GetNextBeat(0.5f) - MusicSystem.GetCurrentBeat() > 0.25f ? 0f : 0.5f;
                heldBeat = MusicSystem.GetNextBeat(0.5f) + waitTime;

                // PLAY MOVE STINGER
                Theme.SetControl(GameManager.Instance.moveStinger, 0.125f);
            }
        }

        // Move the piece right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                heldBeat = 0;

            if (MusicSystem.GetCurrentBeat() > heldBeat)
            {
                transform.position += new Vector3(1, 0, 0);
                if (!board.IsValidPosition(transform))
                {
                    transform.position -= new Vector3(1, 0, 0);
                }
                float waitTime = MusicSystem.GetNextBeat(0.5f) - MusicSystem.GetCurrentBeat() > 0.25f ? 0f : 0.5f;
                heldBeat = MusicSystem.GetNextBeat(0.5f) + waitTime;

                // PLAY MOVE STINGER
                Theme.SetControl(GameManager.Instance.moveStinger, 0.125f);
            }
        }

        // Rotate the piece right
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            if (!board.IsValidPosition(transform))
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }

            // PLAY MOVE STINGER
            Theme.SetControl(GameManager.Instance.moveStinger, 0.125f);
        }

        // Rotate the piece left
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            if (!board.IsValidPosition(transform))
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            }

            // PLAY MOVE STINGER
            Theme.SetControl(GameManager.Instance.moveStinger, 0.125f);
        }

        // Drop the piece instantly
        if (Input.GetKeyDown(KeyCode.Space))
        {
            while (board.IsValidPosition(transform))
            {
                transform.position += new Vector3(0, -1, 0);
            }
            transform.position -= new Vector3(0, -1, 0);

            StartCoroutine(WaitForBeatToSettle());
            this.enabled = false;
            return;
        }

    }

    IEnumerator WaitForBeatToSettle()
    {
        Theme.SetControl(GameManager.Instance.dropStinger, 0.25f);
        yield return new MusicSystem.WaitForNextBeat(1f, -0.125f);
        StartCoroutine(GameManager.Instance.DropCamBounce());
        SettledColor();
        GameManager.Instance.PieceSettled();
        board.StorePiece(transform);
    }

    void SettledColor()
    {
        foreach (Transform child in transform)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            renderer.material.color = new Color(0.05f, 0.05f, 0.05f);
        }
    }

}
