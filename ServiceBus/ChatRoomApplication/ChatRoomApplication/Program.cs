using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            string chatTopic = ConfigurationManager.AppSettings["ChatTopic"];

            Console.WriteLine("Welcome to Chat room application");
            Console.WriteLine("Invoke multiple instances of this application to see messages from differnt users");
            Console.WriteLine("Use exit for exiting this chat room application");
            Console.WriteLine();

            Console.WriteLine($"Enter your name to join the {chatTopic} chat room");
            string userName = Console.ReadLine();

            ChatRoomManager manager = new ChatRoomManager(connectionString, chatTopic, userName);

            manager.InitiateChat();
        }
    }
}
