using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elision.Ioc.Tests
{
    [TestClass]
    public class OnDemandResolverTests
    {
        [TestMethod]
        public void ResolvesInterfaceToDefaultType()
        {
            var resolver = new OnDemandResolver();
            var result = resolver.Resolve(typeof (OnDemandResolverTests.ITestClass));
            result.Should().NotBeNull();
            result.Should().BeOfType<OnDemandResolverTests.TestClass>();
        }

        [TestMethod]
        public void DoesNotResolveInterfaceWithMultipleImplementations()
        {
            var resolver = new OnDemandResolver();
            var result = resolver.Resolve(typeof(OnDemandResolverTests.ITestClassMultipleImpl));
            result.Should().BeNull();
        }

        [TestMethod]
        public void CanResolveTypeWithoutConstructor()
        {
            var resolver = new OnDemandResolver();
            var result = resolver.Resolve(typeof(OnDemandResolverTests.ITestClassNoCtor));
            result.Should().NotBeNull();
            result.Should().BeOfType<OnDemandResolverTests.TestClassNoCtor>();
        }

        [TestMethod]
        public void UsesParameterizedConstructorOverDefaultConstructor()
        {
            var resolver = new OnDemandResolver();
            var result = (OnDemandResolverTests.ITestClassMultipleCtor)resolver.Resolve(typeof(OnDemandResolverTests.ITestClassMultipleCtor));
            result.CtorCalled.Should().Be(2);
        }

        [TestMethod]
        public void UsesDefaultConstructorIfNecessary()
        {
            var resolver = new OnDemandResolver();
            var result = (OnDemandResolverTests.ITestClassDefaultCtor)resolver.Resolve(typeof(OnDemandResolverTests.ITestClassDefaultCtor));
            result.CtorCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ResolvesInstanceOfConcreteConstructorParameter()
        {
            var resolver = new OnDemandResolver();
            var result = (OnDemandResolverTests.ITestClassMultipleCtor)resolver.Resolve(typeof(OnDemandResolverTests.ITestClassMultipleCtor));
            result.TestClassInst.Should().NotBeNull();
        }

        #region Test Types

        public interface ITestClass { }
        public class TestClass : ITestClass { }

        public interface ITestClassMultipleImpl { }
        public class TestClassMultipleImpl1 : ITestClassMultipleImpl { }
        public class TestClassMultipleImpl2 : ITestClassMultipleImpl { }

        public interface ITestClassNoCtor { }
        public class TestClassNoCtor : ITestClassNoCtor { }

        public interface ITestClassDefaultCtor {
            bool CtorCalled { get; set; }
        }
        public class TestClassDefaultCtor : ITestClassDefaultCtor
        {
            public bool CtorCalled { get; set; }
            public TestClassDefaultCtor() { CtorCalled = true; }
        }

        public interface ITestClassMultipleCtor {
            ITestClass TestClassInst { get; set; }
            int CtorCalled { get; set; }
        }
        public class TestClassMultipleCtor : ITestClassMultipleCtor
        {
            public ITestClass TestClassInst { get; set; }
            public int CtorCalled { get; set; }
            public TestClassMultipleCtor() { CtorCalled = 1; }
            public TestClassMultipleCtor(ITestClass obj)
            {
                TestClassInst = obj;
                CtorCalled = 2;
            }
        }

        #endregion
    }
}
