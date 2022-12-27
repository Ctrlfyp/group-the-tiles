using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTileComponent : MonoBehaviour
{
    public GameTile gameTile;

    public Animator animator;

    public TileAnimationController horizontalLineClearAnimator;
    public TileAnimationController verticalLineClearAnimator;
    public TileAnimationController bombClearAnimator;

    public SpriteRenderer spriteRenderer;
    public Image lineHSpriteRenderer;
    public Image lineVSpriteRenderer;
    public Image bombSpriteRenderer;
    public Image rainbowSpriteRenderer;

    public Text text;

    public Vector2 targetLocation;

    public Boolean isDespawned = false;
    public Boolean isRainbow = false;
    public Color targetColor;

    // set scale
    public float targetScale = 0.2f;

    public Boolean selfDestruct = false;
    public Boolean isMoved = false;

    public event EventHandler TileOver;
    public event EventHandler TileExit;
    public event EventHandler TileDown;
    public event EventHandler TileUp;

    public GameTile.TILE_TYPE tileType;
    public SpriteRenderer topDivider;
    public SpriteRenderer rightDivider;
    public SpriteRenderer bottomDivider;
    public SpriteRenderer leftDivider;

    private void Awake()
    {
        text.text = "";
        tileType = GameTile.TILE_TYPE.NORMAL;
        text.transform.position = new Vector3(0, 0);
    }

    void OnMouseOver()
    {
        TileOver?.Invoke(this, EventArgs.Empty);
    }

    void OnMouseExit()
    {
        TileExit?.Invoke(this, EventArgs.Empty);
    }

    void OnMouseDown()
    {
        TileDown?.Invoke(this, EventArgs.Empty);
    }

    void OnMouseUp()
    {
        TileUp?.Invoke(this, EventArgs.Empty);
    }

    // call to turn self invisible
    public void PlayDespawnAnimation()
    {
        switch (tileType)
        {
            case GameTile.TILE_TYPE.BOMB:
                bombClearAnimator.Play("BombClearSpawn");
                break;
            case GameTile.TILE_TYPE.LINEH:
                horizontalLineClearAnimator.Play("HorizontalClearSpawn");
                break;
            case GameTile.TILE_TYPE.LINEV:
                verticalLineClearAnimator.Play("VerticalClearSpawn");
                break;
            default:
                break;
        }

        animator.Play("TileDespawn");
        SetTileType(GameTile.TILE_TYPE.NORMAL);
        isDespawned = true;
    }

    // call to set spawn
    public void PlaySpawnAnimation()
    {
        animator.Play("TileSpawn");
        isDespawned = false;
    }

    // init fly to top left
    public void PlaySelfDestructAnimation(GameTile.TILE_TYPE tileType)
    {
        SetTileType(tileType);
        animator.Play("TileIdle");
        selfDestruct = true;

        // move to top of the screen
        Vector3 top = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        targetLocation = new Vector2(top.x, top.y);

    }

    public void SetTileColor(int colorIndex)
    {
        float currentAlpha = spriteRenderer.color.a;
        gameTile.colourIndex = colorIndex;
        spriteRenderer.color = new Color(gameTile.colour.r, gameTile.colour.g, gameTile.colour.b, currentAlpha);
    }

    public void SetTileType(GameTile.TILE_TYPE tileType)
    {
        this.tileType = tileType;
        bombSpriteRenderer.gameObject.SetActive(false);
        lineHSpriteRenderer.gameObject.SetActive(false);
        lineVSpriteRenderer.gameObject.SetActive(false);
        rainbowSpriteRenderer.gameObject.SetActive(false);
        switch (tileType)
        {
            case GameTile.TILE_TYPE.BOMB:
                bombSpriteRenderer.gameObject.SetActive(true);
                break;
            case GameTile.TILE_TYPE.LINEH:
                lineHSpriteRenderer.gameObject.SetActive(true);
                break;
            case GameTile.TILE_TYPE.LINEV:
                lineVSpriteRenderer.gameObject.SetActive(true);
                break;
            case GameTile.TILE_TYPE.RAINBOW:
                rainbowSpriteRenderer.gameObject.SetActive(true);
                spriteRenderer.color = new Color(0.38f, 0.38f, 0.38f, 1);
                break;
        }
    }

    // kill self
    public void Kill()
    {
        Destroy(gameObject);
    }

    public void Update()
    {

        if (selfDestruct)
        {

            // move self to the global position
            if (Mathf.Abs(targetLocation.x - transform.position.x) > 1f
                || Mathf.Abs(targetLocation.y - transform.position.y) > 1f)
            {
                transform.position = new Vector3(
                    Mathf.Lerp(transform.position.x, targetLocation.x, Time.deltaTime * 3f),
                    Mathf.Lerp(transform.position.y, targetLocation.y, Time.deltaTime * 3f),
                    transform.position.z
                );
            }
            else
            {
                isMoved = true;
            }

            // scale targetScale with both x and y
            if (!isMoved)
            {
                transform.localScale = new Vector3(
                    Mathf.Lerp(transform.localScale.x, targetScale, Time.deltaTime * 3f),
                    Mathf.Lerp(transform.localScale.y, targetScale, Time.deltaTime * 3f),
                    transform.localScale.z
                );
            }

            if (isMoved)
            {
                Kill();
            }
        }
    }
}
