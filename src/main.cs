
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
                   var path = Environment.GetEnvironmentVariable("PATH");
           var command = string.Empty;        
        while (true)
        {
           Console.Write("$ ");
            var prompt = Console.ReadLine();
            var execFound = false;
            if(prompt.StartsWith("echo "))
            {
                Console.WriteLine($"{prompt.Substring(5)}");
                continue;
            }
            if(prompt.StartsWith("type "))
            {
                command = prompt.Substring(5);
                if(command == "echo" 
                || command == "type"
                || command == "exit")
                {
                    Console.WriteLine($"{prompt.Substring(5)} is a shell builtin");
                    continue;
                }

                if(string.IsNullOrEmpty(path))
                    Console.WriteLine($"{command}: not found");
                else
                {
                    var fileInfo = FindCommandIntoPath(command, path);
                    if(fileInfo != null)
                    {
                        execFound = IsExecutable(fileInfo.FullName);
                        if(execFound)
                            Console.WriteLine($"{command} is {fileInfo.FullName}");
                    }
                    if(!execFound)
                        Console.WriteLine($"{command}: not found");
                }
            }
            else
            {
                //Check if prompt contains space if yes split get every args
                //Check if prompt is a file and executable then try to execute it with args
                Console.WriteLine($"prompt : {prompt}");
                switch(prompt)
                {
                    case "exit 0":
                        return;
                    case "":
                        continue;
                    default:
                    {
                         var fileInfo = FindCommandIntoPath(command, path);
                        if(fileInfo != null)
                        {
                            execFound = IsExecutable(fileInfo.FullName);
                            if(execFound)
                                Console.WriteLine($"{prompt}");
                        }
                        if(!execFound)
                            Console.WriteLine($"{command}: not found");   
                    }
                    continue;
                }
            }
        }
    }

    static bool FindCommandAndCheckIfExecutable(string command)
    {
        return false;
    }

    static FileInfo? FindCommandIntoPath(string command, string path)
    {
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
                                return file;
                            }   
                        }
                    }
                    }
                } 
                
        return null;
    }

    static bool IsExecutable(string filePath)
    {
        if (!OperatingSystem.IsWindows())
        {
             var fileInfo = new FileInfo(filePath);
            var filePermissions = fileInfo.UnixFileMode;
            return (filePermissions & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0;
        }
      
            var executableExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
            { ".exe", ".bat", ".cmd", ".com" };
            var fileExtension = Path.GetExtension(filePath);
            return executableExtensions.Contains(fileExtension);
    }

    static void ExecuteCommand(string command, string[] args)
    {
        Console.WriteLine(command);
    }
}
