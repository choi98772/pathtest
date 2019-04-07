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
        const int MAX_UNIT = 200;

        CNavigationData m_ND = null;
        CAStarPathFinding m_PathFinding = null;
        Timer m_TM = null;
        Graphics m_Buf = null;
        Bitmap m_Bitmap = null;

        CUnit[] m_pUnits = null;


        public Form1()
        {
            InitializeComponent();
            InitializeComponent2();
            m_ND = new CNavigationData();
            m_PathFinding = new CAStarPathFinding();
            
            m_pUnits = new CUnit[MAX_UNIT];

            Random rnd = new Random();

            for (int i = 0;i < MAX_UNIT; ++i)
            {
                m_pUnits[i] = new CUnit();

                if (i == 0)
                {
                    m_pUnits[i].EnableTracking(true);
                    //m_pUnits[i].StartTracking(true);  //주석을 해제 하면 자동 추적한다
                }
                else
                {
                    m_pUnits[i].EnableTracking(false);
                    m_pUnits[i].SetTarget(m_pUnits[0]);
                }
            }
        }

        private void InitializeComponent2()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.ClientSize = new System.Drawing.Size(640, 640);
            this.Text = "PathTest";
            this.Paint += new PaintEventHandler(this.BasicX_Paint);
        }

        private void BasicX_Paint(object sender, PaintEventArgs e)
        {
            DrawMap();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            if (m_ND != null)
            {
                if (!m_ND.Load("map.txt"))
                {
                    MessageBox.Show("failed to loading map data");
                }
                else
                {
                    for (int i = 0;i < MAX_UNIT; ++i)
                    {
                        if (!m_pUnits[i].IsTracking())
                        {
                            m_pUnits[i].RandomPos(m_ND);
                        }
                    }
                }
            }

            m_TM = new Timer();
            m_TM.Interval = 1;
            m_TM.Tick += new EventHandler(Update);
            m_TM.Enabled = true;


            m_Bitmap = new Bitmap(640, 640);
            m_Buf = Graphics.FromImage(m_Bitmap);
        }

        void Update(object sender, EventArgs e)
        {
            if (m_ND != null)
            {
               for (int i = 0;i < MAX_UNIT; ++i)
               {
                   if (m_pUnits[i].IsTracking()) m_pUnits[i].SelectTarget(m_pUnits, MAX_UNIT);

                   m_pUnits[i].Update(m_ND, m_PathFinding);
               }
            }            

            DrawMap();

           
        }

        private void DrawMap()
        {
            if (m_ND == null) return;
            
            int cx, cy;
            for (int y = 0, py = 0;y < m_ND.GetHeight(); ++y, py += 16)
            {
                for (int x = 0, px = 0; x < m_ND.GetWidth(); ++x, px += 16)
                {
                    cx = x * 16;
                    cy = y * 16;
                    if (m_ND.IsValidPos(x, y))
                    {
                        m_Buf.FillRectangle(Brushes.Yellow, (float)cx, (float)cy, 16.0f, 16.0f);
                    }
                    else
                    {
                        m_Buf.FillRectangle(Brushes.Red, (float)cx, (float)cy, 16.0f, 16.0f);
                    }
                }
            }

            for (int i = 0; i < MAX_UNIT; ++i)
            {
                cx = m_pUnits[i].GetX() * 16;
                cy = m_pUnits[i].GetY() * 16;

                if (m_pUnits[i].IsTracking()) m_Buf.FillRectangle(Brushes.Blue, (float)cx, (float)cy, 16.0f, 16.0f);
                else m_Buf.FillRectangle(Brushes.Green, (float)cx, (float)cy, 16.0f, 16.0f);
            }
            Graphics g = CreateGraphics();
            g.DrawImage(m_Bitmap, 0.0f, 0.0f);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int cx = e.X / 16;
            int cy = e.Y / 16;
            base.OnMouseDown(e);

            if (m_PathFinding != null)
            {
                for (int i = 0;i < MAX_UNIT; ++i)
                {
                    if (m_pUnits[i].IsTracking())
                    {
                        CNaviNode pStart = CNaviNode.Create(m_pUnits[i].GetX(), m_pUnits[i].GetY());
                        CNaviNode pEnd = CNaviNode.Create(cx, cy);
                        List<CNaviNode> vecPath = new List<CNaviNode>();
                        if (!m_PathFinding.FindPath(pStart, pEnd, ref vecPath, m_ND))
                        {
                        }
                        else
                        {
                            m_pUnits[i].SetPath(vecPath);
                        }
                    }
                }                
            }
        }
    }
}