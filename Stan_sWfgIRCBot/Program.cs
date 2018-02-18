using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Stan_sWfgIRCBot
{
    static class Program
    {
        static void Main(string[] args)
        {
            var ircBot = new IRCbot(
                server: "irc.quakenet.org",
                port: 6667,
                user: "USER IRCbot 0 * :IRCbot",
                nick: "WelcomeBOT",
                channel: "#0ad"
            );

            ircBot.Start();

            Console.ReadKey();
        }
    }

    internal class IRCbot
    {
        private readonly string _server;
        private readonly int _port;
        private readonly string _user;
        private readonly string _nick;
        private readonly string _channel;
        private static int _maxRetries = 5;
        private StreamWriter _writer;
        private StreamReader _reader;

        List<string> _greeted = new List<string>
        {
            "xanax`",
            "xanax",
            "xanax_"
        };

        readonly List<string> _salutes = new List<string>{
            "yo",
            "buenas",
            "tardes",
            "Bonjour",
            "bjr",
            "salut",
            "hey",
            "holã",
            "hola",
            "hello",
            "hi",
            "(hi",
            "hi)",
            "guten tag",
            "ciao",
            "olà",
            "oi",
            "hello\\",
            "namaste",
            "salaam",
            "hoi",
            "hallo"
            };

        readonly List<string> _helpersNames = new List<string>{
            "bb",
            "bb_",
            "bb1",
            "bb_afk",
            "bb_away",
            "bb___",
            "bb2",
            "Stan",
            "Stan_",
            "Stan`",
            "Itms",
            "elexis",
            "Vladislav",
            "Vladislav1"
            };

        public IRCbot(string server, int port, string user, string nick, string channel)
        {
            _server = server;
            _port = port;
            _user = user;
            _nick = nick;
            _channel = channel;
        }

        private void WriteMessage(string message)
        {
            _writer.WriteLine("PRIVMSG " + _channel + " :" + message);
        }

        private void SendPrivateMessage(string nick, string message)
        {
            _writer.WriteLine("PRIVMSG " + nick + " :" + message);
        }

        private List<string> GetUsers()
        {
            _writer.WriteLine("NAMES " + _channel);
            return _reader.ReadLine().Replace(":", "").Split(' ').ToList();
        }

        private void PingPeople(List<string> users)
        {
            var temp = users.FindAll(a => _helpersNames.Any(b => b.Equals(a)));
            var stringBuilder = new StringBuilder();
            if (temp != null && temp.Count > 0)
            {
                if (temp.Count != 1)
                    foreach (var user in temp)
                        stringBuilder.Append(user + ",");
                else
                    stringBuilder.Append(temp[0]);
            }
            WriteMessage(stringBuilder.ToString());
        }

        public async void Start()
        {
            _greeted = _greeted.Concat(_helpersNames).ToList();


            var retry = false;
            var retryCount = 0;
            do
            {
                try
                {
                    using (var irc = new TcpClient(AddressFamily.InterNetwork))
                    {
                        var ipAdressEntry = await Dns.GetHostEntryAsync(_server);
                        var ipAdress = ipAdressEntry.AddressList[1];
                        await irc.ConnectAsync(ipAdress, _port);

                        using (var stream = irc.GetStream())
                        using (_reader = new StreamReader(stream))
                        using (_writer = new StreamWriter(stream))
                        {
                            _writer.AutoFlush = true;
                            _writer.WriteLine("NICK " + _nick);
                            _writer.WriteLine(_user);

                            while (true)
                            {
                                string inputLine;
                                while ((inputLine = _reader.ReadLine()) != null)
                                {
                                    Console.WriteLine("<- " + inputLine);

                                    var splitInput = inputLine.Split(' ').ToList();

                                    if (splitInput.Any(a => a == "PING"))
                                        _writer.WriteLine("PONG " + splitInput[1]);

                                    if (splitInput.Any(a => a.Replace(":", "").Equals("#Ping")))
                                        WriteMessage("Pong");

                                    if (splitInput.Any(a => _salutes.Any(b => b.ToUpper().Equals(a.Replace(":", "").ToUpper()))) && !_greeted.Any(a => a.Equals(inputLine.Split('!')[0].Replace(":", ""))))
                                    {
                                        WriteMessage("Hello " + inputLine.Split('!')[0].Replace(":", "") + ". Please wait around few minutes for someone to answer.");
                                        SendPrivateMessage(inputLine.Split('!')[0].Replace(":", ""), "If you are looking to play with other people, try the multiplayer lobby. Please note you can only create one account per hour");
                                        SendPrivateMessage(inputLine.Split('!')[0].Replace(":", ""), "If your game crashes before starting games, try disabling GLSL and PostProcessing in the game options");
                                        SendPrivateMessage(inputLine.Split('!')[0].Replace(":", ""), "In the mean time you can look at https://wildfiregames.com/forum/index.php?/topic/15796-known-problems-please-read-before-posting/");
                                        SendPrivateMessage(inputLine.Split('!')[0].Replace(":", ""), "Also https://wildfiregames.com/forum/index.php?/topic/21541-please-help-us-create-an-faq-for-the-web-site/&tab=comments#comment-337781");
                                        PingPeople(GetUsers());
                                        _greeted.Add(inputLine.Split('!')[0].Replace(":", ""));
                                    }

                                    switch (splitInput[1])
                                    {
                                        case "001":
                                            _writer.WriteLine("JOIN " + _channel);
                                            _writer.Flush();
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    // shows the exception, sleeps for a little while and then tries to establish a new connection to the IRC server
                    Console.WriteLine(e.ToString());
                    Thread.Sleep(5000);
                    retry = ++retryCount <= _maxRetries;
                }
            } while (retry);
        }
    }
}
