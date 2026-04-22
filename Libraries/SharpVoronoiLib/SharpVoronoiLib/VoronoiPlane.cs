using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SharpVoronoiLib.Exceptions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SharpVoronoiLib
{
    /// <summary>
    /// An Euclidean plane where a Voronoi diagram can be constructed from <see cref="VoronoiSite"/>s
    /// producing a tesselation of cells with <see cref="VoronoiEdge"/> line segments and <see cref="VoronoiPoint"/> vertices.
    /// </summary>
    public class VoronoiPlane
    {
        [PublicAPI]
        public List<VoronoiSite> Sites
        {
            get
            {
                if (_sites == null) throw new VoronoiDoesntHaveSitesException();

                return _sites;
            }
        }

        [PublicAPI]
        public List<VoronoiEdge> Edges
        {
            get
            {
                if (Sites == null) throw new VoronoiDoesntHaveSitesException();
                if (_edges == null) throw new VoronoiNotTessellatedException();

                return _edges;
            }
        }

        [PublicAPI]
        public List<VoronoiPoint> Points
        {
            get
            {
                if (Sites == null) throw new VoronoiDoesntHaveSitesException();
                if (_edges == null) throw new VoronoiNotTessellatedException();

                if (_points == null)
                {
                    HashSet<VoronoiPoint> points = new HashSet<VoronoiPoint>(VoronoiPointComparer.Instance);

                    foreach (VoronoiEdge edge in _edges)
                    {
                        points.Add(edge.Start);
                        points.Add(edge.End);
                    }

                    _points = points.ToList();

                    // This is not efficient but tracking points both during tessellation and clipping/closing is way too complex to bother with
                }

                return _points;
            }
        }

        [PublicAPI] public double MinX { get; }

        [PublicAPI] public double MinY { get; }

        [PublicAPI] public double MaxX { get; }

        [PublicAPI] public double MaxY { get; }


        [PublicAPI]
        public int DuplicateCount
        {
            get
            {
                if (Sites == null) throw new VoronoiDoesntHaveSitesException();
                if (_edges == null) throw new VoronoiNotTessellatedException();

                return _duplicateSitesCount;
            }
        }

        [PublicAPI] public bool Tesselated => _edges != null;


        private int _version;

        private List<VoronoiSite>? _sites;

        private List<VoronoiEdge>? _edges;

        private List<VoronoiPoint>? _points;

        private RandomUniformPointGeneration? _randomUniformPointGeneration;
        private RandomGaussianPointGeneration? _randomGaussianPointGeneration;

        private ITessellationAlgorithm? _tessellationAlgorithm;

        private IBorderClippingAlgorithm? _borderClippingAlgorithm;

        private IBorderClosingAlgorithm? _borderClosingAlgorithm;

        private IRelaxationAlgorithm? _relaxationAlgorithm;

        private ISiteMergingAlgorithm? _siteMergingAlgorithm;

        private BruteForceNearestSiteLookup? _bruteForceNearestSiteLookup;
        private KDTreeNearestSiteLookup? _kdTreeNearestSiteLookupAlgorithm;

        private BorderEdgeGeneration _lastBorderGeneration;
        private bool _lastWrapHorizontal;

        private int _duplicateSitesCount;


        public VoronoiPlane(double minX, double minY, double maxX, double maxY)
        {
            if (minX >= maxX) throw new ArgumentException();
            if (minY >= maxY) throw new ArgumentException();

            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }


        [PublicAPI]
        public void SetSites(List<VoronoiSite> sites)
        {
            if (sites == null) throw new ArgumentNullException(nameof(sites));

            _sites = sites;

            _edges = null;
            _duplicateSitesCount = 0; // will be recomputed on tessellation

            _version++;
        }

        /// <summary>
        /// Creates a bunch of randomly-placed sites within the bounds of the plane with the given algorithm.
        /// The generated sites are guaranteed to not repeat (they may be close, but they at least will not have a hash collision).
        /// The generated sites are guaranteed not to lie on the border of the plane (although they may be very close).
        /// </summary>
        [PublicAPI]
        public List<VoronoiSite> GenerateRandomSites(int amount,
            PointGenerationMethod method = PointGenerationMethod.Uniform)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));


            IPointGenerationAlgorithm algorithm = GetPointGenerationAlgorithm(method);

            List<VoronoiSite> sites = algorithm.Generate(MinX, MinY, MaxX, MaxY, amount);

            _sites = sites;

            _edges = null;
            _duplicateSitesCount = 0; // will be recomputed on tessellation

            _version++;

            return sites;
        }

        [PublicAPI]
        public List<VoronoiEdge> Tessellate(
            BorderEdgeGeneration borderGeneration = BorderEdgeGeneration.MakeBorderEdges, bool wrapHorizontal = false)
        {
            if (Sites == null) throw new VoronoiDoesntHaveSitesException();


            _lastBorderGeneration = borderGeneration;
            _lastWrapHorizontal = wrapHorizontal;

            // Tessellate

            if (_tessellationAlgorithm == null)
                _tessellationAlgorithm = new FortunesTessellationHorizontalWrap();

            List<VoronoiEdge> edges = _tessellationAlgorithm.Run(Sites, MinX, MinY, MaxX, MaxY, out int duplicateCount);
            _duplicateSitesCount = duplicateCount;

            // Clip

            // todo: make clipping optional

            if (_borderClippingAlgorithm == null)
                _borderClippingAlgorithm = new GenericClipping();

            edges = _borderClippingAlgorithm.Clip(edges, MinX, MinY, MaxX, MaxY);

            // Enclose

            if (borderGeneration == BorderEdgeGeneration.MakeBorderEdges)
            {
                if (_borderClosingAlgorithm == null)
                    _borderClosingAlgorithm = new GenericBorderClosing();

                edges = _borderClosingAlgorithm.Close(edges, MinX, MinY, MaxX, MaxY, Sites);
            }

            if (wrapHorizontal)
            {
                //at the end of fortune's algo, add the wraparound nodes as neighbors

                Func<VoronoiSite, double, double, double> computeDistance =
                    (VoronoiSite site, double height, double Y) =>
                    {
                        return Mathf.Sqrt((float)(Mathf.Pow((float)(site.Y - Y), 2) +
                                                  Mathf.Pow((float)(site.X - height), 2)));
                    };

                double epsilon = 0.001f;

                Func<double, double, bool> ApproxEqual = (double a, double b) =>
                {
                    return (a - b) < epsilon && (b - a) < epsilon;
                };

                Action<VoronoiSite, VoronoiEdge, double> updateWraparoundNeighbors =
                    (VoronoiSite siteWithNullNeighbor, VoronoiEdge edge, double Y) =>
                    {
                        double minDistance = double.MaxValue;
                        VoronoiSite minDistanceSite = null;
                        VoronoiEdge minDistanceEdge = null;
                        //find closest site near same position at start
                        for (int i = 0; i < edges.Count; i++)
                        {
                            double distance = 0;
                            if (edges[i].Left != null)
                            {
                                distance = computeDistance(edges[i].Left, edge.Mid.X, Y);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    minDistanceSite = edges[i].Left;
                                    minDistanceEdge = edges[i];
                                }
                            }

                            if (edges[i].Right != null)
                            {
                                distance = computeDistance(edges[i].Right, edge.Mid.X, Y);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    minDistanceSite = edges[i].Right;
                                    minDistanceEdge = edges[i];
                                }
                            }
                        }

                        if (minDistanceSite != null)
                        {
                            minDistanceSite.AddNeighbour(siteWithNullNeighbor);

                            if (edge.Left == null)
                            {
                                edge.Left = minDistanceSite;
                            }
                            else if (edge.Right == null)
                            {
                                edge.Right = minDistanceSite;
                            }

                            if (minDistanceEdge.Left == null)
                            {
                                minDistanceEdge.Left = siteWithNullNeighbor;
                            }
                            else if (minDistanceEdge.Right == null)
                            {
                                minDistanceEdge.Right = siteWithNullNeighbor;
                            }
                        }
                    };
                foreach (var edge in edges)
                {
                    VoronoiSite siteWithNullNeighbor = null;
                    if (edge.Left == null)
                    {
                        siteWithNullNeighbor = edge.Right;
                    }

                    if (edge.Right == null)
                    {
                        siteWithNullNeighbor = edge.Left;
                    }

                    if (siteWithNullNeighbor != null)
                    {
                        //border edge, ensure it's on the left side before adding neighbor
                        if (edge.Mid.Y.ApproxEqual(MaxY))
                        {
                            updateWraparoundNeighbors(siteWithNullNeighbor, edge, MinY);
                        }
                        else if (edge.Mid.Y.ApproxEqual(MinY))
                        {
                            updateWraparoundNeighbors(siteWithNullNeighbor, edge, MaxY);
                        }
                    }
                }
            }

            // Done

            _edges = edges;

            _version++;

            return edges;
        }

        [PublicAPI]
        public List<VoronoiEdge> Relax(int iterations = 1, float strength = 1.0f, bool reTessellate = true)
        {
            if (Sites == null) throw new VoronoiDoesntHaveSitesException();
            if (_edges == null) throw new VoronoiNotTessellatedException();
            if (iterations < 1) throw new ArgumentOutOfRangeException(nameof(iterations));
            if (strength <= 0f || strength > 1f) throw new ArgumentOutOfRangeException(nameof(strength));


            if (_relaxationAlgorithm == null)
                _relaxationAlgorithm = new LloydsRelaxation();

            for (int i = 0; i < iterations; i++)
            {
                // Relax once
                _relaxationAlgorithm.Relax(Sites, MinX, MinY, MaxX, MaxY, strength);

                if (i < iterations -
                    1 || // always have to tessellate if this isn't the last iteration, otherwise this makes no sense
                    reTessellate)
                {
                    // Re-tesselate with the new site locations
                    Tessellate(_lastBorderGeneration, _lastWrapHorizontal); // will set Edges
                }
            }

            _version++;

            return _edges;
        }

        /// <summary>
        /// WARNING: Work in progress, use at your own risk, see https://github.com/RudyTheDev/SharpVoronoiLib/issues/5.
        /// </summary>
        [PublicAPI]
        public List<VoronoiSite> MergeSites(VoronoiSiteMergeQuery mergeQuery)
        {
            if (Sites == null) throw new VoronoiDoesntHaveSitesException();
            if (_edges == null) throw new VoronoiNotTessellatedException();
            if (mergeQuery == null) throw new ArgumentNullException(nameof(mergeQuery));


            if (_siteMergingAlgorithm == null)
                _siteMergingAlgorithm = new GenericSiteMergingAlgorithm();

            _siteMergingAlgorithm.MergeSites(Sites, _edges, mergeQuery);

            _version++;

            return Sites;
        }

        [PublicAPI]
        public VoronoiSite GetNearestSiteTo(double x, double y,
            NearestSiteLookupMethod lookupMethod = NearestSiteLookupMethod.KDTree)
        {
            if (Sites == null) throw new VoronoiDoesntHaveSitesException();
            if (_edges == null) throw new VoronoiNotTessellatedException();


            INearestSiteLookup algorithm = GetNearestSiteLookupAlgorithm(lookupMethod);

            return algorithm.GetNearestSiteTo(Sites, x, y, _version, _duplicateSitesCount);
        }


        [PublicAPI]
        public static List<VoronoiEdge> TessellateRandomSitesOnce(int numberOfSites, double minX, double minY,
            double maxX, double maxY, BorderEdgeGeneration borderGeneration = BorderEdgeGeneration.MakeBorderEdges,
            bool wrapHorizontal = false)
        {
            if (numberOfSites < 0) throw new ArgumentOutOfRangeException(nameof(numberOfSites));


            VoronoiPlane plane = new VoronoiPlane(minX, minY, maxX, maxY);

            plane.GenerateRandomSites(numberOfSites);

            return plane.Tessellate(borderGeneration, wrapHorizontal);
        }

        [PublicAPI]
        public static List<VoronoiEdge> TessellateOnce(List<VoronoiSite> sites, double minX, double minY, double maxX,
            double maxY, BorderEdgeGeneration borderGeneration = BorderEdgeGeneration.MakeBorderEdges,
            bool wrapHorizontal = false)
        {
            if (sites == null) throw new ArgumentNullException(nameof(sites));


            VoronoiPlane plane = new VoronoiPlane(minX, minY, maxX, maxY);

            plane.SetSites(sites);

            return plane.Tessellate(borderGeneration, wrapHorizontal);
        }


        private IPointGenerationAlgorithm GetPointGenerationAlgorithm(PointGenerationMethod pointGenerationMethod)
        {
            return pointGenerationMethod switch
            {
                PointGenerationMethod.Uniform => _randomUniformPointGeneration ??= new RandomUniformPointGeneration(),
                PointGenerationMethod.Gaussian =>
                    _randomGaussianPointGeneration ??= new RandomGaussianPointGeneration(),

#if DEBUG
                PointGenerationMethod.Naughty => new RandomNaughtyPointGeneration(),
#endif

                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private INearestSiteLookup GetNearestSiteLookupAlgorithm(NearestSiteLookupMethod nearestSiteLookupMethod)
        {
            return nearestSiteLookupMethod switch
            {
                NearestSiteLookupMethod.BruteForce =>
                    _bruteForceNearestSiteLookup ??= new BruteForceNearestSiteLookup(),
                NearestSiteLookupMethod.KDTree => _kdTreeNearestSiteLookupAlgorithm ??= new KDTreeNearestSiteLookup(),

                _ => throw new ArgumentOutOfRangeException()
            };
        }


        public override string ToString()
            => ToString("F3");

        [PublicAPI]
        public string ToString(string floatFormat)
        {
            return
                "(" + MinX.ToString(floatFormat) + "," + MinY.ToString(floatFormat) + ") -> (" +
                MaxX.ToString(floatFormat) + "," + MaxY.ToString(floatFormat) + ")" +
                (_sites != null ? " with " + Sites.Count + " sites" : "") +
                (_edges != null ? (_sites != null ? ", " : " with ") + _edges.Count + " edges" : "");
        }
    }


    public enum BorderEdgeGeneration
    {
        DoNotMakeBorderEdges = 0,
        MakeBorderEdges = 1
    }
}