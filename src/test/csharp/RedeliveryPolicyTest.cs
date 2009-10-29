﻿using System;
using System.Collections.Generic;
using System.Text;
using Apache.NMS;
using Apache.NMS.Policies;
using NUnit.Framework;

namespace Apache.NMS.Test
{
    [TestFixture]
    public class RedeliveryPolicyTest
    {
        [Test]
        public void Executes_redelivery_policy_with_backoff_enabled_correctly()
        {
            RedeliveryPolicy policy = new RedeliveryPolicy();

            policy.BackOffMultiplier = 2;
            policy.InitialRedeliveryDelay = 5;
            policy.UseExponentialBackOff = true;

            // simulate a retry of 10 times
            Assert.IsTrue(policy.RedeliveryDelay(0) == 5, "redelivery delay not 5 is " + policy.RedeliveryDelay(0));
            Assert.IsTrue(policy.RedeliveryDelay(1) == 10, "redelivery delay not 10 is " + policy.RedeliveryDelay(1));
            Assert.IsTrue(policy.RedeliveryDelay(2) == 20, "redelivery delay not 20 is " + policy.RedeliveryDelay(2));
            Assert.IsTrue(policy.RedeliveryDelay(3) == 40, "redelivery delay not 40 is " + policy.RedeliveryDelay(3));
            Assert.IsTrue(policy.RedeliveryDelay(4) == 80, "redelivery delay not 80 is " + policy.RedeliveryDelay(4));
            Assert.IsTrue(policy.RedeliveryDelay(5) == 160, "redelivery delay not 160 is " + policy.RedeliveryDelay(5));
            Assert.IsTrue(policy.RedeliveryDelay(6) == 320, "redelivery delay not 320 is " + policy.RedeliveryDelay(6));
            Assert.IsTrue(policy.RedeliveryDelay(7) == 640, "redelivery delay not 640 is " + policy.RedeliveryDelay(7));
            Assert.IsTrue(policy.RedeliveryDelay(8) == 1280, "redelivery delay not 1280 is " + policy.RedeliveryDelay(8));
            Assert.IsTrue(policy.RedeliveryDelay(9) == 2560, "redelivery delay not 2560 is " + policy.RedeliveryDelay(9));
        }

        [Test]
        public void Executes_redelivery_policy_with_backoff_of_3_enabled_correctly()
        {
            RedeliveryPolicy policy = new RedeliveryPolicy();

            policy.BackOffMultiplier = 3;
            policy.InitialRedeliveryDelay = 3;
            policy.UseExponentialBackOff = true;

            // simulate a retry of 10 times
            Assert.IsTrue(policy.RedeliveryDelay(0) == 3, "redelivery delay not 5 is " + policy.RedeliveryDelay(0));
            Assert.IsTrue(policy.RedeliveryDelay(1) == 9, "redelivery delay not 10 is " + policy.RedeliveryDelay(1));
            Assert.IsTrue(policy.RedeliveryDelay(2) == 27, "redelivery delay not 20 is " + policy.RedeliveryDelay(2));
            Assert.IsTrue(policy.RedeliveryDelay(3) == 81, "redelivery delay not 40 is " + policy.RedeliveryDelay(3));
            Assert.IsTrue(policy.RedeliveryDelay(4) == 243, "redelivery delay not 80 is " + policy.RedeliveryDelay(4));
            Assert.IsTrue(policy.RedeliveryDelay(5) == 729, "redelivery delay not 160 is " + policy.RedeliveryDelay(5));
            Assert.IsTrue(policy.RedeliveryDelay(6) == 2187, "redelivery delay not 320 is " + policy.RedeliveryDelay(6));
            Assert.IsTrue(policy.RedeliveryDelay(7) == 6561, "redelivery delay not 640 is " + policy.RedeliveryDelay(7));
            Assert.IsTrue(policy.RedeliveryDelay(8) == 19683, "redelivery delay not 1280 is " + policy.RedeliveryDelay(8));
            Assert.IsTrue(policy.RedeliveryDelay(9) == 59049, "redelivery delay not 2560 is " + policy.RedeliveryDelay(9));
        }

