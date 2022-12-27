using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DoodleGrid : MonoBehaviour
{

    public int size = 150;
    public int totalDoodles = 100;
    public DoodleImage doodleImagePrefab;
    public GameObject bottomLeftMarker;
    public GameObject topRightMarker;
    public GameObject doodleContainer;
    private HashSet<Vector3> doodlePositions;

    private System.Random rand;

    private int[,] doodleGrid;
    public int maxDoodleSize = 4;


    public Boolean loadCustomDoodles = false;
    private List<Sprite> images;

    void Start()
    {

        LoadImages();

        rand = new System.Random();
        doodlePositions = new HashSet<Vector3>();

        // get the position of the screen via markers
        Vector3 bottomLeft = bottomLeftMarker.transform.position;
        Vector3 topRight = topRightMarker.transform.position;

        // hide markers
        bottomLeftMarker.SetActive(false);
        topRightMarker.SetActive(false);

        // create grid via markers
        int width = (int)((topRight.x - bottomLeft.x) / size);
        int height = (int)((topRight.y - bottomLeft.y) / size);
        doodleGrid = new int[width, height];

        for (int i = 0; i < totalDoodles; i++)
        {
            Boolean isValidCoords = false;
            int maxLoop = 5;
            int x, y, dooSize = -1;
            Vector3 position = new Vector3();

            // try to generate or until maxLoop and give up
            while (!isValidCoords && maxLoop > 0)
            {
                x = rand.Next(0, width);
                y = rand.Next(0, height);
                dooSize = rand.Next(1, maxDoodleSize);
                if (IsValidCoords(x, y, dooSize))
                {
                    isValidCoords = true;
                    int randomPaddingX = rand.Next(0, size);
                    int randomPaddingY = rand.Next(0, size);
                    position = new Vector3(bottomLeft.x + (x * size) + randomPaddingX, bottomLeft.y + (y * size) + randomPaddingY, 1);
                }
                else
                {
                    maxLoop--;
                }
            }

            if (isValidCoords)
            {
                // place doodle image
                DoodleImage doodleImage = Instantiate(doodleImagePrefab, position, Quaternion.identity, doodleContainer.transform);

                // random image
                int randomIndex = rand.Next(0, images.Count);

                // random size
                int randomSize = rand.Next((int)((size * dooSize) * 0.75), size * dooSize);

                // random angle
                int randomAngle = rand.Next(0, 360);

                // random color
                Color randomColor = new Color(rand.Next(0, 255) / 255.0f, rand.Next(0, 255) / 255.0f, rand.Next(0, 255) / 255.0f);

                doodleImage.SetImage(images[randomIndex], randomSize, randomAngle, randomColor);
            }
        }
    }

    /**
    * Check if the given coordinates are valid for the given size on the grid
    */
    private Boolean IsValidCoords(int x, int y, int size)
    {
        if (x + size >= doodleGrid.GetLength(0) || y + size >= doodleGrid.GetLength(1))
        {
            return false;
        }
        for (int i = x; i <= x + size; i++)
        {
            for (int j = y; j <= y + size; j++)
            {
                if (doodleGrid[i, j] != 0)
                {
                    return false;
                }
            }
        }

        // reaching here means the coordinates are valid ⇒ mark grid as used
        for (int i = x; i <= x + size; i++)
        {
            for (int j = y; j <= y + size; j++)
            {
                doodleGrid[i, j] = 1;
            }
        }
        return true;
    }

    private void LoadImages()
    {
        // load default images
        Sprite[] doodles = Resources.LoadAll<Sprite>("Images/Doodles/Default");

        List<Sprite> allDoodles = new List<Sprite>();
        allDoodles.AddRange(doodles);

        // length of sprites
        if (loadCustomDoodles)
        {
            Sprite[] customDoodles = Resources.LoadAll<Sprite>("Images/Doodles/Custom");
            allDoodles.AddRange(customDoodles);
        }

        images = allDoodles;
    }

    private Vector3 RandomPosition(Vector3 bottomLeft, Vector3 topRight)
    {
        // get random position between bottomLeft and topRight
        float x = bottomLeft.x + (float)rand.NextDouble() * (topRight.x - bottomLeft.x);
        float y = bottomLeft.y + (float)rand.NextDouble() * (topRight.y - bottomLeft.y);

        // snap to grid
        return new Vector3(bottomLeft.x + ((float)Math.Round(x / size) * size), bottomLeft.y + (float)(Math.Round(y / size) * size), 1);
    }

}
