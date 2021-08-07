using System;

namespace LinesIntersection
{
    class Intersection
    {
        //Start and end coordinates of lines
        static double X1 = 37;
        static double Y1 = 18;
        static double X2 = 25;
        static double Y2 = 13;
        static double X3 = 23;
        static double Y3 = 20;
        static double X4 = 40;
        static double Y4 = 17;

        

        public void FindIntersection()
        {
            //Check if intersection is available
            if (Math.Max(X1, X2) < Math.Min(X3, X4) | Math.Max(Y1, Y2) < Math.Min(Y3, Y4))
            {
                Console.WriteLine("There are No avilable intersecions.");
                return;
            }

            double Xa = 0;
            double Ya = 0;

            //Check if lines are vertical
            if (X1 == X2 | X3 == X4)
                GetCoordinatesIfVertical(ref Ya);
            else
            {
                //Get coefficients values of line equation 
                double A1 = (Y1 - Y2) / (X1 - X2);
                double A2 = (Y3 - Y4) / (X3 - X4);
                double b1 = Y1 - (A1 * X1);
                double b2 = Y3 - (A2 * X3);

                //Parallel segments
                if (A1 == A2)
                {
                    Console.WriteLine("Lines are parallel.");
                    return;
                }

                //Get intersection coordinates
                Xa = (b2 - b1) / (A1 - A2);
                Ya = A1 * Xa + b1;
            }

            

            //Transform float to string with double precision
            string XaOut = String.Format("{0:0.00}", Xa);
            string YaOut = String.Format("{0:0.00}", Ya);

            //Output
            Console.WriteLine(XaOut);
            Console.WriteLine(YaOut);

            //Check if intersecion is out of bounds
            if (CheckIntersection(Xa, Ya) == false)
                Console.WriteLine("Intersection is out of bound");
            else
                Console.WriteLine("Intersection is present.");


            Console.ReadLine();
        }

        bool CheckIntersection(double Xa, double Ya)
        {
            if (Xa == 0)
            {
                 if (Ya < Math.Max(Math.Min(Y1, Y2), Math.Min(Y3, Y4)))
                    return false;
                else if (Ya > Math.Min(Math.Max(Y1, Y2), Math.Max(Y3, Y4)))
                    return false;
            }
            else
            {
                if (Xa < Math.Max(Math.Min(X1, X2), Math.Min(X3, X4)))
                    return false;
                else if (Xa > Math.Min(Math.Max(X1, X2), Math.Max(X3, X4)))
                    return false;
            }          

            return true;

        }

        void GetCoordinatesIfVertical(ref double Ya)
        {
            if (X1==X2)
            {
                double Xa = X1;

                double A2 = (Y3 - Y4) / (X3 - X4);
                double b2 = Y3 - (A2 * X3);
                Ya = A2 * Xa + b2;
            }
            else
            {
                double Xa = X3;
                double A1 = (Y1 - Y2) / (X1 - X2);
                double b1 = Y1 - (A1 * X1);

                Ya = A1 * Xa + b1;
            }
        }
            
    }
}
