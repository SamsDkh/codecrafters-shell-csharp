class Program
{
    static void Main()
    {
        // TODO: Uncomment the code below to pass the first stage
        while (true)
        {
           Console.Write("$ ");
            var prompt = Console.ReadLine();
            if (string.IsNullOrEmpty(prompt))
            {
                continue;
            }
            else
            {
                if (prompt == "exit 0")
                    break;
                Console.WriteLine($"{prompt}: command not found");
            }
        }
    }
}
