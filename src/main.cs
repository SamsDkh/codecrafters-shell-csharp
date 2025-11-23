
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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
                switch(prompt)
                {
                    case "exit 0":
                        return;
                    case "":
                        continue;
                    default:
                    {
                        var cmd = ExtractCommandFromPrompt(prompt.TrimStart());
                        // Console.WriteLine($"cmd :k{cmd}");
                        var fileInfo = FindCommandIntoPath(cmd, path);
                        if(fileInfo != null)
                        {
                            execFound = IsExecutable(fileInfo.FullName);
                            if(execFound)
                            {
                                var args = prompt.Substring(cmd.Length, prompt.Length-cmd.Length).TrimStart();
                                if(args.Length > 0)
                                {
                                    // Console.WriteLine($"Executing {fileInfo.Name} with args {args}");
                                     ProcessStartInfo startInfo = new()
                                     {
                                       FileName =  fileInfo.Name,
                                       Arguments =  args,
                                       RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                       UseShellExecute = false,
                                       CreateNoWindow = true
                                     };      
                                    var process = new Process { StartInfo = startInfo };
                                    process.Start();

                                    // Print stdout
                                    Console.Write(process.StandardOutput.ReadToEnd());
                                    Console.Error.Write(process.StandardError.ReadToEnd());

                                    process.WaitForExit();                                }
                                }
                                
                        }
                        if(fileInfo == null || !execFound)
                            Console.WriteLine($"{cmd}: not found");   
                    }
                    continue;
                }
            }
        }
    }

    static string ExtractCommandFromPrompt(string prompt)
    {
        if(string.IsNullOrEmpty(prompt))
            return prompt;
        string command = string.Empty;
        foreach(char c in prompt)
        {
            if(char.IsWhiteSpace(c))
                break;
            command += c;
        }
        // Console.WriteLine($"Extracted command : {command}");
        return command.TrimEnd();
    }

    static List<string> ExctractArgsFromPrompt(string prompt)
    {
        var promptStartTrimmed = prompt.TrimStart();
        Console.WriteLine($"ExctractArgsFromPrompt : {promptStartTrimmed}");
        if(string.IsNullOrEmpty(promptStartTrimmed))
            return [];
        var promptLength = promptStartTrimmed.Length;
        var promptLengthNoSpace = promptStartTrimmed.Trim().Length;
        List<string> args = [];
        var currentStr = "";
        int argsIndex = 0;
        for(int i = 0; i<promptLength;i++)
        {
            currentStr += promptStartTrimmed[i];
            Console.WriteLine($"current string : {currentStr}");
            if(char.IsWhiteSpace(promptStartTrimmed[i]) || i == promptLength-1)
            {
                args.Add(currentStr.Trim());
                Console.WriteLine($"current string {args[argsIndex]} to args at index {argsIndex}");
                currentStr = "";
                argsIndex++;
            }
        }

        foreach(string arg in args)
        {
            Console.WriteLine($"Extracted arg : {arg}");
        }
        return args;
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
