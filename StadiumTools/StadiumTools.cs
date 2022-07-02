﻿using System.Collections.Generic;
using static System.Math;

namespace StadiumTools
{
    /// <summary>
    /// Stores a collection of unit conversion coeffecients to handle various modeling unit spaces
    /// </summary>
    public struct UnitHandler
    {
        public const double mm = 1000.0;
        public const double cm = 100.0;
        public const double m = 1.0;
        public const double inch = 39.3701;
        public const double feet = 3.28084;
        public const double yard = 1.09361;
    }

    /// <summary>
    /// Represents a point in 2D space (h,v)
    /// </summary>
    public struct Pt2d
    {
        //Properties
        /// <summary>
        /// Horizontal distance from origin
        /// </summary>
        public double H { get; set; }
        /// <summary>
        /// Vertical distance from origin
        /// </summary>
        public double V { get; set; }

        //Constructors
        public Pt2d(double h, double v)
        {
            this.H = h;
            this.V = v;
        }

        //Methods
    }

    /// <summary>
    /// Represents a vector in 2D space (h,v)
    /// </summary>
    public struct Vec2d
    {
        //Properties
        /// <summary>
        /// Horizontal component of vector.
        /// </summary>
        public double H { get; set; }
        /// <summary>
        /// Vertical component of vector.
        /// </summary>
        public double V { get; set; }
        /// <summary>
        /// Length of vector
        /// </summary>
        public double L { get; set; }

        //Constructors
        /// <summary>
        /// Construct a 2d vector from its components
        /// </summary>
        /// <param name="h"></param>
        /// <param name="v"></param>
        /// <param name="l"></param>
        public Vec2d(double h, double v, double l)
        {
            this.H = h;
            this.V = v;
            this.L = l;
        }

        /// <summary>
        /// Construct a 2d vector from two Pt2d point objects
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Vec2d(Pt2d start, Pt2d end)
        {
            this.H = end.H - start.H;
            this.V = end.V - start.V;
            this.L = Sqrt((end.H - start.H) * (end.H - start.H) + (end.V - start.V) * (end.V - start.V));
        }

        //Methods
    }

    /// <summary>
    /// Represents a point in 3D space (x, y, z)
    /// </summary>
    public struct Pt3d
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    /// <summary>
    /// Represents the properties of a single Spectator. 
    /// </summary>
    public class Spectator
    {
        //Properties
        /// <summary>
        /// the index of the spectator's tier within a section
        /// </summary>
        public int tierNum { get; set; }
        /// <summary>
        /// the index of the spectators row within a tier
        /// </summary>
        public int rowNum { get; set; }
        /// <summary>
        /// a point representing the location of the spectator seated in 2d space relative to the P.O.F
        /// </summary>
        public Pt2d Loc2d { get; set; }
        /// <summary>
        /// a point representing the location of spectator standing in 2d space relative to the P.O.F
        /// </summary>
        public Pt2d Loc2dStanding { get; set; }
        /// <summary>
        /// a point represneting the location of the P.O.F (Point of focus)
        /// </summary>
        public Pt2d pof { get; set; }
        /// <summary>
        /// a Vec2d representing the 2d (h(x), v(y)) components and Length of a seated spectator's sightline to the P.O.F
        /// </summary>
        public Vec2d sightLine { get; set; }
        /// <summary>
        /// a Vec2d representing the 2d (h(x), v(y)) components and Length of a standing spectator's sightline to the P.O.F
        /// </summary>
        public Vec2d sightLineStanding { get; set; }
        /// <summary>
        /// True if the spectator has an unobstructed sightline to the P.O.F
        /// </summary>
        public bool hasSightLine { get; set; } = false;
        /// <summary>
        /// The C-Value of the spectator
        /// </summary>
        public double cVal { get; set; } = 0.0;

        //Constructors 
        /// <summary>
        /// Initializes a new Spectator2D
        /// </summary>
        public Spectator()
        {
            Initialize();
        }

        /// <summary>
        /// construct a Spectator from a collection of values
        /// </summary>
        /// <param name="tierN"></param>
        /// <param name="rowN"></param>
        /// <param name="pt"></param>
        /// <param name="ptSt"></param>
        /// <param name="pof"></param>
        /// <param name="sLine"></param>
        /// <param name="sLineSt"></param>
        public Spectator(int tierN, int rowN, Pt2d pt, Pt2d ptSt, Pt2d pof, Vec2d sLine, Vec2d sLineSt)
        {
            this.tierNum = tierN;
            this.rowNum = rowN;
            this.Loc2d = pt;
            this.Loc2dStanding = ptSt;
            this.pof = pof;
            this.sightLine = sLine;
            this.sightLineStanding = sLineSt;
        }

        //Methods

        /// <summary>
        /// Initializes a spectator with default values.
        /// </summary>
        public void Initialize()
        {
            this.Loc2d = new Pt2d();
            this.Loc2dStanding = new Pt2d();
            this.pof = new Pt2d();
            this.sightLine = new Vec2d();
        }
    }

    /// <summary>
    /// Represents the data of a single seating tier.
    /// </summary>
    public class Tier
    {
        //Enums
        /// <summary>
        /// Available methods to generate a reference point for a tier
        /// </summary>
        public enum RefPtType
        {
            ByPOF,
            ByEndOfPrevTier
        }

