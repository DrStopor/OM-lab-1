using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.UI;
using System.Drawing;

namespace WindowsFormsApplication2.Core
{
    class GraphQueueTime : MyGraph
    {
        // Fields
        private int curHeight;
        private List<Pair> fillQueue;
        private int queueLong;
        private double scale;
        private double scaleY;

        // Methods
        public GraphQueueTime(PictureBox pict, Point centr, List<Pair> fillQueue1, int queueLong1) : base(pict, centr)
        {
            this.queueLong = queueLong1;
            this.fillQueue = fillQueue1;
            pict.Paint += new PaintEventHandler(this.pict_Paint);
            this.sortQueue();
        }

        public override void drawGraph()
        {
            this.curHeight = 50;
            this.drawVerticalAxis();
            this.scale = ((base.picture.Width - ((2 * base.picture.Width) / 50)) * 1.0) / ((double)(MyGraph.t2 - MyGraph.t1));
            this.scaleY = ((base.picture.Height - ((3 * base.picture.Height) / 10)) * 1.0) / 50.0;
            for (int i = 0; i < this.fillQueue.Count; i++)
            {
                if (i != (this.fillQueue.Count - 1))
                {
                    this.drawQueueTimeElement(this.fillQueue[i], this.fillQueue[i + 1]);
                }
                else
                {
                    this.drawQueueTimeElement(this.fillQueue[i], this.fillQueue[i]);
                }
            }
        }

        private void drawQueueTimeElement(Pair pair, Pair pairNext)
        {
            Pen pen = new Pen(Color.Aqua, 2f);
            int num = (int)(this.scale * (Convert.ToInt32(pair.First) - MyGraph.t1));
            int num2 = (int)(this.scale * (Convert.ToInt32(pairNext.First) - MyGraph.t1));
            base.formGraphics.DrawLine(new Pen(Color.Gray, 1f), new Point(num + (base.picture.Width / 50), ((int)(this.scaleY * this.curHeight)) + ((base.picture.Height / 10) * 2)), new Point(num + (base.picture.Width / 50), 0));
            Point[] points = new Point[3];
            points[0] = new Point(num + (base.picture.Width / 50), ((int)(this.scaleY * this.curHeight)) + ((base.picture.Height / 10) * 2));
            if (Convert.ToInt32(pair.Second) == 1)
            {
                this.curHeight--;
            }
            if (Convert.ToInt32(pair.Second) == -1)
            {
                this.curHeight++;
            }
            points[1] = new Point(num + (base.picture.Width / 50), ((int)(this.scaleY * this.curHeight)) + ((base.picture.Height / 10) * 2));
            points[2] = new Point(num2 + (base.picture.Width / 50), ((int)(this.scaleY * this.curHeight)) + ((base.picture.Height / 10) * 2));
            base.formGraphics.DrawLines(pen, points);
        }

        private void drawVerticalAxis()
        {
            int num = 50;
            int num2 = 0;
            base.drawText("N очередь", new Point(this.centr.X, 0));
            for (int i = 0; i < 10; i++)
            {
                base.drawText(num.ToString(), new Point(this.centr.X, (((base.picture.Height / 10) * 2) + num2) - (base.picture.Height / 20)));
                base.formGraphics.DrawLine(new Pen(Color.Gray, 1f), new Point(0, ((base.picture.Height / 10) * 2) + num2), new Point(base.picture.Width, ((base.picture.Height / 10) * 2) + num2));
                num2 += (base.picture.Height - ((3 * base.picture.Height) / 10)) / 10;
                num -= 5;
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

        private void sortQueue()
        {
            for (int i = 0; i < this.fillQueue.Count; i++)
            {
                for (int j = this.fillQueue.Count - 1; j > i; j--)
                {
                    if (Convert.ToInt32(this.fillQueue[j - 1].First) > Convert.ToInt32(this.fillQueue[j].First))
                    {
                        Pair pair = this.fillQueue[j - 1];
                        this.fillQueue[j - 1] = this.fillQueue[j];
                        this.fillQueue[j] = pair;
                    }
                }
            }
        }

    }
}
