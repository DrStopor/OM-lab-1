using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Core
{
    abstract class MyGraph
    {

        // Fields
        public Point centr;
        protected Graphics formGraphics;
        protected PictureBox picture;
        public static List<List<double[]>> services;
        public static int t1;
        public static int t2;
        public static double[][] task;

        // Methods
        static MyGraph()
        {
            t1 = 0;
            t2 = 100;
        }

        public MyGraph(PictureBox pict, Point centr)
        {
            this.centr = centr;
            this.picture = pict;
            this.formGraphics = pict.CreateGraphics();
            this.initialization();
        }

        private void drawCoordinateAxis()
        {
            Pen pen = new Pen(Color.Black, 3f);
            Point[] points = new Point[] { new Point(this.centr.X, 10), this.centr, new Point(this.picture.Width - 10, this.centr.Y) };
            this.formGraphics.DrawLines(pen, points);
            this.formGraphics.DrawLine(pen, this.centr, new Point(0, this.centr.Y));
            Point[] pointArray2 = new Point[3];
            pointArray2[0].X = points[0].X - 6;
            pointArray2[0].Y = points[0].Y + 6;
            pointArray2[1] = points[0];
            pointArray2[2].X = points[0].X + 6;
            pointArray2[2].Y = points[0].Y + 6;
            this.formGraphics.DrawLines(pen, pointArray2);
            pointArray2[0].X = points[2].X - 6;
            pointArray2[0].Y = points[2].Y - 6;
            pointArray2[1] = points[2];
            pointArray2[2].X = points[2].X - 6;
            pointArray2[2].Y = points[2].Y + 6;
            this.formGraphics.DrawLines(pen, pointArray2);
            pen.Dispose();
            Point pt = new Point(pointArray2[0].X - 20, pointArray2[0].Y + 10);
            this.drawText("t,сек", pt);
        }

        public abstract void drawGraph();

        protected void drawText(string str, Point pt)
        {
            Font font = new Font("Arial", 10f);
            SolidBrush brush = new SolidBrush(Color.Black);
            this.formGraphics.DrawString(str, font, brush, (PointF)pt);
            brush.Dispose();
        }

        public void initialization()
        {
            this.formGraphics.Clear(Color.White);
            this.drawCoordinateAxis();
        }

    }
}
