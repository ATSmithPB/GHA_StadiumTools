﻿using System;
using System.Collections.Generic;
using static System.Math;

namespace StadiumTools
{
    /// <summary>
    /// Represents a Plane in 3D space 
    /// </summary>
    public struct Pln3d
    {
        //Enums
        /// <summary>
        /// Available default plane orientations
        /// </summary>
        public enum ReferencePtType
        {
            XY,
            YZ,
            XZ
        }

        //Prtoperties
        /// <summary>
        /// True if plane components are valid.
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// Pt3d representing the origin point of the Plane
        /// </summary>
        public Pt3d OriginPt { get; set; }
        /// <summary>
        /// x coordinate of plane origin
        /// </summary>
        public double OriginX { get; set; }
        /// <summary>
        /// y coordinate of plane origin
        /// </summary>
        public double OriginY { get; set; }
        /// <summary>
        /// z coordinate of plane origin
        /// </summary>
        public double OriginZ { get; set; }
        /// <summary>
        /// Vec3d representing the x axis
        /// </summary>
        public Vec3d Xaxis { get; set; }
        /// <summary>
        /// Vec3d representing the y axis
        /// </summary>
        public Vec3d Yaxis { get; set; }
        /// <summary>
        /// Vec3d representing the z axis
        /// </summary>
        public Vec3d Zaxis { get; set; }

        //Constructors
        /// <summary>
        /// construct a Pln3d with an origin point and three vectors
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Pln3d(Pt3d origin, Vec3d x, Vec3d y, Vec3d z)
        {
            this.IsValid = false;
            this.OriginPt = origin;
            this.OriginX = origin.X;
            this.OriginY = origin.Y;
            this.OriginZ = origin.Z;
            this.Xaxis = Vec3d.Normalize(x);
            this.Yaxis = Vec3d.Normalize(y);
            this.Zaxis = Vec3d.Normalize(z);
            GetValidity(this);
        }

        public Pln3d(Pt3d origin, Vec3d x, Vec3d y)
        {
            this.IsValid = false;
            this.OriginPt = origin;
            this.OriginX = origin.X;
            this.OriginY = origin.Y;
            this.OriginZ = origin.Z;
            this.Xaxis = Vec3d.Normalize(x);
            this.Yaxis = Vec3d.Normalize(y);
            this.Zaxis = Vec3d.CrossProduct(this.Xaxis, this.Yaxis);
            GetValidity(this);
        }

        public Pln3d(Pln2d plane)
        {
            this.IsValid = false;
            Pt3d origin = new Pt3d(plane.OriginPt, 0.0);
            this.OriginPt = origin;
            this.OriginX = origin.X;
            this.OriginY = origin.Y;
            this.OriginZ = origin.Z;
            this.Xaxis = new Vec3d(plane.Xaxis, 0.0);
            this.Yaxis = new Vec3d(plane.Yaxis, 0.0);
            this.Zaxis = Vec3d.ZAxis;
            GetValidity(this);
        }

        public Pln3d(Pt3d origin, Pt3d ptOnZAxis)
        {
            this.IsValid = false;
            this.OriginPt = origin;
            this.OriginX = origin.X;
            this.OriginY = origin.Y;
            this.OriginZ = origin.Z;
            Vec3d normalZ = Vec3d.Normalize(new Vec3d(origin, ptOnZAxis));
            this.Zaxis = normalZ;
            this.Xaxis = Vec3d.Normalize(Vec3d.PerpTo(normalZ));
            this.Yaxis = Vec3d.Normalize(Vec3d.CrossProduct(normalZ, this.Xaxis));
            GetValidity(this);
        }

        /// <summary>
        /// Construct a plane aligned to defauly XYZ with a specified origin
        /// </summary>
        /// <param name="origin"></param>
        public Pln3d(Pt3d origin)
        {
            this.IsValid = false;
            this.OriginPt = origin;
            this.OriginX = origin.X;
            this.OriginY = origin.Y;
            this.OriginZ = origin.Z;
            this.Xaxis = Vec3d.XAxis;
            this.Yaxis = Vec3d.YAxis;
            this.Zaxis = Vec3d.ZAxis;
            GetValidity(this);
        }

