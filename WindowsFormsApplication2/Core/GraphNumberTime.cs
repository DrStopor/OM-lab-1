using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Core
{
    class GraphNumberTime : MyGraph
    {
        // Fields
        private List<double> aborted;
        private int curHeight;
        private double scaleX;
        private double scaleY;

        // Methods
        public GraphNumberTime(PictureBox pict, Point centr, List<double> aborted1) : base(pict, centr)
        {
            this.aborted = aborted1;
            pict.Paint += new PaintEventHandler(this.pict_Paint);
        }

        private void drawAborted()
        {
            this.drawArray(new Pen(Color.Green, 2f), this.aborted);
        }

        private void drawArray(Pen pen, List<double> lst)
        {
            this.curHeight = MyGraph.task.Length;
            for (int i = 0; i < lst.Count; i++)
            {
                if (i != (lst.Count - 1))
                {
                    this.drawArrayElement(pen, lst[i], lst[i + 1]);
                }
                else
                {
                    this.drawArrayElement(pen, lst[i], lst[i]);
                }
            }
        }

        private void drawArrayElement(Pen pen, double cur, double next)
        {
            int num = (int)(this.scaleX * (cur - MyGraph.t1));
            int num2 = (int)(this.scaleX * (next - MyGraph.t1));
            Point[] points = new Point[3];
            points[0] = new Point(num + (base.picture.Width / 50), ((int)(this.scaleY * this.curHeight)) + ((base.picture.Height / 10) * 2));
            this.curHeight--;
            points[1] = new Point(num + (base.picture.Width / 50), ((int)(this.scaleY * this.curHeight)) + ((base.picture.Height / 10) * 2));
            points[2] = new Point(num2 + (base.picture.Width / 50), ((int)(this.scaleY * this.curHeight)) + ((base.picture.Height / 10) * 2));
            base.formGraphics.DrawLines(pen, points);
        }

        private void drawArrivaled()
        {
            List<double> lst = new List<double>();
            for (int i = 0; i < MyGraph.task.Length; i++)
            {
                lst.Add(MyGraph.task[i][0]);
            }
            this.drawArray(new Pen(Color.Red, 2f), lst);
        }

        public override void drawGraph()
        {
            this.drawVerticalAxis();
            this.scaleY = ((base.picture.Height - ((base.picture.Height / 10) * 3)) * 1.0) / ((double)MyGraph.task.Length);
            this.scaleX = ((base.picture.Width - ((base.picture.Width / 50) * 2)) * 1.0) / ((double)(MyGraph.t2 - MyGraph.t1));
            this.drawAborted();
            this.drawServiced();
            this.drawArrivaled();
        }

        private void drawServiced()
        {
            List<double> lst = new List<double>();
            for (int i = 0; i < MyGraph.services.Count<List<double[]>>(); i++)
            {
                for (int j = 0; j < MyGraph.services[i].Count; j++)
                {
                    lst.Add(MyGraph.services[i][j][0] + MyGraph.services[i][j][1]);
                }
            }
            lst.Sort();
            this.drawArray(new Pen(Color.Yellow, 2f), lst);
        }

        private void drawVerticalAxis()
        {
            int length = MyGraph.task.Length;
            int num2 = 0;
            base.formGraphics.DrawString("Narrival", new Font("Arial", 8f), new SolidBrush(Color.Red), (PointF)new Point(this.centr.X, 0));
            base.formGraphics.DrawString("Nserviced", new Font("Arial", 8f), new SolidBrush(Color.Yellow), (PointF)new Point(this.centr.X + 40, 0));
            base.formGraphics.DrawString("Naborted", new Font("Arial", 8f), new SolidBrush(Color.Green), (PointF)new Point(this.centr.X + 100, 0));
            for (int i = 0; i < 10; i++)
            {
                base.drawText(length.ToString(), new Point(this.centr.X, (((base.picture.Height / 10) * 2) + num2) - (base.picture.Height / 20)));
                base.formGraphics.DrawLine(new Pen(Color.Gray, 1f), new Point(0, ((base.picture.Height / 10) * 2) + num2), new Point(base.picture.Width, ((base.picture.Height / 10) * 2) + num2));
                num2 += (base.picture.Height - ((3 * base.picture.Height) / 10)) / 10;
                length -= MyGraph.task.Length / 10;
            }
        }

        private void pict_Paint(object sender, PaintEventArgs e)
        {
            base.formGraphics = e.Graphics;
            this.centr.X = base.picture.Width / 50;
            this.centr.Y = base.picture.Height - (base.picture.Height / 10);
            base.initialization();
            this.drawGraph();
        }
    }
}
