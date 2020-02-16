using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerController : MonoBehaviour
{
    private VideoPlayer mVideoPlayer;

    private void Awake()
    {
        mVideoPlayer = GetComponent<VideoPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