        //Delegates
        /// <summary>
        /// Gets Vector with Default XAxis components (1.0, 0.0, 0.0)
        /// </summary>
        public static Pln3d XYPlane => new Pln3d(Pt3d.Origin, Vec3d.XAxis, Vec3d.YAxis, Vec3d.ZAxis);

        //Methods
        /// <summary>
        /// Check if all plane components construct a valid plane.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>bool</returns>
        private static bool GetValidity(Pln3d p)
        {
            bool isValid = true;

            return isValid;
        }

        public Pt3d ToPt3d(Pln3d pln)
        {
            Pt3d pt3d = pln.OriginPt;
            return pt3d;
        }

        /// <summary>
        /// returns a collection of perpendicular planes to a list of points
        /// </summary>
        /// <param name="pts"></param>
        /// <returns>Pln3d[]</returns>
        public static Pln3d[] PerpPlanes(Pt3d[] pts)
        {
            if (pts.Length < 2)
            {
                throw new ArgumentException("pts.Length must be >1 to calculate perp plane");
            }
            Pln3d[] pln3ds = new Pln3d[pts.Length];
            for (int i = 0; i < pts.Length - 1; i++)
            {
                pln3ds[i] = new Pln3d(pts[i], pts[i + 1]);
            }
            return pln3ds;
        }

        /// <summary>
        /// returns a collection of perpendicular planes to a list of points
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        public static Pln3d[] PerpPlanes(List<Pt3d> pts)
        {
            if (pts.Count < 2)
            {
                throw new ArgumentException("pts.Count must be >1 to calculate perp plane");
            }
            Pln3d[] pln3ds = new Pln3d[pts.Count];
            for (int i = 0; i < pts.Count - 1; i++)
            {
                pln3ds[i] = new Pln3d(pts[i], pts[i + 1]);
            }
            return pln3ds;
        }

        /// <summary>
        /// returns true if two Pln3d planes are coplanar within a given absolute tolerance
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <param name="angleTolerance"></param>
        /// <returns></returns>
        public static bool IsCoPlanar(Pln3d a, Pln3d b, double tolerance)
        {
            bool result = false;
            if (Vec3d.IsParallel(a.Zaxis, b.Zaxis, tolerance))
            {
                if(Math.Abs(Pt3d.LocalCoordinates(a.OriginPt, b).Z) < tolerance)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// returns a point on the plane closest to specified point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns>Pt3d</returns>
        public Pt3d ClosestPointTo(Pt3d pt)
        {
            double u = 0.0;
            double v = 0.0;
            ClosestPointTo(pt, u, v);
            return PointAt(u, v);
        }

        /// <summary>
        /// returns a point on the place closest to the specified (u,v) parameters
        /// </summary>
        /// <param name="p"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool ClosestPointTo(Pt3d p, double u, double v)
        {
          Vec3d vec = new Vec3d(p - this.OriginPt);
            if (u != 0)
            {
                u = vec * this.Xaxis;
            }
            if (v != 0)
            {
                v = vec * this.Yaxis;
            }
            return true;
        }

        /// <summary>
        /// returns a point (in world coordinates) at the specified u,v parameters of this Plane. 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns>Pt3d</returns>
        public Pt3d PointAt(double u, double v)
        {
            return this.OriginPt + (u * this.Xaxis) + (v * this.Yaxis);   
        }

        /// <summary>
        /// returns a point (in world coordinates) at the specified u,v,w parameters of this Plane. 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns>Pt3d</returns>
        public Pt3d PointAt(double u, double v, double w)
        {
            return this.OriginPt + (u * this.Xaxis) + (v * this.Yaxis) + (w * this.Zaxis);
        }

    }
}
