using System;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Diagnostics;

class Program
{
    public static string valid = "", invalid = "";

    static void Main(string[] args)
    {
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

        Console.WriteLine(@"

  _   _  ______      __     _____ _    _ ______ _____ _  ________ _____  
 | \ | |/ __ \ \    / /\   / ____| |  | |  ____/ ____| |/ /  ____|  __ \ 
 |  \| | |  | \ \  / /  \ | |    | |__| | |__ | |    | ' /| |__  | |__) |
 | . ` | |  | |\ \/ / /\ \| |    |  __  |  __|| |    |  < |  __| |  _  / 
 | |\  | |__| | \  / ____ \ |____| |  | | |___| |____| . \| |____| | \ \ 
 |_| \_|\____/   \/_/    \_\_____|_|  |_|______\_____|_|\_\______|_|  \_\
                                                                         
                                                                         

");
        Console.WriteLine("[+] Welcome to NovaChecker, the best, reliable, faster token checker for Discord.");
        Console.WriteLine("[+] For more informations contact me on Discord: ZioEren#1337");
        Console.WriteLine("[+] You can also join our Discord server: https://discord.gg/evESnCfskv\r\n");

        string allString = "";

        foreach (string arg in args)
        {
            if (allString == "")
            {
                allString = arg;
            }
            else
            {
                allString += " " + arg;
            }
        }

        if (allString.StartsWith("\""))
        {
            allString = allString.Substring(1, allString.Length - 1);
        }

        if (allString.EndsWith("\""))
        {
            allString = allString.Substring(0, allString.Length - 1);
        }

        string theFile = "";

        if (allString != "")
        {
            if (!System.IO.File.Exists(allString))
            {
                Console.WriteLine("[-] The specified file does not exist!");
                Console.WriteLine("[+] Please, insert the file directory here:");

                do
                {
                    theFile = Console.ReadLine();

                    if (theFile.StartsWith("\""))
                    {
                        theFile = theFile.Substring(1, theFile.Length - 1);
                    }

                    if (theFile.EndsWith("\""))
                    {
                        theFile = theFile.Substring(0, theFile.Length - 1);
                    }

                    if (!System.IO.File.Exists(theFile))
                    {
                        Console.WriteLine("[-] This file does not exist! Please, try again.");
                    }
                }
                while (!System.IO.File.Exists(theFile));
            }
        }
        else
        {
            Console.WriteLine("[-] The specified file does not exist!");
            Console.WriteLine("[+] Please, insert the file directory here:");

            do
            {
                theFile = Console.ReadLine();

                if (theFile.StartsWith("\""))
                {
                    theFile = theFile.Substring(1, theFile.Length - 1);
                }

                if (theFile.EndsWith("\""))
                {
                    theFile = theFile.Substring(0, theFile.Length - 1);
                }

                if (!System.IO.File.Exists(theFile))
                {
                    Console.WriteLine("[-] This file does not exist! Please, try again.");
                }
            }
            while (!System.IO.File.Exists(theFile));
        }

        Console.WriteLine("[+] Checking your tokens list...\r\n[+] Nextly, if you want to save the tokens after finish, press 'ENTER' to close the program and save.\r\n");

        new Thread(() => doTokenChecker(theFile)).Start();
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static void doTokenChecker(string theFile)
    {
        foreach (string token in System.IO.File.ReadAllLines(theFile))
        {
            new Thread(() => checkToken(token.Trim().Replace(" ", "").Replace('\t'.ToString(), "").Replace('\n'.ToString(), "").Replace('\r'.ToString(), "").Replace(Environment.NewLine, ""))).Start();
        }

        Console.ReadLine();

        System.IO.File.WriteAllText("valid.txt", valid);
        System.IO.File.WriteAllText("invalid.txt", invalid);
    }

    public static void checkToken(string token)
    {
        if ((token.Length != 59 && token.Length != 88) || (token == ""))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid: " + token);

            return;
        }

        if (token.Length == 59)
        {
            try
            {
                string firstPart = token.Substring(0, 24);
                string decoded = Base64Decode(firstPart);

                if (!Microsoft.VisualBasic.Information.IsNumeric(decoded) || decoded.Length != 18)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid: " + token);

                    if (invalid == "")
                    {
                        invalid = token;
                    }
                    else
                    {
                        invalid += "\r\n" + token;
                    }

                    return;
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid: " + token);

                if (invalid == "")
                {
                    invalid = token;
                }
                else
                {
                    invalid += "\r\n" + token;
                }

                return;
            }
        }

        var http = new HttpClient();

        try
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://discord.com/api/v8/users/@me/library"),
                Method = HttpMethod.Get,
                Headers = { { HttpRequestHeader.Authorization.ToString(), token }, { HttpRequestHeader.ContentType.ToString(), "multipart/mixed" }, },
            };

            if (http.SendAsync(request).Result.StatusCode.Equals(HttpStatusCode.OK))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Valid: " + token);

                if (valid == "")
                {
                    valid = token;
                }
                else
                {
                    valid += "\r\n" + token;
                }

                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid: " + token);

                if (invalid == "")
                {
                    invalid = token;
                }
                else
                {
                    invalid += "\r\n" + token;
                }

                return;
            }
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid: " + token);

            if (invalid == "")
            {
                invalid = token;
            }
            else
            {
                invalid += "\r\n" + token;
            }

            return;
        }
    }
}