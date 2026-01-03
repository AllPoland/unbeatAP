using System;
using System.IO;

namespace UNBEATAP.Helpers;

public static class SaveBackup
{
    public const string BackupFolderName = "APBackups";
    public static string ChaboPath => FileStorage.profile?.chaboPath;


    private static void BackupFile(string srcPath, string dstDir, int count)
    {
        string fileName = Path.GetFileName(srcPath);
        if(!File.Exists(srcPath))
        {
            Plugin.Logger.LogError($"Target backup file {fileName} doesn't exist!");
            return;
        }

        if(!Directory.Exists(dstDir))
        {
            Directory.CreateDirectory(dstDir);
        }

        for(int i = count - 1; i >= 0; i--)
        {
            byte[] fileContents;
            if(i == 0)
            {
                fileContents = File.ReadAllBytes(srcPath);
            }
            else
            {
                string prevBackupName = $"{fileName}.bak{i - 1}";
                string prevBackupPath = Path.Combine(dstDir, prevBackupName);
                if(!File.Exists(prevBackupPath))
                {
                    // This backup hasn't been made yet
                    continue;
                }

                fileContents = File.ReadAllBytes(prevBackupPath);
            }

            string newFileName = $"{fileName}.bak{i}";
            string newFilePath = Path.Combine(dstDir, newFileName);
            File.WriteAllBytes(newFilePath, fileContents);
        }
    }


    public static void BackupChallengeBoard(int backupCount)
    {
        try
        {
            if(string.IsNullOrEmpty(ChaboPath))
            {
                Plugin.Logger.LogError($"Attempted to backup challenge board without loaded profile!");
                return;
            }

            if(!Directory.Exists(ChaboPath))
            {
                Plugin.Logger.LogWarning($"Found no challenge board folder!");
                return;
            }

            string[] filesToBackup = Directory.GetFiles(ChaboPath, "*.json", SearchOption.TopDirectoryOnly);
            if(filesToBackup == null || filesToBackup.Length <= 0)
            {
                Plugin.Logger.LogWarning($"Found no .json files in challenge board folder!");
                return;
            }
            
            foreach(string fileName in filesToBackup)
            {
                BackupFile(
                    Path.Combine(ChaboPath, fileName),
                    Path.Combine(ChaboPath, BackupFolderName),
                    backupCount
                );
            }
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to save challenge board backups with error: {e.Message}, {e.StackTrace}");
        }
    }
}