using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebGame.PhysicsTest
{
    [TestClass]
    public class MissileTests
    {
        [TestMethod]
        public void MissileTowardTarget()
        {
            var ship = new Ship();
            ship.ImpulsePercentage = 100;
            ship.Update(TimeSpan.FromSeconds(0.25));
            ship.Update(TimeSpan.FromSeconds(0.25));
            ship.Update(TimeSpan.FromSeconds(0.25));
            ship.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreNotEqual(0, ship.Position.X, "The ship should have moved along the x axis");
            var missile = new Projectile();
            Assert.AreEqual(0, missile.Position.X, "the missile was not at the center");
            missile.Target = ship;
            var oldDiff = ship.Position.X - missile.Position.X;
            missile.Update(TimeSpan.FromSeconds(0.1));
            var newDiff = ship.Position.X - missile.Position.X;
            Assert.IsTrue(newDiff < oldDiff, "The missile didn't get closer to the ship");
        }

        [TestMethod]
        public void LaunchMissileTowardTarget()
        {
            var game = new Game();
            var system = new StarSystem();
            game.Add(system);
            var enemy = new Ship();
            system.AddEntity(enemy);
            enemy.ImpulsePercentage = 100;
            enemy.Update(TimeSpan.FromSeconds(0.25));
            enemy.Update(TimeSpan.FromSeconds(0.25));
            enemy.Update(TimeSpan.FromSeconds(0.25));
            enemy.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreNotEqual(0, enemy.Position.X, "The enemy should have moved along the x axis");
            var attacker = new Ship();
            system.AddEntity(attacker);
            Assert.AreEqual(0, attacker.Position.X, "the attacker was not at the center");
            var missile = attacker.LaunchProjectile(enemy);
            Assert.AreEqual(0, missile.Position.X, "the missile was not at the center");
            var oldDiff = enemy.Position.X - missile.Position.X;
            missile.Update(TimeSpan.FromSeconds(0.25));
            var newDiff = enemy.Position.X - missile.Position.X;
            Assert.IsTrue(newDiff < oldDiff, "The missile didn't get closer to the ship");
        }

        public void MissileCollision()
        {
            var game = new Game();
            var system = new StarSystem();
            game.Add(system);
            var enemy = new Ship();
            system.AddEntity(enemy);
            enemy.ImpulsePercentage = 100;
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreNotEqual(0, enemy.Position.X, "The enemy should have moved along the x axis");
            var attacker = new Ship();
            system.AddEntity(attacker);
            Assert.AreEqual(0, attacker.Position.X, "the attacker was not at the center");
            var missile = attacker.LaunchProjectile(enemy);
            Assert.AreEqual(0, missile.Position.X, "the missile was not at the center");
            var oldDiff = enemy.Position.X - missile.Position.Y;
            game.Update(TimeSpan.FromSeconds(0.25));
            var newDiff = enemy.Position.X - missile.Position.Y;
            Assert.IsTrue(newDiff < oldDiff, "The missile didn't get closer to the ship");
            Assert.IsFalse(missile.IsDestroyed, "The missile is somehow already detonated.");
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.IsTrue(missile.IsDestroyed, "The missile should have hit the ship by now.");
        }
    }
}
