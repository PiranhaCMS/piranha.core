using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// A C# console application that performs the following tasks:
/// 1. Recursively finds and deletes all 'bin' and 'obj' directories 
///    in the current working directory.
/// 2. Recursively discovers all solution (*.sln) files.
/// 3. Executes 'dotnet restore' for every discovered solution file.
/// </summary>
public class CleanupAndRestore
{
    // Removed the hardcoded SolutionFile constant

    public static void Main(string[] args)
    {
        string currentDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"Cleaning up bin and obj folders in directory: {currentDir}");
        Console.WriteLine("------------------------------------------");

        try
        {
            // --- Task 1: Find and Delete bin/obj folders ---
            
            // Use EnumerateDirectories to efficiently find all subdirectories.
            // ToList() is used here to execute the enumeration before we start modifying the file system.
            var foldersToClean = Directory.EnumerateDirectories(
                currentDir, 
                "*", 
                SearchOption.AllDirectories)
                .Where(d => Path.GetFileName(d).Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                            Path.GetFileName(d).Equals("obj", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!foldersToClean.Any())
            {
                Console.WriteLine("No 'bin' or 'obj' folders found to clean.");
            }
            else
            {
                foreach (var dir in foldersToClean)
                {
                    // Check if the directory still exists, as a parent deletion might have already removed it.
                    if (Directory.Exists(dir))
                    {
                        DeleteDirectory(dir);
                    }
                }
            }

            // --- Task 2: Discover and Run dotnet restore on all solution files ---
            
            // Discover all *.slnx files recursively. This achieves the effect of "**/*.slnx".
            var solutionFiles = Directory.EnumerateFiles(
                currentDir,
                "*.slnx",
                SearchOption.AllDirectories)
                .ToList();

            if (solutionFiles.Any())
            {
                Console.WriteLine($"\nFound {solutionFiles.Count} solution files. Starting restore process...");
                foreach (var slnFile in solutionFiles)
                {
                    RunDotnetRestore(slnFile);
                }
            }
            else
            {
                Console.WriteLine("\nNo solution (*.sln) files found to restore.");
            }

            Console.WriteLine("\nCleanup and Restore process complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Safely attempts to delete a directory and all its contents recursively.
    /// </summary>
    private static void DeleteDirectory(string targetDir)
    {
        try
        {
            Console.WriteLine($"Removing {targetDir}");
            // 'true' indicates recursive deletion (deleting contents and subdirectories)
            Directory.Delete(targetDir, true);
        }
        catch (IOException ex)
        {
            // Handle common IO issues (e.g., file locked)
            Console.WriteLine($"[WARNING] Failed to remove directory {targetDir} due to IO error: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"[WARNING] Failed to remove directory {targetDir} due to permission error: {ex.Message}");
        }
        catch (Exception ex)
        {
             Console.WriteLine($"[ERROR] Failed to remove directory {targetDir}: {ex.Message}");
        }
    }

    /// <summary>
    /// Executes the 'dotnet restore' command for the specified solution file.
    /// </summary>
    private static void RunDotnetRestore(string solutionFile)
    {
        // Use relative path for cleaner output, but use the full path in the command
        string relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), solutionFile);
        Console.WriteLine($"\n--- Running dotnet restore on {relativePath} ---");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            // The argument uses 'restore' command followed by the solution file path (quoted)
            Arguments = $"restore \"{solutionFile}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            if (process == null)
            {
                Console.WriteLine("ERROR: Could not start 'dotnet'. Is it installed and in your PATH?");
                return;
            }

            // Stream and log output in real-time
            process.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data); };
            process.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[DOTNET ERROR] {e.Data}"); };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"\nSuccessfully ran dotnet restore for {relativePath}.");
            }
            else
            {
                Console.WriteLine($"\n!!! dotnet restore FAILED for {relativePath} with exit code {process.ExitCode}.");
            }
        }
    }
}

