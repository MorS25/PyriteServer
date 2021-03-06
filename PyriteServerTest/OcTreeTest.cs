﻿// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="OcTreeTest.cs" company="Microsoft Corporation">
// // // Copyright (c) Microsoft Corporation. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace PyriteServerTest
{
    using System.Collections.Generic;
    using System.Linq;
    using PyriteServer.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xna.Framework;

    [TestClass]
    public class OcTreeTest
    {
        private readonly BoundingBox oneBoundingBox = new BoundingBox(Vector3.Zero, Vector3.One);
        private readonly BoundingBox zeroBoundingBox = new BoundingBox(Vector3.Zero, Vector3.Zero);

        [TestMethod]
        public void BoundingBoxIntersectionTest()
        {
            List<CubeBounds> testBounds = new List<CubeBounds>();

            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(Vector3.Zero, 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(1, 1, 1), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(2, 2, 2), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(3, 3, 3), 1) });

            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>(this.zeroBoundingBox, testBounds);
            testOcTree.UpdateTree();
            OcTreeUtilities.Dump(testOcTree);

            Assert.IsTrue(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);
            Assert.AreEqual(Vector3.Zero, testOcTree.Region.Min);
            Assert.AreEqual(new Vector3(4, 4, 4), testOcTree.Region.Max);

            Assert.AreEqual(0, testOcTree.Items.Count);

            // Box Intersects
            BoundingBox box1 = this.MakeCube(new Vector3(-0.5f, -0.5f, -0.5f), 1);
            Assert.AreEqual(1, testOcTree.AllIntersections(box1).Count());

            BoundingBox box2 = this.MakeCube(new Vector3(0.5f, 0.5f, 0.5f), 1);
            Assert.AreEqual(2, testOcTree.AllIntersections(box2).Count());

            BoundingBox box3 = this.MakeCube(new Vector3(1.5f, 1.5f, 1.5f), 1);
            Assert.AreEqual(2, testOcTree.AllIntersections(box3).Count());

            BoundingBox box4 = this.MakeCube(new Vector3(2.5f, 2.5f, 2.5f), 1);
            Assert.AreEqual(2, testOcTree.AllIntersections(box4).Count());

            BoundingBox box5 = this.MakeCube(new Vector3(3.5f, 3.5f, 3.5f), 1);
            Assert.AreEqual(1, testOcTree.AllIntersections(box5).Count());

            // Box Contains & Intersects
            BoundingBox box6 = this.MakeCube(new Vector3(-0.5f, -0.5f, -0.5f), 2);
            Assert.AreEqual(2, testOcTree.AllIntersections(box6).Count());

            BoundingBox box7 = this.MakeCube(new Vector3(0.5f, 0.5f, 0.5f), 2);
            Assert.AreEqual(3, testOcTree.AllIntersections(box7).Count());

            // Box Contains
            BoundingBox box8 = this.MakeCube(new Vector3(3, 3, 3), 2);
            Assert.AreEqual(2, testOcTree.AllIntersections(box8).Count());

            // Box No Intersection
            BoundingBox box9 = this.MakeCube(new Vector3(0, 0, 3), 1);
            Assert.AreEqual(0, testOcTree.AllIntersections(box9).Count());
        }

        [TestMethod]
        public void Initialize()
        {
            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>();
            Assert.IsFalse(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.AreEqual(this.zeroBoundingBox, testOcTree.Region);
            OcTreeUtilities.Dump(testOcTree);
        }

        [TestMethod]
        public void InitializeWithRegion()
        {
            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>(this.oneBoundingBox);
            Assert.IsFalse(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);
            Assert.AreEqual(Vector3.One, testOcTree.Region.Max);
            OcTreeUtilities.Dump(testOcTree);
        }

        [TestMethod]
        public void InitializeWithRegionAndData()
        {
            List<CubeBounds> testBounds = new List<CubeBounds>();

            testBounds.Add(new CubeBounds { BoundingBox = this.oneBoundingBox });
            testBounds.Add(new CubeBounds { BoundingBox = new BoundingBox(new Vector3(3, 3, 3), new Vector3(3, 1, 0)) });

            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>(this.zeroBoundingBox, testBounds);

            Assert.IsFalse(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);
            Assert.AreEqual(Vector3.Zero, testOcTree.Region.Max);

            testOcTree.UpdateTree();

            Assert.IsTrue(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);
            Assert.AreEqual(Vector3.Zero, testOcTree.Region.Min);
            Assert.AreEqual(new Vector3(4, 4, 4), testOcTree.Region.Max);

            OcTreeUtilities.Dump(testOcTree);
        }

        [TestMethod]
        public void IsEmpty()
        {
            List<CubeBounds> testBounds = new List<CubeBounds>();

            testBounds.Add(new CubeBounds { BoundingBox = this.oneBoundingBox });
            testBounds.Add(new CubeBounds { BoundingBox = new BoundingBox(new Vector3(3, 3, 3), new Vector3(3, 1, 0)) });

            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>(this.zeroBoundingBox, testBounds);

            // pending insertions also invalidate IsEmpty
            Assert.IsFalse(testOcTree.IsEmpty);

            Assert.IsFalse(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);
            Assert.AreEqual(Vector3.Zero, testOcTree.Region.Max);

            testOcTree.UpdateTree();

            Assert.IsFalse(testOcTree.IsEmpty);
        }

        [TestMethod]
        public void  ItemsTest()
        {
            List<CubeBounds> testBounds = new List<CubeBounds>();

            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(Vector3.Zero, 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(1, 1, 1), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(2, 2, 2), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(3, 3, 3), 1) });

            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>(this.zeroBoundingBox, testBounds);
            testOcTree.UpdateTree();
            OcTreeUtilities.Dump(testOcTree);

            Assert.IsTrue(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);

            var allItems = testOcTree.AllItems();
            Assert.AreEqual(4, allItems.Count());

            var octants = testOcTree.Octants.Where(o => o != null);
            foreach (var octant in octants)
            {
                var childItems = octant.AllItems();
                Assert.AreEqual(2, childItems.Count());
            }
        }

        [TestMethod]
        public void RayIntersectionTest()
        {
            List<CubeBounds> testBounds = new List<CubeBounds>();

            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(Vector3.Zero, 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(1, 1, 1), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(2, 2, 2), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(3, 3, 3), 1) });

            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>(this.zeroBoundingBox, testBounds);
            testOcTree.UpdateTree();
            OcTreeUtilities.Dump(testOcTree);

            Assert.IsTrue(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);
            Assert.AreEqual(Vector3.Zero, testOcTree.Region.Min);
            Assert.AreEqual(new Vector3(4, 4, 4), testOcTree.Region.Max);

            Assert.AreEqual(0, testOcTree.Items.Count);

            // Ray Intersects
            Ray ray1 = new Ray(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            Assert.AreEqual(4, testOcTree.AllIntersections(ray1).Count());

            Ray ray2 = new Ray(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            Assert.AreEqual(1, testOcTree.AllIntersections(ray2).Count());

            // No Intersects
            Ray ray3 = new Ray(new Vector3(3, 0, 0), new Vector3(1, 1, 1));
            Assert.AreEqual(0, testOcTree.AllIntersections(ray3).Count());
        }

        [TestMethod]
        public void SphereIntersectionTest()
        {
            List<CubeBounds> testBounds = new List<CubeBounds>();

            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(Vector3.Zero, 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(1, 1, 1), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(2, 2, 2), 1) });
            testBounds.Add(new CubeBounds { BoundingBox = this.MakeCube(new Vector3(3, 3, 3), 1) });

            OcTree<CubeBounds> testOcTree = new OcTree<CubeBounds>(this.zeroBoundingBox, testBounds);
            testOcTree.UpdateTree();
            OcTreeUtilities.Dump(testOcTree);

            Assert.IsTrue(testOcTree.HasChildren);
            Assert.IsTrue(testOcTree.IsRoot);
            Assert.IsNotNull(testOcTree.Region);
            Assert.AreEqual(Vector3.Zero, testOcTree.Region.Min);
            Assert.AreEqual(new Vector3(4, 4, 4), testOcTree.Region.Max);

            Assert.AreEqual(0, testOcTree.Items.Count);

            // Sphere Intersects - unlike bounding box, surface intersections are ignored
            BoundingSphere sphere1 = new BoundingSphere(new Vector3(2, 2, 2), 0.5f);
            Assert.AreEqual(2, testOcTree.AllIntersections(sphere1).Count());

            BoundingSphere sphere2 = new BoundingSphere(new Vector3(2, 2, 2), 1f);
            Assert.AreEqual(2, testOcTree.AllIntersections(sphere2).Count());

        }

        private BoundingBox MakeCube(Vector3 min, float size)
        {
            Vector3 max = new Vector3 { X = min.X + size, Y = min.Y + size, Z = min.Z + size };
            return new BoundingBox { Min = min, Max = max };
        }
    }
}