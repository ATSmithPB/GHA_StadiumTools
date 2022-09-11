﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StadiumTools
{
    public struct Circle : ICurve
    {
        //Properties
        public Pln3d Center { get; set; }
        public double Radius { get; set; }
        public Pt3d Start { get; set; }
        public Pt3d End { get; set; }

        //Constructors
        public Circle(Pln3d center, double radius)
        {
            Center = center;
            Radius = radius;
            Pt3d seamPt = center.OriginPt + (radius * center.Xaxis);
            Start = seamPt;
            End = seamPt;
        }

        public Circle(Arc arc)
        {
            Center = arc.Plane;
            Radius = arc.Radius;
            Start = arc.Start;
            End = arc.Start;
        }

        //Methods
        /// <summary>
        /// returns the two intersection points of two circles if intersecting
        /// </summary>
        /// <param name="circleA"></param>
        /// <param name="circleB"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Pt3d[] Intersect(Circle circleA, Circle circleB, double tolerance)
        {
            if (!Pln3d.IsCoPlanar(circleA.Center, circleB.Center, tolerance))
            {
                throw new Exception("The two circles are not coplanar");
            }
            
            if (!IsIntersecting(circleA, circleB, out double dist))
            {
                throw new Exception("The two circles do not intersect");
            }
            
            Circle[] circles = new Circle[2] { circleA, circleB };
            if (circleB.Radius >= circleA.Radius)
            {
                circles[0] = circleB;
                circles[1] = circleA;
            }

            double rad0 = circles[0].Radius;
            double rad1 = circles[1].Radius;
            Vec3d vecD = new Vec3d(circles[1].Center.OriginPt - circles[0].Center.OriginPt);
            double disD = vecD.M;
            double x = disD - (rad0 + rad1);

            if (x < tolerance && x > -tolerance)
            {
                Pt3d[] intersectionPts = new Pt3d[1];
                intersectionPts[0] = Pt3d.Midpoint(circles[0].Center.OriginPt, circles[1].Center.OriginPt);
                return intersectionPts;
            }

            else 
            {
                Pt3d[] intersectPts = new Pt3d[2];
                vecD.Normalize();
                Vec3d Dperp = Vec3d.CrossProduct(vecD, circles[0].Center.Zaxis);
                double d1 = (rad0 * rad0 - rad1 * rad1 + disD * disD) / (2 * disD);
                double a1 = rad0 * rad0 - d1 * d1;

                if (a1 < 0)
                {
                    a1 = 0;
                }
                
                a1 = Math.Sqrt(a1);

                if (a1 < .5 * tolerance)
                {
                    intersectPts[0] = circles[0].Center.OriginPt + d1 * vecD;
                }
                else
                {
                    intersectPts[0] = circles[0].Center.OriginPt + d1 * vecD + a1 * Dperp;
                    intersectPts[1] = circles[0].Center.OriginPt + d1 * vecD - a1 * Dperp;
                }
                return intersectPts;
            }
        }

        public static bool IsIntersecting(Circle a, Circle b)
        {
            bool result = false;
            double d = Pt3d.Distance(a.Center.OriginPt, b.Center.OriginPt);
            if (a.Radius + b.Radius >= d || d >= Math.Abs(a.Radius - b.Radius))
            {
                result = true;
            }
            return result;
        }

        public static bool IsIntersecting(Circle a, Circle b, out double distance)
        {
            bool result = false;
            distance = Pt3d.Distance(a.Center.OriginPt, b.Center.OriginPt);
            if (a.Radius + b.Radius >= distance || distance >= Math.Abs(a.Radius - b.Radius))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// returns the angle of a chord 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="chordLength"></param>
        /// <returns>double</returns>
        public static double ChordAngle(double radius, double chordLength)
        {
            return 2 * Math.Asin(chordLength/ (2* radius));
        }

        /// <summary>
        /// returns the distance between a chord and it's circle's centerpoint
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="chordLength"></param>
        /// <returns>double</returns>
        public static double ChordMidCen(double radius, double chordLength)
        {
            double half0 = ChordAngle(radius, chordLength) / 2;
            double halfChord = chordLength / 2;
            return Math.Sqrt((radius * radius) - (halfChord * halfChord));
        }

        /// <summary>
        /// returns the perpendicular distance between a chord's midpoint and the circle perimeter away from the circles center  
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="chordLength"></param>
        /// <returns>double</returns>
        public static double ChordMidCirc(double radius, double chordLength)
        {
            return radius - ChordMidCen(radius, chordLength);
        }

        /// <summary>
        /// returns the midpoint of a circle. Diametrically opposed point to the start
        /// </summary>
        /// <returns>Pt3d</returns>
        public Pt3d Midpoint()
        {
            return this.Center.OriginPt + (-this.Radius * this.Center.Xaxis);
        }

        /// <summary>
        /// returns a point at a specified parameter on a circle.
        /// </summary>
        /// <param name="angleRadians"></param>
        /// <returns></returns>
        public Pt3d PointAt(double angleRadians)
        {
            return Pt3d.Rotate(this.Center, this.Start, angleRadians);
        }

        /// <summary>
        /// returns the circumference of a circle
        /// </summary>
        /// <returns>double</returns>
        public double Length()
        {
            return 2 * Math.PI * this.Radius;
        }

        /// <summary>
        /// returns true if successful. out offset ICurve. A negative value will offset towards circle center. 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="offsetCirc"></param>
        /// <returns>bool</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool Offset(double distance, out ICurve offsetCurve)
        {
            bool result = this.Offset(distance, out Circle offsetCircle);
            offsetCurve = offsetCircle;
            return result;
        }

        /// <summary>
        /// returns true if successful. out offset Circle. A negative value will offset towards circle center. 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="offsetCircle"></param>
        /// <returns>bool</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool Offset(double distance, out Circle offsetCircle)
        {
            if (distance <= -this.Radius)
            {
                throw new ArgumentException($"offset distance [{distance}] must exceed -1 * radius [{-this.Radius}]");
            }
            offsetCircle = new Circle(this.Center, this.Radius + distance);
            return true;
        }
    }

}
