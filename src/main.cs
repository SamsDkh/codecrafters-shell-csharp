class Program
{
    static void Main()
    {
        // TODO: Uncomment the code below to pass the first stage
        while (true)
        {
           Console.Write("$ ");
            var prompt = Console.ReadLine();
            Console.WriteLine($"{prompt}: command not found");
        }
    }
}
