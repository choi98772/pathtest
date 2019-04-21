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

        public CNaviNode parentNode = null;

        //주어진 노드의 내용을 복사하기
        public void Copy(CNaviNode node)
        {
            x = node.x;
            y = node.y;
            dist = node.dist;
            depth = node.depth;
            parentNode = node.parentNode;
        }

        //주어진 노드와 같은 위치인가?
        public bool IsSamePos(CNaviNode node)
        {
            if (x != node.x || y != node.y)
                return false;

            return true;
        }

        //이노드의 내용을 복사한 새로운 노드를 구성
        public CNaviNode Clone()
        {
            CNaviNode node = new CNaviNode();

            node.x = x;
            node.y = y;
            node.dist = dist;
            node.depth = depth;
            node.parentNode = null;

            return node;
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
            CNaviNode node = new CNaviNode();
            node.x = sx;
            node.y = sy;

            int deltx = dx - sx;
            int delty = dy - sy;

            node.dist = (deltx * deltx) + (delty * delty);
            node.depth = dep;

            return node;
        }

        //주어진 좌표를 기준으로 노드를 구성하기
        public static CNaviNode Create(int sx, int sy)
        {
            CNaviNode node = new CNaviNode();

            node.x = sx;
            node.y = sy;

            return node;
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
        public void SetParent(CNaviNode parent) { parentNode = parent; }

        public CNaviNode GetParent() { return parentNode; }
    }
}
