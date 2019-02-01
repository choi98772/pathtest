using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace pathtest
{
    //단순 텍스트 파서
    public class CPaser
    {
        private int m_iCodePage;
        private byte[] m_Array;
        private long m_iSize;
        private int m_iPos;

        public CPaser()
        {
            m_iCodePage = 949;
            m_Array = null;
            m_iSize = 0;
            m_iPos = 0;
        }

        public int GetCodePage() { return m_iCodePage; }
        public void SetCodePage(int iCodePage) { m_iCodePage = iCodePage; }

        public bool Load(string strName)
        {
            m_iPos = 0;
            m_iSize = 0;
            m_Array = null;

            if (!File.Exists(strName)) return false;

            byte[] temp = File.ReadAllBytes(strName);
            if (temp == null || temp.Length == 0) return false;

            m_iSize = temp.Length;

            m_Array = new byte[temp.Length + 1];
            Array.Copy(temp, 0, m_Array, 0, temp.Length);
            m_Array[temp.Length] = 0;

            temp = null;

            return true;
        }

        public int GetOne(byte[] buf)
        {
            int i = 0;
            for (; m_iPos < m_iSize; ++m_iPos)
            {
                if (m_Array[m_iPos] == 0x20 || m_Array[m_iPos] == 0x0a || m_Array[m_iPos] == 0x0d || m_Array[m_iPos] == 0x09)
                {
                    continue;
                }
                else break;
            }

            for (; m_iPos < m_iSize; ++m_iPos)
            {
                if (m_Array[m_iPos] == 0x20 || m_Array[m_iPos] == 0x0a || m_Array[m_iPos] == 0x0d || m_Array[m_iPos] == 0x09)
                {
                    break;
                }
                else
                {
                    if (m_Array[m_iPos] == 0x22)
                    {
                        ++m_iPos;
                        for (; m_iPos < m_iSize; ++m_iPos)
                        {
                            if (m_Array[m_iPos] == 0x22) break;
                            buf[i++] = m_Array[m_iPos];
                        }
                        ++m_iPos;
                        break;
                    }
                    else buf[i++] = m_Array[m_iPos];
                }
            }

            if (i == 0) return 0;
            else buf[i] = 0;

            return i;
        }

        public int GetInt(byte[] buf)
        {
            int len = GetOne(buf);
            if (len == 0) return 0;

            string temp = Encoding.GetEncoding(m_iCodePage).GetString(buf, 0, len);

            return int.Parse(temp);
        }

        public uint GetUInt(byte[] buf)
        {
            int len = GetOne(buf);
            if (len == 0) return 0;

            string temp = Encoding.GetEncoding(m_iCodePage).GetString(buf, 0, len);

            return uint.Parse(temp);
        }

        public float GetFloat(byte[] buf)
        {
            int len = GetOne(buf);
            if (len == 0) return 0;

            string temp = Encoding.GetEncoding(m_iCodePage).GetString(buf, 0, len);

            return float.Parse(temp);
        }

        public string GetString(byte[] buf)
        {
            int len = GetOne(buf);
            if (len == 0) return null;

            string temp = Encoding.GetEncoding(m_iCodePage).GetString(buf, 0, len);

            return temp;
        }
        
        public bool Compare(byte[] bt1, string str)
        {
            byte[] arr = Encoding.GetEncoding(m_iCodePage).GetBytes(str);

            return Compare(bt1, arr);
        }

        public bool Compare(byte[] buf1, byte[] buf2)
        {
            if (buf1 == null || buf2 == null) return false;

            for (int i = 0; buf1[i] != 0; ++i)
            {
                if (buf2[i] == 0) return false;
                if (buf1[i] != buf2[i]) return false;
            }

            return true;
        }
    }
}
