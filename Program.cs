using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace minification
{
    class Program
    {
        static string LerAte(System.IO.StreamReader sr, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            int i, fim,x;
            i = 0;
            fim = delimiter.Length;
            while (i < fim && (x = sr.Read()) > -1)
            {
                if (delimiter[i] == (char)x)
                    i++;
                else
                    i = 0;
                sb.Append((char)x);
            }
            sb.Remove(sb.Length - fim,fim);
            return sb.ToString().Trim();
        }

        static string convertIso(string Message)
        {
            Encoding iso = Encoding.Default;//Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(Message);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            string msg= iso.GetString(isoBytes);
            return msg;
        }
        static string convertToUTF8(string Message)
        {
            Encoding iso = Encoding.Default;//Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] isoBytes = iso.GetBytes(Message);
            byte[] utfBytes = Encoding.Convert(iso,utf8,isoBytes);
            string msg = utf8.GetString(utfBytes);
            return msg;
        }

        static void converteFile(string name)
        {
            string s;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(name, Encoding.Default))
                s = sr.ReadToEnd();
            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(name, false, Encoding.UTF8))
                sr.Write(convertToUTF8(s));
        }

        static void minificarJS(string file)
        {
            file = System.IO.Path.GetFullPath(file);
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            //info.Arguments = @"terser --compress --mangle -- " + System.IO.Path.GetFileName(file);
            //converteFile(file);
            string cmd = @"terser --compress --mangle -- " + System.IO.Path.GetFileName(file);
            info.FileName = "cmd";
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.StandardOutputEncoding = Encoding.UTF8;
            info.UseShellExecute = false;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            info.WorkingDirectory = System.IO.Path.GetDirectoryName(file);
            info.CreateNoWindow = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);

            LerAte(p.StandardOutput, info.WorkingDirectory+'>');

            p.StandardOutput.DiscardBufferedData();
            p.StandardInput.WriteLine(cmd);
            p.StandardOutput.ReadLine();
            string s = LerAte(p.StandardOutput, info.WorkingDirectory + '>');
            string novo = convertToUTF8(s);
            //Console.WriteLine(s);
            //Console.WriteLine(novo);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file, false, Encoding.UTF8))
                sw.Write(novo);
            p.Kill();
            Console.WriteLine("Minification file {0} ok!", file);
        }
        static void minificarCSS(string file)
        {
            file = System.IO.Path.GetFullPath(file);
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            //info.Arguments = @"terser --compress --mangle -- " + System.IO.Path.GetFileName(file);
            string cmd = string.Format("node-minify --compressor clean-css --input {0} --output {0}", System.IO.Path.GetFileName(file));
            info.FileName = "cmd";
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            info.WorkingDirectory = System.IO.Path.GetDirectoryName(file);
            info.CreateNoWindow = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);

            LerAte(p.StandardOutput, info.WorkingDirectory + '>');

            p.StandardOutput.DiscardBufferedData();

            p.StandardInput.WriteLine(cmd);
            p.StandardOutput.ReadLine();
            string s = LerAte(p.StandardOutput, info.WorkingDirectory + '>');
            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
            //    sw.Write(s);
            p.Kill();
            Console.WriteLine("Minification file {0} ok!", file);
        }
        static void minificarHTML(string file)
        {
            file = System.IO.Path.GetFullPath(file);
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            //info.Arguments = @"terser --compress --mangle -- " + System.IO.Path.GetFileName(file);
            string cmd = string.Format("node-minify --compressor html-minifier --input {0} --output {0}", System.IO.Path.GetFileName(file));
            info.FileName = "cmd";
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            info.WorkingDirectory = System.IO.Path.GetDirectoryName(file);
            info.CreateNoWindow = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);

            LerAte(p.StandardOutput, info.WorkingDirectory + '>');

            p.StandardOutput.DiscardBufferedData();

            p.StandardInput.WriteLine(cmd);
            p.StandardOutput.ReadLine();
            string s = LerAte(p.StandardOutput, info.WorkingDirectory + '>');
//            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
//                sw.Write(s);
            p.Kill();
            Console.WriteLine("Minification file {0} ok!", file);
        }

        static void minificar(string file)
        {
            switch( System.IO.Path.GetExtension(file).ToLower())
            {
                case ".js": minificarJS(file); break;
                case ".html": minificarHTML(file); break;
                case ".htm": minificarHTML(file); break;
                case ".css": minificarCSS(file); break;
            }
        }
        static void Main(string[] args)
        {
            //string path = args[0];
            string path = @"C:\Projetos\Azadive\AzaDive V1.0\resources\app\backend\models";
            foreach (string s in System.IO.Directory.GetFiles(path, "*.js", System.IO.SearchOption.AllDirectories))
                minificar(s);
            foreach (string s in System.IO.Directory.GetFiles(path, "*.html", System.IO.SearchOption.AllDirectories))
                minificar(s);
            foreach (string s in System.IO.Directory.GetFiles(path, "*.css", System.IO.SearchOption.AllDirectories))
                minificar(s);
        }
    }
}
