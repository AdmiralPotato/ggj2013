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

        [TestMethod]
        public void RunOutOfEnergy()
        {
            var ship = new Ship();
            ship.ImpulsePercentage = 100;
            Assert.IsTrue(ship.Energy > 0, "Ship didn't have any energy");
            var oldVelocity = ship.Velocity;
            Assert.AreEqual(0, ship.Velocity.Magnitude(), "Ship should be at rest at first");

            for (int i = 0; i < 200; i++)
            {
                ship.Update(TimeSpan.FromSeconds(0.25));
                ship.Update(TimeSpan.FromSeconds(0.25));
                ship.Update(TimeSpan.FromSeconds(0.25));
                ship.Update(TimeSpan.FromSeconds(0.25));
                Assert.IsTrue(oldVelocity.Magnitude() < ship.Velocity.Magnitude(), "The ship didn't increase in speed");
                oldVelocity = ship.Velocity;
            }
            Assert.AreEqual(0, ship.Energy, "The ship didn't run out of energy");
            ship.Update(TimeSpan.FromSeconds(1));
            Assert.AreEqual(oldVelocity, ship.Velocity, "The ship should not have been able to increase it's velocity without energy.");
        }
    }
}
