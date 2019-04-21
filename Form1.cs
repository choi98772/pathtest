using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace pathtest
{
    public partial class Form1 : Form
    {
        /*
         * 맵은 n * n개의 셀로 구성된 이차원 배열입니다.
         * 각셀은 화면에 CELL_SIZE * CELL_SIZE 크기의 박스로 그려집니다.
         * */
        const int CELL_SIZE = 16;
        const int MAX_UNIT = 200;

        CNavigationData mNavigationData = null; //맵데이터
        CAStarPathFinding mAStar = null; //길찾기 알고리즘
        Timer mTimer = null; //맵의 그리기 위한 요도의 타이머
        Graphics mGraphicsBuffer = null; 
        Bitmap mBitmap = null; //맵 데이터를 위한 비트맵 객체

        CUnit[] mUnits = null; //화면에 표시되는 유닛들


        public Form1()
        {
            InitializeComponent();
            InitializeComponent2();

            mNavigationData = new CNavigationData();
            mAStar = new CAStarPathFinding();

            mUnits = new CUnit[MAX_UNIT];

            for (int i = 0;i < MAX_UNIT; ++i)
            {
                mUnits[i] = new CUnit();

                if (i == 0)
                {
                    //가정 첫번째 유닛을 추적자로 설정
                    mUnits[i].EnableTracking(true);
                    //m_pUnits[i].StartTracking(true);  //주석을 해제 하면 자동 추적한다
                }
                else
                {
                    mUnits[i].EnableTracking(false);
                    mUnits[i].SetTarget(mUnits[0]);
                }
            }
        }

        private void InitializeComponent2()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Text = "PathTest";
            this.Paint += new PaintEventHandler(this.BasicX_Paint);
        }

        private void BasicX_Paint(object sender, PaintEventArgs e)
        {
            DrawMap();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //맵 데이터를 파싱
            if (mNavigationData != null)
            {
                if (!mNavigationData.Load("map.txt"))
                {
                    MessageBox.Show("failed to loading map data");
                }
                else
                {
                    for (int i = 0;i < MAX_UNIT; ++i)
                    {
                        if (!mUnits[i].IsTracking())
                        {
                            mUnits[i].RandomPos(mNavigationData);
                        }
                    }
                }
            }

            //맵의 셀개수를 기반으로 클라이언트 사이즈를 조정합니다. 윈도우의 크기는 (셀의 가로크기 * 셀사이즈) * (셀의 세로크기 * 셀사이즈) 가됩니다.
            this.ClientSize = new System.Drawing.Size(mNavigationData.GetWidth() * CELL_SIZE, mNavigationData.GetHeight() * CELL_SIZE);

            mTimer = new Timer();
            mTimer.Interval = 1;
            mTimer.Tick += new EventHandler(Update);
            mTimer.Enabled = true;

            //맵을 비트맵에 그리기 위해서 맵의 픽셀크기 만큼의 비트맵을 만듭니다.
            mBitmap = new Bitmap(mNavigationData.GetWidth() * CELL_SIZE, mNavigationData.GetWidth() * CELL_SIZE);
            mGraphicsBuffer = Graphics.FromImage(mBitmap);
        }

        void Update(object sender, EventArgs e)
        {
            if (mNavigationData != null)
            {
               for (int i = 0;i < MAX_UNIT; ++i)
               {
                   if (mUnits[i].IsTracking()) mUnits[i].SelectTarget(mUnits, MAX_UNIT);

                    mUnits[i].Update(mNavigationData, mAStar);
               }
            }            

            DrawMap();
        }

        private void DrawMap()
        {
            if (mNavigationData == null)
                return;
            
            int cx, cy;

            //먼저 맵데이터를 기반으로 맵을 그려줍니다.
            for (int y = 0, py = 0;y < mNavigationData.GetHeight(); ++y, py += CELL_SIZE)
            {
                for (int x = 0, px = 0; x < mNavigationData.GetWidth(); ++x, px += CELL_SIZE)
                {
                    cx = x * CELL_SIZE;
                    cy = y * CELL_SIZE;

                    if (mNavigationData.IsValidPos(x, y))
                    {
                        //이동가능한 지점이면 노란색 박스로
                        mGraphicsBuffer.FillRectangle(Brushes.Yellow, (float)cx, (float)cy, CELL_SIZE, CELL_SIZE);
                    }
                    else
                    {
                        //이동불가능한 지점이면 빨간색 박스로
                        mGraphicsBuffer.FillRectangle(Brushes.Red, (float)cx, (float)cy, CELL_SIZE, CELL_SIZE);
                    }
                }
            }

            //다음으로 유닛들을 화면에 그려줍니다.
            for (int i = 0; i < MAX_UNIT; ++i)
            {
                cx = mUnits[i].GetX() * CELL_SIZE;
                cy = mUnits[i].GetY() * CELL_SIZE;

                if (mUnits[i].IsTracking())
                {
                    //추적자이면 파란색 박스로
                    mGraphicsBuffer.FillRectangle(Brushes.Blue, (float)cx, (float)cy, CELL_SIZE, CELL_SIZE);
                }
                else
                {
                    //아니면 녹색 박스로
                    mGraphicsBuffer.FillRectangle(Brushes.Green, (float)cx, (float)cy, CELL_SIZE, CELL_SIZE);
                }
            }

            //비트맵의 내용을 화면에 갱신해줍니다.
            Graphics g = CreateGraphics();
            g.DrawImage(mBitmap, 0.0f, 0.0f);
        }

        //마우스 버튼이 눌린경우
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //어느셀이 선택되었는지 확인하기
            int cx = e.X / CELL_SIZE;
            int cy = e.Y / CELL_SIZE;

            base.OnMouseDown(e);

            if (mAStar != null)
            {
                for (int i = 0;i < MAX_UNIT; ++i)
                {
                    //추적중인 유닛이면
                    if (mUnits[i].IsTracking())
                    {
                        //유닛의 현재위치를 시작지점으로
                        CNaviNode pStart = CNaviNode.Create(mUnits[i].GetX(), mUnits[i].GetY());

                        //마우스로 클릭한 지점의 셀 위치를 끝지점으로
                        CNaviNode pEnd = CNaviNode.Create(cx, cy);

                        List<CNaviNode> vecPath = new List<CNaviNode>();

                        //경로룰 구해서 구해지면 유닛에 설정해주기
                        if (mAStar.FindPath(pStart, pEnd, ref vecPath, mNavigationData))
                        {
                            //경로가 구해지면 경로를 유닛에 설정해주기
                            mUnits[i].SetPath(vecPath);
                        }
                    }
                }                
            }
        }
    }
}