﻿using System.Collections.Generic;
using static System.Math;

namespace StadiumTools
{
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
            if (tiers[0].RefPtType != Tier.ReferencePtType.ByPOF)
            {
                tiers[0].RefPtType = Tier.ReferencePtType.ByPOF;
            }

            //Apply the section POF to all contained tiers 
            foreach (Tier t in tiers)
            {
                t.POF = this.POF;
            }

            //Calculate points for all tiers in the section
            CalcPts(this);
        }

        /// <summary>
        /// Construct a Section from an array of tiers
        /// </summary>
        /// <param name="tiers"></param>
        public Section(Tier[] tiers)
        {
            this.Tiers = tiers;
            this.POF = new Pt2d(0.0, 0.0);

            for (int i = 0; i < tiers.Length; i++)
            {
                tiers[i].SectionIndex = i;
            }

            CalcPts(this);
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

        /// <summary>
        /// Return a jagged array of all tier points in this section
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Pt2d[][] GetSectionPts(Section s)
        {
            Pt2d[][] sectionPts = new Pt2d[s.Tiers.Length][];

            for (int i = 0; i < s.Tiers.Length; i++)
            {
                sectionPts[i] = s.Tiers[i].Points2d;
            }
            return sectionPts;
        }

        /// <summary>
        /// Return a jagged array of all spectator points in this section
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Pt2d[][] GetSpectatorPts(Section section, bool standing)
        {
            Pt2d[][] specPts = new Pt2d[section.Tiers.Length][];

            if (standing)
            {
                for (int i = 0; i < section.Tiers.Length; i++)
                {
                    Pt2d[] thisTierSpecPts = new Pt2d[section.Tiers[i].RowCount];
                    for (int j = 0; j < section.Tiers[i].RowCount; j++)
                    {
                        Pt2d specPt = section.Tiers[i].Spectators[j].Loc2dStanding;
                        thisTierSpecPts[j] = specPt;
                    }
                    specPts[i] = thisTierSpecPts;
                }
            }
            else
            {
                for (int i = 0; i < section.Tiers.Length; i++)
                {
                    Pt2d[] thisTierSpecPts = new Pt2d[section.Tiers[i].RowCount];
                    for (int j = 0; j < section.Tiers[i].RowCount; j++)
                    {
                        Pt2d specPt = section.Tiers[i].Spectators[j].Loc2d;
                        thisTierSpecPts[j] = specPt;
                    }
                    specPts[i] = thisTierSpecPts;
                }
            }

            return specPts;
        }

        /// <summary>
        /// Returns an array of Vec2d objects for each seated spectator sightline
        /// </summary>
        /// <param name="section"></param>
        /// <returns>Vec2d[][]</returns>
        public static Vec2d[][] GetSightlines(Section section, bool standing)
        {
            Vec2d[][] sightLines = new Vec2d[section.Tiers.Length][];
            
            if (standing)
            {
                for (int i = 0; i < section.Tiers.Length; i++)
                {
                    Vec2d[] thisTierSightLines = new Vec2d[section.Tiers[i].RowCount];
                    for (int j = 0; j < section.Tiers[i].RowCount; j++)
                    {
                        Vec2d sightLine = section.Tiers[i].Spectators[j].SightLineStanding;
                        thisTierSightLines[j] = sightLine;
                    }
                    sightLines[i] = thisTierSightLines;
                }
            }
            else
            {
                for (int i = 0; i < section.Tiers.Length; i++)
                {
                    Vec2d[] thisTierSightLines = new Vec2d[section.Tiers[i].RowCount];
                    for (int j = 0; j < section.Tiers[i].RowCount; j++)
                    {
                        Vec2d sightLine = section.Tiers[i].Spectators[j].SightLine;
                        thisTierSightLines[j] = sightLine;
                    }
                    sightLines[i] = thisTierSightLines;
                }
            }
            return sightLines;
        }

        /// <summary>
        /// Returns a list of 2d points that define a seating tier
        /// </summary>
        /// <param name="t"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private static void CalcPts(Section section)
        {
            for (int t = 0; t < section.Tiers.Length; t++)
            {
                CalcTierPoints(section, section.Tiers[t]);
            }
        }

        /// <summary>
        /// Returns an array of Pt2d objects for a given tier within a section
        /// </summary>
        /// <param name="section"></param>
        /// <param name="tier"></param>
        /// <returns>Pt2d[]</returns>
        private static void CalcTierPoints(Section section, Tier tier)
        {
            if (tier.RefPtType == Tier.ReferencePtType.ByEndOfPrevTier)
            {
                //This tier's RefPt == Last point of previous tier
                tier.RefPt = section.Tiers[tier.SectionIndex - 1].Points2d[section.Tiers[tier.SectionIndex - 1].Points2dCount - 1];
            }
            CalcRowPoints(tier);
        }

        /// <summary>
        /// Calculates B and C points for a tier iterativly.
        /// </summary>
        /// <param name="tier"></param>
        private static void CalcRowPoints(Tier tier)
        {
            //Points increment
            int p = 0;

            //Calc optional Fascia Point
            if (tier.FasciaH != 0.0)
            {
                tier.Points2d[p] = (new Pt2d(tier.RefPt.H + tier.StartH, ((tier.RefPt.V + tier.StartV) - tier.FasciaH)));
                p++;
            }

            //Calc first tier point (PtA)
            Pt2d prevPt = new Pt2d(tier.RefPt.H + tier.StartH, tier.RefPt.V + tier.StartV);
            tier.Points2d[p] = prevPt;
            p++;

            //Calc riser points for each row iterativly | Pts(B) & Pts(C)
            for (int row = 0; row < (tier.RowCount - 1); row++)
            {
                //Get rear riser bottom point (PtB) for current row
                Pt2d currentPt = new Pt2d();
                currentPt.H = prevPt.H + (tier.RowWidth[row]);
                currentPt.V = prevPt.V;
                tier.Points2d[p] = currentPt;
                p++;

                //Instance a spectator for current row
                CalcRowSpectator(tier, currentPt, row);

                //Calc rear riser top point (PtC) for current row and add to list
                double n = RiserHeightFromCVal(tier, currentPt, row, false);
                currentPt.V += n;
                tier.Points2d[p] = currentPt;
                p++;

                prevPt = currentPt;
            }

            //Add final tier point (PtD) to tier
            prevPt.H += (tier.RowWidth[tier.RowCount - 1]);
            tier.Points2d[p] = prevPt;
            CalcRowSpectator(tier, prevPt, tier.RowCount - 1);
        }

        /// <summary>
        /// Creates and adds a row's spectator to the tier. PtB argument should be rear lower riser point.
        /// </summary>
        /// <param name="tier"></param>
        /// <param name="pt"></param>
        private static void CalcRowSpectator(Tier tier, Pt2d ptB, int row)
        {
            Pt2d specPt = new Pt2d(ptB.H - tier.EyeH, ptB.V + tier.EyeV);
            Pt2d specPtSt = new Pt2d(ptB.H - tier.SEyeH, ptB.V + tier.SEyeV);
            Vec2d sLine = new Vec2d(specPt, tier.POF);
            Vec2d sLineSt = new Vec2d(specPtSt, tier.POF);
            Spectator spectator = new Spectator(tier.SectionIndex, row, specPt, specPtSt, tier.POF, sLine, sLineSt);
            tier.Spectators[row] = spectator;
        }

        /// <summary>
        /// Calculates the riser height based on minimum C-Value requirement using triangle proportionality
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <param name="rounding"></param>
        /// <returns></returns>
        private static double RiserHeightFromCVal(Tier tier, Pt2d ptB, int row, bool standing)
        {
            double c = tier.Cvalue;
            double h = ptB.V + tier.EyeV;
            double t = tier.RowWidth[row + 1];
            double d = (ptB.H - tier.EyeH) + t;

            //Function of N, Triangle Proportionality Theroum 
            double r = ((c + h) / (d - t)) * (d);

            double n = (r - tier.EyeV - ptB.V);
            double nR = n;
            return n;
        }




    }
}
