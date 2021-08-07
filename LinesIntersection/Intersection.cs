using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LinesIntersection
{
    class Intersection
    {
        public static List<LineCoordinates> InputXY = new List<LineCoordinates>()
        {
            new LineCoordinates() {},
            new LineCoordinates() {}
        };

        public void Calculte(ref bool IntersectionExist, out double Xa, out double Ya, 
            out double A1, out double A2)
        {
            Xa = 0;
            Ya = 0;
            A1 = 0;
            A2 = 0;

            if (IfIntersectionAvailable() == false)
            {
                //MessageBox.Show("There are No avilable intersecions.");
                return;
            }


            //Check if lines are vertical
            if (InputXY[0].X1 == InputXY[0].X2 || InputXY[1].X1 == InputXY[1].X2)
                GetCoordinatesIfVertical(ref Xa, ref Ya);
            else
            {
                //Get coefficients values of line equation 
                A1 = (InputXY[0].Y1 - InputXY[0].Y2) / (InputXY[0].X1 - InputXY[0].X2);
                A2 = (InputXY[1].Y1 - InputXY[1].Y2) / (InputXY[1].X1 - InputXY[1].X2);
                double b1 = InputXY[0].Y1 - (A1 * InputXY[0].X1);
                double b2 = InputXY[1].Y1 - (A2 * InputXY[1].X1);

                //Parallel segments
                if (A1 == A2)
                {
                    //MessageBox.Show("Lines are parallel.");
                    return;
                }

                //Get intersection coordinates
                Xa = (b2 - b1) / (A1 - A2);
                Ya = A1 * Xa + b1;
            }

            //Check if intersecion is out of bounds
            if (CheckIntersection(Xa, Ya) == false)
            {
                return;
            }

            IntersectionExist = true;

            Point3d InterPoint = new Point3d(Xa, Ya, 0);
            Main.IntersectionsList.Add(InterPoint);           
        }

        static bool CheckIntersection(double Xa, double Ya)
        {
            if (Xa == 0)
            {
                if (Ya < Math.Max(Math.Min(InputXY[0].Y1, InputXY[0].Y2), Math.Min(InputXY[1].Y1, InputXY[1].Y2)))
                    return false;
                else if (Ya > Math.Min(Math.Max(InputXY[0].Y1, InputXY[0].Y2), Math.Max(InputXY[1].Y1, InputXY[1].Y2)))
                    return false;
            }
            else
            {
                if (Xa < Math.Max(Math.Min(InputXY[0].X1, InputXY[0].X2), Math.Min(InputXY[1].X1, InputXY[1].X2)))
                    return false;
                else if (Xa > Math.Min(Math.Max(InputXY[0].X1, InputXY[0].X2), Math.Max(InputXY[1].X1, InputXY[1].X2)))
                    return false;
            }

            return true;

        }

        static bool IfIntersectionAvailable()
        {
            //Check if intersection is available
            if (Math.Max(InputXY[0].X1, InputXY[0].X2) < Math.Min(InputXY[1].X1, InputXY[1].X2) |
                Math.Max(InputXY[0].Y1, InputXY[0].Y2) < Math.Min(InputXY[1].Y1, InputXY[1].Y2))
                return false;
            else if (Math.Max(InputXY[1].X1, InputXY[1].X2) < Math.Min(InputXY[0].X1, InputXY[0].X2) |
                Math.Max(InputXY[1].Y1, InputXY[1].Y2) < Math.Min(InputXY[0].Y1, InputXY[0].Y2))
                return false;
            return true;
        }

        static void GetCoordinatesIfVertical(ref double Xa, ref double Ya)
        {
            if (InputXY[0].X1 == InputXY[0].X2)
            {
                Xa = InputXY[0].X1;

                double A2 = (InputXY[1].Y1 - InputXY[1].Y2) / (InputXY[1].X1 - InputXY[1].X2);
                double b2 = InputXY[1].Y1 - (A2 * InputXY[1].X1);
                Ya = A2 * Xa + b2;
            }
            else
            {
                Xa = InputXY[1].X1;
                double A1 = (InputXY[0].Y1 - InputXY[0].Y2) / (InputXY[0].X1 - InputXY[0].X2);
                double b1 = InputXY[0].Y1 - (A1 * InputXY[0].X1);

                Ya = A1 * Xa + b1;
            }
        }

    }
}
