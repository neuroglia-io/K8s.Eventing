using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure
{

    /// <summary>
    /// Represents a wrapper for a key/value based file
    /// </summary>
    /// <remarks>Code from <see href="https://www.codeproject.com/Articles/42872/Save-Key-Value-Pairs-in-a-File"/></remarks>
    public class HashFile
        : IEnumerable<KeyValuePair<string, string>>
    {

        ushort RecordSize;
        ushort KeySize = 50;
        ushort ValueSize = 50;

        private uint NumberOfKeys = 0;

        ushort ByteIndex;
        uint IndexPosition = 0, IndexOffset = 0;

        const byte Chars = 91;
        const ushort BlockSize = 4096;
        const int RecordOffset = 4;

        byte[] IndexBlock = new byte[91 * 8];

        private FileStream FileStreamData = null;
        private BinaryWriter BinaryWriterData = null;
        private BinaryReader BinaryReaderData = null;

        private FileStream FileStreamIdx = null;
        private BinaryWriter BinaryWriterIdx = null;
        private BinaryReader BinaryReaderIdx = null;

        string StringLastError = "";
        int IndexError, Difference, CurrentKey = 0;

        uint LastPosition = 0;

        byte[] LastKeyBytes, KeyBytes;
        bool flag;

        string key, kvalue;

        private int count = 0;
        /// <summary>
        /// Initializes a new <see cref="HashFile"/>
        /// </summary>
        public HashFile()
        {
            for (int i = 0; i < 91 * 8; i++)
            {
                IndexBlock[i] = 255;
            }
        }

        /// <summary>
        /// Destructs the <see cref="HashFile"/>
        /// </summary>
        ~HashFile()
        {
            if (FileStreamData != null)
            {
                FileStreamData.Close();
            }

            if (FileStreamIdx != null)
            {

                FileStreamIdx.Close();
            }

            FileStreamData = null;
            BinaryWriterData = null;
            BinaryReaderData = null;

            FileStreamIdx = null;
            BinaryWriterIdx = null;
            BinaryReaderIdx = null;

        }

        /// <summary>
        /// Initializes the <see cref="HashFile"/>
        /// </summary>
        /// <param name="fileName">The <see cref="HashFile"/>'s file name</param>
        /// <param name="keySize">The <see cref="HashFile"/>'s key size, in bytes</param>
        /// <param name="valueSize">The <see cref="HashFile"/>'s value size, in bytes</param>
        public void Initialize(string fileName, ushort keySize, ushort valueSize)
        {
            KeySize = keySize;
            ValueSize = valueSize;

            RecordSize = (ushort)(keySize + valueSize + 1); // 1 for delete flag

            FileStreamData = new FileStream(fileName + ".dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            BinaryWriterData = new BinaryWriter(FileStreamData);
            BinaryReaderData = new BinaryReader(FileStreamData);

            FileStreamIdx = new FileStream(fileName + ".idx", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriterIdx = new BinaryWriter(FileStreamIdx);
            BinaryReaderIdx = new BinaryReader(FileStreamIdx);

            if (FileStreamData.Length == 0)
            {
                BinaryWriterData.Write(NumberOfKeys);
                BinaryWriterData.Flush();
            }

            FileStreamData.Seek(0, 0);
            NumberOfKeys = BinaryReaderData.ReadUInt32();

            if (FileStreamIdx.Length == 0)
                ADDIndexBlock();
        }

        private int ADDIndexBlock()
        {
            try
            {
                FileStreamIdx.Seek(FileStreamIdx.Length, 0);
                BinaryWriterIdx.Write(IndexBlock);

            }
            catch (Exception e)
            {
                StringLastError = e.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Insertys the specified key
        /// </summary>
        /// <param name="key">The key to insert</param>
        /// <param name="value">The value to insert</param>
        /// <param name="flag">A boolean representing the flag of the entry to insert</param>
        /// <returns>The index of the newly inserted key</returns>
        public int InsertKey(string key, string value, bool flag)
        {

            if (key.Length > KeySize) return -50;
            if (value.Length > ValueSize) return -51;

            try
            {
                LastPosition = (uint)RecordOffset + NumberOfKeys * RecordSize;

                if ((IndexError = CreateIndex(key, value)) < 0)
                {
                    if (IndexError == -4) return 1;
                    return IndexError;
                }

                try
                {

                    if ((FileStreamData.Length - LastPosition) < 256)
                        FileStreamData.SetLength(FileStreamData.Length + BlockSize);


                    FileStreamData.Seek(LastPosition, 0);
                    BinaryWriterData.Write(key);
                    BinaryWriterData.Write(flag);
                    BinaryWriterData.Write(value);

                    NumberOfKeys++;

                    FileStreamData.Seek(0, 0);
                    BinaryWriterData.Write(NumberOfKeys);
                    BinaryWriterData.Flush();


                }
                catch (Exception e)
                {
                    StringLastError = e.Message;
                    return -1;
                }

            }
            catch (IOException e)
            {
                StringLastError = e.Message;
                return -2;
            }
            return 1;
        }

        private static int GetDif(ref byte[] P_a, ref byte[] P_b)
        {
            // Comments
            // If all a[n] == b[n] then return zero
            // If a[n] > b[n] then return negative number - move the Last key to the next level
            // if a[n] < b[n] then return positve numver  - move new key to the next level
            // where n < min(a.length,b.length)


            int MinLength = Math.Min(P_a.Length, P_b.Length);
            for (int i = 0; i < MinLength; i++)
            {
                if (P_a[i] > P_b[i])
                    return -(i + 1);
                else if (P_a[i] < P_b[i])
                    return (i + 1);
            }

            if (P_a.Length > P_b.Length)
                return -(P_b.Length + 1);
            else if (P_a.Length < P_b.Length)
                return P_a.Length + 1;
            else return 0;
        }

        private int CreateIndex(string P_key, string P_value)
        {
            // Key Should contain ASCII 32-122 only       

            KeyBytes = Encoding.UTF8.GetBytes(P_key);

            uint RecordPointer = uint.MaxValue, NewRecordPointer = uint.MaxValue, IndexPointer = uint.MaxValue, NewIndexOffset = 0;

            string LastKey = "";

            NewRecordPointer = (uint)LastPosition; // Start of Next Key

            IndexOffset = 0;
            ByteIndex = 0;

            do
            {
                IndexPosition = (uint)(IndexOffset + (KeyBytes[ByteIndex] - 32) * 8);
                count++;
                try // Read next Branch (ASCII 32-122)
                {
                    if (KeyBytes[ByteIndex] < 32 || KeyBytes[ByteIndex] > 122) return -52; // Validate Character Range

                    FileStreamIdx.Seek(IndexPosition, 0);
                    RecordPointer = BinaryReaderIdx.ReadUInt32();
                    IndexPointer = BinaryReaderIdx.ReadUInt32();
                }
                catch (Exception e)
                {
                    StringLastError = e.Message;
                    return -1;
                }

                if (IndexPointer == uint.MaxValue)
                {
                    try
                    {
                        FileStreamIdx.Seek(IndexPosition, 0);
                        BinaryWriterIdx.Write(NewRecordPointer);
                        BinaryWriterIdx.Write(IndexPosition);
                        FileStreamIdx.Flush();
                        return 1; // Inserted new Branch successfully     
                    }
                    catch (Exception e)
                    {
                        StringLastError = e.Message;
                        return -1;
                    }
                }
                else
                {
                    try
                    {
                        FileStreamData.Seek(RecordPointer, 0);
                        LastKey = BinaryReaderData.ReadString();
                        LastKeyBytes = ASCIIEncoding.UTF8.GetBytes(LastKey);
                    }
                    catch (Exception e)
                    {
                        StringLastError = e.Message;
                        return -10;
                    }


                    Difference = GetDif(ref LastKeyBytes, ref KeyBytes);
                    if (Difference == 0)
                    {
                        if (BinaryReaderData.PeekChar() == 0)
                        {
                            return -3;
                        }
                        else
                        {
                            FileStreamData.Seek(RecordPointer, 0);
                            BinaryWriterData.Write(P_key);
                            BinaryWriterData.Write(false);
                            BinaryWriterData.Write(P_value);

                            BinaryWriterData.Flush();

                            return -4; // if the record flag is deleted, update record.
                        }
                    }
                    else
                    {
                        if (Difference > 0)
                        {
                            if (IndexPointer == IndexPosition)
                            {
                                try
                                {
                                    NewIndexOffset = (uint)FileStreamIdx.Length;
                                    FileStreamIdx.Seek(IndexPosition + 4, 0);
                                    BinaryWriterIdx.Write(NewIndexOffset);
                                    ADDIndexBlock();

                                    IndexPosition = (uint)(NewIndexOffset + (KeyBytes[ByteIndex + 1] - 32) * 8);

                                    FileStreamIdx.Seek(IndexPosition, 0);

                                    BinaryWriterIdx.Write(NewRecordPointer);
                                    BinaryWriterIdx.Write(IndexPosition);
                                    FileStreamIdx.Flush();

                                    return 1; // Inserted new key successfully     
                                }
                                catch (Exception e)
                                {
                                    StringLastError = e.Message;
                                    return -16;
                                }
                            }
                            else
                            {
                                IndexOffset = IndexPointer;
                            }

                        }
                        else
                        {
                            if (IndexPointer == IndexPosition)
                            {
                                try
                                {
                                    NewIndexOffset = (uint)FileStreamIdx.Length;
                                    FileStreamIdx.Seek(IndexPosition, 0);

                                    BinaryWriterIdx.Write(NewRecordPointer);
                                    BinaryWriterIdx.Write(NewIndexOffset);
                                    ADDIndexBlock();

                                    NewRecordPointer = RecordPointer;

                                    KeyBytes = ASCIIEncoding.UTF8.GetBytes(LastKey);
                                    P_key = LastKey;

                                    IndexOffset = NewIndexOffset;

                                    IndexPosition = (uint)(NewIndexOffset + (KeyBytes[ByteIndex + 1] - 32) * 8);

                                    FileStreamIdx.Seek(IndexPosition, 0);

                                    BinaryWriterIdx.Write(NewRecordPointer);
                                    BinaryWriterIdx.Write(IndexPosition);
                                    FileStreamIdx.Flush();

                                    return 1; // Inserted new key successfully     

                                }
                                catch (Exception e)
                                {
                                    StringLastError = e.Message;
                                    return -17;
                                }


                            }
                            else
                            {
                                try
                                {

                                    FileStreamIdx.Seek(IndexPosition, 0);
                                    BinaryWriterIdx.Write(NewRecordPointer);

                                    NewRecordPointer = RecordPointer;
                                    KeyBytes = ASCIIEncoding.UTF8.GetBytes(LastKey);
                                    P_key = LastKey;

                                    FileStreamIdx.Flush();

                                    IndexOffset = IndexPointer;

                                }

                                catch (Exception e)
                                {
                                    StringLastError = e.Message;
                                    return -18;
                                }
                            }

                        }
                    }
                }

                ByteIndex++;
            } while (ByteIndex < KeyBytes.Length);

            return -20;
        }

        /// <summary>
        /// Finds the index of the specified key
        /// </summary>
        /// <param name="key">The key to find</param>
        /// <returns>The index of the specified key</returns>
        public uint FindKey(string key)
        {
            uint RecordPointer = uint.MaxValue, IndexPointer = uint.MaxValue;

            KeyBytes = ASCIIEncoding.UTF8.GetBytes(key);
            ByteIndex = 0;
            IndexOffset = 0;

            for (ByteIndex = 0; ByteIndex < KeyBytes.Length; ByteIndex++)
            {
                #region
                count++;
                IndexPosition = (uint)(IndexOffset + (KeyBytes[ByteIndex] - 32) * 8);

                try
                {
                    FileStreamIdx.Seek(IndexPosition, 0);
                    RecordPointer = BinaryReaderIdx.ReadUInt32();
                    IndexPointer = BinaryReaderIdx.ReadUInt32();

                    if (IndexPointer == uint.MaxValue)
                    {
                        return uint.MaxValue;
                    }

                    FileStreamData.Seek(RecordPointer, 0);

                    Key = BinaryReaderData.ReadString();
                    Flag = BinaryReaderData.ReadBoolean();
                    Value = BinaryReaderData.ReadString();

                    if (Key.Equals(key) && !Flag)
                    {
                        return RecordPointer;
                    }
                    else
                    {
                        IndexOffset = IndexPointer;
                    }
                }
                catch (Exception e)
                {
                    StringLastError = e.Message;
                    return uint.MaxValue; ;
                }
                #endregion
            }

            return uint.MaxValue;
        }

        private int GetFirstKey()
        {
            int RecordPointer = int.MaxValue;

            #region
            try
            {
                CurrentKey = 0;

                if (NumberOfKeys == 0) return -1;

                RecordPointer = RecordOffset + CurrentKey * RecordSize;

                FileStreamData.Seek(RecordPointer, 0);

                Key = BinaryReaderData.ReadString();
                Flag = BinaryReaderData.ReadBoolean();
                Value = BinaryReaderData.ReadString();

                return RecordPointer;
            }
            catch (Exception e)
            {
                StringLastError = e.Message;
                return -3;
            }
            #endregion


        }

        private int GetNextKey()
        {
            int RecordPointer = int.MaxValue;

            #region
            try
            {
                CurrentKey++;

                if (CurrentKey == NumberOfKeys)
                {
                    CurrentKey = (int)NumberOfKeys - 1;
                    return -2;
                }

                RecordPointer = RecordOffset + CurrentKey * RecordSize;
                FileStreamData.Seek(RecordPointer, 0);

                Key = BinaryReaderData.ReadString();
                Flag = BinaryReaderData.ReadBoolean();
                Value = BinaryReaderData.ReadString();

                return RecordPointer;
            }
            catch (Exception e)
            {
                StringLastError = e.Message;
                return -3;
            }
            #endregion


        }

        private int GetPrevKey()
        {
            int RecordPointer = int.MaxValue;

            #region
            try
            {
                CurrentKey--;

                if (CurrentKey < 0)
                {
                    CurrentKey = 0;
                    return -1;
                }

                RecordPointer = RecordOffset + CurrentKey * RecordSize;

                FileStreamData.Seek(RecordPointer, 0);

                Key = BinaryReaderData.ReadString();
                Flag = BinaryReaderData.ReadBoolean();
                Value = BinaryReaderData.ReadString();

                return RecordPointer;
            }
            catch (Exception e)
            {
                StringLastError = e.Message;
                return -3;
            }
            #endregion


        }

        private uint DeleteKey(string P_key)
        {
            uint RecordPointer = uint.MaxValue, IndexPointer = uint.MaxValue;

            KeyBytes = ASCIIEncoding.UTF8.GetBytes(P_key);
            ByteIndex = 0;
            IndexOffset = 0;

            for (ByteIndex = 0; ByteIndex < KeyBytes.Length; ByteIndex++)
            {
                #region
                count++;
                IndexPosition = (uint)(IndexOffset + (KeyBytes[ByteIndex] - 32) * 8);

                try
                {
                    FileStreamIdx.Seek(IndexPosition, 0);
                    RecordPointer = BinaryReaderIdx.ReadUInt32();
                    IndexPointer = BinaryReaderIdx.ReadUInt32();

                    if (IndexPointer == uint.MaxValue)
                    {
                        return uint.MaxValue;
                    }

                    FileStreamData.Seek(RecordPointer, 0);
                    Key = BinaryReaderData.ReadString();

                    if (Key.Equals(P_key))
                    {
                        if (BinaryReaderData.PeekChar() == 0)
                        {
                            BinaryWriterData.Write(true);
                        }
                        return RecordPointer;
                    }
                    else
                    {
                        IndexOffset = IndexPointer;
                    }
                }
                catch (Exception e)
                {
                    StringLastError = e.Message;
                    return uint.MaxValue; ;
                }
                #endregion
            }

            return uint.MaxValue;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            int index = this.GetFirstKey();
            while(index >= 0)
            {
                yield return new KeyValuePair<string, string>(this.Key, this.Value);
                index = this.GetNextKey();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the current key
        /// </summary>
        public string Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;
            }
        }

        /// <summary>
        /// Gets the current value
        /// </summary>
        public string Value
        {
            get
            {
                return kvalue;
            }

            set
            {
                kvalue = value;
            }
        }

        /// <summary>
        /// Gets the current flag
        /// </summary>
        public bool Flag
        {
            get
            {
                return flag;
            }

            set
            {
                flag = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="HashFile"/>'s entry count
        /// </summary>
        public uint Count
        {
            get
            {
                return NumberOfKeys;
            }
        }

    }

}
