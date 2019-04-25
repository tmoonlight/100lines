using System;
using System.IO;

namespace NSmartFile
{
    class Program
    {
        static void Main(string[] args)
        {
            //System.IO.
            //Console.WriteLine("Hello World!");
            var hash = new HashFile("c:\\hash1.db");
            string str = "10";
            string value = "qw";
            hash.Put(System.Text.ASCIIEncoding.ASCII.GetBytes(str),

                System.Text.ASCIIEncoding.ASCII.GetBytes(value)
                );
            Console.WriteLine(System.Text.ASCIIEncoding.ASCII.GetString(hash.Get(System.Text.ASCIIEncoding.ASCII.GetBytes(str))));
            Console.Read();
        }
    }
}
