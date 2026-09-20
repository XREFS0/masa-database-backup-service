using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BackupProgram
{
    class Program
    {

        static void Main(string[] args)
        {

            Program.DatabaseBackup("C:/wamp/bin/mysql/mysql5.5.20/bin/mysqldump.exe", "database_name");

        }

        public static void DatabaseBackup(string ExeLocation, string DBName)
        {
            try
            {
                string dateBackedup = "";
                String dateStarted = DateTime.Now.ToString();
                dateBackedup = DateTime.Now.ToString("yyyy/dd/M--HH.mm.ss") + "-" + DBName + ".sql";
                string fileName = dateBackedup.Replace("/", "-");
                string directoryPath = "E:/Backups/";
                string directoryPath1 = "E:/Backups/" + fileName;
                System.IO.Directory.CreateDirectory(directoryPath);
                fileName = directoryPath + "/" + fileName;
                StreamWriter file = new StreamWriter(fileName);
                ProcessStartInfo proc = new ProcessStartInfo();
                string cmd = string.Format(@"-u{0} -p{1} -h{2} {3}", "root", "yourpassword", "localhost", DBName);
                 
                proc.FileName = ExeLocation;
                proc.RedirectStandardInput = false;
                proc.RedirectStandardOutput = true;
                proc.Arguments = cmd;
                proc.UseShellExecute = false;
                Process p = Process.Start(proc);
                string res;
                res = p.StandardOutput.ReadToEnd();
                file.WriteLine(res);
                p.WaitForExit();
                file.Close();
                string dateEnd = DateTime.Now.ToString();
                sendMail(fileName, directoryPath1, dateStarted, dateEnd);
                Console.WriteLine("Backup Completed");

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message.ToString());

            }
        }
         
         
         
         


        public static void sendMail(string fileName, string directoryPath1, string dateStarted, string dateEnd)
        {
            try
            {
                MailMessage mm = new MailMessage("senderemail", "recipient");
                mm.Subject = "subject";
                mm.Body = "Database" + DateTime.Now.ToShortDateString() + " has been Backed up successfully." + "<br />" + "Time Started: \n" + dateStarted + "<br /> " + "<i>" + " Ended at " + dateEnd + "<i>" + "<br />" + "File Directory " + directoryPath1 + "<br />" + "Attached is the Backup file.";
                mm.IsBodyHtml = true;
                mm.Attachments.Add(new Attachment(fileName));
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = "senderemail";
                NetworkCred.Password = "senderpassword";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Could not send the Email Reason /n" + e.ToString());
            }

        }
    }
}