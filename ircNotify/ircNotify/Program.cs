using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace ircNotify
{
    class Program
    {
        public static string SERVER;
        public static int PORT = 6667;
        public static string CHANNEL;
        static Random rand = new Random();
        static int randomNumber = rand.Next(1, 99999);
        public static string NICK = "ircNotify" + randomNumber;
        private static string USER = "USER ircNotfy 0 * :ircNotify";
        static StreamWriter writer;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter server: ");
            SERVER = Console.ReadLine();
            Console.WriteLine("Enter port: ");
            try
            {
                PORT = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("That's not a number...");
            }
            Console.WriteLine("Enter channel: ");
            CHANNEL = Console.ReadLine();
            Console.WriteLine("Is this information correct? (Y/N):\nServer: " + SERVER + "\nPort: " + PORT + "\nChannel: " + CHANNEL);
            string infoCorrect = Console.ReadLine().ToLower();
            if (infoCorrect == "y")
            {
                Console.WriteLine("Connecting...");
                sysBOT();
            }
            else if (infoCorrect == "n")
            {
                Console.WriteLine("Please restart the program and re-enter the correct information");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("That's not an option...");
            }
        }
        static void playPos()
        {
            var playPos = new System.Media.SoundPlayer();
            playPos.Stream = Properties.Resources.notifyPos;
            playPos.Play();
        }
        static void playNeg()
        {
            var playNeg = new System.Media.SoundPlayer();
            playNeg.Stream = Properties.Resources.notifyNeg;
            playNeg.Play();
        }
        static void playMsg()
        {
            var playMsg = new System.Media.SoundPlayer();
            playMsg.Stream = Properties.Resources.notifyMsg;
            playMsg.Play();
        }
        static void sysBOT()
        {
            NetworkStream stream;
            TcpClient irc;
            string inputLine;
            StreamReader reader;

            try
            {

                irc = new TcpClient(SERVER, PORT);
                stream = irc.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                writer.WriteLine("NICK " + NICK);
                writer.Flush();
                writer.WriteLine(USER);
                writer.Flush();
                while (true)
                {
                Start:
                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        //Console.WriteLine("<-" + inputLine);
                        string[] splitInput = inputLine.Split(new Char[] { ' ' });
                        string output = inputLine.Substring(inputLine.IndexOf("&") + 1);
                        string[] str = output.Split(' ');

                        if (splitInput[0] == "PING")
                        {
                            string PongReply = splitInput[1];
                            writer.WriteLine("PONG " + PongReply);
                            writer.Flush();
                            continue;
                        }

                        switch (splitInput[1])
                        {
                            case "001":
                                string JoinString = "JOIN " + CHANNEL;
                                writer.WriteLine(JoinString);
                                writer.Flush();
                                Console.WriteLine("Connected");
                                break;
                            default:
                                break;
                        }
                        if (output != null)
                        {
                            if (inputLine.Contains("JOIN"))
                            {
                                string commandSender = inputLine.Split(new char[] { ':', '!' })[1];
                                if (commandSender == NICK)
                                {
                                    goto Start;
                                }
                                else
                                {
                                    //Begin Console Writing
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(commandSender + " has joined!");
                                    Console.ResetColor();
                                    //End Console Writing
                                    playPos();
                                }
                            }
                            else if (inputLine.Contains("PART"))
                            {
                                string commandSender = inputLine.Split(new char[] { ':', '!' })[1];
                                //Begin Console Writing
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(commandSender + " has left :<");
                                Console.ResetColor();
                                //End Console Writing
                                playNeg();
                            }
                            else if (inputLine.Contains("PRIVMSG"))
                            {
                                string commandSender = inputLine.Split(new char[] { ':', '!' })[1];
                                if (commandSender.Contains(NICK))
                                {
                                    goto Start;
                                }
                                else
                                {
                                    //Begin Console Writing
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine(commandSender + " has sent a message!");
                                    Console.ResetColor();
                                    //End Console Writing
                                    playMsg();
                                }
                            }
                        }
                    }
                    writer.Close();
                    reader.Close();
                    irc.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Thread.Sleep(5000);
                string[] argv = { };

            }
        }
    }
}
