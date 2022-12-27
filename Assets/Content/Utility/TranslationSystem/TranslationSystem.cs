using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;

public static class TranslationSystem
{
    public static string GetText(string table, string key, object[] args = null)
    {
        AsyncOperationHandle<string> operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, key, args);
        operation.WaitForCompletion();
        return operation.Result;
    }
}
