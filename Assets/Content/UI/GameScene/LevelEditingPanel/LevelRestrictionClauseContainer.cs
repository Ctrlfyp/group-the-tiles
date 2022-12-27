using UI.DesignSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelRestrictionClauseContainer : MonoBehaviour
    {
        private GameLevel gameLevel;
        private LevelRestrictionClause levelRestrictionClause;

        public UIDropdown clauseTypeDropdown;
        public UIButton deleteButton;

        public GameObject countInputHolder;
        public UIInputField minCountInputField;
        public UIInputField exactCountInputField;
        public UIInputField maxCountInputField;

        public GameObject tileCountInputHolder;
        public UIInputField minTileCountInputField;
        public UIInputField exactTileCountInputField;
        public UIInputField maxTileCountInputField;

        public GameObject groupSpecificInputHolder;

        public GameObject groupMinimumInputHolder;

        public GameObject colourFieldPrefab;

        public void SetRestrictionClause(GameLevel gameLevel, LevelRestrictionClause levelRestrictionClause)
        {
            this.gameLevel = gameLevel;

            this.levelRestrictionClause = levelRestrictionClause;

            countInputHolder.gameObject.SetActive(false);
            tileCountInputHolder.gameObject.SetActive(false);
            groupSpecificInputHolder.gameObject.SetActive(false);
            groupMinimumInputHolder.gameObject.SetActive(false);

            if (levelRestrictionClause is LevelRestrictionClause.GroupCountClause)
            {
                LevelRestrictionClause.GroupCountClause countClause = levelRestrictionClause as LevelRestrictionClause.GroupCountClause;
                clauseTypeDropdown.dropdownContainer.value = 0;

                minCountInputField.text = countClause.minSize.ToString();
                exactCountInputField.text = countClause.requiredSize.ToString();
                maxCountInputField.text = countClause.maxSize.ToString();

                countInputHolder.gameObject.SetActive(true);
            }
            else if (levelRestrictionClause is LevelRestrictionClause.GroupTileCountClause)
            {
                LevelRestrictionClause.GroupTileCountClause tileCountClause = levelRestrictionClause as LevelRestrictionClause.GroupTileCountClause;
                clauseTypeDropdown.dropdownContainer.value = 1;

                minTileCountInputField.text = tileCountClause.minSize.ToString();
                exactTileCountInputField.text = tileCountClause.requiredSize.ToString();
                maxTileCountInputField.text = tileCountClause.maxSize.ToString();

                tileCountInputHolder.gameObject.SetActive(true);
            }
            else if (levelRestrictionClause is LevelRestrictionClause.GroupSpecificClause)
            {
                LevelRestrictionClause.GroupSpecificClause groupSpecificClause = levelRestrictionClause as LevelRestrictionClause.GroupSpecificClause;
                clauseTypeDropdown.dropdownContainer.value = 2;

                groupSpecificInputHolder.gameObject.SetActive(true); 
                ComponentUtility.RemoveChildren(groupSpecificInputHolder.transform);

                for (int i = 0; i < gameLevel.totalColours; i++)
                {
                    GameObject colourField = Instantiate(colourFieldPrefab, groupSpecificInputHolder.transform);
                    Text text = colourField.transform.GetChild(0).GetComponent<Text>();
                    text.name = $"Colour {i}";
                }
            }
            else if (levelRestrictionClause is LevelRestrictionClause.GroupMinimumClause)
            {
                LevelRestrictionClause.GroupMinimumClause groupMinimumClause = levelRestrictionClause as LevelRestrictionClause.GroupMinimumClause;
                clauseTypeDropdown.dropdownContainer.value = 3;

                groupMinimumInputHolder.gameObject.SetActive(true);
                ComponentUtility.RemoveChildren(groupMinimumInputHolder.transform);
                for (int i = 0; i < gameLevel.totalColours; i++)
                {
                    // refer to this post https://forum.unity.com/threads/how-to-add-non-persistent-unityaction-with-a-single-parameter-to-a-buttons-onclick-event.267047/
                    int local_i = i;
                     GameObject colourField = Instantiate(colourFieldPrefab, groupMinimumInputHolder.transform);
                    Text text = colourField.transform.GetChild(0).GetComponent<Text>();
                    text.text = $"Colour {i + 1}";
                    UIInputField uIInputField = colourField.transform.GetChild(1).GetComponent<UIInputField>();
                    uIInputField.inputField.text = groupMinimumClause.referenceTileValues.ContainsKey(i) ? groupMinimumClause.referenceTileValues[i].ToString() : "-1";
                    uIInputField.OnValueChanged += (string value) => { Debug.Log(local_i); ; OnSpecificMinimumChanged(local_i, value); };
                }
            }
        }

        public void OnClauseTypeChanged(int op)
        {
            LevelRestrictionClause newClause;
            switch (op)
            {
                case 1:
                    newClause = new LevelRestrictionClause.GroupTileCountClause();
                    break;
                case 2:
                    newClause = new LevelRestrictionClause.GroupSpecificClause();
                    break;
                case 3:
                    newClause = new LevelRestrictionClause.GroupMinimumClause();
                    break;
                default:
                case 0:
                    newClause = new LevelRestrictionClause.GroupCountClause();
                    break;
            }

            levelRestrictionClause.parent.ReplaceLevelRestriction(levelRestrictionClause, newClause);
            this.levelRestrictionClause = newClause;
            SetRestrictionClause(gameLevel, newClause);
        }

        #region Group Count
        public void OnMinCountChanged(string value)
        {
            LevelRestrictionClause.GroupCountClause countClause = levelRestrictionClause as LevelRestrictionClause.GroupCountClause;
            int count = int.TryParse(value, out count) ? count : countClause.minSize;
            countClause.minSize = count;
            minCountInputField.text = count.ToString();
        }

        public void OnExactCountChanged(string value)
        {
            LevelRestrictionClause.GroupCountClause countClause = levelRestrictionClause as LevelRestrictionClause.GroupCountClause;
            int count = int.TryParse(value, out count) ? count : countClause.requiredSize;
            countClause.requiredSize = count;
            exactCountInputField.text = count.ToString();
        }

        public void OnMaxCountChanged(string value)
        {
            LevelRestrictionClause.GroupCountClause countClause = levelRestrictionClause as LevelRestrictionClause.GroupCountClause;
            int count = int.TryParse(value, out count) ? count : countClause.maxSize;
            countClause.maxSize = count;
            maxCountInputField.text = count.ToString();
        }
        #endregion

        #region Tile Count
        public void OnMinTileCountChanged(string value)
        {
            LevelRestrictionClause.GroupTileCountClause tileCountClause = levelRestrictionClause as LevelRestrictionClause.GroupTileCountClause;
            int count = int.TryParse(value, out count) ? count : tileCountClause.minSize;
            tileCountClause.minSize = count;
            minCountInputField.text = count.ToString();
        }

        public void OnExactTileCountChanged(string value)
        {
            LevelRestrictionClause.GroupTileCountClause tileCountClause = levelRestrictionClause as LevelRestrictionClause.GroupTileCountClause;
            int count = int.TryParse(value, out count) ? count : tileCountClause.requiredSize;
            tileCountClause.requiredSize = count;
            exactCountInputField.text = count.ToString();
        }

        public void OnMaxTileCountChanged(string value)
        {
            LevelRestrictionClause.GroupTileCountClause tileCountClause = levelRestrictionClause as LevelRestrictionClause.GroupTileCountClause;
            int count = int.TryParse(value, out count) ? count : tileCountClause.maxSize;
            tileCountClause.maxSize = count;
            maxCountInputField.text = count.ToString();
        }
        #endregion

        #region Specific 
        public void OnSpecificSpecificChanged(int colour, string value)
        {
            LevelRestrictionClause.GroupSpecificClause groupSpecificClause = levelRestrictionClause as LevelRestrictionClause.GroupSpecificClause;
            int count = int.TryParse(value, out count) ? count : -1;
            if (count == -1)
            {
                groupSpecificClause.referenceTileValues.Remove(colour);
                return;
            }

            groupSpecificClause.referenceTileValues[colour] = count;
            minCountInputField.text = count.ToString();
        }
        #endregion

        #region Minimum
        public void OnSpecificMinimumChanged(int colour, string value)
        {
            LevelRestrictionClause.GroupMinimumClause groupMinimumClause = levelRestrictionClause as LevelRestrictionClause.GroupMinimumClause;
            int count = int.TryParse(value, out count) ? count : -1;
            if (count == -1)
            {
                groupMinimumClause.referenceTileValues.Remove(colour);
                return;
            }

            groupMinimumClause.referenceTileValues[colour] = count;
            minCountInputField.text = count.ToString();
        }
        #endregion

        public void OnDeleteGroupClicked()
        {
            levelRestrictionClause.parent.RemoveLevelRestriction(levelRestrictionClause);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
