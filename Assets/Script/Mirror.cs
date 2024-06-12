using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] private Camera mirrorCamera;
    [SerializeField] private GameObject mirrorScreen;
    [SerializeField] private float size;
    [SerializeField] private Player player;
    [SerializeField] private int resolution;
    private RenderTexture renderTexture;

    private void Start()
    {
        renderTexture = new RenderTexture(resolution, resolution, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.B10G11R11_UFloatPack32);
        renderTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D16_UNorm;
        mirrorCamera.targetTexture = renderTexture;
        Renderer renderer = mirrorScreen.GetComponent<Renderer>();
        Material mat = new Material(renderer.material);
        renderer.material = mat;
        renderer.material.SetTexture("_MainTex", renderTexture);
    }

    void Update()
    {
        mirrorScreen.transform.localScale = new Vector3(-size * 2, size * 2, size * 2);
        Vector3 playerVector = transform.position - Camera.main.transform.position;
        Vector3 normalVector = transform.forward;
        Vector3 diffVector = 2 *  normalVector * Vector3.Dot(normalVector, playerVector) - playerVector;
        mirrorCamera.transform.position = transform.position + diffVector;
        mirrorCamera.transform.LookAt(transform.position);
        mirrorScreen.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        mirrorCamera.fieldOfView = 2 * Mathf.Atan(size / playerVector.magnitude) * Mathf.Rad2Deg;
        if (Input.GetMouseButtonDown(0)) 
        { 
            GenerateTexture();
        }
    }

    private void GenerateTexture()
    {
        renderTexture.Release();
        renderTexture.height = resolution;
        renderTexture.width = resolution;
        renderTexture.Create();
    }
}
