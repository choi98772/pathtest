using System;
using System.Collections.Generic;
using System.Text;

namespace pathtest
{
    class CAStarPathFinding
    {
        private List<CNaviNode> mOpenNode = null;
		private List<CNaviNode> mCloseNode = null;
		
		
		public void Delete()
		{
            mOpenNode = null;
            mCloseNode = null;
            mOpenNode = new List<CNaviNode>();
            mCloseNode = new List<CNaviNode>();
		}

        //시작 위치 PStart에서 목표위치 pEnd까지의 경로를 구한다. 경로가 구해진경우 true그렇지 않은경우 fale
        //finalPath : 구해진경로
        //navigation : 지형데이터를 제공한다
        public bool FindPath(CNaviNode startPos, CNaviNode endPos, ref List<CNaviNode> finalPath, CNavigationData navigation)
		{
			Delete();

			CNaviNode currentNode = startPos.Clone();

            /*
			 * insert start position to open node, 시작점을 열린노드에 삽입한다.
			 * */
            mOpenNode.Add(currentNode);

			int depth = 0;

            currentNode.depth = depth;

			List<CNaviNode> childs;

            childs = new List<CNaviNode>();
			
			while(true)
			{
				if (mOpenNode.Count == 0)
				{//if opennode has not contents, it's meaning that path not found. 만일 열린노드에 더이상 데이터가 없다면 길이 존재하지 않는것이다.
					break;
				}



                currentNode = mOpenNode[0]; //get first content, 열린노드의 가장처음항목을 하나 가져온다

                mOpenNode.RemoveAt(0); //delete content from open node, 가져온것은 열린노드에서 제거한다

				if (endPos.IsSamePos(currentNode)) //if that node is end position, we found path, 만일 가져온 노드가 목표점이라면 해당 노드를 패스목록에 추가하고 길탐색을 종료한다
				{
					while(currentNode != null) //tracking it's parent node for it's parent is null
					{
                        finalPath.Add(currentNode); //add node to path list
                        currentNode = currentNode.GetParent(); //get current node's parent
					}

					return true;
				}

                currentNode = InsertCloseNode(currentNode); //insert current node to close list, 목표점이 아니면 해당 노드를 닫힌노드에 삽입한다.


                ++depth; //탐색깊이를 하나 증가 시킨다

                childs.Clear();

                navigation.GetNeighbor(currentNode, ref childs);    //해당노드의 인접한 노드들을 모두 가져와서

				for (int i = 0;i < childs.Count; ++i)
				{
					if (FindFromCloseNode(childs[i]))    //만일 닫힌노드에 있는것이면 무시하고
					{
						continue;
					}

                    //닫힌노드에 없는것이라면, 거리를 구한다음에 열린노드에 삽입한다.
                    childs[i].CalcDist(endPos, depth);
                    childs[i].SetParent(currentNode);
					InsertOpenNode(childs[i]);
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
			if (p1.dist < p2.dist)
                return true;

			if (p1.dist > p2.dist)
                return false;

			if (p1.depth <= p2.depth)
                return true;

			return false;
		}

        //열란노드에 노드 삽입, 중복된 노드가 삽입되지 않도록 처리한다
		private void InsertOpenNode(CNaviNode pNode)
		{
			for (int i = 0;i < mOpenNode.Count; ++i)
			{
				if (mOpenNode[i].IsSamePos(pNode))
				{
					InsertCloseNode(mOpenNode[i]);
                    mOpenNode[i] = pNode;
					return;
				}
			}

            mOpenNode.Add(pNode);
		}

        //닫힌노드에 삽입
		private CNaviNode InsertCloseNode(CNaviNode pNode)
		{
            mCloseNode.Add(pNode);
			return pNode;
		}

        //열린 노드를 비용에 따라서 정렬한다
		private void SortOpenNode()
		{
            //열린 노드에 오직 하나이하의 노드만 있으면 정렬할 필요없다.
			if (mOpenNode.Count < 2)
                return;

			CNaviNode node;

            //반복여부
			bool looping = true;

			while(looping)
			{
                looping = false;

				for (int i = 0;i < mOpenNode.Count - 1; ++i)
				{
					if (!NodeCompare(mOpenNode[i], mOpenNode[i+1]))
					{
                        node = mOpenNode[i];
                        mOpenNode[i] = mOpenNode[i+1];
                        mOpenNode[i+1] = node;
                        looping = true;
					}
				}
			}
		}


        //열린노드에 해당 노드가 있는지 확인한다
		private bool FindFromOpenNode(CNaviNode pNode)
		{
			for (int i = 0;i < mOpenNode.Count; ++i)
			{
				if (mOpenNode[i].IsSamePos(pNode))
                    return true;
			}

			return false;
		}

        //닫힌노드에 해당 노드가 있는지 확인한다
		private bool FindFromCloseNode(CNaviNode pNode)
		{
			for (int i = 0;i < mCloseNode.Count; ++i)
			{
				if (mCloseNode[i].IsSamePos(pNode))
                    return true;
			}

			return false;
		}
    }
}
