/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using NUnit.Framework;

namespace Apache.NMS.Test
{
	[TestFixture]
	public abstract class DurableTest : NMSTestSupport
	{
		private static string TOPIC = "TestTopicDurableConsumer";
		private static String CLIENT_ID = "DurableClientId";
		private static String CONSUMER_ID = "ConsumerId";

		private int count = 0;

		protected void RegisterDurableConsumer()
		{
			using(IConnection connection = Factory.CreateConnection())
			{
				connection.ClientId = CLIENT_ID;
				connection.Start();

				using(ISession session = connection.CreateSession(AcknowledgementMode.DupsOkAcknowledge))
				{
					ITopic topic = session.GetTopic(TOPIC);
					using(IMessageConsumer consumer = session.CreateDurableConsumer(topic, CONSUMER_ID, "2 > 1", false))
					{
					}
				}

				connection.Stop();
			}
		}

		protected void SendPersistentMessage()
		{
			using(IConnection connection = Factory.CreateConnection())
			{
				connection.Start();
				using (ISession session = connection.CreateSession(AcknowledgementMode.DupsOkAcknowledge))
				{
					ITopic topic = session.GetTopic(TOPIC);
					ITextMessage message = session.CreateTextMessage("Persistent Hello");
					message.NMSPersistent = true;
					using(IMessageProducer producer = session.CreateProducer())
					{
						producer.Send(topic, message);
					}
				}

				connection.Stop();
			}
		}

		[Test]
		public void TestDurableConsumer()
		{
			count = 0;

			RegisterDurableConsumer();
			SendPersistentMessage();

			using (IConnection connection = Factory.CreateConnection())
			{
				connection.ClientId = CLIENT_ID;
				connection.Start();

				using (ISession session = connection.CreateSession(AcknowledgementMode.DupsOkAcknowledge))
				{
					ITopic topic = session.GetTopic(TOPIC);
					using(IMessageConsumer consumer = session.CreateDurableConsumer(topic, CONSUMER_ID, "2 > 1", false))
					{
						consumer.Listener += new MessageListener(consumer_Listener);
						// Don't know how else to give the system enough time.
						System.Threading.Thread.Sleep(5000);
						Assert.AreEqual(1, count);
						Console.WriteLine("Count = " + count);
						SendPersistentMessage();
						System.Threading.Thread.Sleep(5000);
						Assert.AreEqual(2, count);
						Console.WriteLine("Count = " + count);
					}
				}

				connection.Stop();
			}
		}

		[Test]
		public void TestDurableConsumerTransactional()
		{
			RegisterDurableConsumer();

			RunTestDurableConsumerTransactional();
			// Timeout required before closing/disposing the connection otherwise orphan
			// connection remains and test will fail when run the second time with a
			// InvalidClientIDException: DurableClientID already connected.
			//System.Threading.Thread.Sleep(5000); 
			RunTestDurableConsumerTransactional();
		}

		protected void RunTestDurableConsumerTransactional()
		{
			count = 0;
			SendPersistentMessage();

			using (IConnection connection = Factory.CreateConnection())
			{
				connection.ClientId = CLIENT_ID;
				connection.Start();

				using (ISession session = connection.CreateSession(AcknowledgementMode.Transactional))
				{
					ITopic topic = session.GetTopic(TOPIC);
					using(IMessageConsumer consumer = session.CreateDurableConsumer(topic, CONSUMER_ID, "2 > 1", false))
					{
						consumer.Listener += new MessageListener(consumer_Listener);
						// Don't know how else to give the system enough time. 

						System.Threading.Thread.Sleep(5000);
						Assert.AreEqual(1, count);
						Console.WriteLine("Count = " + count);
						SendPersistentMessage();
						System.Threading.Thread.Sleep(5000);
						Assert.AreEqual(2, count);
						Console.WriteLine("Count = " + count);

						session.Commit();
					}
				}

				connection.Stop();
			}
		}

		/// <summary>
		/// Asynchronous listener call back method.
		/// </summary>
		/// <param name="message"></param>
		private void consumer_Listener(IMessage message)
		{
			++count;
		}
	}
}