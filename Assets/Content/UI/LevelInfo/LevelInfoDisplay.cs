using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class LevelInfoDisplay : MonoBehaviour
    {

        public GameObject levelShapeObject;
        public GameObject levelModeObject;

        public void SetLevelInfoPanel(GAME_BOARD_SHAPE gameBoardShape, GameLevel.GAME_MODE gameMode, bool hideText = false)
        {
            // set level shape
            foreach (Transform child in levelShapeObject.transform)
            {
                if (child.name == gameBoardShape.ToString())
                {
                    child.gameObject.SetActive(true);
                }
                else if (child.name == "Text")
                {
                    if (hideText)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);
                        child.GetComponent<Text>().text = $"SHAPE: {gameBoardShape.ToString()}";
                    }
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            // set level mode
            foreach (Transform child in levelModeObject.transform)
            {
                if (child.name == gameMode.ToString())
                {
                    child.gameObject.SetActive(true);
                }
                else if (child.name == "Text")
                {
                    if (hideText)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);
                        child.GetComponent<Text>().text = $"MODE: {gameMode.ToString()}";
                    }
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

    }

}