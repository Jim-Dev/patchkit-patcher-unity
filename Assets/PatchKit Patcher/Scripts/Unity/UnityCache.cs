﻿using PatchKit.Patcher.AppData.Local;
using PatchKit.Patcher.Utilities;
using UnityEngine;

namespace PatchKit.Patcher.Unity
{
    class UnityCache : ICache
    {
        public void SetValue(string key, string value)
        {
            UnityDispatcher.Invoke(() => PlayerPrefs.SetString(key, value)).WaitOne();
        }

        public string GetValue(string key, string defaultValue = null)
        {
            string result = string.Empty;
            UnityDispatcher.Invoke(() => result = PlayerPrefs.GetString(key, defaultValue)).WaitOne();
            return result;
        }
    }
}