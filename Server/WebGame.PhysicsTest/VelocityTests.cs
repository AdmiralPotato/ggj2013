using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace WebGame.PhysicsTest
{
    [TestClass]
    public class VelocityTests
    {
        [TestMethod]
        public void IncreaseVelocity()
        {
            var ship = new Ship(1000);
            ship.ImpulsePercentage = 100;
            Assert.AreEqual(ship.Velocity, Vector3.Zero, "Ship wasn't created at rest");
            ship.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreNotEqual(ship.Velocity, Vector3.Zero, "Ship isn't moving.");
            Assert.AreNotEqual(ship.Position, Vector3.Zero, "Ship hasn't moved.");
        }
    }
}