        //Properties
        /// <summary>
        /// Method for determining the reference point of the tier
        /// </summary>
        public RefPtType RefPt { get; set; }
        /// <summary>
        /// Coeffecient for model unit space of the tier (mm, m, in, ft) 
        /// </summary>
        public double Unit;
        /// <summary>
        /// Point of focus for the spectators of this tier 
        /// </summary>
        public Pt2d POF { get; set; }
        /// <summary>
        /// Horizontal offset of Tier Start from reference point
        /// </summary>
        public double StartH { get; set; }
        /// <summary>
        /// Vertical offset of tier start from reference point
        /// </summary>
        public double StartV { get; set; }
        /// <summary>
        /// Maximum allowable c-value for spectators in this tier.
        /// </summary>
        public double Cvalue { get; set; }
        /// <summary>
        /// Horizontal offset of seated spectator eyes from riser nose 
        /// </summary>
        public double EyeH { get; set; }
        /// <summary>
        /// Vertical offset of seated spectator eyes from floor
        /// </summary>
        public double EyeV { get; set; }
        /// <summary>
        /// Horizontal offset of standing spectator eyes from riser nose 
        /// </summary>
        public double SEyeH { get; set; }
        /// <summary>
        /// Vertical offset of seated spectator eyes from floor
        /// </summary>
        public double SEyeV { get; set; }
        /// <summary>
        /// Number of rows in this tier not including super risers
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// Width(s) of row (distance from riser to riser)
        /// </summary>
        public List<double> RowWidth { get; set; }
        /// <summary>
        /// True if tier contains a vomitory
        /// </summary>
        public bool VomHas { get; set; }
        /// <summary>
        /// Row number of vomitory start
        /// </summary>
        public int VomStart { get; set; }
        /// <summary>
        /// Height of vomitory in rows 
        /// </summary>
        public int VomHeight { get; set; }
        /// <summary>
        /// Vertical height of fascia below the first row.
        /// </summary>
        public double FasciaH { get; set; }
        /// <summary>
        /// True if tier has a super riser
        /// </summary>
        public bool HasSuper { get; set; }
        /// <summary>
        /// Start row for inserting super riser
        /// </summary>
        public int SuperRow { get; set; }
        /// <summary>
        /// Optional curb distance before super riser 
        /// </summary>
        public double SuperCurb { get; set; }
        /// <summary>
        /// Horizontal offset of spectator eyes from nose of super riser 
        /// </summary>
        public double SuperEyeH { get; set; }
        /// <summary>
        /// Vertical offset of spectator eyes from floor of super riser
        /// </summary>
        public double SuperEyeV { get; set; }
        /// <summary>
        /// Optional value to round riser increments
        /// </summary>
        public double RoundTo { get; set; }
        /// <summary>
        /// An ordered list of Spectators in the tier
        /// </summary>
        public Spectator[] Spectators { get; set; }

        //Constructors
        /// <summary>
        /// Initializes a new default Tier
        /// </summary>
        public Tier()
        {
            Initialize();
        }

        //Methods
        /// <summary>
        /// Initializes a tier 2d instance with default values
        /// </summary>
        public void Initialize()
        {
            this.Unit = UnitHandler.m;
            this.RefPt = RefPtType.ByPOF;
            this.StartH = 5.0 * Unit;
            this.StartV = 1.0 * Unit;
            this.Cvalue = 0.10 * Unit;
            this.EyeH = 0.8 * Unit;
            this.EyeV = 1.2 * Unit;
            this.SEyeH = 0.8 * Unit;
            this.SEyeV = 2.5 * Unit;
            this.RowCount = 20;

            // Initialize all row widths to default value
            List<double> rowWidths = new List<double>();
            double defaultRW = 0.8;
            for (int i = 0; i < this.RowCount; i++)
            {   
                rowWidths.Add(defaultRW);
            }

            this.RowWidth = rowWidths;
            this.VomHas = true;
            this.VomStart = 5;
            this.VomHeight = 5;
            this.FasciaH = 1.0 * Unit;
            this.HasSuper = true;
            this.SuperRow = 15;
            this.SuperCurb = 0.01 * Unit;
            this.SuperEyeH = 0.8 * Unit;
            this.SuperEyeV = 2.5 * Unit;
            this.RoundTo = 0.001 * Unit;
        }
    }

    /// <summary>
    /// Represents an ordered collection of Tier objects that exist in the same plane
    /// </summary>
    public class Section
    {
        //Properties
        /// <summary>
        /// An ordered list of tiers to comprise the section.
        /// </summary>
        public Tier[] Tiers { get; set; }
        /// <summary>
        /// A list of 2d points that represent the outline of a seating tier
        /// </summary>
        public Pt2d POF { get; set; }

        //Constructors
        /// <summary>
        /// Construct a Section from a list of tiers 
        /// </summary>
        /// <param name="tiers"></param>
        public Section(List<Tier> tierList)
        {
            Tier[] tiers = tierList.ToArray();
            this.Tiers = tiers;
            this.POF = new Pt2d(0.0, 0.0);

            //Force first tier to get reference point from Point of Focus
            if (tiers[0].RefPt != Tier.RefPtType.ByPOF)
            {
                tiers[0].RefPt = Tier.RefPtType.ByPOF;
            }

            //Apply the section POF to all contained tiers 
            foreach (Tier t in tiers)
            {
                t.POF = this.POF;
            }
        }

        /// <summary>
        /// Construct a Section from an array of tiers
        /// </summary>
        /// <param name="tiers"></param>
        public Section(Tier[] tiers)
        {
            this.Tiers = tiers;
            this.POF = new Pt2d(0.0, 0.0);
        }

        /// <summary>
        /// Construct an empty Section from n tiers 
        /// </summary>
        /// <param name="nTiers"></param>
        public Section(int nTiers)
        {
            Tier[] tiers = new Tier[nTiers];
            this.Tiers = tiers;
            this.POF = new Pt2d(0.0, 0.0);
        }

        //Methods


    }
}
