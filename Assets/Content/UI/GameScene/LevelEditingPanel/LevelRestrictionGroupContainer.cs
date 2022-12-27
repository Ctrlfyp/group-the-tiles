using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LevelRestrictionGroupContainer : MonoBehaviour
    {
        private GameLevel currentLevel;
        public LevelRestrictionClauseContainer levelRestrictionClauseContainerPrefab;
        public LevelRestrictionGroupContainer levelRestrictionGroupContainerPrefab;
        public UI.DesignSystem.UIButton deleteButton;

        [HideInInspector]
        public LevelRestrictionGroup levelRestrictionGroup;

        public void SetRestrictionGroup(GameLevel currentLevel, LevelRestrictionGroup levelRestrictionGroup)
        {
            this.currentLevel = currentLevel;
            this.levelRestrictionGroup = levelRestrictionGroup;
            if (levelRestrictionGroup.parent == null)
            {
                deleteButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }

            foreach (LevelRestriction levelRestriction in levelRestrictionGroup.restrictions)
            {
                if (levelRestriction is LevelRestrictionClause)
                {
                    LevelRestrictionClauseContainer container = AddClauseContainer();
                    container.SetRestrictionClause(currentLevel, levelRestriction as LevelRestrictionClause);
                }
                else
                {
                    LevelRestrictionGroupContainer container = AddGroupContainer();
                    container.SetRestrictionGroup(currentLevel, levelRestriction as LevelRestrictionGroup);
                }
            }
        }

        public void OnOperatorChanged(int op)
        {
            levelRestrictionGroup.groupOperator = (LevelRestriction.BOOLEAN_OPERATOR)op;
        }

        public void OnDeleteGroupClicked()
        {
            levelRestrictionGroup.parent.RemoveLevelRestriction(levelRestrictionGroup);
            Destroy(gameObject.transform.parent.gameObject);
        }

        public void OnAddGroupClicked()
        {
            LevelRestrictionGroup group = new LevelRestrictionGroup(new List<LevelRestriction>(), LevelRestriction.BOOLEAN_OPERATOR.AND);
            levelRestrictionGroup.AddLevelRestriction(group);

            LevelRestrictionGroupContainer container = AddGroupContainer();
            container.SetRestrictionGroup(currentLevel, group);
        }

        public void OnAddClauseClicked()
        {
            LevelRestrictionClause.GroupCountClause clause = new LevelRestrictionClause.GroupCountClause();
            levelRestrictionGroup.AddLevelRestriction(clause);
            LevelRestrictionClauseContainer container = AddClauseContainer();
            container.SetRestrictionClause(currentLevel, clause);
        }

        private LevelRestrictionGroupContainer AddGroupContainer()
        {
            LevelRestrictionGroupContainer levelRestrictionGroupContainer = Instantiate(levelRestrictionGroupContainerPrefab);

            GameObject gameObject = new GameObject();
            gameObject.name = "RuleGroupHolder";
            gameObject.AddComponent<RectTransform>();
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 150);
            gameObject.transform.SetParent(transform);

            levelRestrictionGroupContainer.transform.SetParent(gameObject.transform);
            levelRestrictionGroupContainer.GetComponent<RectTransform>().offsetMin += new Vector2(40, 0);
            levelRestrictionGroupContainer.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta - new Vector2(20, 0);
            return levelRestrictionGroupContainer;
        }

        private LevelRestrictionClauseContainer AddClauseContainer()
        {
            LevelRestrictionClauseContainer levelRestrictionClauseContainer = Instantiate(levelRestrictionClauseContainerPrefab);

            GameObject gameObject = new GameObject();
            gameObject.name = "RuleClauseHolder";
            gameObject.AddComponent<RectTransform>();
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 250);
            gameObject.transform.SetParent(transform);

            levelRestrictionClauseContainer.transform.SetParent(gameObject.transform);
            levelRestrictionClauseContainer.GetComponent<RectTransform>().offsetMin += new Vector2(40, 0);
            levelRestrictionClauseContainer.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta - new Vector2(20, 0);
            return levelRestrictionClauseContainer;
        }
    }
}
