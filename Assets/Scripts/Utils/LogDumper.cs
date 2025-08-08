using System;
using System.IO;
using UnityEngine;

namespace Utils
{
    public class LogDumper : MonoBehaviour
    {
        private readonly string logPath = new FileInfo("debug_log.txt").FullName;

        private void Start()
        {
            Log("シーン開始。ここからログ出力を開始します。");
        }

        public void Log(string message)
        {
            Debug.Log(message);
            File.AppendAllText(logPath, $"{DateTime.Now:HH:mm:ss} {message}\n");
        }
    }
}