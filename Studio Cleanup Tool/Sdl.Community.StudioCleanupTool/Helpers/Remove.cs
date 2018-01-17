﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class Remove
    {
	    private static string _backupFolderPath =
		    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
	    public static async Task BackupFiles(List<StudioDetails> foldersToBackup)
	    {
		    await Task.Run(() => CreateBackupFolder(foldersToBackup));
	    }
	    public static async Task RestoreBackupFiles(List<StudioDetails> foldersToBackup)
	    {
		    await Task.Run(() => RestoreFiles(foldersToBackup));
	    }

	    private static void RestoreFiles(List<StudioDetails> foldersToBackup)
	    {
			foreach (var folder in foldersToBackup)
			{
				//creates original folders if doesn't exist
				if (!Directory.Exists(folder.OriginalFilePath))
				{
					Directory.CreateDirectory(folder.OriginalFilePath);
				}
				try
				{
					//Get files  from backup
					var files = Directory.GetFiles(folder.BackupFilePath);
					if (files.Length > 0)
					{
						MoveToBackUp(files, folder.OriginalFilePath);
					}

					var subdirectories = Directory.GetDirectories(folder.BackupFilePath);
					foreach (var subdirectory in subdirectories)
					{
						var currentDirInfo = new DirectoryInfo(subdirectory);
						CheckForSubfolders(currentDirInfo, folder.OriginalFilePath);
					}
				}
				catch (Exception e)
				{
					//throw e;
				}
			}
		}

	    public static async Task FromSelectedLocations(List<StudioDetails> foldersToRemove)
	    {
		    try
		    {
			    foreach (var folder in foldersToRemove)
			    {
				    var directory = await Task.FromResult(IsDirectory(folder.OriginalFilePath));

				    if (!directory)
				    {
					    File.Delete(folder.OriginalFilePath);
				    }
				    else
				    {
						var directoryInfo = new DirectoryInfo(folder.OriginalFilePath);
					    await Task.Run(()=>Empty(directoryInfo));
				    }
			    }
		    }
		    catch (Exception e)
		    {
			    
		    }
	    }

	    private static void CreateBackupFolder(List<StudioDetails> foldersToBackup)
	    {
			    foreach (var folder in foldersToBackup)
			    {
				    if (!Directory.Exists(folder.BackupFilePath))
				    {
					    Directory.CreateDirectory(folder.BackupFilePath);
				    }
				    try
				    {
					    if (Directory.Exists(folder.OriginalFilePath))
					    {
						    //Get files 
						    var files = Directory.GetFiles(folder.OriginalFilePath);
						    if (files.Length > 0)
						    {
							    MoveToBackUp(files, folder.BackupFilePath);
						    }

						    //check for subdirectories
						    var subdirectories = Directory.GetDirectories(folder.OriginalFilePath);
						    foreach (var subdirectory in subdirectories)
						    {
							    var currentDirInfo = new DirectoryInfo(subdirectory);
							    CheckForSubfolders(currentDirInfo, folder.BackupFilePath);
						    }
					    }
				    }
				    catch (Exception e)
				    {
					    throw e;
				    }
			    }
	    }

	    private static void CheckForSubfolders(DirectoryInfo currentDirInfo, string backupFolderRoot)
	    {
		    var subdirectories = currentDirInfo.GetDirectories();
		    if (currentDirInfo.Parent != null)
		    {
			    var pathToCorespondingBackupFolder = Path.Combine(backupFolderRoot, currentDirInfo.Name);
			    var subdirectoryFiles = Directory.GetFiles(currentDirInfo.FullName);
			    if (subdirectoryFiles.Length > 0)
			    {
				    if (!Directory.Exists(pathToCorespondingBackupFolder))
				    {
					    Directory.CreateDirectory(pathToCorespondingBackupFolder);
				    }
				    MoveToBackUp(subdirectoryFiles, pathToCorespondingBackupFolder);
			    }
			    if (subdirectories.Length > 0)
			    {
					foreach (var subdirectory in subdirectories)
					{
						CheckForSubfolders(subdirectory, pathToCorespondingBackupFolder);
					}
				}
			}
	    }

	    private static void MoveToBackUp(string[] files, string backupPath)
	    {
		    foreach (var file in files)
		    {
			    var fileName = Path.GetFileName(file);
			    try
			    {
				    if (fileName != null)
				    {
					    File.Copy(file, Path.Combine(backupPath, fileName), true);
				    }
			    }
			    catch (Exception e)
			    {
				    throw e;
			    }
		    }
	    }
	    private static void Empty(DirectoryInfo directoryInfo)
	    {
		    if (directoryInfo.Exists)
		    {
			    SetAttributesNormal(directoryInfo);

				//removes all files from root directory
			    //foreach (var file in directoryInfo.GetFiles())
			    //{
				   // file.Delete();
			    //}

			    ////removes all the directories from root directory
			    //foreach (var directory in directoryInfo.GetDirectories())
			    //{
				   // directory.Delete(true);
			    //}
			    //remove the root directory
			    directoryInfo.Delete(true);
			}
			
		}

	    private static void SetAttributesNormal(DirectoryInfo directory)
	    {
			foreach (var subDir in directory.GetDirectories())
				SetAttributesNormal(subDir);
		    foreach (var file in directory.GetFiles())
		    {
			    file.Attributes = FileAttributes.Normal;
		    }
		}

	    private static bool IsDirectory(string path)
	    {
		    var attributes = File.GetAttributes(path);
		    return attributes.HasFlag(FileAttributes.Directory);
	    }
    }
}
