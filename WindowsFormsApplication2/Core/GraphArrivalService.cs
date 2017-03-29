using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Core
{
    class GraphArrivalService : MyGraph
    {
        // Fields
        private double scale;

        // Methods
        public GraphArrivalService(PictureBox pict, Point centr) : base(pict, centr)
        {
            pict.Paint += new PaintEventHandler(this.pict_Paint);
        }

        private void drawArrival()
        {
            for (int i = 0; i < MyGraph.task.Length; i++)
            {
                if ((MyGraph.task[i][0] >= MyGraph.t1) && (MyGraph.task[i][0] <= MyGraph.t2))
                {
                    this.drawArrivalElement((int)MyGraph.task[i][0], i);
                }
            }
        }

        private void drawArrivalElement(int time, int i)
        {
            int num = (int)(this.scale * (time - MyGraph.t1));
            Pen pen = new Pen(Color.Gray, 1f);
            Point[] points = new Point[2];
            points[0].X = num + (base.picture.Width / 50);
            points[0].Y = base.picture.Height;
            points[1].X = num + (base.picture.Width / 50);
            points[1].Y = this.centr.Y - ((base.picture.Height / 10) * 7);
            base.formGraphics.DrawLines(pen, points);
            pen.Dispose();
            Point pt = new Point(points[1].X, points[1].Y - (base.picture.Height / 15));
            base.drawText("t" + i, pt);
        }

        public override void drawGraph()
        {
            this.scale = ((base.picture.Width - ((base.picture.Width / 50) * 2)) * 1.0) / ((double)(MyGraph.t2 - MyGraph.t1));
            this.drawArrival();
            this.drawService();
        }

        private void drawService()
        {
            int height = base.picture.Height;
            for (int i = 0; i < MyGraph.services.Count; i++)
            {
                height -= (base.picture.Height / 10) * 2;
                for (int j = 0; j < MyGraph.services[i].Count; j++)
                {
                    if (((MyGraph.services[i][j][0] + MyGraph.services[i][j][1]) >= MyGraph.t1) && (MyGraph.services[i][j][0] <= MyGraph.t2))
                    {
                        this.drawServiceElement(MyGraph.services[i][j], height);
                    }
                }
            }
        }

        private void drawServiceElement(double[] service, int height)
        {
            int num = (int)(this.scale * (service[0] - MyGraph.t1));
            int width = (int)(this.scale * service[1]);
            Rectangle rect = new Rectangle(num + (base.picture.Width / 50), height, width, base.picture.Height / 10);
            base.formGraphics.FillRectangle(new SolidBrush(Color.LightBlue), rect);
            base.formGraphics.DrawRectangle(new Pen(Color.Green, 1f), rect);
            Point pt = new Point((num + (width / 2)) - 5, height / 1);
            base.drawText("tau" + service[2], pt);
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
