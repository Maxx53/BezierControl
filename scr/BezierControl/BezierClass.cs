// <copyright file="BezierClass.cs" company="Maxx53">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Maxx53</author>
// <date>09/03/2014</date>
// <summary>Drawing bezier curve and mouse control for WinForms</summary>


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace BezierControl
{
    [Flags]
    public enum PointID : byte
    {
        Anchor = 0,
        Control1 = 1,
        Control2 = 2,
    }

    public class BezierLine : List<BezierPoint>
    {
        private int toMove = -1;
        private double radius;
        private Point center;
        private BezierPoint second;
        private bool firstOrLast = false;
        private GraphicsPath gp = new GraphicsPath();
        private Control canvas;
        private Color pathColor = Color.Black;
        private bool showControls = true;
        private bool showAnchors = true;
        private bool isStart = false;


        public BezierLine(Control p, Color c)
        {
            canvas = p;
            pathColor = c;
        }


        public void AddPoint(int x, int y, PointID id)
        {
            this.Add(new BezierPoint(new Point(x, y), id));
        }

        public void AddPoint(Point point, PointID id)
        {
            this.Add(new BezierPoint(point, id));
        }

        public BezierPoint GetToMove()
        {
            return this[toMove];
        }

        private Point[] toPointArray()
        {
            var resArr = new Point[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                resArr[i] = this[i].Coord;
            }
            return resArr;

            //Linq
            // return this.Select(x => x.Coodr).ToArray();

        }

        public void AddNode(int x, int y)
        {
            if (this.Count != 0)
            {
                this.AddPoint(this[this.Count - 1].Coord, PointID.Control1);
                this.AddPoint(x, y, PointID.Control2);
                this.AddPoint(x, y, PointID.Anchor);
            }
        }

        public void RemoveNode()
        {
            if (this.Count > 4)
            {
                if (toMove == 0)
                {
                    this.RemoveAt(toMove);
                    this.RemoveAt(toMove);
                    this.RemoveAt(toMove);
                }
                else
                    if (toMove == this.Count - 1)
                    {
                        this.RemoveAt(toMove - 2);
                        this.RemoveAt(toMove - 2);
                        this.RemoveAt(toMove - 2);
                    }
                    else
                    {
                        this.RemoveAt(toMove);
                        this.RemoveAt(toMove);
                        this.RemoveAt(toMove - 1);
                    }
            }
        }

        internal void SetAnchorPos(Point point)
        {

            var deltaX = this[toMove].Coord.X - point.X;
            var deltaY = this[toMove].Coord.Y - point.Y;

            this[toMove].Coord = point;
            this[toMove].Rect = new Rectangle(point.X - 6, point.Y - 6, 12, 12);


            //Phuckin' copypaste!
            if (toMove == 0)
            {
                this[toMove + 1].Coord = new Point(this[toMove + 1].Coord.X - deltaX, this[toMove + 1].Coord.Y - deltaY);
                this[toMove + 1].Rect = new Rectangle(this[toMove + 1].Coord.X - 4, this[toMove + 1].Coord.Y - 4, 8, 8);
            }
            else if (toMove == this.Count - 1)
            {
                this[toMove - 1].Coord = new Point(this[toMove - 1].Coord.X - deltaX, this[toMove - 1].Coord.Y - deltaY);
                this[toMove - 1].Rect = new Rectangle(this[toMove - 1].Coord.X - 4, this[toMove - 1].Coord.Y - 4, 8, 8);
            }
            else
            {
                this[toMove - 1].Coord = new Point(this[toMove - 1].Coord.X - deltaX, this[toMove - 1].Coord.Y - deltaY);
                this[toMove - 1].Rect = new Rectangle(this[toMove - 1].Coord.X - 4, this[toMove - 1].Coord.Y - 4, 8, 8);

                this[toMove + 1].Coord = new Point(this[toMove + 1].Coord.X - deltaX, this[toMove + 1].Coord.Y - deltaY);
                this[toMove + 1].Rect = new Rectangle(this[toMove + 1].Coord.X - 4, this[toMove + 1].Coord.Y - 4, 8, 8);
            }

        }

        internal void SetControlsPos(Point point)
        {
            this[toMove].Coord = point;
            this[toMove].Rect = new Rectangle(point.X - 4, point.Y - 4, 8, 8);

            if (!firstOrLast)
            {
                second.Coord = Utils.setRotation(radius, second.Coord, center, Math.PI + Utils.getAngleRad(point, center));
                second.Rect = new Rectangle(second.Coord.X - 4, second.Coord.Y - 4, 8, 8);
            }
        }

        internal void InsertAtPos(int pos, Point point)
        {
            this.Insert(pos, new BezierPoint(point, PointID.Control1));
            this.Insert(pos, new BezierPoint(point, PointID.Anchor));
            this.Insert(pos, new BezierPoint(point, PointID.Control2));

        }

        public void ImportPoints(List<Point> list)
        {
            byte x = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    this.AddPoint(list[i], PointID.Anchor);
                    x++;
                    continue;
                }

                switch (x)
                {
                    case 0:
                        this.AddPoint(list[i], PointID.Anchor);
                        break;
                    case 1:
                        this.AddPoint(list[i], PointID.Control1);
                        break;
                    case 2:
                        this.AddPoint(list[i], PointID.Control2);
                        x = 0;
                        continue;
                }

                x++;
            }
        }

        public void Draw(System.Windows.Forms.PaintEventArgs e)
        {
            if (canvas == null) return;

            if (this.Count > 1)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                gp.Reset();

                gp.AddBeziers(toPointArray());


                //Draw Path

                e.Graphics.DrawPath(new Pen(pathColor, 2), gp);


                //Draw Controls Lines
                if (ShowControls)
                {
                    for (int i = 0; i < this.Count; i++)
                    {

                        if (i != 0)
                        {
                            if (this[i - 1].Id != PointID.Control1)
                                e.Graphics.DrawLine(Pens.Blue, this[i - 1].Coord, this[i].Coord);
                        }
                    }
                }

                //Draw anchors
                if (ShowAnchors)
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Id == PointID.Anchor)
                        {

                            e.Graphics.FillEllipse(Brushes.Red, this[i].Rect);

                        }
                    }
                }

                //Draw Controls Points
                if (ShowControls)
                {
                    for (int i = 0; i < this.Count; i++)
                    {


                        if ((this[i].Id == PointID.Control1) || (this[i].Id == PointID.Control2))
                        {

                            e.Graphics.FillEllipse(Brushes.Green, this[i].Rect);
                        }
                    }
                }
            }
        }

        public void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (this.Count == 0) return;
            isStart = false;
            toMove = -1;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Rect.Contains(e.Location) && (this[i].Id != PointID.Anchor))
                    {
                        this.firstOrLast = false;
                        this.toMove = i;

                        if ((i != 1) & (i != this.Count - 2))
                        {

                            Point center;
                            BezierPoint second;

                            if (this[i].Id == PointID.Control1)
                            {
                                center = this[i - 1].Coord;
                                second = this[i - 2];
                            }
                            else
                            {
                                center = this[i + 1].Coord;
                                second = this[i + 2];

                            }

                            this.center = center;
                            this.second = second;


                            this.radius = Utils.getRadius(second.Coord, center);

                        }
                        else this.firstOrLast = true;

                        return;
                    }

                }

                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Rect.Contains(e.Location) && (this[i].Id == PointID.Anchor))
                    {
                        this.toMove = i;
                        isStart = (i == 0);
                        return;
                    }

                }


                if (this.gp.IsOutlineVisible(e.Location, new Pen(Color.Black, 10)))
                {

                    int polys = this.Count / 3;
                    int inc = 0;

                    for (int k = 0; k < polys; k++)
                    {
                        Point[] rrr = new Point[4];

                        for (int i = 0; i < 4; i++)
                        {
                            rrr[i] = this[inc + i].Coord;
                        }

                        inc += 3;

                        //Oh, it's a line!
                        if ((rrr[0] == rrr[1]) && (rrr[2] == rrr[3]))
                        {
                            if (Utils.IsOnLine(new Point(rrr[0].X, rrr[0].Y), new Point(rrr[2].X, rrr[2].Y), e.Location))
                            {
                                int newPos = (k + 1) * 3;
                                this.InsertAtPos(newPos - 1, e.Location);
                                canvas.Refresh();
                                return;
                            }
                        }
                        else
                        {
                            if (Utils.IsInPolygon(rrr, e.Location))
                            {
                                int newPos = (k + 1) * 3;
                                this.InsertAtPos(newPos - 1, e.Location);
                                canvas.Refresh();
                                return;
                            }
                        }
                    }
                }



            }
            else
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    this.AddNode(e.Location.X, e.Location.Y);
                    canvas.Refresh();
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                {

                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Rect.Contains(e.Location) && (this[i].Id == PointID.Anchor))
                        {
                            this.toMove = i;

                            this.RemoveNode();
                            canvas.Refresh();

                            return;
                        }

                    }


                }
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (toMove != -1)
                {
                    var ourPoint = this.GetToMove();

                    switch (ourPoint.Id)
                    {
                        case PointID.Anchor:
                            SetAnchorPos(e.Location);
                            break;


                        case PointID.Control1:

                            SetControlsPos(e.Location);

                            break;
                        case PointID.Control2:

                            SetControlsPos(e.Location);

                            break;
                        default:
                            break;
                    }
                    canvas.Refresh();
                }
            }

        }

        internal void ClearLine()
        {
            this.Clear();
            this.AddPoint(10, 100, PointID.Anchor);
            this.AddNode(160, 100);
            canvas.Refresh();
        }

        internal List<Point> ToPixels()
        {
            var temp = new List<Point>();

            for (int i = 0; i < gp.PointCount - 1; i++)
            {
                var xsss = Utils.GetPointsOnLine((int)gp.PathPoints[i].X, (int)gp.PathPoints[i].Y, (int)gp.PathPoints[i + 1].X, (int)gp.PathPoints[i + 1].Y);
                temp.AddRange(xsss);

                if (i != gp.PointCount - 1)
                    temp.RemoveAt(temp.Count - 1);
            }

            return temp;
        }


        private static PointF[] getFlattenPoints(GraphicsPath input)
        {
            GraphicsPath tempGp = new GraphicsPath();

            tempGp.AddBeziers(input.PathPoints);
            tempGp.Flatten(null, 0.1f);

            return tempGp.PathPoints;
        }

        public List<Point> ToPolyline()
        {
            var temp = new List<Point>();

            var points = getFlattenPoints(gp);

            for (int i = 0; i < points.Length; i++)
            {
                temp.Add(new Point((int)points[i].X, (int)points[i].Y));
            }

            return temp;
        }


        public List<PointF> ToPolylineF()
        {
            var temp = new List<PointF>();
            var points = getFlattenPoints(gp);

            for (int i = 0; i < points.Length; i++)
            {
                temp.Add(points[i]);
            }

            return temp;
        }

        public List<Point> ToPointList()
        {
            var temp = new List<Point>();
          
            var points = getFlattenPoints(gp);

            for (int i = 0; i < points.Length - 1; i++)
            {
                var segment = Utils.GetPointsOnLine((int)points[i].X, (int)points[i].Y, (int)points[i + 1].X, (int)points[i + 1].Y);
                temp.AddRange(segment);

                if (i != points.Length - 1)
                    temp.RemoveAt(temp.Count - 1);
            }
            return temp;
        }


        public void Spawn(int x, int y)
        {
            if (this.Count == 0)
            {

                this.AddPoint(x, y, PointID.Anchor);
                this.AddNode(x + 60, y);
                canvas.Refresh();
            }
        }

        public bool ShowControls
        {
            get
            {
                return showControls;
            }
            set
            {
                showControls = value;
                canvas.Refresh();
            }
        }

        public bool ShowAnchors
        {
            get
            {
                return showAnchors;
            }
            set
            {
                showAnchors = value;
                canvas.Refresh();
            }
        }
    }


    public class BezierPoint
    {
        public BezierPoint(Point coord, PointID id)
        {
            this.Coord = coord;
            this.Id = id;

            switch (id)
            {
                case PointID.Anchor:
                    Rect = new Rectangle(coord.X - 6, coord.Y - 6, 12, 12);
                    break;
                case PointID.Control1:
                case PointID.Control2:
                    Rect = new Rectangle(coord.X - 4, coord.Y - 4, 8, 8);
                    break;
            }
        }

        public Point Coord { set; get; }
        public PointID Id { set; get; }
        public Rectangle Rect { set; get; }

    }


    class Utils
    {
        private const double SELECTION_FUZZINESS = 3;

        public static bool IsOnLine(Point a, Point b, Point point)
        {
            Point leftPoint;
            Point rightPoint;

            // Normalize start/end to left right to make the offset calc simpler.
            if (a.X <= b.X)
            {
                leftPoint = a;
                rightPoint = b;
            }
            else
            {
                leftPoint = b;
                rightPoint = a;
            }

            // If point is out of bounds, no need to do further checks.                  
            if (point.X + SELECTION_FUZZINESS < leftPoint.X || rightPoint.X < point.X - SELECTION_FUZZINESS)
                return false;
            else if (point.Y + SELECTION_FUZZINESS < Math.Min(leftPoint.Y, rightPoint.Y) || Math.Max(leftPoint.Y, rightPoint.Y) < point.Y - SELECTION_FUZZINESS)
                return false;

            double deltaX = rightPoint.X - leftPoint.X;
            double deltaY = rightPoint.Y - leftPoint.Y;

            // If the line is straight, the earlier boundary check is enough to determine that the point is on the line.
            // Also prevents division by zero exceptions.
            if (deltaX == 0 || deltaY == 0)
                return true;

            double slope = deltaY / deltaX;
            double offset = leftPoint.Y - leftPoint.X * slope;
            double calculatedY = point.X * slope + offset;

            // Check calculated Y matches the points Y coord with some easing.
            bool lineContains = point.Y - SELECTION_FUZZINESS <= calculatedY && calculatedY <= point.Y + SELECTION_FUZZINESS;

            return lineContains;
        }



        public static List<Point> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            bool isreverse = false;
            List<Point> temp = new List<Point>();

            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }

            if (x0 > x1)
            {
                isreverse = true;
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                temp.Add(new Point((steep ? y : x), (steep ? x : y)));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            if (isreverse)
            {
                temp.Reverse();
            }
            return temp;
        }



        public static bool IsInPolygon(Point[] poly, Point p)
        {
            Point p1, p2;


            bool inside = false;


            if (poly.Length < 3)
            {
                return inside;
            }


            var oldPoint = new Point(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);


            for (int i = 0; i < poly.Length; i++)
            {
                var newPoint = new Point(poly[i].X, poly[i].Y);


                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;

                    p2 = newPoint;
                }

                else
                {
                    p1 = newPoint;

                    p2 = oldPoint;
                }


                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && (p.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }


                oldPoint = newPoint;
            }


            return inside;
        }



        public static Point setRotation(double radius, Point point, Point centerPoint, double angle)
        {
            return new Point((int)(Math.Cos(angle) * radius + centerPoint.X), (int)(Math.Sin(angle) * radius + centerPoint.Y));
        }


        public static double getAngleRad(Point origin, Point target)
        {
            return Math.Atan2(origin.Y - target.Y, origin.X - target.X);
        }


        public static double getRadius(Point pointA, Point pointB)
        {
            double width;
            double height;

            if (pointA.X > pointB.Y)
            {
                width = pointA.X - pointB.X;
            }
            else
            {
                width = pointB.X - pointA.X;
            }

            if (pointA.Y > pointB.Y)
            {
                height = pointA.Y - pointB.Y;
            }
            else
            {
                height = pointB.Y - pointA.Y;
            }

            return Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
        }


    }

}
