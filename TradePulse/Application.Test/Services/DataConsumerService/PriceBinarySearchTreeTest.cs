using Application.Services.DataConsumerService;

namespace Application.Test.Services.DataConsumerService
{
    public class PriceBinarySearchTreeTest
    {
        [Test]
        public void CheckInsertFeatureByCleaningOldPrices_Positive_Test()
        {
            PriceBinarySearchTree priceBinarySearchTree = new(TimeSpan.FromSeconds(5));

            priceBinarySearchTree.Insert(900, (long)TimeSpan.FromSeconds(3).TotalMilliseconds);
            priceBinarySearchTree.Insert(34, (long)TimeSpan.FromSeconds(5).TotalMilliseconds);

            Assert.Multiple(() =>
            {
                Assert.That(priceBinarySearchTree.MaxPrice.Value, Is.EqualTo(900));
                Assert.That(priceBinarySearchTree.MinPrice.Value, Is.EqualTo(34));
            });

            priceBinarySearchTree.Insert(89, (long)TimeSpan.FromSeconds(9).TotalMilliseconds);

            Assert.Multiple(() =>
            {
                Assert.That(priceBinarySearchTree.MaxPrice.Value, Is.EqualTo(89));
                Assert.That(priceBinarySearchTree.MinPrice.Value, Is.EqualTo(34));
            });

            priceBinarySearchTree.Insert(3, (long)TimeSpan.FromSeconds(11).TotalMilliseconds);

            Assert.Multiple(() =>
            {
                Assert.That(priceBinarySearchTree.MaxPrice.Value, Is.EqualTo(89));
                Assert.That(priceBinarySearchTree.MinPrice.Value, Is.EqualTo(3));
            });
        }

        [Test]
        public void CheckSearchFeature_Positive_Test()
        {
            PriceBinarySearchTree priceBinarySearchTree = new(TimeSpan.FromSeconds(5));

            priceBinarySearchTree.Insert(11, 1);
            priceBinarySearchTree.Insert(5, 2);
            priceBinarySearchTree.Insert(13, 3);
            priceBinarySearchTree.Insert(17, 4);

            PriceNode? actual = priceBinarySearchTree.SearchPrice(13);

            Assert.That(actual!.Value, Is.EqualTo(13));
        }

        [Test]
        public void CheckClearUntilFeature_Positive_Test()
        {
            PriceBinarySearchTree priceBinarySearchTree = new(TimeSpan.FromSeconds(5));

            priceBinarySearchTree.Insert(11, 1);
            priceBinarySearchTree.Insert(5, 2);
            priceBinarySearchTree.Insert(13, 3);
            priceBinarySearchTree.Insert(17, 4);

            priceBinarySearchTree.ClearUntil(3);
            Assert.Multiple(() =>
            {
                Assert.That(priceBinarySearchTree.SearchPrice(11), Is.EqualTo(null));
                Assert.That(priceBinarySearchTree.SearchPrice(5), Is.EqualTo(null));
                Assert.That(priceBinarySearchTree.SearchPrice(13), Is.EqualTo(null));
                Assert.That(priceBinarySearchTree.SearchPrice(17)?.Value, Is.EqualTo(17));
            });
        }

        [Test]
        public void InsertWrongTimeframeValue_Negative_Test()
        {
            PriceBinarySearchTree priceBinarySearchTree = new(TimeSpan.FromSeconds(5));

            priceBinarySearchTree.Insert(11, 1);
            priceBinarySearchTree.Insert(5, 2);
            try
            {
                priceBinarySearchTree.Insert(13, 2);

                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }
    }
}