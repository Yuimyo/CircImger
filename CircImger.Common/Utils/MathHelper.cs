using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircImger.Common.Utils
{
    public static class MathHelper
    {
        public static int Chmin(ref int value, int target) => value = (value > target) ? target : value;
        public static int Chmax(ref int value, int target) => value = (value < target) ? target : value;
        public static float Chmin(ref float value, float target) => value = (value > target) ? target : value;
        public static float Chmax(ref float value, float target) => value = (value < target) ? target : value;
        public static double Chmin(ref double value, double target) => value = (value > target) ? target : value;
        public static double Chmax(ref double value, double target) => value = (value < target) ? target : value;
    }
}
