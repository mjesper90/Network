using ExampleGameTicTacToe;
using Network;
using System.Net;

Logger.PrintAllLoggsImmediately = true;

Console.WriteLine("Host/Play");
while (true)
{
    string input = Console.ReadLine()!.ToLower();
    if (input == "host")
    {
        Console.Clear();
        new TicTacToeServer().Start();
    }
    else if (input == "play")
    {
        Console.Clear();

        Console.WriteLine("Write in IP");
        IPEndPoint ip;
        var client = new TicTacToeClient();

        while (true)
        {
            input = Console.ReadLine()!.ToLower();

            if (IPEndPoint.TryParse(input, out var res))
            {
                ip = res;
                break;
            }
            Console.WriteLine("Invalid IP");
        }

        client.Start(ip);
    }
    else if (input == "exit" || input == "stop" || input == "e")
    {
        return;
    }
}
