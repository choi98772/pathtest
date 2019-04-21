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
        int mWidth = 0;
        int mHeight = 0;

        byte[] mMapData = null;    //단순히 0과 1로 표현되는 맵데이터

        public CNavigationData()
        {
        }

        //해당 지점이 이동가능한 지점인가 확인한다
        virtual public bool IsValidPos(int x, int y)
        {
            //맵을 벗어난 지점인가?
            if (x < 0 || x >= mWidth)
                return false;

            //맵을 벗어난 지점인가?
            if (y < 0 || y >= mHeight)
                return false;

            //이동 불가능한 지점인가?
            if (mMapData[(y * mWidth) + x] == 0)
                return false;

            return true;
        }

        //해당 지점이 이동가능한 지점인가 확인한다
        virtual public bool IsValidPos(CNaviNode pos)
        {
            //맵을 벗어난 지점인가?
            if (pos.x < 0 || pos.x >= mWidth)
                return false;

            //맵을 벗어난 지점인가?
            if (pos.y < 0 || pos.y >= mHeight)
                return false;

            //이동 불가능한 지점인가?
            if (mMapData[(pos.y * mWidth) + pos.x] == 0)
                return false;

            return true;
        }


        /*해당 노드에 인접한 이동가능한 이웃노드들을 모두구한다, 리턴값은 이웃의 개수
         * 좌상단, 좌, 좌하단, 상단, 하단, 우상단, 우, 우하단 8방향을 검사해서
         * 이동가능한 지점만 vecList목록에 담는다.
         * */
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

                    //중앙 위치는 필요없다.
                    if (cx == pos.x && cy == pos.y)
                        continue;

                    //이동불가능한 지점도 필요없음
                    if (!IsValidPos(cx, cy))
                        continue;

                    //이동가능한 지점이면 목록에 추가하기
                    vecList.Add(CNaviNode.Create(cx, cy));
                }
            }

            //구해진 목록의 개수를 리턴
            return vecList.Count;
        }
        
        //맵의 폭과 높이
        public int GetWidth() { return mWidth; }
        public int GetHeight() { return mHeight; }

        //텍스트로 저장된 맵데이터를 파싱한다
        public bool Load(string strFilename)
        {
            CPaser ps = new CPaser();

            if (!ps.Load(strFilename))
                return false;

            ps.SetCodePage(949);

            mMapData = null;

            byte[] buf = new byte[1024];

            int pos = 0;

            while (true)
            {
                int len = ps.GetOne(buf);
                if (len == 0) break;

                if (ps.Compare(buf, "width")) //맵의 가로 셀 개수
                {
                    mWidth = ps.GetInt(buf);
                    continue;
                }
                else if (ps.Compare(buf, "height")) //맵의 세로 셀 개수
                {
                    mHeight = ps.GetInt(buf);
                    continue;
                }
                else if (ps.Compare(buf, "mapstart")) //이후부터 맵데이터가 시작된다.
                {
                    mMapData = new byte[mWidth * mHeight]; //셀 개수만큼 배열을 할당하고
                    pos = 0;
                }
                else
                {
                    //맵데이터 파싱하기

                    string conv = Encoding.GetEncoding(949).GetString(buf, 0, len);

                    if (conv == null)
                        return false;

                    mMapData[pos++] = (byte)int.Parse(conv);
                }
            }

            return true;
        }


    }
}
