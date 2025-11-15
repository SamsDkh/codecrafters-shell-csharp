
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Collections.Generic;
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
                var hasExecutePermission = false;
                var Searched = false;
                HashSet<string> searched = [];
                var pathSeparator = Path.PathSeparator;
                var directorySeparator = Path.DirectorySeparatorChar;
                
                // logger.LogInformation("PATH :"+path);
                foreach (var dir in path.Split(pathSeparator))
                {
                    var subDir = string.Empty;
                    if(string.IsNullOrEmpty(dir))
                        break;
                    var currentdir = dir.TrimEnd(directorySeparator);
                    logger.LogInformation("Current DIR :"+currentdir);
                    DirectoryInfo di = new(currentdir);
                    logger.LogInformation("Directory Exists :"+di.Exists);
                    if(!di.Exists)
                        break;
                    FileInfo[] files = di.GetFiles("*"+command+"*");
                    logger.LogInformation("Files Length :"+files.Length);
                    if(files.Length == 0)
                        continue;
                    else
                    {
                        foreach(var file in files)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file.Name);
                        logger.LogInformation("File Name :"+fileName);
                        if(fileName.Equals(command, StringComparison.OrdinalIgnoreCase))
                        {
                            logger.LogInformation("Matched File Name :"+fileName);
                            var fileAttributes = file.Attributes;
                            logger.LogInformation("File Attributes :"+fileAttributes);
                            var executableExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".exe", ".bat", ".cmd", ".com" };
                            if(executableExtensions.Contains(file.Extension))
                            {
                                Console.WriteLine($"{command} is {Path.Combine(currentdir, fileName)}");
                                execFound = true;
                                break;
                            }
                        }
                        }
                    }
                    
                    if(execFound)
                        break;
                }
                if(!execFound)
                    Console.WriteLine($"{command}: not found");
                continue;
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
}
