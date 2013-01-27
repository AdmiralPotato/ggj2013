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
            var attacker = new Ship();
            system.AddEntity(attacker);
            attacker.LoadProjectile();
            enemy.ImpulsePercentage = 100;
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreNotEqual(0, enemy.Position.X, "The enemy should have moved along the x axis");
            Assert.AreEqual(0, attacker.Position.X, "the attacker was not at the center");
            var missile = attacker.LaunchProjectile(enemy);
            Assert.AreEqual(0, missile.Position.X, "the missile was not at the center");
            var oldDiff = enemy.Position.X - missile.Position.X;
            missile.Update(TimeSpan.FromSeconds(0.25));
            var newDiff = enemy.Position.X - missile.Position.X;
            Assert.IsTrue(newDiff < oldDiff, "The missile didn't get closer to the ship");
        }

        [TestMethod]
        public void MissileCollision()
        {
            var game = new Game();
            var system = new StarSystem();
            game.Add(system);
            var enemy = new Ship();
            system.AddEntity(enemy);
            var attacker = new Ship();
            system.AddEntity(attacker);
            attacker.LoadProjectile();
            enemy.ImpulsePercentage = 100;
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreNotEqual(0, enemy.Position.X, "The enemy should have moved along the x axis");
            Assert.AreEqual(0, attacker.Position.X, "the attacker was not at the center");
            var missile = attacker.LaunchProjectile(enemy);
            Assert.AreEqual(0, missile.Position.X, "the missile was not at the center");
            var oldDiff = enemy.Position.X - missile.Position.X;
            missile.Update(TimeSpan.FromSeconds(0.25));
            var newDiff = enemy.Position.X - missile.Position.X;
            Assert.IsTrue(newDiff < oldDiff, "The missile didn't get closer to the ship");
            Assert.IsFalse(missile.IsDestroyed, "The missile is somehow already detonated.");
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.IsTrue(missile.IsDestroyed, "The missile should have hit the ship by now.");
        }

        [TestMethod]
        public void LoadProjectileLaunchWithoutLoading()
        {
            var game = new Game();
            var system = new StarSystem();
            system.RandomlySpawnEnemies = false;
            game.Add(system);
            var ship = new Ship();
            system.AddEntity(ship);
            var projectile = ship.LaunchProjectile(ship);
            Assert.IsNull(projectile, "The projectile shouldn't have been launched if it hadn't been loaded.");
        }

        [TestMethod]
        public void LoadProjectileLaunchWithoutWaitingAfterLoading()
        {
            var game = new Game();
            var system = new StarSystem();
            system.RandomlySpawnEnemies = false;
            game.Add(system);
            var ship = new Ship();
            system.AddEntity(ship);
            ship.LoadProjectile();
            var projectile = ship.LaunchProjectile(ship);
            Assert.IsNull(projectile, "The projectile shouldn't have been launched if there hasn't been enough time to load it.");
        }

        [TestMethod]
        public void LoadProjectileDoubleLoad()
        {
            var game = new Game();
            var system = new StarSystem();
            system.RandomlySpawnEnemies = false;
            game.Add(system);
            var ship = new Ship();
            system.AddEntity(ship);
            var loaded = ship.LoadProjectile();
            Assert.IsTrue(loaded, "Couldn't load the first projectile");
            loaded = ship.LoadProjectile();
            Assert.IsFalse(loaded, "Somehow Loaded a second projectile.");
        }

        [TestMethod]
        public void DestroyShip()
        {
            var game = new Game();
            var system = new StarSystem();
            system.RandomlySpawnEnemies = false;
            game.Add(system);
            var ship = new Ship();
            system.AddEntity(ship);
            ship.ImpulsePercentage = 100;
            Assert.IsTrue(ship.Energy > 0, "Ship didn't have any energy");
            var oldVelocity = ship.Velocity;
            Assert.AreEqual(0, ship.Velocity.Magnitude(), "Ship should be at rest at first");

            for (int i = 0; i < 20; i++)
            {
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                Assert.IsTrue(oldVelocity.Magnitude() < ship.Velocity.Magnitude(), "The ship didn't increase in speed");
                oldVelocity = ship.Velocity;
            }
            var missile = new Projectile();
            system.AddEntity(missile);
            missile.Target = ship;
            oldVelocity = missile.Velocity;
            Assert.AreEqual(0, missile.Velocity.Magnitude(), "Missile should be at rest at first.");
            for (int i = 0; i < 9; i++)
            {
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                Assert.IsFalse(missile.IsDestroyed, "the missile got destroyed.");
                Assert.IsTrue(oldVelocity.Magnitude() < missile.Velocity.Magnitude(), "The missile didn't increase in speed");
                oldVelocity = missile.Velocity;
            }
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.IsTrue(missile.IsDestroyed, "the missile didn't get destroyed.");
            oldVelocity = missile.Velocity;
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreEqual(oldVelocity.Magnitude(), missile.Velocity.Magnitude(), "The (dead) missile didn't increase in speed");
        }

        [TestMethod]
        public void RunOutOfEnergy()
        {
            var game = new Game();
            var system = new StarSystem();
            system.RandomlySpawnEnemies = false;
            game.Add(system);
            var ship = new Ship();
            system.AddEntity(ship);
            ship.ImpulsePercentage = 100;
            Assert.IsTrue(ship.Energy > 0, "Ship didn't have any energy");
            var oldVelocity = ship.Velocity;
            Assert.AreEqual(0, ship.Velocity.Magnitude(), "Ship should be at rest at first");

            for (int i = 0; i < 200; i++)
            {
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                Assert.IsTrue(oldVelocity.Magnitude() < ship.Velocity.Magnitude(), "The ship didn't increase in speed");
                oldVelocity = ship.Velocity;
            }
            Assert.AreEqual(0, ship.Energy, "The ship didn't run out of energy");
            game.Update(TimeSpan.FromSeconds(1));
            Assert.AreEqual(oldVelocity, ship.Velocity, "The ship should not have been able to increase it's velocity without energy.");
            game.Update(TimeSpan.FromSeconds(100000)); // get that ship very far away, so the missile won't detonate
            var missile = new Projectile();
            system.AddEntity(missile);
            missile.Target = ship;
            oldVelocity = missile.Velocity;
            Assert.AreEqual(0, missile.Velocity.Magnitude(), "Missile should be at rest at first.");
            for (int i = 0; i < 20; i++)
            {
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                game.Update(TimeSpan.FromSeconds(0.25));
                Assert.IsFalse(missile.IsDestroyed, "the missile got destroyed.");
                Assert.IsTrue(oldVelocity.Magnitude() < missile.Velocity.Magnitude(), "The missile didn't increase in speed");
                oldVelocity = missile.Velocity;
            }
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.IsFalse(missile.IsDestroyed, "the missile didn't get destroyed.");
            oldVelocity = missile.Velocity;
            game.Update(TimeSpan.FromSeconds(0.25));
            Assert.AreEqual(oldVelocity.Magnitude(), missile.Velocity.Magnitude(), "The missile didn't run out of energy after over 20 seconds");

        }

    }
}

