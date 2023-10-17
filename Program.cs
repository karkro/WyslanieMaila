using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

namespace WyslanieMaila
{
    internal class Program
    {
        public class Msg
        { 
            public Msg(string email, string kwota) { 
                this.email = email;
                this.kwota = kwota;
            
            }

            public string email { get; set; }
            public string kwota { get; set; }
        }


        static List<Msg> ParseFile(string path = @"C:/.../repos/WyslanieMaila/WyslanieMaila/odbiorcy.csv")
        {
            List<Msg> result = new List<Msg>();

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] columns = lines[i].Split(';');
                    if (columns.Length == 2)
                    {                        
                        result.Add(new Msg(columns[0], columns[1]));
                    }
                }
            }
            else
                Console.WriteLine("Plik CSV nie istnieje.");

            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Console.ReadKey();

            var messages = ParseFile();

            foreach(var msg in messages)
            {
                SendEmail(msg.email, msg.kwota);
            }
       
        }

        // Funkcja do wysłania maila
        static void SendEmail(string recipientEmail, string kwota)
        {
            string smtpServer = "serverName";
            int smtpPort = 587;
            string smtpUsername = "userName";
            string smtpPassword = GetHiddenInput();
            string senderEmail = "emailAddress";

            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(senderEmail);
                mailMessage.Subject = "Zaległości";
                mailMessage.Body = $"Twoja spersonalizowana wiadomość. Kwota do zapłaty: {kwota}";

                mailMessage.To.Add(recipientEmail);

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine($"Wysłano e-mail do {recipientEmail}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wysyłania wiadomości e-maila do {recipientEmail}: {ex.Message}");
                }
            }
        }


        // Funkcja do ukrywania wpisywanych znaków (*)
        public static string GetHiddenInput()
        {
            string input = "";
            ConsoleKeyInfo keyInfo;
            Console.WriteLine("Podaj hasło do poczty: ");

            do
            {
                keyInfo = Console.ReadKey(true);

                // Jeśli użytkownik naciśnie Enter, zakończ wprowadzanie
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
                // Jeśli użytkownik naciśnie Backspace, usuń ostatni znak
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1);
                        Console.Write("\b \b"); // Usuń znak z konsoli
                    }
                }
                else
                {
                    input += keyInfo.KeyChar;
                    Console.Write("*"); // Wyświetl gwiazdkę zamiast wprowadzanego znaku
                }
            } while (true);
            Console.WriteLine();
            return input;
        }
    }
}
