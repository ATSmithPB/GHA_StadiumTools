﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StadiumTools
{
    //Properties
    public struct Line : ICurve
    {
        /// <summary>
        /// Start point of line
        /// </summary>
        public Pt3d Start { get; set; }
        /// <summary>
        /// End point in line
        /// </summary>
        public Pt3d End { get; set; }

        //Constructors
        public Line(Pt2d start, Pt2d end)
        {
            this.Start = new Pt3d(start, 0.0);
            this.End = new Pt3d(end, 0.0);
        }

        public Line(Pt3d start, Pt3d end)
        {
            this.Start = start;
            this.End = end;
        }

        //Methods
        public static Line[] RectangleCentered(Pln2d plane, double sizeX, double sizeY)
        {
            Line[] result = new Line[4];
            Pt2d[] pts = Pt2d.RectangleCentered(plane, sizeX, sizeY);
            result[0] = new Line(pts[0], pts[1]);
            result[1] = new Line(pts[1], pts[2]);
            result[2] = new Line(pts[2], pts[3]);
            result[3] = new Line(pts[3], pts[0]);
            return result;
        }

        public static Line[] RectangleCentered(Pln3d plane, double sizeX, double sizeY)
        {
            Line[] result = new Line[4];
            Pt3d[] pts = Pt3d.RectangleCentered(plane, sizeX, sizeY);
            result[0] = new Line(pts[0], pts[1]);
            result[1] = new Line(pts[1], pts[2]);
            result[2] = new Line(pts[2], pts[3]);
            result[3] = new Line(pts[3], pts[0]);
            return result;
        }
    }




}