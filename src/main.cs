
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
                    var pathCommand = string
                    .Format(@"{0}{1}{2}",currentdir,Path.DirectorySeparatorChar,command);
                    // logger.LogInformation(pathCommand);
                    execFound = File.Exists(pathCommand);
                    logger.LogInformation($"{pathCommand} Exists :"+execFound);
                    if(execFound)
                    {
                        Console.WriteLine($"{command} is {pathCommand}");
                        break;
                    }        
                }
                if(!execFound)
                    Console.WriteLine($"{path}/{command}: not found");
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
