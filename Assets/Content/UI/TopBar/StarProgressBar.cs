using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class StarProgressBar : MonoBehaviour
    {

        public UI.StarObject starObjectPrefab;
        public UI.DesignSystem.UIProgressBar progressBar;
        public GameObject starContainer;
        public float starObjectScale = 0.4f;

        private UI.StarObject[] starObjects;

        private int maxScore;
        private int[] scoreRequired;


        // Start is called before the first frame update
        void Start()
        {
            // starObjects = starContainer.GetComponentsInChildren<UI.StarObject>();
        }

        public void Init(GameLevel currentLevel)
        {
            // get the width of self since the starContainer is the same size as self
            float starContainerWidth = GetComponent<RectTransform>().rect.width;

            // should be 3 but counting it anyways
            int starCount = currentLevel.difficultyScoreRequired.Count;

            starObjects = new UI.StarObject[starCount];
            scoreRequired = new int[starCount];

            // get the last value as the max score
            maxScore = currentLevel.difficultyScoreRequired[starCount];

            foreach (KeyValuePair<int, int> scoreRequired in currentLevel.difficultyScoreRequired)
            {
                UI.StarObject starObject = Instantiate(starObjectPrefab, starContainer.transform);
                starObject.SetStar(false, null);
                starObject.transform.localScale = new Vector3(starObjectScale, starObjectScale, starObjectScale);

                // calculate the position of the star
                float starPosition = (float)scoreRequired.Value / (float)maxScore;
                starObject.transform.localPosition = new Vector3(starContainerWidth * starPosition, 0, 0);
                starObjects[scoreRequired.Key - 1] = starObject;
                this.scoreRequired[scoreRequired.Key - 1] = scoreRequired.Value;
            }
        }

        public void UpdateStatus(int currentScore)
        {
            // set the stars
            for (int i = 0; i < starObjects.Length; i++)
            {
                if (currentScore >= scoreRequired[i])
                {
                    starObjects[i].SetStar(true);
                }
                else
                {
                    starObjects[i].SetStar(false);
                }
            }

            // set progress bar
            float progress = (float)currentScore / (float)maxScore;
            progressBar.SetProgress(progress);
        }

    }
}