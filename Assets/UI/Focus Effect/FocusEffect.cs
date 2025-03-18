using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FocusEffect : MonoBehaviour
{
    [SerializeField] [Range(0, 2)] float focusTime = 1;
    [SerializeField] AnimationCurve fadeCurve;
    [SerializeField] [Range(0, 50)] float Amount;
    [SerializeField] AnimationCurve zoomCurve;

    public static FocusEffect Instance { get; private set; }
    private Camera mainCamera;

    private Image[] focusCorners;

    private Coroutine effectCoroutine;

    private void Start()
    {
        Instance = this;

        focusCorners = GetComponentsInChildren<Image>();

        focusCorners[0].rectTransform.eulerAngles = new Vector3(0, 0, -45);
        focusCorners[1].rectTransform.eulerAngles = new Vector3(0, 0, -135);
        focusCorners[2].rectTransform.eulerAngles = new Vector3(0, 0, 135);
        focusCorners[3].rectTransform.eulerAngles = new Vector3(0, 0, 45);

        for (int i = 0; i < focusCorners.Length; i++)
        {
            Color color = focusCorners[i].color;
            color.a = 0;
            focusCorners[i].color = color;
        }

        mainCamera = Camera.main;
    }

    public void Focus(Transform bloxer)
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }

        effectCoroutine = StartCoroutine(AnimateFocus(bloxer));
    }

    public Vector2[] GetScreenCorners(Transform bloxer)
    {
        if (mainCamera == null || bloxer == null)
        {
            Debug.LogError("Camera or Cube not assigned!");
            return null;
        }

        // Get the Renderer to determine the world-space bounds
        Renderer cubeRenderer = bloxer.GetComponent<Renderer>();
        if (cubeRenderer == null)
        {
            Debug.LogError("No Renderer found on the target cube!");
            return null;
        }

        // Get the world-space bounding box corners
        Bounds bounds = cubeRenderer.bounds;
        Vector3[] worldCorners = new Vector3[8];

        // Calculate the 8 corners of the cube's bounding box
        worldCorners[0] = bounds.min; // Bottom-Left-Back
        worldCorners[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z); // Bottom-Left-Front
        worldCorners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z); // Top-Left-Back
        worldCorners[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z); // Top-Left-Front
        worldCorners[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z); // Bottom-Right-Back
        worldCorners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z); // Bottom-Right-Front
        worldCorners[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z); // Top-Right-Back
        worldCorners[7] = bounds.max; // Top-Right-Front

        // Convert all world corners to screen space
        Vector2[] screenCorners = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            screenCorners[i] = mainCamera.WorldToScreenPoint(worldCorners[i]);
        }

        // Find the min and max screen coordinates to form a tight-fitting rectangle
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var screenCorner in screenCorners)
        {
            minX = Mathf.Min(minX, screenCorner.x);
            maxX = Mathf.Max(maxX, screenCorner.x);
            minY = Mathf.Min(minY, screenCorner.y);
            maxY = Mathf.Max(maxY, screenCorner.y);
        }

        // Return the 4 screen space corners of the bounding box
        return new Vector2[]
        {
            new Vector2(minX, minY), // Bottom-Left
            new Vector2(minX, maxY), // Top-Left
            new Vector2(maxX, maxY), // Top-Right
            new Vector2(maxX, minY)  // Bottom-Right
        };
    }

    private IEnumerator AnimateFocus(Transform bloxer)
    {
        Vector2[] zoomOutAdditive =
        {
            new Vector2(-Amount, -Amount),
            new Vector2(-Amount, Amount),
            new Vector2(Amount, Amount),
            new Vector2(Amount, -Amount)
        };

        float time = 0;
        while (time < focusTime)
        {
            time += Time.deltaTime;

            Vector2[] screenPositions = GetScreenCorners(bloxer);
            
            for (int i = 0; i < screenPositions.Length; i++)
            {
                Vector2 screenCornerPosition = screenPositions[i];
                screenCornerPosition += zoomOutAdditive[i] * zoomCurve.Evaluate(time / focusTime);

                focusCorners[i].rectTransform.position = screenCornerPosition;

                Color color = focusCorners[i].color;
                color.a = fadeCurve.Evaluate(time / focusTime);
                focusCorners[i].color = color;
            }

            yield return null;
        }
    } 
}
