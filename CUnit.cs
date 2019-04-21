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

        //이동 경로 설정하기
        public void SetPath(List<CNaviNode> vecPath)
        {
            m_vecPath = vecPath;
            m_bMove = true;
        }

        //추적자인가?
        public bool IsTracking() { return m_bTracking; }

        //맵에서 임의의 위치를 선택해서 현재위치를 설정하기
        public void RandomPos(CNavigationData ND)
        {
            //유효한 임의의 위치를 구할때까지 반복
            while (true)
            {
                //임의의 위치를 구하고
                int cx = m_RND.Next(ND.GetWidth());
                int cy = m_RND.Next(ND.GetHeight());

                //해당 위치가 이동가능한 위치면, 유닛 좌표를 해당위치로 설정한후 while 종료
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

        //목표유닛 설정
        public void SetTarget(CUnit target) { m_Target = target; }

        public void EnableTracking(bool bEnable) { m_bTracking = bEnable; } //추적 모드를 활성화 한다.
        public void StartTracking(bool bStart) { m_bStartTracking = bStart; }   //추적을 시작한다

        //유닛의 맵좌표 얻기
        public int GetX() { return m_iPos[0]; }
        public int GetY() { return m_iPos[1]; }

        //매프레임 호출되어 유닛을 이동시킴
        public void Update(CNavigationData ND, CAStarPathFinding FF)
        {
            //현재 이동중이지 않을 경우
            if (!m_bMove)
            {
                //묙표유닛이 존재한다면
                if (m_Target != null)
                {//
                    //묙표유닛과 자신과의 거리를 구한다
                    int dx = m_iPos[0] - m_Target.GetX();
                    int dy = m_iPos[1] - m_Target.GetY();

                    if (m_bTracking)
                    {//타겟을 자동추적하는 모드라면

                        //추적중이라면
                        if (m_bStartTracking)
                        {
                            //목표와의 거리가 3셀이상이면
                            if (Math.Abs(dx) > 3 || Math.Abs(dy) > 3)
                            {
                                //나의 위치를 시작점, 목표위치를 끝점으로해서
                                CNaviNode pStart = CNaviNode.Create(m_iPos[0], m_iPos[1]);
                                CNaviNode pEnd = CNaviNode.Create(m_Target.GetX(), m_Target.GetY());

                                //경로를 구하고
                                m_vecPath = new List<CNaviNode>();
                                if (!FF.FindPath(pStart, pEnd, ref m_vecPath, ND))
                                {
                                }
                                else
                                {
                                    //경로가 구해졌으면 유닛이동을 활성화
                                    m_bMove = true;
                                }
                            }
                        }
                    }
                    else
                    {//타겟을 회피하는 모드다

                        //목표와의 거리가 4셀 이하이고, 현재 이동중이지 않으면
                        if (Math.Abs(dx) < 4 && Math.Abs(dy) < 4 && !m_bMove)
                        {
                            //무한반복
                            while (true)
                            {
                                //맵에서 임의의 위치를 하나 선택하고
                                int cx = m_RND.Next(ND.GetWidth());
                                int cy = m_RND.Next(ND.GetHeight());

                                //해당 위치가 이동가능한곳이면
                                if (ND.IsValidPos(cx, cy))
                                {
                                    //유닛의 현재 위치를 시작점, 위에서 선택한 임의의 위치를 끝점으로 해서
                                    CNaviNode pStart = CNaviNode.Create(m_iPos[0], m_iPos[1]);
                                    CNaviNode pEnd = CNaviNode.Create(cx, cy);

                                    //경로를 구하고
                                    m_vecPath = new List<CNaviNode>();
                                    if (!FF.FindPath(pStart, pEnd, ref m_vecPath, ND))
                                    {
                                        //경로가 구해지지 않았으면 while 루프를 다시 반복
                                        continue;
                                    }
                                    else
                                    {
                                        //경로가 구해졌으면 유닛을 이동상태로 설정하고, while 루프를 종료
                                        m_bMove = true;
                                        break;
                                    }
                                }
                            }
                        }                        
                    }
                }
            }
            else //유닛이 현재 이동중인경우면
            {
                //경로가 존재하지 않으면, 이동모드를 중지
                if (m_vecPath == null)
                {
                    m_bMove = false;
                }
                else
                {
                    int curindex = m_vecPath.Count - 1;

                    //경로의 목표점에 도달했으면
                    if (curindex < 0)
                    {
                        //이동모드를 중지하고, 경로는 클리어
                        m_bMove = false;
                        m_vecPath = null;
                    }
                    else
                    {
                        //경로의 현재 점을 유닛의 위치로 설정하고, 사용한 좌표는 경로 목록에서 제거하기
                        m_iPos[0] = m_vecPath[curindex].x;
                        m_iPos[1] = m_vecPath[curindex].y;
                        m_vecPath.RemoveAt(curindex);
                    }
                }
            }
        }

        
    }
}
