using System;
using System.Collections.Generic;
using System.Text;

namespace pathtest
{
    public class CNaviNode
    {
        public int x, y; //위치
        public int dist; //목표점까지의 거리
        public int depth;

        public CNaviNode pParent = null;

        public void Copy(CNaviNode pNode)
        {
            x = pNode.x;
            y = pNode.y;
            dist = pNode.dist;
            depth = pNode.depth;
            pParent = pNode.pParent;
        }

        public bool IsSamePos(CNaviNode pNode)
        {
            if (x != pNode.x || y != pNode.y) return false;

            return true;
        }

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

        public static CNaviNode Create(int sx, int sy)
        {
            CNaviNode pNode = new CNaviNode();
            pNode.x = sx;
            pNode.y = sy;
            return pNode;
        }

        public void CalcDist(CNaviNode pDest, int cdepth)
        {
            int deltx = pDest.x - x;
            int delty = pDest.y - y;
            dist = (deltx * deltx) + (delty * delty);
            depth = cdepth;
        }

        public void SetParent(CNaviNode p) { pParent = p; }
        public CNaviNode GetParent() { return pParent; }
    }
}
