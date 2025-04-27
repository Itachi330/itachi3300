using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    private static DiscordSocketClient _client;
    private static List<string> truths = new List<string>
    {
        "What's your biggest fear?",
        "Who was your first crush?",
        "Have you ever lied to your best friend?",
        "What's your most embarrassing moment?",
        "If you could have any superpower, what would it be?"
    };

    private static List<string> dares = new List<string>
    {
        "Sing a song in voice chat!",
        "Do 20 push-ups right now!",
        "Send a funny selfie!",
        "Speak in an accent for the next 10 minutes!",
        "Write a poem about another player!"
    };

    static async Task Main(string[] args)
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.SlashCommandExecuted += SlashCommandHandler;
        _client.ButtonExecuted += ButtonHandler;

        string token = "MTM2NTk1NjYyODY2NjUxOTYzMg.GFf0Nf.CnRwpNrQm9tSanZEoc1khX5X9NJOaGOPzZM3cc"; // <-- Bot টোকেন এখানে বসাও

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private static async Task ReadyAsync()
    {
        var guild = _client.GetGuild(1363943006943707389); // <-- গিল্ড ID বসাও

        if (guild == null)
        {
            Console.WriteLine("Guild not found!");
            return;
        }

        // প্রথমে আগের সব কমান্ড মুছে ফেলো
        var existingCommands = await guild.GetApplicationCommandsAsync();
        foreach (var cmd in existingCommands)
        {
            await cmd.DeleteAsync();
        }

        // এখন শুধু একটাই কমান্ড রাখবো: /play
        var playCommand = new SlashCommandBuilder()
            .WithName("play")
            .WithDescription("Start Truth or Dare Game!")
            .Build();

        try
        {
            await guild.CreateApplicationCommandAsync(playCommand);
            Console.WriteLine("Slash Command /play registered!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering /play: {ex.Message}");
        }
    }

    private static async Task SlashCommandHandler(SocketSlashCommand command)
    {
        if (command.Data.Name == "play")
        {
            var embed = new EmbedBuilder()
                .WithTitle("🎉 Welcome to the Ultimate Truth or Dare! 🎉")
                .WithDescription("Let's make memories together! ✨\n\nChoose wisely below and let the fun begin! 🚀")
                .WithThumbnailUrl("https://i.imgur.com/AfFp7pu.png") // সুন্দর থাম্বনেইল ইমেজ
                .WithColor(new Color(255, 215, 0)) // গোল্ড কালার
                .WithFooter(footer =>
                {
                    footer.Text = "Powered by ITACHI UCHIHA 🖤 | Stay Legendary!";
                    footer.IconUrl = "https://cdn-icons-png.flaticon.com/512/732/732228.png";
                })
                .Build();

            var builder = new ComponentBuilder()
                .WithButton("🧠 Truth", "btn_truth", ButtonStyle.Primary, emote: new Emoji("🧠"))
                .WithButton("🔥 Dare", "btn_dare", ButtonStyle.Danger, emote: new Emoji("🔥"))
                .WithButton("🎲 Random", "btn_random", ButtonStyle.Success, emote: new Emoji("🎲"));

            await command.RespondAsync(embed: embed, components: builder.Build());
        }
    }


    private static async Task ButtonHandler(SocketMessageComponent component)
    {
        var random = new Random();

        if (component.Data.CustomId == "btn_truth")
        {
            var truth = truths[random.Next(truths.Count)];

            var embed = new EmbedBuilder()
                .WithTitle("🧠 Time for the Truth!")
                .WithDescription($"**🔵 Question:** {truth}\n\n_Answer it honestly!_ 🌟")
                .WithColor(Color.Blue)
                .WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/2922/2922522.png")
                .WithFooter(footer => footer.Text = "Be real, be honest! 💬")
                .Build();

            await component.RespondAsync(embed: embed);
        }
        else if (component.Data.CustomId == "btn_dare")
        {
            var dare = dares[random.Next(dares.Count)];

            var embed = new EmbedBuilder()
                .WithTitle("🔥 Dare Accepted!")
                .WithDescription($"**🔴 Challenge:** {dare}\n\n_Show your courage!_ 💪")
                .WithColor(Color.Red)
                .WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/2983/2983616.png")
                .WithFooter(footer => footer.Text = "Be bold, be brave! ⚡")
                .Build();

            await component.RespondAsync(embed: embed);
        }
        else if (component.Data.CustomId == "btn_random")
        {
            bool chooseTruth = random.Next(2) == 0;

            if (chooseTruth)
            {
                var truth = truths[random.Next(truths.Count)];

                var embed = new EmbedBuilder()
                    .WithTitle("🎲 Random: It's Truth!")
                    .WithDescription($"**🌀 Question:** {truth}\n\n_Good luck!_ 🍀")
                    .WithColor(Color.Purple)
                    .WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/1041/1041916.png")
                    .WithFooter(footer => footer.Text = "Fate chose you! 🎯")
                    .Build();

                await component.RespondAsync(embed: embed);
            }
            else
            {
                var dare = dares[random.Next(dares.Count)];

                var embed = new EmbedBuilder()
                    .WithTitle("🎲 Random: It's Dare!")
                    .WithDescription($"**🌀 Challenge:** {dare}\n\n_Are you ready?_ 🔥")
                    .WithColor(Color.Orange)
                    .WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/2569/2569594.png")
                    .WithFooter(footer => footer.Text = "Face the challenge! 💥")
                    .Build();

                await component.RespondAsync(embed: embed);
            }
        }
    }
}
