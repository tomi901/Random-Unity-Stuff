using UnityEngine;


[RequireComponent(typeof(Renderer), typeof(Portal))]
public class PortalRenderer : MonoBehaviour {

    Portal portal;

    Camera renderCamera;

    RenderTexture renderTexture;
    bool doRender = false;

    new Renderer renderer;
    MaterialPropertyBlock propertyBlock;
    int mainTexID;

    private void Awake()
    {
        portal = GetComponent<Portal>();
        renderer = GetComponent<Renderer>();

        propertyBlock = new MaterialPropertyBlock();
        mainTexID = Shader.PropertyToID("_MainTex");
    }

    private void Update()
    {
        DoRender(renderer.isVisible);
    }

    void DoRender (bool isVisible)
    {
        renderCamera?.gameObject.SetActive(isVisible);

        doRender = isVisible;
    }


    private void LateUpdate()
    {
        if (!doRender || portal?.otherPortal == null) return;

        if (!renderCamera) CreateRenderCamera();

        //Move the render camera
        portal.SetPortalRelativeTransform(renderCamera.transform, Camera.main.transform);

        //Adjust the render camera clipping plane
        renderCamera.nearClipPlane = Mathf.Max(0.2f,
            Vector3.Distance(renderCamera.transform.position, portal.otherPortal.Position) - 2);

        // Apply render texture
        renderer.SetPropertyBlock(propertyBlock);
    }

    void CreateRenderCamera()
    {
        if (renderCamera) return;

        renderCamera = new GameObject("Portal Camera").AddComponent<Camera>();

        renderTexture = new RenderTexture((int)(Screen.width * 0.5f), (int)(Screen.height * 0.5f), 16);
        renderCamera.targetTexture = renderTexture;

        renderCamera.transform.parent = transform;

        propertyBlock.SetTexture(mainTexID, renderTexture);
    }

}
