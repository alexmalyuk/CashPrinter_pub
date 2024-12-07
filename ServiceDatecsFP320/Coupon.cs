using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDatecsFP320
{
    public class Coupon
    {
        private const int nonPrintHeaderHeight = 45;
        private const int lineHeight = 15;
        private const int lineInterval = 6;
        private const decimal pixelRatio = 2.045455M;
        private const int maxCouponHeight = 1440;
        private int lastCouponHeight = 0;
        private int couponCount = 0;
        //private int currentPositionX = 0;
        private int currentPositionY = 0;

        public struct CouponLine 
        {
            public string Text;
            public bool DoubleHeight;
            public bool DoubleWidth;
            public int PositionX;
            public int PositionY;
            public int CouponNumber;
        }

        List<CouponLine> couponLines;

        public Coupon()
        {
            couponLines = new List<CouponLine>();
            couponCount = 0;
        }

        public void AddTextLine(CouponLine couponLine)
        {
            if (couponLine.DoubleHeight)
                currentPositionY += (int)Math.Round(lineInterval*1.5M);
            
            couponLines.Add(couponLine);
            
            currentPositionY += lineHeight * (couponLine.DoubleHeight ? 2 : 1) + lineInterval;
            lastCouponHeight = (int)Math.Round(currentPositionY * pixelRatio) + nonPrintHeaderHeight;

            if (lastCouponHeight > maxCouponHeight)
            {
                couponCount++;
                currentPositionY = 0;
            }
        }


    }
}
