using System;
using NUnit.Framework;

namespace TestHarnessExample
{
	[TestFixture]
	public class SampleTests : TestFixtureBase
	{
		[Test]
		public void SampleDatabaseWriteTest()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest2()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest3()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseReadTest()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}
	}
}