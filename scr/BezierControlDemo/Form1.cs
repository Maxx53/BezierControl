using System.Windows.Forms;
using System.Drawing;
using BezierControl;



/// Using:
/// private BezierLine bezier = new BezierLine();
/// <code>
/// bezier.Draw(e); to Control_Paint() event
/// bezier.MouseDown(e); to Control_MouseDown() event
/// bezier.MouseMove(e); to Control_MouseMove() event
/// bezier.MouseUp(e); to Control_MouseUp() event
/// </code>


/// Getting output:
/// <code>
/// List<Point> pixels = bezier.ToPixels;
/// List<Point> polyline = bezier.ToPolyline();
/// </code>


namespace BezierControlDemo
{
    public partial class Form1 : Form
    {
        private BezierLine bezier;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            bezier = new BezierLine(pictureBox1);

            //If you need snapping to grid!
            //bezier.isSnap = true;
            //bezier.snapRes = 20;

            //If you need to change colors
            //bezier.pathPenColor = Color.Black;
            //bezier.anchorBrushColor = Color.Gold;
            //bezier.ctrlBrushColor = Color.Blue;
            //bezier.ctrlPenColor = Color.Green;

            //If you need to change sizes
            //bezier.anchorSize = 20;
            //bezier.controlSize = 10;
            //bezier.pathPenWidth = 20;
            //bezier.ctrlPen.Width = 10;

            //Wanna get fast draw?
            //bezier.Smoothing = false;

            bezier.Spawn(40, 40);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            bezier.Draw(e);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            bezier.MouseDown(e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            bezier.MouseMove(e);
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            bezier.ShowAnchors = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
        {
            bezier.ShowControls = checkBox2.Checked;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            bezier.MouseUp(e);
        }
    }
}
