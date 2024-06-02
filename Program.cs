using System;
using System.Net.Sockets;
using System.Text;

string HOST = "192.168.1.4";
int PORT = 50000;

try
{
    using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
    {
        sock.Connect(HOST, PORT);

        string msg = "{\"id\":\"2\",\"name\":\"Sorayya Asadi\",\"email\":\"sorayyaasadi6@gmail.com\"}";

        byte[] data = Encoding.UTF8.GetBytes(msg);

        sock.Send(data);

        Console.WriteLine("Data sent to logstash ...");

    }
}
catch (SocketException ex)
{
    Console.Error.WriteLine($"[ERROR] {ex.Message}");
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[ERROR] {ex.Message}");
    Environment.Exit(2);
}

Console.ReadLine();