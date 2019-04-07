using System;
using System.Collections.Generic;
using System.Text;

namespace pathtest
{
    public class CNaviNode
    {
        public int x, y; //위치
        public int dist; //목표점까지의 거리
        public int depth; //노드의 탐색깊이

        public CNaviNode pParent = null;

        //주어진 노드의 내용을 복사하기
        public void Copy(CNaviNode pNode)
        {
            x = pNode.x;
            y = pNode.y;
            dist = pNode.dist;
            depth = pNode.depth;
            pParent = pNode.pParent;
        }

        //주어진 노드와 같은 위치인가?
        public bool IsSamePos(CNaviNode pNode)
        {
            if (x != pNode.x || y != pNode.y) return false;

            return true;
        }

        //이노드의 내용을 복사한 새로운 노드를 구성
        public CNaviNode Clone()
        {
            CNaviNode pNode = new CNaviNode();
            pNode.x = x;
            pNode.y = y;
            pNode.dist = dist;
            pNode.depth = depth;
            pNode.pParent = null;
            return pNode;
        }

        /// <summary>
        /// 주어 진 좌표를 기준으로 노드를 구성하기
        /// </summary>
        /// <param name="sx">노드위치x</param>
        /// <param name="sy">노드위치y</param>
        /// <param name="dx">목표위치x</param>
        /// <param name="dy">목표위치y</param>
        /// <param name="dep">탐색깊이</param>
        /// <returns></returns>
        public static CNaviNode Create(int sx, int sy, int dx, int dy, int dep)
        {
            CNaviNode pNode = new CNaviNode();
            pNode.x = sx;
            pNode.y = sy;

            int deltx = dx - sx;
            int delty = dy - sy;
            pNode.dist = (deltx * deltx) + (delty * delty);
            pNode.depth = dep;

            return pNode;
        }

        //주어진 좌표를 기준으로 노드를 구성하기
        public static CNaviNode Create(int sx, int sy)
        {
            CNaviNode pNode = new CNaviNode();
            pNode.x = sx;
            pNode.y = sy;
            return pNode;
        }

        /// <summary>
        /// 이노드에서 목표점까지의 거리를 구한다.
        /// </summary>
        /// <param name="pDest">목표점</param>
        /// <param name="cdepth">탐색깊이</param>
        public void CalcDist(CNaviNode pDest, int cdepth)
        {
            int deltx = pDest.x - x;
            int delty = pDest.y - y;
            dist = (deltx * deltx) + (delty * delty);
            depth = cdepth;
        }

        //부모노드 설정
        public void SetParent(CNaviNode p) { pParent = p; }

        public CNaviNode GetParent() { return pParent; }
    }
}
