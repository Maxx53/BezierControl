﻿// <copyright file="BezierClass.cs" company="Maxx53">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Maxx53</author>
// <date>26/03/2015</date>
// <summary>Class for drawing cubic bezier curve with mouse, for use in WinForms.</summary>

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
        #region Variables
        
        private int toMove = -1;
        private int highLited = -1;
        private int polyNum = -1;

        private double radius1;
        private Point center1;
        private BezierPoint second1;
        private bool skip1 = false;

        private double radius2;
        private Point center2;
        private BezierPoint second2;
        private bool skip2 = false;

        private Point oldMousePoint;
        private GraphicsPath gp = new GraphicsPath();
        private Control canvas;

        private bool showControls = true;
        private bool showAnchors = true;

        private Pen pathPenLight = new Pen(Color.Aqua, 2);
        private Pen ctrlPenLight = new Pen(Color.CadetBlue, 2);

        private SolidBrush anchorBrush = new SolidBrush(Color.Red);
        private SolidBrush ctrlBrush = new SolidBrush(Color.Green);
        private SolidBrush anchorLight = new SolidBrush(Color.Pink);
        private SolidBrush ctrlLight = new SolidBrush(Color.LawnGreen);

        public bool Smoothing = true;
        public bool isStart = false;
        public bool polyMoving = false;

        public bool isSnap = false;
        public int snapRes = 20;

        public Pen pathPen = new Pen(Color.Gray, 2);
        public Pen ctrlPen = new Pen(Color.Blue, 0);

        public static int anchorSize = 12;
        public static int controlSize = 8;

        #endregion

        #region Properties

        public BezierLine(Control p)
        {
            canvas = p;
        }

        public Color pathPenColor
        {
            get
            {
                return pathPen.Color;
            }
            set
            {
                pathPen.Color = value;
                pathPenLight.Color = Utils.HighlightColor(value);
             }
        }

        public float pathPenWidth
        {
            get
            {
                return pathPen.Width;
            }
            set
            {
                pathPen.Width = value;
                pathPenLight.Width = pathPen.Width + 1;
            }
        }

        public Color ctrlPenColor
        {
            get
            {
                return ctrlPen.Color;
            }
            set
            {
                ctrlPen.Color = value;
                ctrlPenLight.Color = Utils.HighlightColor(value);
            }
        }

        public Color anchorBrushColor
        {
            get
            {
                return anchorBrush.Color;
            }
            set
            {
                anchorBrush.Color = value;
                anchorLight.Color = Utils.HighlightColor(value);
            }
        }

        public Color ctrlBrushColor
        {
            get
            {
                return ctrlBrush.Color;
            }
            set
            {
                ctrlBrush.Color = value;
                ctrlLight.Color = Utils.HighlightColor(value);
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

        public int AnchorSize
        {
            get
            {
                return anchorSize;
            }
            set
            {
                anchorSize = value;
            }
        }

        public int ControlSize
        {
            get
            {
                return controlSize;
            }
            set
            {
                controlSize = value;
            }
        }

        #endregion

        #region Editing line
        
        public void AddPoint(int x, int y, PointID id)
        {
            this.Add(new BezierPoint(new Point(x, y), id));
        }

        public void AddPoint(Point point, PointID id)
        {
            this.Add(new BezierPoint(point, id));
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

        public void RemoveNode(int idx)
        {
            if (this.Count > 4)
            {
                if (idx == 0)
                {
                    this.RemoveAt(idx);
                    this.RemoveAt(idx);
                    this.RemoveAt(idx);
                }
                else
                    if (idx == this.Count - 1)
                    {
                        this.RemoveAt(idx - 2);
                        this.RemoveAt(idx - 2);
                        this.RemoveAt(idx - 2);
                    }
                    else
                    {
                        this.RemoveAt(idx);
                        this.RemoveAt(idx);
                        this.RemoveAt(idx - 1);
                    }
            }
        }

        public void Spawn(int x, int y)
        {
            if (this.Count == 0)
            {

                this.AddPoint(x, y, PointID.Anchor);
                this.AddNode(x, y + 40);
                canvas.Refresh();
            }
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

        public void ClearLine()
        {
            this.Clear();
            this.AddPoint(10, 100, PointID.Anchor);
            this.AddNode(160, 100);
            canvas.Refresh();
        }

        private void InsertAtPos(int pos, Point point)
        {
            this.Insert(pos, new BezierPoint(point, PointID.Control1));
            this.Insert(pos, new BezierPoint(point, PointID.Anchor));
            this.Insert(pos, new BezierPoint(point, PointID.Control2));

        }

        private void InsertAtPoly(int PolyNum, Point loc)
        {
            this.InsertAtPos((PolyNum + 1) * 3 - 1, loc);
            canvas.Refresh();
        }

        public void UpdateSizes()
        {
            for (int i = 0; i < this.Count; i++)
            {
               this[i].ForceUpdateRect();
            }
            canvas.Invalidate();
        }

        #endregion

        #region Setting Positions

        private void SetAnchorPos(Point point)
        {
          
            var deltaX = this[toMove].Coord.X - point.X;
            var deltaY = this[toMove].Coord.Y - point.Y;

            this[toMove].Coord = point;
            
            if (toMove == 0)
            {
                this[toMove + 1].Coord = new Point(this[toMove + 1].Coord.X - deltaX, this[toMove + 1].Coord.Y - deltaY);
            }
            else if (toMove == this.Count - 1)
            {
                this[toMove - 1].Coord = new Point(this[toMove - 1].Coord.X - deltaX, this[toMove - 1].Coord.Y - deltaY);
            }
            else
            {
                this[toMove - 1].Coord = new Point(this[toMove - 1].Coord.X - deltaX, this[toMove - 1].Coord.Y - deltaY);
                this[toMove + 1].Coord = new Point(this[toMove + 1].Coord.X - deltaX, this[toMove + 1].Coord.Y - deltaY);
            }

        }

        private void SetControlsPos(Point point)
        {
            this[toMove].Coord = point;       

            if (!skip1)
            {
                second1.Coord = Utils.setRotation(radius1, second1.Coord, center1, Math.PI + Utils.getAngleRad(point, center1));
            }
        }

        private void SetPolyPos(Point point)
        {
            var CtrlNum1 = (polyNum) * 3 + 1;
            var CtrlNum2 = (polyNum) * 3 + 2;

            var deltaX = oldMousePoint.X - point.X;
            var deltaY = oldMousePoint.Y - point.Y;
            
            this[CtrlNum1].Coord = new Point(this[CtrlNum1].Coord.X - deltaX, this[CtrlNum1].Coord.Y - deltaY);
            this[CtrlNum2].Coord = new Point(this[CtrlNum2].Coord.X - deltaX, this[CtrlNum2].Coord.Y - deltaY);


            if (!skip1)
            {
                var firstSync = this[CtrlNum1];
                second1.Coord = Utils.setRotation(radius1, second1.Coord, center1, Math.PI + Utils.getAngleRad(firstSync.Coord, center1));
            }

            if (!skip2)
            {
                var secondSync = this[CtrlNum2];
                second2.Coord = Utils.setRotation(radius2, second2.Coord, center2, Math.PI + Utils.getAngleRad(secondSync.Coord, center2));
            }

            oldMousePoint = point;

        }

        #endregion

        #region Control Events
        
        public void Draw(PaintEventArgs e)
        {
            if (canvas == null) return;

            if (this.Count > 1)
            {
                if (Smoothing)
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                }

                gp.Reset();

                gp.AddBeziers(ToPointArray());


                //Draw Real this

                e.Graphics.DrawPath(pathPen, gp);

                //Drawing highligt bezier
                int nu = (polyNum) * 3;
                if ((polyNum != -1) && (nu + 3 < this.Count))
                {

                    e.Graphics.DrawBezier(pathPenLight, this[nu].Coord, this[nu + 1].Coord, this[nu + 2].Coord, this[nu + 3].Coord);
                }

                //Draw Controls Lines
                if (ShowControls)
                {
                    for (int i = 0; i < this.Count; i++)
                    {

                        if (i != 0)
                        {
                            if (this[i - 1].Id != PointID.Control1)
                            {
                                e.Graphics.DrawLine(ctrlPen, this[i - 1].Coord, this[i].Coord);
                            }
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
                            if ((highLited != -1) && (i == highLited))
                            {
                                e.Graphics.FillEllipse(anchorLight, this[i].Rect);
                            }
                            else
                                e.Graphics.FillEllipse(anchorBrush, this[i].Rect);
                                //e.Graphics.DrawEllipse(Pens.Red, this[i].Rect);
                        }
                    }
                }

                //Draw Controls Points
                //Backward loop
                if (ShowControls)
                {
                    for (int i = this.Count-1; i > -1; i--)
                    {
                        if ((this[i].Id == PointID.Control1) | (this[i].Id == PointID.Control2))
                        {
                            if ((highLited != -1) && (i == highLited))
                            {
                                e.Graphics.FillEllipse(ctrlLight, this[i].Rect);
                            }
                            else
                                e.Graphics.FillEllipse(ctrlBrush, this[i].Rect);
                                //e.Graphics.DrawEllipse(Pens.Green, this[i].Rect);
                        }
                    }
                }
 
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
            MouseDown(e.Location, e.Button);
        }

        public void MouseDown(Point location, MouseButtons mb )
        {
            if (this.Count == 0) return;

            isStart = false;
            toMove = -1;
           // polyNum = -1;
            polyMoving = false;

            if (mb == MouseButtons.Left)
            {
                //Checking controls
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Rect.Contains(location) && (this[i].Id != PointID.Anchor))
                    {
                        skip1 = false;
                        toMove = i;

                        if ((i != 1) & (i != this.Count - 2))
                        {
                            //Prepare for moving

                            if (this[i].Id == PointID.Control1)
                            {
                                center1 = this[i - 1].Coord;
                                second1 = this[i - 2];
                            }
                            else
                            {
                                center1 = this[i + 1].Coord;
                                second1 = this[i + 2];

                            }

                            radius1 = Utils.getRadius(second1.Coord, center1);

                        }
                        else skip1 = true;

                        return;
                    }

                }

                //Checking anchors
                //Separate cycle because elements overlap each other
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Rect.Contains(location) && (this[i].Id == PointID.Anchor))
                    {
                        toMove = i;
                        isStart = (i == 0);
                        return;
                    }

                }


               //Checking segment
               if (GetSegmentNumber(location))
               {
                   skip1 = false;
                   skip2 = false;
                   int polyPos = polyNum * 3;

                   if (polyPos != 0)
                   {
                       center1 = this[polyPos].Coord;
                       second1 = this[polyPos - 1];
                       radius1 = Utils.getRadius(second1.Coord, center1);
                   }
                   else
                       skip1 = true;

                   if (polyPos != this.Count - 4)
                   {
                       center2 = this[polyPos + 3].Coord;
                       second2 = this[polyPos + 4];
                       radius2 = Utils.getRadius(second2.Coord, center2);
                   }
                   else
                       skip2 = true;
                  

               }

            }
            else
                if (mb == System.Windows.Forms.MouseButtons.Right)
                {
                    if (polyNum == -1)
                    {
                        var snupping = Utils.GetSnappingPoint(location, snapRes);

                        this.AddNode(snupping.X, snupping.Y);
                        canvas.Refresh();
                    }
                }
                else if (mb == System.Windows.Forms.MouseButtons.Middle)
                {
                    if (polyNum == -1)
                    {

                        for (int i = 0; i < this.Count; i++)
                        {
                            if (this[i].Rect.Contains(location) && (this[i].Id == PointID.Anchor))
                            {
                                this.toMove = i;

                                this.RemoveNode(toMove);
                                canvas.Refresh();

                                return;
                            }

                        }
                    }
                    else
                    {
                        int pos = polyNum * 3;
                        this.RemoveNode(pos);
                        this.RemoveNode(pos);
                        polyNum = -1;
                        canvas.Refresh();

                        return;                       
                    }

                }
        }

        public void MouseUp(MouseEventArgs e)
        {
            MouseUp(e.Location, e.Button);
        }

        public void MouseUp(Point location, MouseButtons mb)
        {

            if ((polyNum != -1) && (!polyMoving))
            {
                InsertAtPoly(polyNum, location);

                polyNum = -1;
            }

        }

        public void MouseMove(MouseEventArgs e)
        {
            MouseMove(e.Location, e.Button);
        }

        public void MouseMove(Point location, MouseButtons mb)
        {
            if (mb == MouseButtons.Left)
            {
                if (polyNum != -1)
                {
                    SetPolyPos(location);
                    polyMoving = true;
                }
                else
                    if (toMove != -1)
                    {
                        var ourPoint = this[toMove];

                        switch (ourPoint.Id)
                        {
                            case PointID.Anchor:

                                Point locPoint = location;

                                if (isSnap)
                                {
                                    locPoint = Utils.GetSnappingPoint(location, snapRes);
                                }

                                SetAnchorPos(locPoint);
                                break;


                            case PointID.Control1:

                                SetControlsPos(location);

                                break;
                            case PointID.Control2:

                                SetControlsPos(location);

                                break;
                            default:
                                break;
                        }

                    }

            }
            else
            {
                //If not Left Mouse Button
                //Finding element to highlight

                highLited = -1;
                polyNum = -1;

                //Finding Control
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Rect.Contains(location) && (this[i].Id != PointID.Anchor))
                    {
                        highLited = i;
                        break;
                    }
                }
                

                //Finding Anchor
                if (highLited == -1)
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Rect.Contains(location) && (this[i].Id == PointID.Anchor))
                        {
                            highLited = i;
                            break;
                        }
                    }
                }

                //Finding Line
                if (highLited == -1)
                    GetSegmentNumber(location);
            }

            canvas.Invalidate();
        }

        private bool GetSegmentNumber(Point location)
        {
            if (this.gp.IsOutlineVisible(location, pathPen))
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

                    var segmentPath = new GraphicsPath();

                    //Oh, it's a line!
                    if ((rrr[0] == rrr[1]) && (rrr[2] == rrr[3]))
                    {
                        segmentPath.AddLine(rrr[0], rrr[2]);
                    }
                    else
                    {
                        segmentPath.AddBezier(rrr[0], rrr[1], rrr[2], rrr[3]);
                    }

                    if (segmentPath.IsOutlineVisible(location, pathPen))
                    {
                        polyNum = k;
                        oldMousePoint = location;
                        break;
                    }

                    segmentPath.Dispose();
                }
            }

            return (polyNum != -1);
        }

        #endregion

        #region Export points

        private Point[] ToPointArray()
        {
            var resArr = new Point[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                resArr[i] = this[i].Coord;
            }
            return resArr;
        }

        public List<Point> ToPixels()
        {
            var temp = new List<Point>();

            GraphicsPath gp = new GraphicsPath();

            gp.AddBeziers(ToPointArray());
            gp.Flatten(null, 0.1f);

            for (int i = 0; i < gp.PointCount - 1; i++)
            {
                var xsss = Utils.GetPointsOnLine((int)gp.PathPoints[i].X, (int)gp.PathPoints[i].Y, (int)gp.PathPoints[i + 1].X, (int)gp.PathPoints[i + 1].Y);
                temp.AddRange(xsss);

                if (i != gp.PointCount - 1)
                    temp.RemoveAt(temp.Count - 1);
            }
            return temp;
        }

        internal List<Point> ToPoints()
        {
            var temp = new List<Point>();
           // gp.Flatten(null, 0.1f);

            for (int i = 0; i < this.Count; i++)
            {
                temp.Add(this[i].Coord);
            }

            return temp;
        }

        public List<Point> ToPolyline()
        {
            var temp = new List<Point>();
            gp.Flatten(null, 0.1f);

            for (int i = 0; i < this.Count; i++)
            {
                temp.Add(this[i].Coord);
            }

            return temp;
        }

        public List<PointF> ToPolylineF()
        {
            var temp = new List<PointF>();
            gp.Flatten(null, 0.1f);

            for (int i = 0; i < gp.PathPoints.Length; i++)
            {
                temp.Add(gp.PathPoints[i]);
            }

            return temp;
        }

        #endregion
    }

    #region Helper classes
    
    public class BezierPoint
    {
        public BezierPoint(Point _coord, PointID id)
        {
            this.Coord = _coord;
            this.Id = id;
            UpdateRect();
        }

        private void UpdateRect()
        {
            int Size;

            if (Id == PointID.Anchor)
                Size = BezierLine.anchorSize;
            else
                Size = BezierLine.controlSize;

            int Rad = Size / 2;

            Rect = new Rectangle(Coord.X - Rad, Coord.Y - Rad, Size, Size);
        }



        internal void ForceUpdateRect()
        {
            UpdateRect();
        }



        //Properties-----------------
        
        private Point coord;
        public Point Coord
        {
            get
            {
                return coord;
            }
            set
            {
                coord = value;
                UpdateRect();
            }
        }

        public PointID Id { set; get; }
        public Rectangle Rect { set; get; }

    }

    public class Utils
    {
        public static Color HighlightColor(Color input)
        {
            if (input.GetBrightness() < 0.7f)
                return ControlPaint.Light(input);
            else
                return ControlPaint.Dark(input);
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

        public static Point GetSnappingPoint(Point mouse, int cellSize)
        {
            //Get x interval based on cell width
            var xInterval = GetInterval(mouse.X, cellSize);

            //Get y interal based in cell height
            var yInterval = GetInterval(mouse.Y, cellSize);

            // return the point on cell grid closest to the mouseposition
            return new Point()
            {
                X = Math.Abs(xInterval.Lower - mouse.X) > Math.Abs(xInterval.Upper - mouse.X) ? xInterval.Upper : xInterval.Lower,
                Y = Math.Abs(yInterval.Lower - mouse.Y) > Math.Abs(yInterval.Upper - mouse.Y) ? yInterval.Upper : yInterval.Lower,
            };
        }

        private static Interval GetInterval(int mousePos, int size)
        {
            return new Interval()
            {
                Lower = ((mousePos / size)) * size,
                Upper = ((mousePos / size)) * size + size
            };
        }

        class Interval
        {
            public int Lower { get; set; }
            public int Upper { get; set; }
        }
    }

    #endregion

}
