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
                || command == "type")
                {
                    Console.WriteLine($"{prompt.Substring(5)} is a shell builtin");
                    continue;
                }
                Console.WriteLine("invalid_command: not found");
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
