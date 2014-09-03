using System.Windows.Forms;
using System.Drawing;
using BezierControl;



/// Getting output:
/// <code>
/// List<Point> pixels = bezier.ToPointList();
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
            bezier = new BezierLine(pictureBox1, Color.Black);
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
    }
}
