using A6k.Messaging.Features;
using Moq;
using Xunit;

namespace A6k.Messaging.Tests
{
    public class FeaturesCollectionTests
    {
        [Fact]
        public void SimpleFeature()
        {
            var fc = new FeaturesCollection();
            var f = new FeatureA();
            fc.Set(f);

            Assert.NotNull(fc.FeatureA);
            Assert.True(ReferenceEquals(f, fc.FeatureA));
        }

        [Fact]
        public void CompositeFeature()
        {
            //arrange
            var fc = new FeaturesCollection();
            var fmock = new Mock<IFeatureB>();
            fmock.Setup(x => x.Blah(It.IsAny<string>()));

            //act
            fc.Set(fmock.Object);
            fc.FeatureB.Blah("fred");

            //assert
            Assert.NotNull(fc.FeatureB);
            Assert.IsAssignableFrom<CompositeFeature<IFeatureB>>(fc.FeatureB);
            var cf = fc.FeatureB as CompositeFeature<IFeatureB>;
            Assert.Equal(1, cf.Count);

            fmock.Verify(x => x.Blah("fred"));
        }

        public class FeaturesCollection : FeatureSet
        {
            public IFeatureA FeatureA { get; private set; }
            public IFeatureB FeatureB { get; private set; } = new CompositeFeatureB();
        }

        public interface IFeatureA
        {
            void Blah(string text);
        }
        public interface IFeatureB
        {
            void Blah(string text);
        }

        public class FeatureA : IFeatureA
        {
            public void Blah(string text) { }
        }
        public class CompositeFeatureB : CompositeFeature<IFeatureB>, IFeatureB
        {
            public void Blah(string text) => Invoke(x => x.Blah(text));
        }
    }
}
