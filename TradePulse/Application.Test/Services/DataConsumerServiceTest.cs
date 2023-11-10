using Application.Services.DataConsumerService;

namespace Application.Test.Services
{
    public class DataConsumerServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckInsertFeatureByCleaningOldPricesTest()
        {
            PriceBinarySearchTree priceBinarySearchTree = new(TimeSpan.FromSeconds(5));

            priceBinarySearchTree.Insert(900, (long)TimeSpan.FromSeconds(3).TotalMilliseconds);
            priceBinarySearchTree.Insert(34, (long)TimeSpan.FromSeconds(5).TotalMilliseconds);

            Assert.Multiple(() =>
            {
                Assert.That(priceBinarySearchTree.MaxPrice, Is.EqualTo(900));
                Assert.That(priceBinarySearchTree.MinPrice, Is.EqualTo(34));
            });

            priceBinarySearchTree.Insert(89, (long)TimeSpan.FromSeconds(9).TotalMilliseconds);

            Assert.Multiple(() =>
            {
                Assert.That(priceBinarySearchTree.MaxPrice, Is.EqualTo(89));
                Assert.That(priceBinarySearchTree.MinPrice, Is.EqualTo(34));
            });

            priceBinarySearchTree.Insert(3, (long)TimeSpan.FromSeconds(11).TotalMilliseconds);

            Assert.Multiple(() =>
            {
                Assert.That(priceBinarySearchTree.MaxPrice, Is.EqualTo(89));
                Assert.That(priceBinarySearchTree.MinPrice, Is.EqualTo(3));
            });
        }

        [Test]
        public void CheckSearchFeatureTest()
        {
            PriceBinarySearchTree priceBinarySearchTree = new(TimeSpan.FromSeconds(5));

            priceBinarySearchTree.Insert(11, 1);
            priceBinarySearchTree.Insert(5, 2);
            priceBinarySearchTree.Insert(13, 3);
            priceBinarySearchTree.Insert(17, 4);

            PriceNode? actual = priceBinarySearchTree.SearchPrice(13);

            Assert.That(actual!.Value, Is.EqualTo(13));
        }
    }
}