using System;
using System.Collections.Generic;
using System.Text;

namespace pathtest
{
    class CAStarPathFinding
    {
        private List<CNaviNode> m_vecOpenNode = null;
		private List<CNaviNode> m_vecCloseNode = null;
		
		
		public void Delete()
		{
			m_vecOpenNode = null;
			m_vecCloseNode = null;
			m_vecOpenNode = new List<CNaviNode>();
			m_vecCloseNode = new List<CNaviNode>();
		}

        //시작 위치 PStart에서 목표위치 pEnd까지의 경로를 구한다. 경로가 구해진경우 true그렇지 않은경우 fale
        //vecPath : 구해진경로
        //pNavi : 지형데이터를 제공한다
		public bool FindPath(CNaviNode pStart, CNaviNode pEnd, ref List<CNaviNode> vecPath, CNavigationData pNavi)
		{
			Delete();

			CNaviNode pNode = pStart.Clone();
			
			/*
			 * insert start position to open node, 시작점을 열린노드에 삽입한다.
			 * */
			m_vecOpenNode.Add(pNode);

			int iDepth = 0;
			pNode.depth = iDepth;

			List<CNaviNode> vecChilds;
			
			vecChilds = new List<CNaviNode>();
			
			while(true)
			{
				if (m_vecOpenNode.Count == 0)
				{//if opennode has not contents, it's meaning that path not found. 만일 열린노드에 더이상 데이터가 없다면 길이 존재하지 않는것이다.
					break;
				}



				pNode = m_vecOpenNode[0]; //get first content, 열린노드의 가장처음항목을 하나 가져온다
				m_vecOpenNode.RemoveAt(0); //delete content from open node, 가져온것은 열린노드에서 제거한다

				if (pEnd.IsSamePos(pNode)) //if that node is end position, we found path, 만일 가져온 노드가 목표점이라면 해당 노드를 패스목록에 추가하고 길탐색을 종료한다
				{
					while(pNode != null) //tracking it's parent node for it's parent is null
					{
						vecPath.Add(pNode); //add node to path list
						pNode = pNode.GetParent(); //get current node's parent
					}

					return true;
				}

				pNode = InsertCloseNode(pNode); //insert current node to close list, 목표점이 아니면 해당 노드를 닫힌노드에 삽입한다.


                ++iDepth; //탐색깊이를 하나 증가 시킨다
				vecChilds.Clear(); 
				pNavi.GetNeighbor(pNode, ref vecChilds);    //해당노드의 인접한 노드들을 모두 가져와서

				for (int i = 0;i < vecChilds.Count; ++i)
				{
					if (FindFromCloseNode(vecChilds[i]))    //만일 닫힌노드에 있는것이면 무시하고
					{
						continue;
					}

                    //닫힌노드에 없는것이라면, 거리를 구한다음에 열린노드에 삽입한다.
					vecChilds[i].CalcDist(pEnd, iDepth);
					vecChilds[i].SetParent(pNode);
					InsertOpenNode(vecChilds[i]);
				}

                //열린노드를 비용에 따라서 정렬한다
				SortOpenNode();
			}

			Delete();
			return false;
		}

        //노드 p1이 노드 p2보다 저비용이라면(거리가 더가까우며, 탐색깊이가 더 작은지) true
		private bool NodeCompare(CNaviNode p1, CNaviNode p2)
		{
			if (p1.dist < p2.dist) return true;
			if (p1.dist > p2.dist) return false;
			if (p1.depth <= p2.depth) return true;
			return false;
		}

        //열란노드에 노드 삽입, 중복된 노드가 삽입되지 않도록 처리한다
		private void InsertOpenNode(CNaviNode pNode)
		{
			for (int i = 0;i < m_vecOpenNode.Count; ++i)
			{
				if (m_vecOpenNode[i].IsSamePos(pNode))
				{
					InsertCloseNode(m_vecOpenNode[i]);
					m_vecOpenNode[i] = pNode;
					return;
				}
			}

			m_vecOpenNode.Add(pNode);
		}

        //닫힌노드에 삽입
		private CNaviNode InsertCloseNode(CNaviNode pNode)
		{
			m_vecCloseNode.Add(pNode);
			return pNode;
		}

        //열린 노드를 비용에 따라서 정렬한다
		private void SortOpenNode()
		{
			if (m_vecOpenNode.Count < 2) return;
			CNaviNode pNode;

			bool bContinue = true;

			while(bContinue)
			{
				bContinue = false;
				for (int i = 0;i < m_vecOpenNode.Count - 1; ++i)
				{
					if (!NodeCompare(m_vecOpenNode[i], m_vecOpenNode[i+1]))
					{
						pNode = m_vecOpenNode[i];
						m_vecOpenNode[i] = m_vecOpenNode[i+1];
						m_vecOpenNode[i+1] = pNode;
						bContinue = true;
					}
				}
			}
		}


        //열린노드에 해당 노드가 있는지 확인한다
		private bool FindFromOpenNode(CNaviNode pNode)
		{
			for (int i = 0;i < m_vecOpenNode.Count; ++i)
			{
				if (m_vecOpenNode[i].IsSamePos(pNode)) return true;
			}
			return false;
		}

        //닫힌노드에 해당 노드가 있는지 확인한다
		private bool FindFromCloseNode(CNaviNode pNode)
		{
			for (int i = 0;i < m_vecCloseNode.Count; ++i)
			{
				if (m_vecCloseNode[i].IsSamePos(pNode)) return true;
			}
			return false;
		}
    }
}
