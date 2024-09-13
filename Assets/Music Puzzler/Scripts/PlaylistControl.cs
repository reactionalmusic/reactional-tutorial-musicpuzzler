using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistControl : MonoBehaviour
{
    public void NextTrack()
    {
        // Play the next track in the playlist
        Reactional.Playback.Playlist.Next();
    }
}
