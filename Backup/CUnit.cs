using System;
using System.Collections.Generic;
using System.Text;

namespace pathtest
{
    //고양이와 쥐를 표현할 유닛
    class CUnit
    {
        public int [] m_iPos = null;        //현재 위치
        private List<CNaviNode> m_vecPath;  //A*알고리즘에 의해서 찾아진길의 노드
        bool m_bMove;                       //현재 목표점으로 이동중인지를 나타내는 플래그
        static Random m_RND = null;         //임의의 위치를 생성하기 위한 난수 생성기
        CUnit m_Target;                     //추적의 대상이 되는 유닛
        bool m_bTracking;                   //추적이 활성화 되어 있는지 나타내는 플래그
        bool m_bStartTracking = false;      //추적이 시작되었는지 나타내는 플래그, 추적이 활성화 되어 있어도 시작되지 않았다면 추적하지 않느다.

        public CUnit()
        {
            m_iPos = new int[2];
            m_iPos[0] = m_iPos[1] = 0;
            m_vecPath = null;
            m_bMove = false;
            if (m_RND == null)
            {
                m_RND = new Random();
            }
            
            m_Target = null;
            m_bTracking = false;
        }

        public void SetPath(List<CNaviNode> vecPath)
        {
            m_vecPath = vecPath;
            m_bMove = true;
        }

        public bool IsTracking() { return m_bTracking; }
        public void RandomPos(CNavigationData ND)
        {
            while (true)
            {
                int cx = m_RND.Next(ND.GetWidth());
                int cy = m_RND.Next(ND.GetHeight());
                if (ND.IsValidPos(cx, cy))
                {
                    m_iPos[0] = cx;
                    m_iPos[1] = cy;
                    break;
                }
            }
        }

        public void SelectTarget(CUnit[] TargetList, int count)
        {
            if (m_Target == null)
            {
                m_Target = TargetList[m_RND.Next(count)];
            }
            else if (m_RND.Next(100) < 10)
            {
                m_Target = TargetList[m_RND.Next(count)];
            }
        }

        public void SetTarget(CUnit target) { m_Target = target; }
        public void EnableTracking(bool bEnable) { m_bTracking = bEnable; } //추적 모드를 활성화 한다.
        public void StartTracking(bool bStart) { m_bStartTracking = bStart; }   //추적을 시작한다

        public int GetX() { return m_iPos[0]; }
        public int GetY() { return m_iPos[1]; }

        public void Update(CNavigationData ND, CAStarPathFinding FF)
        {
            if (!m_bMove)
            {
                if (m_Target != null)
                {//
                    int dx = m_iPos[0] - m_Target.GetX();
                    int dy = m_iPos[1] - m_Target.GetY();

                    if (m_bTracking)
                    {//타겟을 추적하는 모드라면

                        if (m_bStartTracking)
                        {
                            if (Math.Abs(dx) > 3 || Math.Abs(dy) > 3)
                            {
                                CNaviNode pStart = CNaviNode.Create(m_iPos[0], m_iPos[1]);
                                CNaviNode pEnd = CNaviNode.Create(m_Target.GetX(), m_Target.GetY());
                                m_vecPath = new List<CNaviNode>();
                                if (!FF.FindPath(pStart, pEnd, ref m_vecPath, ND))
                                {
                                }
                                else
                                {
                                    m_bMove = true;
                                }
                            }
                        }
                    }
                    else
                    {//타겟을 회피하는 모드다
                        if (Math.Abs(dx) < 4 && Math.Abs(dy) < 4 && !m_bMove)
                        {
                            while (true)
                            {
                                int cx = m_RND.Next(ND.GetWidth());
                                int cy = m_RND.Next(ND.GetHeight());
                                if (ND.IsValidPos(cx, cy))
                                {
                                    m_vecPath = new List<CNaviNode>();
                                    CNaviNode pStart = CNaviNode.Create(m_iPos[0], m_iPos[1]);
                                    CNaviNode pEnd = CNaviNode.Create(cx, cy);
                                    if (!FF.FindPath(pStart, pEnd, ref m_vecPath, ND))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        m_bMove = true;
                                        break;
                                    }
                                }
                            }
                        }                        
                    }
                }
            }
            else
            {
                if (m_vecPath == null)
                {
                    m_bMove = false;
                }
                else
                {
                    int curindex = m_vecPath.Count - 1;
                    if (curindex < 0)
                    {
                        m_bMove = false;
                        m_vecPath = null;
                    }
                    else
                    {
                        m_iPos[0] = m_vecPath[curindex].x;
                        m_iPos[1] = m_vecPath[curindex].y;
                        m_vecPath.RemoveAt(curindex);
                    }
                }
            }
        }

        
    }
}
