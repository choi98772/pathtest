using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace pathtest
{
    /*
     * 2차원맵 데이터를 제공하고, 갈수 있는 곳과 못가는 곳 등의 정보를 제공한다
     * 맵은 가로 세로 n * n 개의 셀로 구성됩니다.
     * */
    public class CNavigationData
    {
        //맵의 폭과 높이(
        int m_iWidth = 0;
        int m_iHeight = 0;

        byte[] m_MapData = null;    //단순히 0과 1로 표현되는 맵데이터

        public CNavigationData()
        {
        }

        //해당 지점이 이동가능한 지점인가 확인한다
        virtual public bool IsValidPos(int x, int y)
        {
            //맵을 벗어난 지점인가?
            if (x < 0 || x >= m_iWidth)
                return false;

            //맵을 벗어난 지점인가?
            if (y < 0 || y >= m_iHeight)
                return false;

            //이동 불가능한 지점인가?
            if (m_MapData[(y * m_iWidth) + x] == 0)
                return false;

            return true;
        }

        //해당 지점이 이동가능한 지점인가 확인한다
        virtual public bool IsValidPos(CNaviNode pos)
        {
            //맵을 벗어난 지점인가?
            if (pos.x < 0 || pos.x >= m_iWidth)
                return false;

            //맵을 벗어난 지점인가?
            if (pos.y < 0 || pos.y >= m_iHeight)
                return false;

            //이동 불가능한 지점인가?
            if (m_MapData[(pos.y * m_iWidth) + pos.x] == 0)
                return false;

            return true;
        }


        //해당 노드에 인접한 이동가능한 이웃노드들을 모두구한다, 리턴값은 이웃의 개수
        virtual public int GetNeighbor(CNaviNode pos, ref List<CNaviNode> vecList)
        {
            int[] distx = new int[3] { -1, 0, 1 };
            int[] disty = new int[3] { -1, 0, 1 };

            for (int y = 0; y < 3; ++y)
            {
                for (int x = 0; x < 3; ++x)
                {
                    int cx = distx[x] + pos.x;
                    int cy = disty[y] + pos.y;
                    if (cx == pos.x && cy == pos.y) continue;

                    if (!IsValidPos(cx, cy)) continue;
                    vecList.Add(CNaviNode.Create(cx, cy));
                }
            }

            return vecList.Count;
        }
        
        //맵의 폭과 높이
        public int GetWidth() { return m_iWidth; }
        public int GetHeight() { return m_iHeight; }

        //텍스트로 저장된 맵데이터를 파싱한다
        public bool Load(string strFilename)
        {
            CPaser ps = new CPaser();
            if (!ps.Load(strFilename)) return false;

            ps.SetCodePage(949);

            m_MapData = null;

            byte[] buf = new byte[1024];

            int iPos = 0;

            while (true)
            {
                int len = ps.GetOne(buf);
                if (len == 0) break;

                if (ps.Compare(buf, "width")) //맵의 가로 셀 개수
                {
                    m_iWidth = ps.GetInt(buf);
                    continue;
                }
                else if (ps.Compare(buf, "height")) //맵의 세로 셀 개수
                {
                    m_iHeight = ps.GetInt(buf);
                    continue;
                }
                else if (ps.Compare(buf, "mapstart")) //이후부터 맵데이터가 시작된다.
                {
                    m_MapData = new byte[m_iWidth * m_iHeight]; //셀 개수만큼 배열을 할당하고
                    iPos = 0;
                }
                else
                {
                    //맵데이터 파싱하기

                    string conv = Encoding.GetEncoding(949).GetString(buf, 0, len);

                    if (conv == null)
                        return false;

                    m_MapData[iPos++] = (byte)int.Parse(conv);
                }
            }

            return true;
        }


    }
}
