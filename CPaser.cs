using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace pathtest
{
    //단순 텍스트파일 파서, 파일의 내용을 읽어서 단어를 하나씩 추출합니다.
    public class CPaser
    {
        private int mCodePage;
        private byte[] mArray;
        private long mArraySize;
        private int mCurrentPos;

        public CPaser()
        {
            mCodePage = 949;
            mArray = null;
            mArraySize = 0;
            mCurrentPos = 0;
        }

        public int GetCodePage()
        {
            return mCodePage;
        }

        public void SetCodePage(int codePage)
        {
            mCodePage = codePage;
        }

        public bool Load(string fileName)
        {
            mCurrentPos = 0;
            mArraySize = 0;
            mArray = null;

            if (!File.Exists(fileName))
                return false;

            byte[] temp = File.ReadAllBytes(fileName);

            if (temp == null || temp.Length == 0)
                return false;

            mArraySize = temp.Length;

            mArray = new byte[temp.Length + 1];
            Array.Copy(temp, 0, mArray, 0, temp.Length);
            mArray[temp.Length] = 0;

            temp = null;

            return true;
        }

        public int GetOne(byte[] buf)
        {
            int i = 0;

            for (; mCurrentPos < mArraySize; ++mCurrentPos)
            {
                //공백, 줄바꿈/줄시작, 탭 문자는 시작시점에 걸러내기
                if (mArray[mCurrentPos] == 0x20 || mArray[mCurrentPos] == 0x0a || mArray[mCurrentPos] == 0x0d || mArray[mCurrentPos] == 0x09)
                    continue;
                else
                    break;
            }

            for (; mCurrentPos < mArraySize; ++mCurrentPos)
            {
                //공백, 줄바꿈/줄시작, 탭 문자면 파싱 종료하기
                if (mArray[mCurrentPos] == 0x20 || mArray[mCurrentPos] == 0x0a || mArray[mCurrentPos] == 0x0d || mArray[mCurrentPos] == 0x09)
                {
                    break;
                }
                else
                {
                    //따옴표이면
                    if (mArray[mCurrentPos] == 0x22)
                    {
                        ++mCurrentPos;

                        //다음 따옴표가 나올떄까지 버퍼에 문자 담기
                        for (; mCurrentPos < mArraySize; ++mCurrentPos)
                        {
                            //다음 따옴표면 종료
                            if (mArray[mCurrentPos] == 0x22)
                                break;

                            buf[i++] = mArray[mCurrentPos];
                        }

                        ++mCurrentPos;
                        break;
                    }
                    else
                        buf[i++] = mArray[mCurrentPos];
                }
            }

            if (i == 0)
                return 0;
            else
                buf[i] = 0;

            return i;
        }

        public int GetInt(byte[] buf)
        {
            int len = GetOne(buf);

            if (len == 0)
                return 0;

            string temp = Encoding.GetEncoding(mCodePage).GetString(buf, 0, len);

            return int.Parse(temp);
        }

        public uint GetUInt(byte[] buf)
        {
            int len = GetOne(buf);

            if (len == 0)
                return 0;

            string temp = Encoding.GetEncoding(mCodePage).GetString(buf, 0, len);

            return uint.Parse(temp);
        }

        public float GetFloat(byte[] buf)
        {
            int len = GetOne(buf);

            if (len == 0)
                return 0;

            string temp = Encoding.GetEncoding(mCodePage).GetString(buf, 0, len);

            return float.Parse(temp);
        }

        public string GetString(byte[] buf)
        {
            int len = GetOne(buf);

            if (len == 0)
                return null;

            string temp = Encoding.GetEncoding(mCodePage).GetString(buf, 0, len);

            return temp;
        }
        
        public bool Compare(byte[] bt1, string str)
        {
            byte[] arr = Encoding.GetEncoding(mCodePage).GetBytes(str);

            return Compare(bt1, arr);
        }

        public bool Compare(byte[] buf1, byte[] buf2)
        {
            if (buf1 == null || buf2 == null)
                return false;

            for (int i = 0; buf1[i] != 0; ++i)
            {
                if (buf2[i] == 0)
                    return false;

                if (buf1[i] != buf2[i])
                    return false;
            }

            return true;
        }
    }
}
