using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCameraTrack : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cineCam;
    /*[SerializeField]
    private float camSize;
    [SerializeField]
    private float yBoundary;
    [SerializeField]
    private float xBoundary;
    [SerializeField]
    private LineRenderer line;*/
    // Start is called before the first frame update
    void Start()
    {
        cineCam.Follow = GameManager.Instance.CameraTrackPoint;
        //camSize = Camera.main.orthographicSize;
    }

    
    
    
    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        //if ()
    }


    /*/// <summary>
    /// my most stupidest code yet,
    /// literally brute forcing,
    /// waiting for pixelperfectcamera to modify the orthosize
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetCamSize()
    {
        bool HasSet = false;
        for (int i = 0; i < 100; i++)
        {
            if (HasSet)
                break;
            if (Camera.main.orthographicSize != camSize)
            {
                camSize = Camera.main.orthographicSize;
                HasSet = true;
            }
            yield return null;
        }
    }*/
}
