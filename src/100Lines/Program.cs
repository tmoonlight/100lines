using System;
using System.IO;

namespace OneHundredLines
{
    class Program
    {
        static void Main(string[] args)
        {
            ////System.IO.
            ////Console.WriteLine("Hello World!");
            //var hash = new HashFile("c:\\hash1.db");
            //string str = "10";
            //string value = "only ascii";
            //hash.Put(System.Text.ASCIIEncoding.ASCII.GetBytes(str),

            //    System.Text.ASCIIEncoding.ASCII.GetBytes(value)
            //    );
            //Console.WriteLine(System.Text.ASCIIEncoding.ASCII.GetString(hash.Get(System.Text.ASCIIEncoding.ASCII.GetBytes(str))));
            //Console.Read();
            //File.Delete("c:\\seq.db");
            var seqf = new SequenceFile("c:\\seq.db");
            for (int i = 0; i < 10000; i++)
            {
                seqf.Add(2);
                seqf.Add(3);
                seqf.Add(453455345);
                seqf.Add(45534345);
                seqf.Add(45543345);
            }

            //seqf.Close();
            var list = seqf.GetRange(0, 4);
        }
    }
}
