
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Collections.Generic;
using System.Text;
class Program
{
    static void Main()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole();
        });
        ILogger logger = loggerFactory.CreateLogger<Program>();
       
        while (true)
        {
           Console.Write("$ ");
            var prompt = Console.ReadLine();
            if(prompt.StartsWith("echo "))
            {
                Console.WriteLine($"{prompt.Substring(5)}");
                continue;
            }
            if(prompt.StartsWith("type "))
            {
                var command = prompt.Substring(5);
                if(command == "echo" 
                || command == "type"
                || command == "exit")
                {
                    Console.WriteLine($"{prompt.Substring(5)} is a shell builtin");
                    continue;
                }
                var path = Environment.GetEnvironmentVariable("PATH");
                if(string.IsNullOrEmpty(path))
                {
                    Console.WriteLine($"{command}: not found");
                    continue;
                }
                var execFound = false;
                HashSet<string> searched = [];
                var pathSeparator = Path.PathSeparator;
                var directorySeparator = Path.DirectorySeparatorChar;
                foreach (var dir in path.Split(pathSeparator))
                {
                    if(string.IsNullOrEmpty(dir))
                        break;
                    var currentdir = dir.TrimEnd(directorySeparator);
                    DirectoryInfo di = new(currentdir);
                    if(di.Exists)
                    {
                        FileInfo[] files = di.GetFiles("*"+command+"*");
                    if(files.Length == 0)
                        continue;
                    else
                    {
                        foreach(var file in files)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file.Name);
                            if(fileName == command && IsExecutable(file.FullName))
                            {
                                if(!searched.Contains(file.FullName))
                                {
                                    Console.WriteLine(file.FullName);
                                    searched.Add(file.FullName);
                                    execFound = true;
                                }
                            }
                        }
                    }
                    }
                    if(execFound)
                        break;
                }
                if(!execFound)
                    Console.WriteLine($"{command}: not found");
            }
            else
            {
                switch(prompt)
                {
                    case "exit 0":
                        return;
                    case "":
                        continue;
                    default:
                        Console.WriteLine($"{prompt}: command not found");
                        continue;
                }
            }
        }
    }

    static bool FindAnExecutableIntoPath(string command)
    {
        return false;
    }

    static bool IsExecutable(string filePath)
    {
        if (OperatingSystem.IsWindows())
        {
            var executableExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".exe", ".bat", ".cmd", ".com" };
            var fileExtension = Path.GetExtension(filePath);
            return executableExtensions.Contains(fileExtension);
        }
        else
        {
            var fileInfo = new FileInfo(filePath);
            var filePermissions = fileInfo.UnixFileMode;
            return (filePermissions & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0;
        }
    }

    static void ExecuteCommand(string command, string[] args)
    {
        
    }
}