        [Test]
        public void Executes_redelivery_policy_without_backoff_enabled_correctly()
        {
            RedeliveryPolicy policy = new RedeliveryPolicy();

            policy.InitialRedeliveryDelay = 5;

            // simulate a retry of 10 times
            Assert.IsTrue(policy.RedeliveryDelay(0) == 5, "redelivery delay not 5 is " + policy.RedeliveryDelay(0));
            Assert.IsTrue(policy.RedeliveryDelay(1) == 5, "redelivery delay not 10 is " + policy.RedeliveryDelay(1));
            Assert.IsTrue(policy.RedeliveryDelay(2) == 5, "redelivery delay not 20 is " + policy.RedeliveryDelay(2));
            Assert.IsTrue(policy.RedeliveryDelay(3) == 5, "redelivery delay not 40 is " + policy.RedeliveryDelay(3));
            Assert.IsTrue(policy.RedeliveryDelay(4) == 5, "redelivery delay not 80 is " + policy.RedeliveryDelay(4));
            Assert.IsTrue(policy.RedeliveryDelay(5) == 5, "redelivery delay not 160 is " + policy.RedeliveryDelay(5));
            Assert.IsTrue(policy.RedeliveryDelay(6) == 5, "redelivery delay not 320 is " + policy.RedeliveryDelay(6));
            Assert.IsTrue(policy.RedeliveryDelay(7) == 5, "redelivery delay not 640 is " + policy.RedeliveryDelay(7));
            Assert.IsTrue(policy.RedeliveryDelay(8) == 5, "redelivery delay not 1280 is " + policy.RedeliveryDelay(8));
            Assert.IsTrue(policy.RedeliveryDelay(9) == 5, "redelivery delay not 2560 is " + policy.RedeliveryDelay(9));
        }

        [Test]
        public void Should_get_collision_percent_correctly()
        {
            RedeliveryPolicy policy = new RedeliveryPolicy();

            policy.CollisionAvoidancePercent = 45;

            Assert.IsTrue(policy.CollisionAvoidancePercent == 45);
        }

        [Test]
        public void Executes_redelivery_policy_with_collision_enabled_correctly()
        {
            RedeliveryPolicy policy = new RedeliveryPolicy();

            policy.BackOffMultiplier = 2;
            policy.InitialRedeliveryDelay = 5;
            policy.UseExponentialBackOff = true;
            policy.UseCollisionAvoidance = true;
            policy.CollisionAvoidancePercent = 10;

            // simulate a retry of 10 times
            int delay = policy.RedeliveryDelay(0);
            Assert.IsTrue(delay >= 4.5 && delay <= 5.5, "not delay >= 4.5 && delay <= 5.5 is " + policy.RedeliveryDelay(0));
            delay = policy.RedeliveryDelay(1);
            Assert.IsTrue(delay >= 9 && delay <= 11, "not delay >= 9 && delay <= 11 is " + policy.RedeliveryDelay(1));
            delay = policy.RedeliveryDelay(2);
            Assert.IsTrue(delay >= 18 && delay <= 22, "not delay >= 18 && delay <= 22 is " + policy.RedeliveryDelay(2));
            delay = policy.RedeliveryDelay(3);
            Assert.IsTrue(delay >= 36 && delay <= 44, "not delay >= 36 && delay <= 44 is " + policy.RedeliveryDelay(3));
            delay = policy.RedeliveryDelay(4);
            Assert.IsTrue(delay >= 72 && delay <= 88, "not delay >= 72 && delay <= 88 is " + policy.RedeliveryDelay(4));
            delay = policy.RedeliveryDelay(5);
            Assert.IsTrue(delay >= 144 && delay <= 176, "not delay >= 144 && delay <= 176 is " + policy.RedeliveryDelay(5));
            delay = policy.RedeliveryDelay(6);
            Assert.IsTrue(delay >= 288 && delay <= 352, "not delay >= 288 && delay <= 352 is " + policy.RedeliveryDelay(6));
            delay = policy.RedeliveryDelay(7);
            Assert.IsTrue(delay >= 576 && delay <= 704, "not delay >= 576 && delay <= 704 is " + policy.RedeliveryDelay(7));
            delay = policy.RedeliveryDelay(8);
            Assert.IsTrue(delay >= 1152 && delay <= 1408, "not delay >= 1152 && delay <= 1408 is " + policy.RedeliveryDelay(8));
            delay = policy.RedeliveryDelay(9);
            Assert.IsTrue(delay >= 2304 && delay <= 2816, "not delay >= 2304 && delay <= 2816 is " + policy.RedeliveryDelay(9));
        }
    }
}