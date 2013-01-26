using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebGame.PhysicsTest
{
    [TestClass]
    public class RotationTests
    {
        [TestMethod]
        public void RotationBigUpdate()
        {
            var ship = new Ship();
            var initialOrientation = ship.Orientation;
            var desiredOrientation = initialOrientation + Math.PI / 4;
            ship.DesiredOrientation = desiredOrientation;
            ship.Update(TimeSpan.MaxValue);
            Assert.AreEqual(desiredOrientation, ship.Orientation, "Orientation didn't match");
        }

        [TestMethod]
        public void RotationBigManySmallUpdates()
        {
            var ship = new Ship();
            var initialOrientation = ship.Orientation;
            var desiredOrientation = initialOrientation + Math.PI / 4;
            ship.DesiredOrientation = desiredOrientation;
            for (int i = 0; i < 10000; i++)
            {
                ship.Update(TimeSpan.FromMilliseconds(1));
            }
            Assert.AreEqual(desiredOrientation, ship.Orientation, "Orientation didn't match");
        }

        [TestMethod]
        public void RotateNotWholeAmount()
        {
            var ship = new Ship();
            var initialOrientation = ship.Orientation;
            var desiredOrientation = initialOrientation + Math.PI / 4;
            ship.DesiredOrientation = desiredOrientation;
            var oldDiffMag = Math.Abs(ship.Orientation - desiredOrientation);
            ship.Update(TimeSpan.FromMilliseconds(1));
            var newDiffMag = Math.Abs(ship.Orientation - desiredOrientation);
            Assert.AreNotEqual(initialOrientation, ship.Orientation, "The orientation hasn't changed.");
            Assert.IsTrue(newDiffMag < oldDiffMag, "We didn't get closer.");
        }

        [TestMethod]
        public void ShouldNotRotateBackwards()
        {
            var ship = new Ship();
            ship.DesiredOrientation = 1;
            for (int i = 0; i < 1000; i++)
            {
                ship.Update(TimeSpan.FromSeconds(0.25));
                Assert.IsFalse(ship.Orientation < 0);
            }
        }
    }
}
