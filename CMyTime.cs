using System;
using System.Collections.Generic;
using System.Text;

namespace pathtest
{
    //심플 타임체커
    class CMyTime
    {
        private long mOldTime = DateTime.Now.Ticks;

        //현재 시간을 캐치
        public void Catch()
        {
            mOldTime = DateTime.Now.Ticks;
        }

        //현재부터 이전에 시간을 캐치한 시점까지의 경과 시간을 틱단위로 얻는다
        public long GetPassedTime()
        {
            return (DateTime.Now.Ticks - mOldTime);
        }

        //현재부터 이전에 시간을 캐치한 시점까지의 경과 시간을 초단위로 얻는다
        public float GetSecond()
        {
            return (float)((DateTime.Now.Ticks - mOldTime) * 0.0000001f);
        }
    }
}
