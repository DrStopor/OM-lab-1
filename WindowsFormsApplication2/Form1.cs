using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using WindowsFormsApplication2.Core;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.arrivalRate = 0.01;
            this.serviceRate = 0.01;
            this.t2 = 100;
            this.nQueueLong.Value = 1M;
        }

        // Fields
        private List<double> aborted;
        private double arrivalRate;
        private Button bStart;
        private IContainer components;
        private List<Pair> fillQueue;
        private GraphArrivalService graphArrivalService;
        private GraphNumberTime graphNumberTime;
        private GraphQueueTime graphQueueTime;
        private GroupBox groupBox;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private NumericUpDown nArrivalRate;
        private NumericUpDown nDegreeErlnga;
        private NumericUpDown nFinishTime;
        private NumericUpDown nQueueLong;
        private NumericUpDown nServerCount;
        private NumericUpDown nServiceRate;
        private NumericUpDown nStartTime;
        private NumericUpDown nTaskCount;
        private PictureBox picArrivalService;
        private PictureBox picNumberTime;
        private PictureBox picQueueTime;
        private Queue<int> queue;
        private int queueLong;
        private QueueStates queueState;
        private int serverCount;
        private double serviceRate;
        private List<List<double[]>> services;
        private int t1;
        private double t1ServerFree;
        private int t2;
        private double t2ServerFree;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private double[][] task;
        private int taskCount;
        private List<double> tServerFree;

        // Methods
        private void automat()
        {
            this.queueState = QueueStates.Empty;
            for (int i = 0; i < this.taskCount; i++)
            {
                switch (this.queueState)
                {
                    case QueueStates.Empty:
                        {
                            if (this.task[i][0] <= ((IEnumerable<double>)this.tServerFree).Min())
                            {
                                break;
                            }
                            this.getNextFromArrival(i);
                            continue;
                        }
                    case QueueStates.NotEmpty:
                        {
                            this.getNextFromQueue(i);
                            continue;
                        }
                    case QueueStates.Fill:
                        {
                            this.getNextFromQueue(i);
                            continue;
                        }
                    default:
                        goto Label_0061;
                }
                this.getNextFromQueue(i);
                continue;
            Label_0061:
                MessageBox.Show("Не известное состояние очереди");
            }
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            this.serverCount = Convert.ToInt32(this.nServerCount.Value);
            this.arrivalRate = Convert.ToDouble(this.nArrivalRate.Value);
            this.serviceRate = Convert.ToDouble(this.nServiceRate.Value);
            this.queueLong = Convert.ToInt32(this.nQueueLong.Value);
            this.taskCount = Convert.ToInt32(this.nTaskCount.Value);
            this.t1 = Convert.ToInt32(this.nStartTime.Value);
            this.t2 = Convert.ToInt32(this.nFinishTime.Value);
            this.services = new List<List<double[]>>(this.serverCount);
            for (int i = 0; i < this.serverCount; i++)
            {
                this.services.Add(new List<double[]>());
            }
            this.tServerFree = new List<double>(this.serverCount);
            for (int j = 0; j < this.serverCount; j++)
            {
                this.tServerFree.Add(0.0);
            }
            this.task = new double[this.taskCount][];
            this.queue = new Queue<int>();
            this.fillQueue = new List<Pair>();
            this.aborted = new List<double>();
            this.generateTask();
            this.automat();
            MyGraph.services = this.services;
            MyGraph.task = this.task;
            MyGraph.t1 = this.t1;
            MyGraph.t2 = this.t2;
            this.graphArrivalService = new GraphArrivalService(this.picArrivalService, new Point(this.picArrivalService.Width / 50, this.picArrivalService.Height - (this.picArrivalService.Height / 10)));
            this.graphArrivalService.drawGraph();
            this.graphQueueTime = new GraphQueueTime(this.picQueueTime, new Point(this.picQueueTime.Width / 50, this.picQueueTime.Height - (this.picQueueTime.Height / 10)), this.fillQueue, this.queueLong);
            this.graphQueueTime.drawGraph();
            this.graphNumberTime = new GraphNumberTime(this.picNumberTime, new Point(this.picNumberTime.Width / 50, this.picNumberTime.Height - (this.picNumberTime.Height / 10)), this.aborted);
            this.graphNumberTime.drawGraph();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void freeQueue(int i)
        {
            int count = -1;
            for (int j = 0; j < this.services.Count; j++)
            {
                count = -1;
                do
                {
                    count = this.queue.Count;
                    for (int k = 0; (k < this.services[j].Count) && (this.queue.Count != 0); k++)
                    {
                        if (((double)this.queue.Last<int>()) == this.services[j][k][2])
                        {
                            if (this.task[i][0] > this.services[j][k][0])
                            {
                                this.queue.Dequeue();
                            }
                            break;
                        }
                    }
                }
                while (this.queue.Count != count);
            }
        }

        private void generateTask()
        {
            double num = 0.0;
            double num2 = 0.0;
            Random random = new Random();
            int num3 = Convert.ToInt32(this.nDegreeErlnga.Value) + 1;
            for (int i = 0; i < (this.taskCount * num3); i++)
            {
                num += this.t(random.NextDouble());
                num2 += this.tau(random.NextDouble());
                if ((i % num3) == 0)
                {
                    this.task[i / num3] = new double[2];
                    if ((i / num3) != 0)
                    {
                        this.task[i / num3][0] = num + this.task[(i / num3) - 1][0];
                    }
                    else
                    {
                        this.task[i / num3][0] = num;
                    }
                    this.task[i / num3][1] = num2;
                    num = 0.0;
                    num2 = 0.0;
                }
            }
        }

        private void getNextFromArrival(int i)
        {
            double[] item = new double[] { this.task[i][0], this.task[i][1], (double)i };
            int index = this.tServerFree.IndexOf(((IEnumerable<double>)this.tServerFree).Min());
            this.tServerFree[index] = this.task[i][0] + this.task[i][1];
            this.services[index].Add(item);
        }

        private void getNextFromQueue(int i)
        {
            this.freeQueue(i);
            if (this.task[i][0] > ((IEnumerable<double>)this.tServerFree).Min())
            {
                if (this.queue.Count == 0)
                {
                    this.getNextFromArrival(i);
                }
            }
            else if (this.queue.Count != this.queueLong)
            {
                List<double> list;
                int num2;
                this.queue.Enqueue(i);
                double[] numArray1 = this.task[i];
                double[] item = new double[3];
                item[1] = this.task[i][1];
                item[2] = i;
                int index = this.tServerFree.IndexOf(((IEnumerable<double>)this.tServerFree).Min());
                item[0] = this.tServerFree[index];
                (list = this.tServerFree)[num2 = index] = list[num2] + this.task[i][1];
                this.services[index].Add(item);
                this.fillQueue.Add(new Pair(this.task[i][0], 1));
                this.fillQueue.Add(new Pair(item[0], -1));
            }
            else
            {
                this.aborted.Add(this.task[i][0]);
            }
            if (this.queue.Count == 0)
            {
                this.queueState = QueueStates.Empty;
            }
            if ((this.queue.Count < this.queueLong) && (this.queue.Count != 0))
            {
                this.queueState = QueueStates.NotEmpty;
            }
            if (this.queue.Count == this.queueLong)
            {
                this.queueState = QueueStates.Fill;
            }
        }

        private void InitializeComponent()
        {
            this.nArrivalRate = new NumericUpDown();
            this.nTaskCount = new NumericUpDown();
            this.label4 = new Label();
            this.nQueueLong = new NumericUpDown();
            this.label3 = new Label();
            this.nServiceRate = new NumericUpDown();
            this.bStart = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.nStartTime = new NumericUpDown();
            this.nFinishTime = new NumericUpDown();
            this.label5 = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.nServerCount = new NumericUpDown();
            this.label8 = new Label();
            this.nDegreeErlnga = new NumericUpDown();
            this.groupBox = new GroupBox();
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.picArrivalService = new PictureBox();
            this.picNumberTime = new PictureBox();
            this.picQueueTime = new PictureBox();
            this.tableLayoutPanel2 = new TableLayoutPanel();
            this.nArrivalRate.BeginInit();
            this.nTaskCount.BeginInit();
            this.nQueueLong.BeginInit();
            this.nServiceRate.BeginInit();
            this.nStartTime.BeginInit();
            this.nFinishTime.BeginInit();
            this.nServerCount.BeginInit();
            this.nDegreeErlnga.BeginInit();
            this.groupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((ISupportInitialize)this.picArrivalService).BeginInit();
            ((ISupportInitialize)this.picNumberTime).BeginInit();
            ((ISupportInitialize)this.picQueueTime).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            base.SuspendLayout();
            this.nArrivalRate.DecimalPlaces = 2;
            this.nArrivalRate.Dock = DockStyle.Fill;
            int[] bits = new int[4];
            bits[0] = 1;
            bits[3] = 0x20000;
            this.nArrivalRate.Increment = new decimal(bits);
            this.nArrivalRate.Location = new Point(0xf4, 0x61);
            int[] numArray2 = new int[4];
            numArray2[0] = 1;
            this.nArrivalRate.Maximum = new decimal(numArray2);
            int[] numArray3 = new int[4];
            numArray3[0] = 1;
            numArray3[3] = 0x20000;
            this.nArrivalRate.Minimum = new decimal(numArray3);
            this.nArrivalRate.Name = "nArrivalRate";
            this.nArrivalRate.Size = new Size(0x62, 20);
            this.nArrivalRate.TabIndex = 6;
            this.nArrivalRate.ThousandsSeparator = true;
            int[] numArray4 = new int[4];
            numArray4[0] = 1;
            numArray4[3] = 0x20000;
            this.nArrivalRate.Value = new decimal(numArray4);
            this.nTaskCount.Dock = DockStyle.Fill;
            this.nTaskCount.Location = new Point(0xf4, 0xee);
            int[] numArray5 = new int[4];
            numArray5[0] = 0x2710;
            this.nTaskCount.Maximum = new decimal(numArray5);
            this.nTaskCount.Name = "nTaskCount";
            this.nTaskCount.Size = new Size(0x62, 20);
            this.nTaskCount.TabIndex = 7;
            this.nTaskCount.ThousandsSeparator = true;
            int[] numArray6 = new int[4];
            numArray6[0] = 50;
            this.nTaskCount.Value = new decimal(numArray6);
            this.label4.AutoSize = true;
            this.label4.Dock = DockStyle.Fill;
            this.label4.Location = new Point(3, 0xeb);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0xeb, 0x2f);
            this.label4.TabIndex = 3;
            this.label4.Text = "Количество требований (N)";
            this.nQueueLong.Dock = DockStyle.Fill;
            this.nQueueLong.Location = new Point(0xf4, 0xbf);
            int[] numArray7 = new int[4];
            numArray7[0] = 50;
            this.nQueueLong.Maximum = new decimal(numArray7);
            this.nQueueLong.Name = "nQueueLong";
            this.nQueueLong.Size = new Size(0x62, 20);
            this.nQueueLong.TabIndex = 8;
            int[] numArray8 = new int[4];
            numArray8[0] = 0x19;
            this.nQueueLong.Value = new decimal(numArray8);
            this.label3.AutoSize = true;
            this.label3.Dock = DockStyle.Fill;
            this.label3.Location = new Point(3, 0xbc);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0xeb, 0x2f);
            this.label3.TabIndex = 2;
            this.label3.Text = "Длина очереди (L)";
            this.nServiceRate.DecimalPlaces = 2;
            this.nServiceRate.Dock = DockStyle.Fill;
            int[] numArray9 = new int[4];
            numArray9[0] = 1;
            numArray9[3] = 0x20000;
            this.nServiceRate.Increment = new decimal(numArray9);
            this.nServiceRate.Location = new Point(0xf4, 0x90);
            int[] numArray10 = new int[4];
            numArray10[0] = 1;
            this.nServiceRate.Maximum = new decimal(numArray10);
            int[] numArray11 = new int[4];
            numArray11[0] = 1;
            numArray11[3] = 0x20000;
            this.nServiceRate.Minimum = new decimal(numArray11);
            this.nServiceRate.Name = "nServiceRate";
            this.nServiceRate.Size = new Size(0x62, 20);
            this.nServiceRate.TabIndex = 9;
            int[] numArray12 = new int[4];
            numArray12[0] = 1;
            numArray12[3] = 0x20000;
            this.nServiceRate.Value = new decimal(numArray12);
            this.bStart.Location = new Point(3, 0x17b);
            this.bStart.Name = "bStart";
            this.bStart.Size = new Size(0xeb, 0x21);
            this.bStart.TabIndex = 10;
            this.bStart.Text = "Моделировать процесс";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new EventHandler(this.bStart_Click);
            this.label1.AutoSize = true;
            this.label1.Dock = DockStyle.Fill;
            this.label1.Location = new Point(3, 0x5e);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0xeb, 0x2f);
            this.label1.TabIndex = 0;
            this.label1.Text = "Интенсивность входного потока (лямбда)";
            this.label2.AutoSize = true;
            this.label2.Dock = DockStyle.Fill;
            this.label2.Location = new Point(3, 0x8d);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0xeb, 0x2f);
            this.label2.TabIndex = 1;
            this.label2.Text = "Интенсивность обслуживания (мю)";
            this.nStartTime.Dock = DockStyle.Fill;
            int[] numArray13 = new int[4];
            numArray13[0] = 0x19;
            this.nStartTime.Increment = new decimal(numArray13);
            this.nStartTime.Location = new Point(0xf4, 0x11d);
            int[] numArray14 = new int[4];
            numArray14[0] = 0x2710;
            this.nStartTime.Maximum = new decimal(numArray14);
            this.nStartTime.Name = "nStartTime";
            this.nStartTime.Size = new Size(0x62, 20);
            this.nStartTime.TabIndex = 11;
            this.nStartTime.ValueChanged += new EventHandler(this.nStartTime_ValueChanged);
            this.nFinishTime.Dock = DockStyle.Fill;
            int[] numArray15 = new int[4];
            numArray15[0] = 0x19;
            this.nFinishTime.Increment = new decimal(numArray15);
            this.nFinishTime.Location = new Point(0xf4, 0x14c);
            int[] numArray16 = new int[4];
            numArray16[0] = 0xf4240;
            this.nFinishTime.Maximum = new decimal(numArray16);
            this.nFinishTime.Name = "nFinishTime";
            this.nFinishTime.Size = new Size(0x62, 20);
            this.nFinishTime.TabIndex = 12;
            int[] numArray17 = new int[4];
            numArray17[0] = 0x3e8;
            this.nFinishTime.Value = new decimal(numArray17);
            this.nFinishTime.ValueChanged += new EventHandler(this.nFinishTime_ValueChanged);
            this.label5.AutoSize = true;
            this.label5.Dock = DockStyle.Fill;
            this.label5.Location = new Point(3, 0x11a);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0xeb, 0x2f);
            this.label5.TabIndex = 13;
            this.label5.Text = "t1";
            this.label6.AutoSize = true;
            this.label6.Dock = DockStyle.Fill;
            this.label6.Location = new Point(3, 0x149);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0xeb, 0x2f);
            this.label6.TabIndex = 14;
            this.label6.Text = "t2";
            this.label7.AutoSize = true;
            this.label7.Dock = DockStyle.Fill;
            this.label7.Location = new Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0xeb, 0x2f);
            this.label7.TabIndex = 15;
            this.label7.Text = "Количество серверов";
            this.nServerCount.Dock = DockStyle.Fill;
            this.nServerCount.Location = new Point(0xf4, 3);
            int[] numArray18 = new int[4];
            numArray18[0] = 4;
            this.nServerCount.Maximum = new decimal(numArray18);
            int[] numArray19 = new int[4];
            numArray19[0] = 1;
            this.nServerCount.Minimum = new decimal(numArray19);
            this.nServerCount.Name = "nServerCount";
            this.nServerCount.Size = new Size(0x62, 20);
            this.nServerCount.TabIndex = 0x10;
            int[] numArray20 = new int[4];
            numArray20[0] = 1;
            this.nServerCount.Value = new decimal(numArray20);
            this.label8.AutoSize = true;
            this.label8.Dock = DockStyle.Fill;
            this.label8.Location = new Point(3, 0x2f);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0xeb, 0x2f);
            this.label8.TabIndex = 0x11;
            this.label8.Text = "Порядок потока Эрланга";
            this.nDegreeErlnga.Dock = DockStyle.Fill;
            this.nDegreeErlnga.Location = new Point(0xf4, 50);
            int[] numArray21 = new int[4];
            numArray21[0] = 500;
            this.nDegreeErlnga.Maximum = new decimal(numArray21);
            this.nDegreeErlnga.Name = "nDegreeErlnga";
            this.nDegreeErlnga.Size = new Size(0x62, 20);
            this.nDegreeErlnga.TabIndex = 0x12;
            this.groupBox.Controls.Add(this.tableLayoutPanel1);
            this.groupBox.Dock = DockStyle.Fill;
            this.groupBox.Location = new Point(3, 3);
            this.groupBox.Name = "groupBox";
            this.tableLayoutPanel2.SetRowSpan(this.groupBox, 2);
            this.groupBox.Size = new Size(0x15f, 0x1ba);
            this.groupBox.TabIndex = 11;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Входные значения";
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70f));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));
            this.tableLayoutPanel1.Controls.Add(this.nFinishTime, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.nDegreeErlnga, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.nStartTime, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.nTaskCount, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.nQueueLong, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.nServiceRate, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.nServerCount, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.nArrivalRate, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.bStart, 0, 8);
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.tableLayoutPanel1.Location = new Point(3, 0x10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
            this.tableLayoutPanel1.Size = new Size(0x159, 0x1a7);
            this.tableLayoutPanel1.TabIndex = 15;
            this.picArrivalService.Dock = DockStyle.Fill;
            this.picArrivalService.Location = new Point(360, 0xe3);
            this.picArrivalService.Name = "picArrivalService";
            this.picArrivalService.Size = new Size(0x234, 0xda);
            this.picArrivalService.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picArrivalService.TabIndex = 12;
            this.picArrivalService.TabStop = false;
            this.picNumberTime.Dock = DockStyle.Fill;
            this.picNumberTime.Location = new Point(360, 3);
            this.picNumberTime.Name = "picNumberTime";
            this.picNumberTime.Size = new Size(0x234, 0xda);
            this.picNumberTime.SizeMode = PictureBoxSizeMode.Zoom;
            this.picNumberTime.TabIndex = 13;
            this.picNumberTime.TabStop = false;
            this.picQueueTime.Dock = DockStyle.Fill;
            this.picQueueTime.Location = new Point(360, 0x1c3);
            this.picQueueTime.Name = "picQueueTime";
            this.picQueueTime.Size = new Size(0x234, 0xdb);
            this.picQueueTime.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picQueueTime.TabIndex = 14;
            this.picQueueTime.TabStop = false;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38.6192f));
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61.3808f));
            this.tableLayoutPanel2.Controls.Add(this.picNumberTime, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.picQueueTime, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.picArrivalService, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.groupBox, 0, 0);
            this.tableLayoutPanel2.Dock = DockStyle.Fill;
            this.tableLayoutPanel2.Location = new Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333f));
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333f));
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333f));
            this.tableLayoutPanel2.Size = new Size(0x39f, 0x2a1);
            this.tableLayoutPanel2.TabIndex = 15;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScroll = true;
            base.ClientSize = new Size(0x39f, 0x2a1);
            base.Controls.Add(this.tableLayoutPanel2);
            this.MinimumSize = new Size(500, 500);
            base.Name = "Form1";
            this.Text = "Form1";
            this.nArrivalRate.EndInit();
            this.nTaskCount.EndInit();
            this.nQueueLong.EndInit();
            this.nServiceRate.EndInit();
            this.nStartTime.EndInit();
            this.nFinishTime.EndInit();
            this.nServerCount.EndInit();
            this.nDegreeErlnga.EndInit();
            this.groupBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((ISupportInitialize)this.picArrivalService).EndInit();
            ((ISupportInitialize)this.picNumberTime).EndInit();
            ((ISupportInitialize)this.picQueueTime).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void nFinishTime_ValueChanged(object sender, EventArgs e)
        {
            this.spinBoxChange();
        }

        private void nStartTime_ValueChanged(object sender, EventArgs e)
        {
            this.spinBoxChange();
        }

        private void spinBoxChange()
        {
            if (this.nStartTime.Value >= this.nFinishTime.Value)
            {
                this.nFinishTime.Value = this.nStartTime.Value + 25M;
            }
            if (this.graphArrivalService != null)
            {
                this.t1 = Convert.ToInt32(this.nStartTime.Value);
                this.t2 = Convert.ToInt32(this.nFinishTime.Value);
                MyGraph.t1 = this.t1;
                MyGraph.t2 = this.t2;
                this.picArrivalService.Invalidate();
                this.picQueueTime.Invalidate();
                this.picNumberTime.Invalidate();
            }
        }

        private double t(double r) =>
    (-Math.Log(r) / this.arrivalRate);

        private double tau(double r) =>
    (-Math.Log(r) / this.serviceRate);

        // Nested Types
        private enum QueueStates
        {
            Empty,
            NotEmpty,
            Fill
        }
    }
}
