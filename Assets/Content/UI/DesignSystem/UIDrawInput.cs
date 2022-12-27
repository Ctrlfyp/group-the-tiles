using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace UI.DesignSystem
{
    public class UIDrawInput : MonoBehaviour
    {
        public string fileName = "drawing";
        public new Camera camera;
        public GameObject brush;
        public GameObject drawArea;
        public BoxCollider2D drawAreaCollider;
        public Color penColour = Color.red;
        public SoundBoard soundBoard;

        LineRenderer lineRenderer;
        Vector2 lastPosition;

        private RectTransform drawAreaRectTransform;

        void Start()
        {
            // init the drawing collider so the user can only draw in the defined drawArea
            drawAreaRectTransform = drawArea.GetComponent<RectTransform>();
            drawAreaCollider.size = drawAreaRectTransform.sizeDelta;
            drawAreaCollider.offset = drawAreaRectTransform.anchoredPosition;
            // drawAreaCollider.size = drawArea.sizeDelta;
            // drawAreaCollider.offset = drawArea.anchoredPosition;

            // load previously drawn lines
            ImportDrawing();
        }

        void Draw()
        {
            // test if cursor is inside the draw area
            Vector2 mousePosition = Input.mousePosition;
            Vector2 mousePositionInWorld = camera.ScreenToWorldPoint(mousePosition);

            if (drawAreaCollider.OverlapPoint(mousePositionInWorld))
            {
                // if cursor is inside the draw area, draw
                if (Input.GetMouseButtonDown(0))
                {
                    // create new line
                    CreateBrush();
                    soundBoard.PlaySound(soundBoard.Draw, true);
                }
                if (Input.GetMouseButton(0))
                {
                    // add point to line
                    Vector2 currentPosition = camera.ScreenToWorldPoint(Input.mousePosition);
                    if (currentPosition != lastPosition)
                    {
                        AddPoint(currentPosition);
                        lastPosition = currentPosition;
                    }
                }
                else
                {
                    // guy's not drawing anymore
                    soundBoard.StopSound();
                    lineRenderer = null;
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    soundBoard.StopSound();
                }
            }
        }

        void CreateBrush()
        {
            // cheeck bounding box
            GameObject brushInstance = Instantiate(brush, Vector2.zero, Quaternion.identity);
            brushInstance.transform.SetParent(drawAreaRectTransform.transform);
            lineRenderer = brushInstance.GetComponent<LineRenderer>();

            Vector2 currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, currentMousePosition);
            lineRenderer.SetPosition(1, currentMousePosition);

            lastPosition = currentMousePosition;
        }

        void AddPoint(Vector2 currentMousePosition)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentMousePosition);
        }

        void AddAllPoints(List<Vector2> points)
        {
            // add a new linerenderer into the drawArea
            // then connect all the dots
            GameObject brushInstance = Instantiate(brush, Vector2.zero, Quaternion.identity);
            brushInstance.transform.SetParent(drawAreaRectTransform.transform);

            lineRenderer = brushInstance.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, points[0]);
            lineRenderer.SetPosition(1, points[1]);
            lineRenderer.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }


        public void ImportDrawing()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
                if (File.Exists(path))
                {
                    Clear();

                    string json = File.ReadAllText(path);
                    List<List<Vector2>> lineRendererPositions = JsonConvert.DeserializeObject<List<List<Vector2>>>(json);

                    // add each line individually to the drawArea
                    foreach (List<Vector2> positions in lineRendererPositions)
                    {
                        AddAllPoints(positions);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        public void ExportDrawing()
        {
            try
            {
                CaptureScreenshot();
                // saving the guy's drawing as a point array,
                // each outer list corresponds to a single linerenderer
                // inner list is all the points of the linerenderer
                List<List<Vector2>> lineRendererPositions = new List<List<Vector2>>();
                LineRenderer[] lineRenderers = drawAreaRectTransform.GetComponentsInChildren<LineRenderer>();

                foreach (LineRenderer lineRenderer in lineRenderers)
                {
                    List<Vector2> positions = new List<Vector2>();
                    for (int i = 0; i < lineRenderer.positionCount; i++)
                    {
                        positions.Add(lineRenderer.GetPosition(i));
                    }
                    lineRendererPositions.Add(positions);
                }

                // save to file
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new JsonUtility.Vec2Conv());
                settings.Formatting = Formatting.Indented;

                string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
                // write all the linerenderer positions to file
                File.WriteAllText(path, JsonConvert.SerializeObject(lineRendererPositions, settings));
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        public void CaptureScreenshot()
        {
            StartCoroutine(TakeScreenshotAndSave());
        }

        public IEnumerator TakeScreenshotAndSave()
        {
            yield return new WaitForEndOfFrame();

            Vector3[] corners = new Vector3[4];
            drawAreaRectTransform.GetWorldCorners(corners);
            int startX = (int)corners[0].x;
            int startY = (int)corners[0].y;

            int width = (int)corners[3].x - (int)corners[0].x;
            int height = (int)corners[1].y - (int)corners[0].y;

            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
            tex.Apply();

            // only keep penColour
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    // compare color by rgb values
                    Color pixelColor = tex.GetPixel(x, y);
                    if (pixelColor.r != penColour.r || pixelColor.g != penColour.g || pixelColor.b != penColour.b)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }

            // Save the screenshot somewhere
            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);

            File.WriteAllBytes(Application.persistentDataPath + "/" + fileName + ".png", bytes);
        }


        public void Clear()
        {
            // kill all the lines
            foreach (Transform child in drawArea.transform)
            {
                Destroy(child.gameObject);
            }
        }

        void Update()
        {
            Draw();
        }
    }
}
