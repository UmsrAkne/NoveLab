using System.Diagnostics;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public class GitLogExporter : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (projectRoot != null)
            {
                var buildFolder = Path.GetDirectoryName(report.summary.outputPath);
                var outputPath = Path.Combine(buildFolder!, "git_log.txt");

                var process = new Process();
                process.StartInfo.FileName = "git";
                process.StartInfo.Arguments = "log --pretty=format:\"%h %ad %s\" --date=format:\"%Y/%m/%d %H:%M\"";
                process.StartInfo.WorkingDirectory = projectRoot;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                File.WriteAllText(outputPath, result);
            }

            UnityEngine.Debug.Log("git log exported.");
        }
    }
}