class Program
{
    static void Main()
    {
        // TODO: Uncomment the code below to pass the first stage
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
                foreach (var dir in path.Split(pathSeparator))
                {
                    Console.WriteLine(dir);
                    var subDir = string.Empty;
                    if(string.IsNullOrEmpty(dir))
                        break;
                    foreach (var item in dir.Split(directorySeparator))
                    {
                        if(string.IsNullOrEmpty(item))
                            break;
                        // Console.WriteLine(item);
                        var currentItem = item.TrimEnd(directorySeparator);
                        subDir += string.Format(@"{0}{1}",currentItem,directorySeparator);
                        var pathCommand = string.Format(@"{0}{1}.exe",subDir,command);
                        Console.WriteLine("pathCommand :"+pathCommand);
                        // if(searched.Contains(pathCommand))
                        // {
                        //     Searched = true;
                        //     break;
                        // }
                        // searched.Add(pathCommand);
                        // Console.WriteLine(pathCommand);
                        execFound = File.Exists(pathCommand);
                        if(execFound)
                        {
                            Console.WriteLine($"{command} is {pathCommand}");
                            break;
                        }
                    }
                    if(execFound)
                        break;
                }
                if(!execFound)
                    Console.WriteLine($"{command}: not found");
                // Console.WriteLine("Searched in : ");
                // foreach (var s in searched)
                // {
                //     Console.WriteLine(s);
                // }
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
