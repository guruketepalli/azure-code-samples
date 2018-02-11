using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomApplication
{
    public class ChatRoomManager
    {
        private string connectionString;
        private string topicName;
        private string userName;

        private NamespaceManager nameSpaceManager;
        private SubscriptionDescription userSubscription;
        private MessagingFactory messagingFactory;
        private TopicClient topicClient;
        private SubscriptionClient subscriptionClient;

        public ChatRoomManager(string connectionString, string topicName, string userName)
        {
            this.connectionString = connectionString;
            this.topicName = topicName;
            this.userName = userName;
        }

        public void InitiateChat()
        {
            // create namespace manager for all chat related acitivities
            nameSpaceManager = NamespaceManager.CreateFromConnectionString(this.connectionString);

            // Create chat topic if not exists
            if (!nameSpaceManager.TopicExists(this.topicName))
            {
                this.nameSpaceManager.CreateTopic(this.topicName);
            }

            // Create a user subscription for the user to receive messages. 
            this.userSubscription = new SubscriptionDescription(this.topicName, userName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };

            // Create the subscription for the user
            this.nameSpaceManager.CreateSubscription(this.userSubscription);

            // Create clients
            this.messagingFactory = MessagingFactory.CreateFromConnectionString(this.connectionString);
            this.topicClient = this.messagingFactory.CreateTopicClient(this.topicName);
            this.subscriptionClient = this.messagingFactory.CreateSubscriptionClient(this.topicName, this.userName);

            // Listen to messages and action when message arrives
            this.subscriptionClient.OnMessage(msg => ProcessMessage(msg));

            var welcomeMessage = new BrokeredMessage($"has entered the {this.topicName} chat room");
            welcomeMessage.Label = this.userName;
            this.topicClient.Send(welcomeMessage);

            while (true)
            {
                // Read Message from the user
                string userMessage = Console.ReadLine();
                if (userMessage.Equals("exit")) { break; }

                // Send message to topic for other subscribers to receive
                var userBrokeredMessage = new BrokeredMessage(userMessage);
                userBrokeredMessage.Label = this.userName;
                this.topicClient.Send(userBrokeredMessage);
            }

            var exitMessage = new BrokeredMessage($"has left the {this.topicName} chat room");
            exitMessage.Label = this.userName;
            this.topicClient.Send(exitMessage);
        }

        private void ProcessMessage(BrokeredMessage msg)
        {
            Console.WriteLine($"{msg.Label} > {msg.GetBody<string>()}");
        }
    }
}
