using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Constants
{
    public static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        Converters =
        {
            new JsonUtility.Vec2Conv()
        },
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    public static readonly Dictionary<int, string> indexToColourStringMap = new Dictionary<int, string>()
    {
        { 0, "TILE_COLOUR_ONE" },
        { 1, "TILE_COLOUR_TWO" },
        { 2, "TILE_COLOUR_THREE" },
        { 3, "TILE_COLOUR_FOUR" },
    };

    public static readonly Dictionary<string, Color> tileColourMap = new Dictionary<string, Color>()
    {
        { "TILE_COLOUR_ONE", new Color(0, 1, 1) },
        { "TILE_COLOUR_TWO", new Color(1, 0, 1) },
        { "TILE_COLOUR_THREE", new Color(1, 1, 0) },
        { "TILE_COLOUR_FOUR", new Color(1, 0, 0) },
    };

    public static readonly string playerPrefThemeKey = "selectedTheme";

    public class GameScoreData
    {
        public int tileCount = 0;
        public int groupSize = 0;
        public int specialCount = 0;
    }

    public class QuestionLeaderboards
    {

        public readonly float correctThreshold = 0.75f;
        public readonly int minQuestionsAnswered = 3;

        public class StudentData
        {
            // % for correction
            // avg time per question
            // how many questions they have already
            public float correctPercentage;
            public float avgTimeSpent;
            public int questionsAnswered;

            public StudentData(float correctPercentage, float avgTimeSpent, int questionsAnswered)
            {
                this.correctPercentage = correctPercentage;
                this.avgTimeSpent = avgTimeSpent;
                this.questionsAnswered = questionsAnswered;
            }
        }

        public Dictionary<int, StudentData> studentQuestionDictionary;

        public QuestionLeaderboards()
        {
            studentQuestionDictionary = new Dictionary<int, StudentData>();
        }

        public void RecieveNextQuestion(int studentId, bool isCorrect, int timeSpent)
        {
            int isCorrectNumber = isCorrect ? 1 : 0;

            if (studentQuestionDictionary.ContainsKey(studentId))
            {
                StudentData studentData = studentQuestionDictionary[studentId];
                studentData.avgTimeSpent = (studentData.avgTimeSpent * studentData.questionsAnswered + timeSpent) / (studentData.questionsAnswered + 1);
                studentData.correctPercentage = (studentData.correctPercentage * studentData.questionsAnswered + isCorrectNumber) / (studentData.questionsAnswered + 1);
                studentData.questionsAnswered += 1;
            }
            else
            {
                studentQuestionDictionary[studentId] = new StudentData(isCorrectNumber, timeSpent, 1);
            }
        }

        public string GetStudentData(int studentId)
        {
            if (studentQuestionDictionary.ContainsKey(studentId))
            {
                StudentData studentData = studentQuestionDictionary[studentId];
                return $"Student Id: {studentId}. Avg Time Spend: {studentData.avgTimeSpent}. Correct Percentage Spend: {studentData.correctPercentage}. Questions Answered Spend: {studentData.questionsAnswered}.";
            }
            else
            {
                return "No Student";
            }
        }

        public List<int> GetFasestEligibleStudents()
        {
            List<int> eligibleStudents = studentQuestionDictionary.Where((studentData) => (studentData.Value.correctPercentage >= correctThreshold &&
                studentData.Value.questionsAnswered >= minQuestionsAnswered)).Select((studentData) => studentData.Key).ToList();

            return eligibleStudents;
        }
    }
}
