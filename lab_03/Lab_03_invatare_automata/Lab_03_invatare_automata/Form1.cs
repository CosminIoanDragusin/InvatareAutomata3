using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_03_invatare_automata
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Pointc
        {
            public Pointc(Point P, Rectangle R)
            {
                p = P;
                r = R;
            }

            public Point p { set; get; }
            public Rectangle r { set; get; }
            public override String ToString() => $"({p},{r})";
        }

        private List<Pointc> points = new List<Pointc>();
        private List<Point>sprectruPoints=new List<Point>();
        private List<Pointc> lista_neuroni = new List<Pointc>();
        private Pointc[,] neuroni = new Pointc[10, 10];
        private int origin_X;
        private int origin_Y;
        private int sprectru=200;
        private double N = 10, t = 1;
        private int vecinatate;

        double Functie_Euclidean(int x1, int x2, int y1, int y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));//nu uita de sqrt cand faci for de i pentru suma

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;
            g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            AfisareSprectruAxe(g);

            using(StreamReader reader=new StreamReader("puncte.txt")) 
            {
                while (!reader.EndOfStream) 
                {
                    string CurrentLine = reader.ReadLine();
                    string[] bits = CurrentLine.Split(' ');
                    int x = int.Parse(bits[1]);
                    int y = int.Parse(bits[2]);
                    Point point = new Point(origin_X+x, origin_Y+y);
                    Rectangle r = new Rectangle(point,new Size(1, 1));
                    Pointc punct = new Pointc(point, r);
                    points.Add(punct);
                    g.FillRectangle(Brushes.Black,punct.r);
                }
            }

        }

        public void AfisareSprectruAxe(Graphics g) 
        {
            origin_X = this.ClientRectangle.Width / 2;
            origin_Y = this.ClientRectangle.Height / 2;
            g.DrawLine(Pens.Black, new Point(origin_X, 0), new Point(origin_X, this.Bottom));
            g.DrawLine(Pens.Black, new Point(0, origin_Y), new Point(this.Right, origin_Y));
            g.FillEllipse(Brushes.Black,new Rectangle(new Point(origin_X - 2, origin_Y - 2),new Size(4,4)));

            // aici se delimiteaza sprectrul
            Point pct1 = new Point(origin_X + sprectru, origin_Y - sprectru);
            sprectruPoints.Add(pct1);
            Point pct2 = new Point(origin_X - sprectru, origin_Y - sprectru);
            sprectruPoints.Add(pct2);
            Point pct3 = new Point(origin_X - sprectru, origin_Y + sprectru);
            sprectruPoints.Add(pct3);
            Point pct4 = new Point(origin_X + sprectru, origin_Y + sprectru);
            sprectruPoints.Add(pct4);
            sprectruPoints.Add(pct1);

            Point[] puncte1 = sprectruPoints.ToArray();
            for(int i=1; i < puncte1.Length ; i++) 
            {
                g.DrawLine(Pens.Red, puncte1[i - 1], puncte1[i]);
            }
        }

        public void AfisarePuncte(Graphics g) 
        {
        foreach (Pointc p in points) 
            {
                g.FillRectangle(Brushes.Black, p.r);
            }
        }

        public void AfisareNeuroni(Graphics g)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    bool ok = true;
                    if ((i == 0) || (i == 9))
                    {
                        if (j != 9)
                        {
                            g.DrawLine(Pens.Blue, neuroni[i, j].p, neuroni[i, j + 1].p);
                        }
                        ok = false;
                    }
                    if ((j == 0) || (j == 9))
                    {
                        if (i != 9)
                        {
                            g.DrawLine(Pens.Blue, neuroni[i, j].p, neuroni[i + 1, j].p);
                        }
                        ok = false;
                    }
                    if (ok == true)
                    {
                        g.DrawLine(Pens.Blue, neuroni[i, j].p, neuroni[i - 1, j].p);
                        g.DrawLine(Pens.Blue, neuroni[i, j].p, neuroni[i + 1, j].p);
                        g.DrawLine(Pens.Blue, neuroni[i, j].p, neuroni[i, j - 1].p);
                        g.DrawLine(Pens.Blue, neuroni[i, j].p, neuroni[i, j + 1].p);
                    }
                }
            }
        }
        private void btnNeuron_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            int delim = 56;
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    var x = -252 + delim * i;
                    var y = -252 + delim * j;
                   // Point point = new Point(x);
                    Point punct = new Point(origin_X + x, origin_Y + y);
                    Rectangle r = new Rectangle(punct, new Size(1, 1));
                    Pointc p = new Pointc(punct, r);
                    neuroni[i, j] = p;
                    lista_neuroni.Add(neuroni[i, j]);
                }
            }
            AfisareNeuroni(g);
        }

        public Pointc NeuronInvingator(Pointc p)
        {
            Pointc winner = null;
            double minDist = Double.MaxValue;
            foreach(Pointc n in lista_neuroni)
            {
                double dist = Functie_Euclidean(p.p.X, p.p.Y, n.p.X, n.p.Y);
                if (dist < minDist)
                {
                    winner = n;
                    minDist = dist;
                }
            }
            return winner;
        }

        public void ActualizareNeuroniVecini(double v, Pointc p, Pointc neuronInvingator, int index1, int index2, double alpha)
        {

            for (int k = index1 - vecinatate; k <= index1 + vecinatate; k++)
            {
                for (int l = index2 - vecinatate; l <= index2 + vecinatate; l++)
                {
                    if ((k >= 0) && (l >= 0) && (k < 10) && (l < 10))
                    {
                        if (neuroni[k, l] != neuronInvingator)
                        {
                            int newX = Convert.ToInt32(neuroni[k, l].p.X + alpha * (p.p.X - neuroni[k, l].p.X));
                            int newY = Convert.ToInt32(neuroni[k, l].p.Y + alpha * (p.p.Y - neuroni[k, l].p.Y));
                            Point point = new Point(newX, newY);
                            Rectangle r = new Rectangle(point, new Size(1, 1));
                            Pointc pnew = new Pointc(point, r);
                            neuroni[k, l] = pnew;
                        }
                    }
                }
            }
        }

        public void ActualizareListaNeuroni()
        {
            lista_neuroni.Clear();
            for (int j = 0; j < 10; j++)
            {
                for (int k = 0; k < 10; k++)
                {
                    lista_neuroni.Add(neuroni[j, k]);
                }
            }
        }

        private void btnSoma_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            double alpha = Math.Exp((-1) * (t / N));
            vecinatate = (int)(6.1 * Math.Exp((-1) * (t / (N * 1.0))));
            while (alpha > 0.001)
            {
                foreach (Pointc p in points)
                {
                    Pointc neuronInvingator = NeuronInvingator(p);
                    int index1 = 0, index2 = 0;
                    //actualizare neuron invingator
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (neuroni[i, j].Equals(neuronInvingator))
                            {
                                int newX = Convert.ToInt32(neuroni[i, j].p.X + alpha * (p.p.X - neuroni[i, j].p.X));
                                int newY = Convert.ToInt32(neuroni[i, j].p.Y + alpha * (p.p.Y - neuroni[i, j].p.Y));
                                Point point = new Point(newX, newY);
                                Rectangle r = new Rectangle(point, new Size(1, 1));
                                Pointc pnew = new Pointc(point, r);
                                index1 = i; index2 = j;
                                neuroni[i, j] = pnew;
                            }
                        }
                    }
                    ActualizareNeuroniVecini(vecinatate, p, neuronInvingator, index1, index2, alpha);
                    ActualizareListaNeuroni();
                }
                t++;
                alpha = Math.Exp((-1) * (t / N));
                vecinatate = (int)(6.1 * Math.Exp((-1) * (t / (N * 1.0))));
                label1.Text = "alpha = " + alpha.ToString();
                label2.Text = "nr epoci = " + t.ToString();
                if (vecinatate < 0) vecinatate = 0;
                g.Clear(BackColor);

                AfisarePuncte(g);
                AfisareNeuroni(g);
            }
        }
    }
}
