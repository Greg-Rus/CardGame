using System;
using Zenject;
using NUnit.Framework;

[TestFixture]
public class ZenjectTest : ZenjectUnitTestFixture
{
    [SetUp]
    public void CommonInstall()
    {
        Container.Bind<Logger>().AsSingle();


    }

    [Test]
    public void TestInitialValues()
    {
        var logger = Container.Resolve<Logger>();

        Assert.That(logger.Log == "");
    }

    [Test]
    public void TestFirstEntry()
    {
        var logger = Container.Resolve<Logger>();

        logger.Write("foo");
        Assert.That(logger.Log == "foo");
    }

    [Test]
    public void TestAppend()
    {
        var logger = Container.Resolve<Logger>();

        logger.Write("foo");
        logger.Write("bar");

        Assert.That(logger.Log == "foobar");
    }
}

public class Logger
{
    public Logger()
    {
        Log = "";
    }

    public string Log
    {
        get;
        private set;
    }

    public void Write(string value)
    {
        if (value == null)
        {
            throw new ArgumentException();
        }

        Log += value;
    }
}