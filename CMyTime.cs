using System;
using System.Collections.Generic;
using System.Text;

namespace pathtest
{
    //심플 타임체커
    class CMyTime
    {
        private long m_lOld = DateTime.Now.Ticks;

        //현재 시간을 캐치
        public void Catch()
        {
            m_lOld = DateTime.Now.Ticks;
        }

        //현재부터 이전에 시간을 캐치한 시점까지의 경과 시간을 틱단위로 얻는다
        public long GetPassedTime()
        {
            return (DateTime.Now.Ticks - m_lOld);
        }

        //현재부터 이전에 시간을 캐치한 시점까지의 경과 시간을 초단위로 얻는다
        public float GetSecond()
        {
            return (float)((DateTime.Now.Ticks - m_lOld) * 0.0000001f);
        }
    }
}
