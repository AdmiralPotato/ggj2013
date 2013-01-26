using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebGame.PhysicsTest
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void NormalizeOrientationUnchanged()
        {
            var angle = Math.PI / 2;
            var normalized = angle.NormalizeOrientation();
            Assert.AreEqual(angle, normalized);
        }
        [TestMethod]
        public void NormalizeTurnUnchanged()
        {
            var angle = Math.PI / 2;
            var normalized = angle.NormalizeTurn();
            Assert.AreEqual(angle, normalized);
        }
        [TestMethod]
        public void NormalizeTurnTooBig()
        {
            var expectedAngle = Math.PI / 2;
            var inputAngle = expectedAngle + Math.PI + Math.PI;
            var normalized = inputAngle.NormalizeTurn();
            Assert.AreEqual(expectedAngle, normalized);
        }
        [TestMethod]
        public void NormalizeTurnTooSmall()
        {
            var expectedAngle = Math.PI / 2;
            var inputAngle = expectedAngle - Math.PI - Math.PI;
            var normalized = inputAngle.NormalizeTurn();
            Assert.AreEqual(expectedAngle, normalized);
        }
    }
}
