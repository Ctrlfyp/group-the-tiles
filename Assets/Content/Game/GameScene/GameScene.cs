using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    [HideInInspector]
    public GameLevel currentLevel;
    [HideInInspector]
    public int currentPoints;
    [HideInInspector]
    public int currentMoves;
    [HideInInspector]
    public float currentTimeRemaining;
    [HideInInspector]
    public float currentTimeElasped;
    [HideInInspector]
    public bool gamePaused = false;

    [HideInInspector]
    public List<GameTileGroup> groups;

    [HideInInspector]
    public int tileSpacing;
    public bool inEditingMode;
    public string inEditorLevelId;

    public new Camera camera;
    public GameGrid gameGrid;
    public GameBoardComponent gameBoardComponent;
    public GameInputSystem.GameInputSystem gameInputSystem;
    public UI.GameOverPanel gameOverPanel;
    public UI.LevelInfo levelInfoPanel;
    private UI.PersistentTopBar persistentTopBar;
    public UI.GameSceneLevelEditingPanel levelEditPanelGameScene;
    public Button openEditPanelButton;
    public Button saveButton;
    public Button clearButton;
    public Button endButton;

    public static event UnityAction<int> onMoveMade;

    public GameObject squarePrefab;
    public GameObject trianglePrefab;
    public GameObject hexagonPrefab;

    public SoundBoard soundBoard;
    private int comboCounter;

    public GameScene()
    {
        currentLevel = null;
        groups = new List<GameTileGroup>();
    }

    void Start()
    {

        if (inEditingMode)
        {
            gameInputSystem.SetupNewManager(nameof(GameInputSystem.LevelEditManager), this);
        }
        else
        {
            gameInputSystem.SetupNewManager(nameof(GameInputSystem.IdleManager), this);
            GameLevel level = GameManager.dataManager.LoadGameLevelFromId(inEditorLevelId);
            SetGameLevel(level);
        }

        if (!inEditorLevelId.Equals("") && inEditingMode)
        {
            GameLevel level = GameManager.dataManager.LoadGameLevelFromId(inEditorLevelId);
            SetGameLevel(level);
        }

        persistentTopBar = ComponentUtility.topBar;
        persistentTopBar.SetTopBarGameMode();

        // Listen for theme change
        SettingsMenu.onThemeChange += OnThemeChange;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Assets/Content/UI/MainMenuScene/MainMenuScene.unity", LoadSceneMode.Single);
        }

        if (currentLevel != null && !inEditingMode && !gamePaused)
        {
            currentTimeRemaining -= Time.deltaTime;
            currentTimeElasped += Time.deltaTime;
            if (currentTimeRemaining <= 0 && (currentLevel.gameMode == GameLevel.GAME_MODE.RACE || currentLevel.gameMode == GameLevel.GAME_MODE.ENDURANCE))
            {
                EndGame();
            }
        }
    }

    public void ResetAndSetGameLevel()
    {
        string oldId = currentLevel.id;
        this.currentLevel = null;
        this.currentLevel = GameManager.dataManager.LoadGameLevelFromId(oldId);
        SetGameLevel(currentLevel);
    }

    public void SetGameLevel(GameLevel gameLevel)
    {
        this.currentLevel = gameLevel;
        currentPoints = 0;
        currentTimeElasped = 0;
        currentMoves = (int)(gameLevel.gameMode == GameLevel.GAME_MODE.CLASSIC ? gameLevel.totalMoves : Mathf.Infinity);
        currentTimeRemaining = gameLevel.totalTime;
        ComponentUtility.RemoveChildren(gameBoardComponent);
        // call the GameInfo to update the info
        onMoveMade?.Invoke(currentMoves);

        if (gameLevel.board.size.x == 0 || gameLevel.board.size.y == 0)
        {
            return;
        }

        gameGrid.shape = gameLevel.board.gameBoardShape;
        switch (currentLevel.board.gameBoardShape)
        {
            case GAME_BOARD_SHAPE.SQUARE:
                DrawSquares(gameLevel);
                break;
            case GAME_BOARD_SHAPE.TRIANGLE:
                DrawTriangles(gameLevel);
                break;
            case GAME_BOARD_SHAPE.HEXAGON:
                DrawHexagons(gameLevel);
                break;
        }

        if (!inEditingMode)
        {
            clearButton.gameObject.SetActive(true);
            endButton.gameObject.SetActive(currentLevel.gameMode == GameLevel.GAME_MODE.ZEN);
            saveButton.gameObject.SetActive(false);
            openEditPanelButton.gameObject.SetActive(false);
            if (gameInputSystem.currentGameInputSelectors.OfType<GameInputSystem.TileSelector>().Count() > 0)
            {
                gameInputSystem.currentGameInputSelectors.OfType<GameInputSystem.TileSelector>().First().SetTiles(gameBoardComponent.GetComponentsInChildren<GameTileComponent>().ToList());
            }
        }
        else
        {
            clearButton.gameObject.SetActive(false);
            endButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(true);
            openEditPanelButton.gameObject.SetActive(true);
            levelEditPanelGameScene.SetLevel(currentLevel);
            if (gameInputSystem.currentGameInputSelectors.OfType<GameInputSystem.TileSelector>().Count() > 0)
            {
                gameInputSystem.currentGameInputSelectors.OfType<GameInputSystem.TileSelector>().First().SetTiles(gameBoardComponent.GetComponentsInChildren<GameTileComponent>().ToList());
            }
            //levelEditPanelGameScene.gameObject.SetActive(true);
        }
        levelInfoPanel.SetLevel(currentLevel);
        gameOverPanel.gameObject.SetActive(false);
    }


    public void SubmitSelection()
    {
        bool isValidMove = CheckIsValidMove();
        if (!isValidMove)
        {
            return;
        }

        StartCoroutine(SubmitSelectionCoroutine());
    }

    private IEnumerator TileDespawner(List<GameTileComponent> tileList)
    {
        foreach (GameTileComponent tile in tileList)
        {

            if (!tile.isDespawned)
            {
                // copy tile as new tile to self destruct
                GameTileComponent newTile = Instantiate(tile, tile.transform.position, Quaternion.identity);
                newTile.PlaySelfDestructAnimation(tile.gameTile.tileType);

                // old tile hide itself while its kid dies
                tile.PlayDespawnAnimation();
            }

        }
        yield return new WaitForSeconds(0.4f);
    }

    public IEnumerator TileSpawner(List<GameTileComponent> tileList)
    {
        foreach (GameTileComponent tile in tileList)
        {
            tile.PlaySpawnAnimation();
        }
        yield return new WaitForSeconds(0.25f);
    }

    public IEnumerator SubmitSelectionCoroutine()
    {

        GameInputSystem.IdleManager idleManager = gameInputSystem.currentGameInputManager as GameInputSystem.IdleManager;

        idleManager.StopListening();

        HashSet<GameTileComponent> activatedTiles = new HashSet<GameTileComponent>();
        List<Constants.GameScoreData> scoreData = new List<Constants.GameScoreData>();

        yield return StartCoroutine(ActivateTiles(groups, activatedTiles, scoreData));

        RandomizeTiles(activatedTiles.ToList());
        while (!AreAvailableMoves())
        {
            RandomizeTiles(activatedTiles.ToList());
        }

        yield return StartCoroutine(TileSpawner(activatedTiles.ToList()));

        // calculate score
        currentPoints += CalculateScore(scoreData);
        currentTimeRemaining += CalculateEarnedTime();

        idleManager.StartListening();

        currentMoves -= 1;
        onMoveMade?.Invoke(currentMoves);

        if (currentMoves <= 0 && currentLevel.gameMode == GameLevel.GAME_MODE.CLASSIC)
        {
            EndGame();
        }
    }

    public IEnumerator ActivateTiles(List<GameTileGroup> groups, HashSet<GameTileComponent> allTraversedTiles, List<Constants.GameScoreData> scoreData)
    {
        comboCounter += 1;
        comboCounter = Math.Min(5, comboCounter);
        List<GameTileComponent> newTiles = new List<GameTileComponent>();
        List<GameTileComponent> bombTiles = new List<GameTileComponent>();
        List<GameTileComponent> lineHTiles = new List<GameTileComponent>();
        List<GameTileComponent> lineVTiles = new List<GameTileComponent>();

        Constants.GameScoreData scoreDataItem = new Constants.GameScoreData();
        scoreDataItem.groupSize = groups.Count;

        foreach (GameTileGroup group in groups)
        {
            foreach (GameTileComponent gameTileComponent in group.tiles.Values)
            {
                // check and sort by types
                GameTile currentTile = gameTileComponent.gameTile;
                newTiles.Add(gameTileComponent);
                scoreDataItem.tileCount += 1;
                switch (currentTile.tileType)
                {
                    case GameTile.TILE_TYPE.BOMB:
                        bombTiles.Add(gameTileComponent);
                        scoreDataItem.specialCount += 1;
                        break;
                    case GameTile.TILE_TYPE.LINEH:
                        lineHTiles.Add(gameTileComponent);
                        scoreDataItem.specialCount += 1;
                        break;
                    case GameTile.TILE_TYPE.LINEV:
                        lineVTiles.Add(gameTileComponent);
                        scoreDataItem.specialCount += 1;
                        break;
                }
            }

            group.ClearGroup();
        }

        groups.Clear();
        scoreData.Add(scoreDataItem);

        List<GameTileGroup> newGroups = new List<GameTileGroup>();

        // do special tiles after all tiles are drawn
        ActivateNormalTiles(newTiles, allTraversedTiles);
        yield return StartCoroutine(TileDespawner(newTiles));

        List<GameTileComponent> newBombedTiles = ActivateSpecialTiles(GameTile.TILE_TYPE.BOMB, bombTiles, allTraversedTiles);
        if (newBombedTiles.Count > 0)
        {
            soundBoard.PlaySound(soundBoard.Bomb);
            yield return StartCoroutine(TileDespawner(newBombedTiles));
            newGroups.Add(new GameTileGroup(newBombedTiles));
        }

        List<GameTileComponent> newLinedHTiles = ActivateSpecialTiles(GameTile.TILE_TYPE.LINEH, lineHTiles, allTraversedTiles);
        if (newLinedHTiles.Count > 0)
        {
            soundBoard.PlaySound(soundBoard.Line);
            yield return StartCoroutine(TileDespawner(newLinedHTiles));
            newGroups.Add(new GameTileGroup(newLinedHTiles));
        }

        List<GameTileComponent> newLinedVTiles = ActivateSpecialTiles(GameTile.TILE_TYPE.LINEV, lineVTiles, allTraversedTiles);
        if (newLinedVTiles.Count > 0)
        {
            soundBoard.PlaySound(soundBoard.Line);
            yield return StartCoroutine(TileDespawner(newLinedVTiles));
            newGroups.Add(new GameTileGroup(newLinedVTiles));
        }

        switch (comboCounter)
        {
            case 2:
                soundBoard.PlaySound(soundBoard.Combo2);
                break;
            case 3:
                soundBoard.PlaySound(soundBoard.Combo3);
                break;
            case 4:
                soundBoard.PlaySound(soundBoard.Combo4);
                break;
            case 5:
                soundBoard.PlaySound(soundBoard.Combo5);
                break;
            default:
                soundBoard.PlaySound(soundBoard.Combo1);
                break;
        }

        // either bombed or lined some new tiles, continue the exploration
        if (newGroups.Count > 0)
        {
            yield return StartCoroutine(ActivateTiles(newGroups, allTraversedTiles, scoreData));
        }

        comboCounter = 0;
    }

    private void ActivateNormalTiles(List<GameTileComponent> tileList, HashSet<GameTileComponent> allTraversedTiles)
    {
        foreach (GameTileComponent tile in tileList)
        {
            if (!allTraversedTiles.Contains(tile))
            {
                allTraversedTiles.Add(tile);
                // TODO: what does this do?: reverse x axis
                transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
            }
        }
    }

    private List<GameTileComponent> ActivateSpecialTiles(GameTile.TILE_TYPE tileType, List<GameTileComponent> tileList, HashSet<GameTileComponent> allTraversedTiles)
    {
        List<GameTileComponent> newActivatedTiles = new List<GameTileComponent>();
        foreach (GameTileComponent gameTileComponent in tileList)
        {
            GameObject tileFab = gameTileComponent.gameObject;
            GameTile currentTile = gameTileComponent.gameTile;

            Dictionary<Vector2, GameTile> tilesAround;
            switch (tileType)
            {
                case GameTile.TILE_TYPE.BOMB:
                    tilesAround = GameUtility.GetAdjacentTiles(currentLevel, currentTile);
                    break;
                case GameTile.TILE_TYPE.LINEH:
                    tilesAround = GameUtility.GetAxisTiles(currentLevel, currentTile, GameUtility.AxisDirection.HORIZONTAL);
                    break;
                case GameTile.TILE_TYPE.LINEV:
                    tilesAround = GameUtility.GetAxisTiles(currentLevel, currentTile, GameUtility.AxisDirection.VERTICAL);
                    break;
                default:
                    tilesAround = new Dictionary<Vector2, GameTile>();
                    break;
            }

            foreach (GameTile tile in tilesAround.Values)
            {
                GameTileComponent gameTileObject = gameBoardComponent.GetComponentsInChildren<GameTileComponent>().FirstOrDefault(x => x.gameTile == tile);
                if (gameTileObject.gameObject && !allTraversedTiles.Contains(gameTileObject))
                {
                    newActivatedTiles.Add(gameTileObject);
                    allTraversedTiles.Add(gameTileObject);
                }
            }
        }
        return newActivatedTiles;
    }

    // Final step of the submission process
    public void RandomizeTiles(List<GameTileComponent> allTraversedTiles)
    {
        foreach (GameTileComponent tile in allTraversedTiles)
        {
            GameTile currentTile = tile.gameTile;

            // RNG special colour
            tile.SetTileColor(UnityEngine.Random.Range(0, currentLevel.totalColours));

            // RNG special tile
            // TODO: calc percentage to rng each tile type
            int rng = UnityEngine.Random.Range(0, 100);
            currentTile.tileType = rng switch
            {
                int x when x <= 80 => GameTile.TILE_TYPE.NORMAL,
                int x when x > 80 && x <= 85 => GameTile.TILE_TYPE.RAINBOW,
                int x when x > 85 && x <= 90 => GameTile.TILE_TYPE.BOMB,
                int x when x > 90 && x <= 95 => GameTile.TILE_TYPE.LINEH,
                _ => GameTile.TILE_TYPE.LINEV
            };
            tile.GetComponent<GameTileComponent>().SetTileType(currentTile.tileType);
        }
    }

    private int CalculateScore(List<Constants.GameScoreData> scoreData)
    {
        // (size + groups)^1.5 + (5 # total special)
        int score = 0;
        for (int i = 0; i < scoreData.Count; i++)
        {
            Constants.GameScoreData data = scoreData[i];
            if (i == 0)
            {
                score += (int)Math.Pow(data.groupSize + (data.tileCount / data.groupSize), 2f);
            }
            else
            {
                score += 5 * data.tileCount;
            }
        }
        return score;
    }

    private int CalculateEarnedTime()
    {
        if (currentLevel.gameMode == GameLevel.GAME_MODE.ENDURANCE)
        {
            return 2;
        }

        return 0;
    }

    private bool CheckIsValidMove()
    {

        if (groups.Count < 2)
        {
            return false;
        }

        if (!currentLevel.restrictions.CheckSubmittion(groups))
        {
            return false;
        }

        // generate up to the highest colour index
        int highestColourIndex = groups.Select(x => x.tiles.Values.Max(y => y.gameTile.colourIndex)).Max();
        List<int> referenceTileColourCount = new List<int>(highestColourIndex);
        for (int i = 0; i <= highestColourIndex; i++)
            referenceTileColourCount.Add(0);

        // find group without rainbow tile to be reference
        GameTileGroup referenceGroup = groups.FirstOrDefault(x => x.tiles.Values.All(y => y.gameTile.tileType != GameTile.TILE_TYPE.RAINBOW));
        int lives = 0;
        // if all group has rainbow then find the one with least
        if (referenceGroup == null)
        {
            referenceGroup = groups.OrderBy(x => x.tiles.Values.Count(y => y.gameTile.tileType == GameTile.TILE_TYPE.RAINBOW)).First();
            lives = referenceGroup.tiles.Values.Count(x => x.gameTile.tileType == GameTile.TILE_TYPE.RAINBOW);
        }

        // count the number of each color in the reference
        foreach (GameTileComponent tile in referenceGroup.tiles.Values)
        {
            if (tile.tileType != GameTile.TILE_TYPE.RAINBOW)
            {
                referenceTileColourCount[tile.gameTile.colourIndex]++;
            }
        }

        // check if the number of each color is the same as the reference group for every group
        foreach (GameTileGroup group in groups)
        {
            int totalLives = lives + group.tiles.Values.Count(x => x.gameTile.tileType == GameTile.TILE_TYPE.RAINBOW);

            // unequal number of tiles per group check
            if (group.tiles.Values.Count != referenceGroup.tiles.Values.Count)
            {
                return false;
            }

            // local colour count for the group
            List<int> groupColourCount = new List<int>(highestColourIndex);
            for (int i = 0; i <= highestColourIndex; i++)
            {
                groupColourCount.Add(0);
            }

            foreach (GameTileComponent tile in group.tiles.Values)
            {
                if (tile.tileType != GameTile.TILE_TYPE.RAINBOW)
                {
                    groupColourCount[tile.gameTile.colourIndex]++;
                }
            }

            // calculate lives based on number difference between reference and current group
            for (int i = 0; i <= highestColourIndex; i++)
            {
                if (referenceTileColourCount[i] != groupColourCount[i])
                {
                    totalLives -= Mathf.Abs(referenceTileColourCount[i] - groupColourCount[i]);
                }
            }

            // too many diffs = fail
            if (totalLives < 0)
            {
                return false;
            }
        }

        return true;
    }

    private void EndGame()
    {
        // gameOverPanel.gameObject.SetActive(true);
        gamePaused = true;
        GameSave.LevelProgress levelProgress = new GameSave.LevelProgress();
        levelProgress.levelId = currentLevel.id;
        switch (currentLevel.gameMode)
        {
            case GameLevel.GAME_MODE.CLASSIC:
                levelProgress.bestScore = currentPoints;
                break;
            case GameLevel.GAME_MODE.ENDURANCE:
                levelProgress.bestScore = currentPoints;
                break;
            case GameLevel.GAME_MODE.RACE:
                levelProgress.bestScore = currentPoints;
                break;
            case GameLevel.GAME_MODE.ZEN:
                levelProgress.bestScore = currentPoints;
                break;
        }
        enabled = false;
        GameManager.saveManager.AddLevelProgress(currentLevel, levelProgress);

        // show they player they're the best or they suck with the gameover panel
        gameOverPanel.InitGameOverPanel(currentLevel, levelProgress, currentPoints);
    }

    public void ReviveGame()
    {
        // TODO: resume game
        OnClearButtonClicked();
        currentMoves += 3;
        currentTimeRemaining += 10;
        enabled = true;
        gamePaused = false;
    }

    public void PauseGame()
    {
        if (!inEditingMode)
        {
            GameInputSystem.IdleManager idleManager = gameInputSystem.currentGameInputManager as GameInputSystem.IdleManager;
            idleManager.StopListening();
            gamePaused = true;
        }
    }

    public void ResumeGame()
    {
        if (!inEditingMode)
        {
            GameInputSystem.IdleManager idleManager = gameInputSystem.currentGameInputManager as GameInputSystem.IdleManager;
            idleManager.StartListening();
            enabled = true;
            gamePaused = false;
        }
    }

    private void OnThemeChange(int themeIndex)
    {
        // refresh color on tiles
        foreach (GameTileComponent tile in gameBoardComponent.GetComponentsInChildren<GameTileComponent>())
        {
            tile.SetTileColor(tile.gameTile.colourIndex);
        }
    }

    private void DrawSquares(GameLevel gameLevel)
    {
        tileSpacing = 0;
        float maxPossibleTileWidth = (gameBoardComponent.GetComponent<RectTransform>().rect.size.x - (tileSpacing * (gameLevel.board.size.x - 1))) / gameLevel.board.size.x;
        float maxPossibleTileHeight = (gameBoardComponent.GetComponent<RectTransform>().rect.size.y - (tileSpacing * (gameLevel.board.size.y - 1))) / gameLevel.board.size.y;
        float finalTileWidth = Mathf.Min(maxPossibleTileHeight, maxPossibleTileWidth);
        float finalTileHeight = finalTileWidth;
        float finalScale = finalTileWidth;
        Vector3 tileOffset = new Vector3(
            (gameLevel.board.size.x - 1) * (finalTileWidth + tileSpacing) / 2,
            (gameLevel.board.size.y - 1) * (finalTileHeight + tileSpacing) / 2
        );
        foreach (GameTile tile in currentLevel.board.tiles.Values)
        {
            GameObject newTile = Instantiate(squarePrefab, gameBoardComponent.transform);
            newTile.transform.localScale = new Vector3(finalScale, finalScale, 2);
            newTile.transform.localPosition = new Vector3(finalTileWidth * tile.location.x, finalTileWidth * tile.location.y, 0);
            newTile.transform.localPosition += new Vector3(tileSpacing * tile.location.x, 0, 0);
            newTile.transform.localPosition += new Vector3(0, tileSpacing * tile.location.y, 0);
            newTile.transform.localPosition -= tileOffset;
            newTile.transform.localPosition = Vector3.Scale(newTile.transform.localPosition, new Vector3(1, -1, 1));

            newTile.GetComponent<GameTileComponent>().text.transform.localPosition = new Vector3();
            newTile.GetComponent<GameTileComponent>().SetTileColor(tile.colourIndex);
            newTile.GetComponent<GameTileComponent>().SetTileType(tile.tileType);
            newTile.GetComponent<GameTileComponent>().gameTile = tile;
        }

        if (inEditingMode)
        {
            for (int x = 0; x < currentLevel.board.size.x; x++)
            {
                for (int y = 0; y < currentLevel.board.size.y; y++)
                {
                    if (!currentLevel.board.tiles.ContainsKey(new Vector2(x, y)))
                    {
                        GameTile tile = new GameTile();
                        tile.location = new Vector2(x, y);
                        GameObject newTile = Instantiate(squarePrefab, gameBoardComponent.transform);
                        newTile.transform.localScale = new Vector3(finalScale, finalScale, 2);
                        newTile.transform.localPosition = new Vector3(finalTileWidth * x, finalTileWidth * y, 0);
                        newTile.transform.localPosition += new Vector3(tileSpacing * x, 0, 0);
                        newTile.transform.localPosition += new Vector3(0, tileSpacing * y, 0);
                        newTile.transform.localPosition -= tileOffset;
                        newTile.transform.localPosition = Vector3.Scale(newTile.transform.localPosition, new Vector3(1, -1, 1));

                        newTile.GetComponent<GameTileComponent>().text.transform.localPosition = new Vector3();
                        newTile.GetComponent<GameTileComponent>().spriteRenderer.enabled = false;
                        newTile.GetComponent<GameTileComponent>().SetTileType(0);
                        newTile.GetComponent<GameTileComponent>().gameTile = tile;
                    }
                }
            }
        }

        gameGrid.size = finalScale;
        float camWidth = camera.orthographicSize * camera.aspect * 2;
        float camHeight = camera.orthographicSize * 2;
        Rect boardRect = gameBoardComponent.gameObject.GetComponent<RectTransform>().rect;
        gameGrid.transform.localPosition = new Vector3(
            (((gameGrid.bounds.x - camWidth) / 2 + (camWidth - boardRect.width) / 2 + gameBoardComponent.gameObject.transform.localPosition.x + (boardRect.width / 2 - tileOffset.x - finalScale / 2)) % finalScale),
            (((gameGrid.bounds.y - camHeight) / 2 + (camHeight - boardRect.height) / 2 + gameBoardComponent.gameObject.transform.localPosition.y + (boardRect.height / 2 - tileOffset.y - finalScale / 2)) % finalScale),
            gameGrid.transform.localPosition.z);
    }

    private void DrawTriangles(GameLevel gameLevel)
    {
        float maxPossibleTileWidth = (gameBoardComponent.GetComponent<RectTransform>().rect.size.x) / (gameLevel.board.size.x / 2 + 0.5f);
        float maxPossibleTileHeight = (gameBoardComponent.GetComponent<RectTransform>().rect.size.y) / (gameLevel.board.size.y);
        float finalTileWidth = Mathf.Min(maxPossibleTileHeight, maxPossibleTileWidth);
        float finalTileHeight = finalTileWidth;
        float finalScale = finalTileWidth;
        Vector3 tileOffset = new Vector3(
            ((gameLevel.board.size.x - 1) / 2) * (finalTileWidth) / 2,
            (gameLevel.board.size.y - 1) * (finalTileHeight) / 2
        );
        foreach (GameTile tile in currentLevel.board.tiles.Values)
        {
            GameObject newTile = Instantiate(trianglePrefab, gameBoardComponent.transform);
            newTile.transform.localScale = new Vector3(finalScale, finalScale, 2);
            newTile.transform.localPosition += new Vector3(finalTileWidth * tile.location.x / 2, finalTileHeight * tile.location.y, 0);
            newTile.transform.localPosition -= tileOffset;
            newTile.transform.localPosition = Vector3.Scale(newTile.transform.localPosition, new Vector3(1, -1, 1));

            if ((tile.location.x % 2 == 0 && tile.location.y % 2 == 1) || (tile.location.x % 2 == 1 && tile.location.y % 2 == 0))
            {
                newTile.transform.Rotate(new Vector3(0, 0, 180), Space.World);
            }

            newTile.GetComponent<GameTileComponent>().text.transform.localPosition = new Vector3();
            newTile.GetComponent<GameTileComponent>().SetTileColor(tile.colourIndex);
            newTile.GetComponent<GameTileComponent>().SetTileType(tile.tileType);
            newTile.GetComponent<GameTileComponent>().gameTile = tile;
        }
    }

    private void DrawHexagons(GameLevel gameLevel)
    {
        float baseTileRatio = Mathf.Sqrt(3) / 2;
        float maxPossibleTileWidth = (gameBoardComponent.GetComponent<RectTransform>().rect.size.x) / (gameLevel.board.size.x * 1.5f + 0.25f);
        float maxPossibleTileHeight = (gameBoardComponent.GetComponent<RectTransform>().rect.size.y) / ((gameLevel.board.size.y / 2) + 0.5f);
        float finalTileWidth, finalTileHeight, finalScale;
        if (maxPossibleTileHeight < maxPossibleTileWidth * baseTileRatio)
        {
            finalTileWidth = maxPossibleTileHeight / baseTileRatio;
            finalTileHeight = maxPossibleTileHeight;
        }
        else
        {
            finalTileWidth = maxPossibleTileWidth;
            finalTileHeight = maxPossibleTileWidth * baseTileRatio;
        }
        Vector3 tileOffset = new Vector3(
            ((gameLevel.board.size.x - 1) * 1.5f + 0.75f) * finalTileWidth / 2,
            ((gameLevel.board.size.y - 1) / 2f) * finalTileHeight / 2
        );
        foreach (GameTile tile in currentLevel.board.tiles.Values)
        {
            GameObject newTile = Instantiate(hexagonPrefab, gameBoardComponent.transform);
            newTile.transform.localScale = new Vector3(maxPossibleTileWidth, maxPossibleTileWidth, 2);
            newTile.transform.localPosition += new Vector3(finalTileWidth * tile.location.x + ((tile.location.x) * finalTileWidth * 0.5f), finalTileHeight * tile.location.y / 2f, 0);
            newTile.transform.localPosition -= tileOffset;
            newTile.transform.localPosition = Vector3.Scale(newTile.transform.localPosition, new Vector3(1, -1, 1));

            if (tile.location.y % 2 == 0)
            {
                newTile.transform.localPosition += new Vector3(finalTileWidth * 0.75f, 0, 0);
            }

            newTile.GetComponent<GameTileComponent>().text.transform.localPosition = new Vector3();
            newTile.GetComponent<GameTileComponent>().SetTileColor(tile.colourIndex);
            newTile.GetComponent<GameTileComponent>().SetTileType(tile.tileType);
            newTile.GetComponent<GameTileComponent>().gameTile = tile;
        }
    }

    public void OnClearButtonClicked()
    {
        foreach (GameTileGroup group in groups)
        {
            group.ClearGroup();
        }
        groups.Clear();
    }

    public void OnEndButtonClicked()
    {
        if (currentLevel.gameMode == GameLevel.GAME_MODE.ZEN)
        {
            EndGame();
        }
    }

    public bool AreAvailableMoves()
    {
        Dictionary<int, HashSet<int>> existingData = new Dictionary<int, HashSet<int>>();

        foreach (KeyValuePair<Vector2, GameTile> tileKeyValuePair in currentLevel.board.tiles)
        {
            int currentColourIndex = tileKeyValuePair.Value.colourIndex;
            if (!existingData.ContainsKey(currentColourIndex))
            {
                existingData[currentColourIndex] = new HashSet<int>();
            }
        }

        foreach (KeyValuePair<Vector2, GameTile> tileKeyValuePair in currentLevel.board.tiles)
        {
            Vector2 southVector = tileKeyValuePair.Key + new Vector2(0, 1);
            Vector2 eastVector = tileKeyValuePair.Key + new Vector2(1, 0);
            bool doSouth = false;

            int currentColourIndex = tileKeyValuePair.Value.colourIndex;
            if (!existingData.ContainsKey(currentColourIndex))
            {
                existingData[currentColourIndex] = new HashSet<int>();
            }

            if (currentLevel.board.tiles.ContainsKey(eastVector))
            {
                int eastColourIndex = currentLevel.board.tiles[eastVector].colourIndex;
                if (existingData[currentColourIndex].Contains(eastColourIndex)
                    || existingData[eastColourIndex].Contains(currentColourIndex))
                {
                    return true;
                }
                existingData[currentColourIndex].Add(eastColourIndex);

                if (currentLevel.board.tiles.ContainsKey(southVector))
                {
                    doSouth = eastColourIndex != currentLevel.board.tiles[southVector].colourIndex;
                }
            }
            else
            {
                doSouth = currentLevel.board.tiles.ContainsKey(southVector);
            }

            // check no dupelicates exist i.e. east/south are the same colour
            if (doSouth)
            {
                int southColourIndex = currentLevel.board.tiles[southVector].colourIndex;
                if (existingData[currentColourIndex].Contains(southColourIndex)
                    || existingData[southColourIndex].Contains(currentColourIndex))
                {
                    return true;
                }
                existingData[currentColourIndex].Add(southColourIndex);
            }
        }

        return false;
    }
}
