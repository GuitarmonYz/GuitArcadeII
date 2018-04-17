using System;
namespace FastEMD {
    public class Feature{
        public double x;
        public double y;
        public Feature(double x, double y) {
            this.x = x;
            this.y = y;
        }
        public double groudDist(Feature f){
            double dx = x - f.x;
            double dy = y - f.y;
            return Math.Sqrt(dx*dx + dy*dy);
        }
    }
}