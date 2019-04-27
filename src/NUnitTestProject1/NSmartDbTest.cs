using System;
using System.IO;
using NUnit.Framework;
using OneHundredLines;

namespace Tests
{
    public class NSmartDbTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void SequenceAdd()
        {
            string filename = "c:\\seq.db";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            var seqf = new SequenceFile(filename);
            seqf.Add(1234567);
            seqf.Add(7654321);
            seqf.Add(11234567);
            seqf.Add(7234567);
            seqf.Add(1234567);
            seqf.Add(7654321);
            seqf.Add(11234567);
            seqf.Add(7234567);
            var list = seqf.GetRange(0, 4);
            if (list[0] == 1234567 &&
                list[1] == 7654321 &&
                list[2] == 11234567 &&
                list[3] == 7234567
            )
            {
                // Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
            var list2 = seqf.GetRange(6, 2);
            if (list2[0] == 11234567 &&
                list2[1] == 7234567)
            {
                // Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
            Assert.Pass();
        }

        [Test]
        public void NSmartDbCrud()
        {
            string filename1 = "c:\\dbfile.db";
            if (File.Exists(filename1))
            {
                File.Delete(filename1);
            }
            string filename2 = "c:\\indexfile.db";
            if (File.Exists(filename2))
            {
                File.Delete(filename2);
            }
            Random rand = new Random();
            OneHundredLines.NSmartDb db = new OneHundredLines.NSmartDb(filename1, filename2);

            for (int i = 0; i < 10000; i++)
            {
                db.Insert(i, $"{{'username':'admin{i}','role':'admin','client_info':'no_info'}}");
            }
            // db.Insert(1000001,"{'username':'admin','role':'admin','client_info':'no_info'}");
            // db.Insert(2000002, "{'username':'admin','role':'admin','client_info':'no_info'}");
            // db.Insert(3000003, "{'username':'admin','role':'admin','client_info':'no_info'}");
            db.Close();
            OneHundredLines.NSmartDb db2 = new OneHundredLines.NSmartDb(filename1, filename2);
            string[] strs = db2.Select(0, 10000);
            Assert.AreEqual(strs[9999],"{'username':'admin9999','role':'admin','client_info':'no_info'}");
            //É¾³ý
            db2.Delete(2);
            db2.Delete(5);
            db2.Delete(7);
            string[] strs2 = db2.Select(0, 10);

        }
    }
}