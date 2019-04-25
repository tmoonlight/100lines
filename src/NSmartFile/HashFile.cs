using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NSmartFile
{
    public class HashFile
    {
        private FileStream rf;
        private BinaryWriter bw;
        private BinaryReader br;
        private long tableSize = 1024L * 1024; //1M
        public HashFile(String file)
        {

            rf = new FileStream(file, FileMode.OpenOrCreate);
            bw = new BinaryWriter(rf);
            br = new BinaryReader(rf);
            if (rf.Length == 0)
            {
                rf.SetLength(tableSize * 8);
            }
        }
        public void Put(byte[] k, byte[] v)
        {
            //key的hash位置
            long i = (Math.Abs(ComputeHash(k)) % tableSize) * 8;
            rf.Position = i;
            byte[] key = new byte[k.Length];
            bool removeFlag = false;
            for (long p = br.ReadInt64(); p != 0;)
            {
                rf.Position = p;
                int keyLen = br.ReadInt32();
                if (keyLen == k.Length)
                {
                    rf.Read(key);
                    int valueLen = br.ReadInt32();
                    if (Enumerable.SequenceEqual(key, k))
                    {
                        if (valueLen == v.Length)
                        { // new value is of the same size, reuse the space
                            bw.Write(v);
                            return;
                        }
                        else
                        { // allocate new space, remove the old item (not physically)
                            removeFlag = true;
                            break;
                        }
                    }
                    else
                    {
                        rf.Seek(valueLen, SeekOrigin.Current);
                        p = br.ReadInt64();
                    }
                }
                else
                {
                    rf.Seek(keyLen, SeekOrigin.Current);
                    int valueLen = br.ReadInt32();
                    bw.Seek(valueLen, SeekOrigin.Current);
                    p = br.ReadInt64();
                }
            }

            if (removeFlag)
            {
                Remove(k);
            }

            rf.Position = i;
            long head = br.ReadInt64();
            long pos = rf.Length;
            // 插入新的数据
            rf.Position = pos;
            bw.Write(k.Length);
            bw.Write(k);
            bw.Write(v.Length);
            bw.Write(v);
            bw.Write(head);
            rf.Position = i;
            bw.Write(pos);
        }
        public byte[] Get(byte[] k)
        {

            long i = Math.Abs(ComputeHash(k)) % tableSize * 8;
            rf.Position = i;
            byte[] key = new byte[k.Length];
            for (long p = br.ReadInt64(); p != 0;)
            {
                rf.Position = p;
                int keyLen = br.ReadInt32();
                if (keyLen == k.Length)
                {
                    br.Read(key);
                    int valueLen = br.ReadInt32();
                    if (Enumerable.SequenceEqual(key, k))
                    {
                        byte[] v = new byte[valueLen];
                        br.Read(v);
                        return v;
                    }
                    else
                    {
                        bw.Seek(valueLen, SeekOrigin.Current);
                        p = br.ReadInt64();
                    }
                }
                else
                {
                    bw.Seek(keyLen,SeekOrigin.Current);
                    int valueLen = br.ReadInt32();
                    bw.Seek( valueLen, SeekOrigin.Current);
                    p = br.ReadInt64();
                }
            }
            return null;
        }

        public void Remove(byte[] k)
        {
            long i = Math.Abs(ComputeHash(k)) % tableSize * 8;
            rf.Position = i;
            byte[] key = new byte[k.Length];
            for (long p = br.ReadInt64(), pre = i; p != 0;)
            {
                rf.Position = p;
                int keyLen = br.ReadInt32();
                if (keyLen == k.Length)
                {
                    br.Read(key);
                    int valueLen = br.ReadInt32();
                    if (Enumerable.SequenceEqual(key, k))
                    {
                        bw.Seek(valueLen, SeekOrigin.Current);
                        long next = br.ReadInt64();
                        rf.Position = pre;
                        bw.Write(next);
                        return;
                    }
                    else
                    {
                        bw.Seek(valueLen, SeekOrigin.Current);
                        p = br.ReadInt32();
                        pre = rf.Position - 8;
                    }
                }
                else
                {
                    bw.Seek(keyLen, SeekOrigin.Current);
                    int valueLen = br.ReadInt32();
                    bw.Seek(valueLen, SeekOrigin.Current);
                    p = br.ReadInt64();
                    pre = rf.Position - 8;
                }
            }
        }

        public void Close()
        {
            bw.Close();
            br.Close();
            rf.Close();
        }

        public void manageFragment()
        {
            // garbage collection, compact the data file
        }

        public int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }

}
