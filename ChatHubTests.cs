using System;
using System.Net.Http;
using System.Threading.Tasks;
using ChatHub.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace ChatHub.IntegrationTests
{
    public class ChatHubTests : IClassFixture<WebApiFactory>
    {
        private readonly WebApiFactory _factory;

        public ChatHubTests(WebApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task sendMessage_should_send_the_user_and_message_to_all_clients()
        {
            // Arrange
            _factory.CreateClient(); // need to create a client for the server property to be available
            var server = _factory.Server;
            
            var connection = await StartConnectionAsync(server.CreateHandler(), "chat");

            connection.Closed += async error =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            // Act
            string user = null;
            string message = null;
            connection.On<string,string>("OnReceiveMessage", (u, m) =>
            {
                user = u;
                message = m;
            });
            //await connection.InvokeCoreAsync("SendMessage", new object[] { "super_user", "Hello World!!" });
            await connection.InvokeAsync("SendMessage", "super_user", "Hello World!!");

            //Assert
            user.Should().Be("super_user");
            message.Should().Be("Hello World!!");
        }

        private static async Task<HubConnection> StartConnectionAsync(HttpMessageHandler handler, string hubName)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"ws://localhost/hubs/{hubName}", o =>
                {
                    o.HttpMessageHandlerFactory = _ => handler;
                })
                .Build();

            await hubConnection.StartAsync();

            return hubConnection;
        }
    }
}