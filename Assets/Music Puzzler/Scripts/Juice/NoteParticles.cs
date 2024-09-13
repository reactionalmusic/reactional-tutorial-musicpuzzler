
using UnityEngine;
using System.Collections;
using Reactional;
using Reactional.Core;
public class NoteParticles : MonoBehaviour
{
    public int particleBurst = 1;
    ParticleSystem ps;
    GhostPiece piece;

    private void Start()
    {
        ReactionalEngine.Instance.onNoteOn += RouteNoteOn;
        ReactionalEngine.Instance.onNoteOff += RouteNoteOff;
        ps = GetComponent<ParticleSystem>();
        piece = FindObjectOfType<GhostPiece>();
    }

    private void OnDisable()
    {
        ReactionalEngine.Instance.onNoteOn -= RouteNoteOn;
        ReactionalEngine.Instance.onNoteOff -= RouteNoteOff;
    }

    private void RouteNoteOn(double beat, int sink, int lane, float pitch, float velocity)
    {
        var partic = ps.main;
        partic.startSizeMultiplier = 1f * velocity;
        ps.Emit(particleBurst);
        var piece = GameManager.Instance.currentPiece;
        if (piece == null)
        {
            return;
        }
        var gp = piece.GetComponent<GhostPiece>();
        if (gp == null)
        {
            return;
        }
        ps.transform.position = gp.ghostTransform.position;
    }

    private void RouteNoteOff(double beat, int sink, int lane, float pitch, float velocity)
    {
    }
}
