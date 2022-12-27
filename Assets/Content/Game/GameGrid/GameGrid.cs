using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public Material squareMaterial;
    public Material triangleMaterial;
    public Material hexagonMaterial;

    public float size
    {
        get { return this.gameObject.GetComponent<MeshRenderer>().bounds.size.y * meshRenderer.material.GetFloat("_GridSize"); }
        set
        {
            meshRenderer.material.SetFloat(Shader.PropertyToID("_GridSize"), value / this.gameObject.GetComponent<MeshRenderer>().bounds.size.y);
            switch (shape)
            {
                case GAME_BOARD_SHAPE.SQUARE:
                    break;
                case GAME_BOARD_SHAPE.TRIANGLE:
                    break;
                case GAME_BOARD_SHAPE.HEXAGON:
                    break;
            }
        }
    }
    public GAME_BOARD_SHAPE shape
    {
        get {
            if (meshRenderer.material.Equals(hexagonMaterial))
            {
                return GAME_BOARD_SHAPE.HEXAGON;
            }
            else if (meshRenderer.material.Equals(triangleMaterial))
            {
                return GAME_BOARD_SHAPE.TRIANGLE;
            }
            else
            {
                return GAME_BOARD_SHAPE.SQUARE;
            }
        }
        set {
            switch (shape)
            {
                case GAME_BOARD_SHAPE.SQUARE:
                    meshRenderer.material = Instantiate<Material>(squareMaterial);
                    break;
                case GAME_BOARD_SHAPE.TRIANGLE:
                    meshRenderer.material = triangleMaterial;
                    break;
                case GAME_BOARD_SHAPE.HEXAGON:
                    meshRenderer.material = hexagonMaterial;
                    break;
            }
        }
    }

    public Vector2 bounds
    {
        get { return this.gameObject.GetComponent<MeshRenderer>().bounds.size; }
    }
}
