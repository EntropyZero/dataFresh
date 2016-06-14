using System;
using NUnit.Framework;

namespace TestHarnessExample
{
	[TestFixture]
	public class MoreSampleTests : TestFixtureBase
	{
		[Test]
		public void SampleDatabaseWriteTest1()
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
		public void SampleDatabaseReadTest4()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest5()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest6()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest7()
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
		public void SampleDatabaseReadTest8()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest9()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest10()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest11()
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
		public void SampleDatabaseReadTest12()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest13()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest14()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest15()
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
		public void SampleDatabaseReadTest16()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest17()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest18()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest19()
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
		public void SampleDatabaseReadTest20()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest21()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest22()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest23()
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
		public void SampleDatabaseReadTest24()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest25()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest26()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest27()
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
		public void SampleDatabaseReadTest28()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest29()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest30()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest31()
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
		public void SampleDatabaseReadTest32()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}

		[Test]
		public void SampleDatabaseWriteTest33()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("My First Book", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
			ExecuteNonQuery("UPDATE Book SET Title='New Title' WHERE BookId=1");
			Assert.AreEqual("New Title", (string) ExecuteScalar("SELECT Title FROM Book WHERE BookId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest34()
		{
			Console.Out.WriteLine("Test");
			Assert.AreEqual("Brockey", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
			ExecuteNonQuery("UPDATE Author SET LastName='TEST'");
			Assert.AreEqual("TEST", (string) ExecuteScalar("SELECT LastName FROM Author WHERE AuthorId=1"));
		}

		[Test]
		public void SampleDatabaseWriteTest35()
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
		public void SampleDatabaseReadTest36()
		{
			Console.Out.WriteLine("Test");
			ExecuteNonQuery("SELECT * FROM Book WHERE BookId=1");
		}
	}
}