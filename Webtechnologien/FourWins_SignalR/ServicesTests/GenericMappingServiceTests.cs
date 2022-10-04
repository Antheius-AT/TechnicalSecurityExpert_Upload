//-----------------------------------------------------------------------
// <copyright file="GenericMappingServiceTests.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace ServicesTests
{
    using NUnit.Framework;
    using SharedData.SharedHubData.ConcreteTypes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GenericMappingServiceTests
    {
        private GenericMappingService<int, string> service;

        [SetUp]
        public void Setup()
        {
            this.service = new GenericMappingService<int, string>();
        }

        [Test]
        [TestCase(20, 1, 5, 3, 10, 25)]
        [TestCase(1, 100, 50, 20, 22, 35)]
        [TestCase(0, 1, 5, 2, 3, 4, 10)]
        public void Is_Entry_ProperlyStored_Using_StoreEntryAsync(params int[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                this.service.StoreEntryAsync(keys[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var resultValues = this.service.GetAllValuesAsync().GetAwaiter().GetResult();

            for (int i = 0; i < resultValues.Count(); i++)
            {
                var currentItem = resultValues.ElementAt(i);
                var currentKey = this.service.GetAssociatedKeyAsync(currentItem).GetAwaiter().GetResult();
                this.service.TryGetValueAsync(currentKey, out string expectedItem);

                Assert.That(this.service.DoesEntryExistAsync(currentKey).GetAwaiter().GetResult() && currentItem == expectedItem);
            }
        }

        [Test]
        [TestCase(5, 0, 1, 2, 10, 100, 50, 22, 37, 5)]
        [TestCase(11, 10, 2, 3, 5, 11)]
        [TestCase(22, 33, 22)]
        public void Does_GetValueAsync_Return_Stored_Value(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var item = this.service.GetValueAsync(requestedKey).GetAwaiter().GetResult();

            var itemNumber = Array.IndexOf(stored, requestedKey) + 1;
            var expectedItem = $"Item number {itemNumber}";

            Assert.True(expectedItem == item);
        }

        [Test]
        [TestCase(5, 0, 1, 2, 10, 100, 50, 22, 37, 5)]
        [TestCase(11, 10, 2, 3, 5, 11)]
        [TestCase(22, 33, 22)]
        public void Does_TryGetValueAsync_Return_Stored_Value(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            this.service.TryGetValueAsync(requestedKey, out string item).GetAwaiter().GetResult();

            var itemNumber = Array.IndexOf(stored, requestedKey) + 1;
            var expectedItem = $"Item number {itemNumber}";

            Assert.True(expectedItem == item);
        }

        [Test]
        [TestCase(5, 0, 1, 2, 10, 100, 50, 22, 37, 5)]
        [TestCase(11, 10, 2, 3, 5, 11)]
        [TestCase(22, 33, 22)]
        public void Does_TryGetValueAsync_ReturnTrue_If_KeyFound(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var successful = this.service.TryGetValueAsync(requestedKey, out string result).GetAwaiter().GetResult();

            Assert.True(successful);
        }

        [Test]
        [TestCase(3, 0, 1, 2, 10, 100, 50, 22, 37, 5)]
        [TestCase(50, 10, 2, 3, 5, 11)]
        [TestCase(44, 33, 22)]
        public void Does_TryGetValueAsync_ReturnFalse_If_KeyNotFound(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var successful = this.service.TryGetValueAsync(requestedKey, out string result).GetAwaiter().GetResult();

            Assert.False(successful);
        }

        [Test]
        [TestCase(3, 0, 1, 2, 10, 100, 50, 22, 37, 5)]
        [TestCase(50, 10, 2, 3, 5, 11)]
        [TestCase(44, 33, 22)]
        public void Does_GetValueAsync_ThrowException_IfKeyNotFound(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            Assert.Throws<KeyNotFoundException>(() =>
            {
                this.service.GetValueAsync(requestedKey).GetAwaiter().GetResult();
            });
        }

        [Test]
        [TestCase(1, 6, 3, 4, 2, 10, 1)]
        [TestCase(50, 2, 50, 100, 3, 1, 0)]
        [TestCase(3, 5, 10, 7, 3, 9, 22, 8)]
        public void Does_TryDeleteEntryAsync_ReturnTrue_IfDeletionSuccessful(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var success = this.service.TryDeleteEntryAsync(requestedKey).GetAwaiter().GetResult();

            Assert.True(success);
        }

        [Test]
        [TestCase(5, 6, 3, 4, 2, 10, 1)]
        [TestCase(40, 2, 50, 100, 3, 1, 0)]
        [TestCase(30, 5, 10, 7, 3, 9, 22, 8)]
        public void Does_TryDeleteEntryAsync_ReturnFalse_IfDeletionSuccessful(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var success = this.service.TryDeleteEntryAsync(requestedKey).GetAwaiter().GetResult();

            Assert.False(success);
        }

        [Test]
        [TestCase(1, 6, 3, 4, 2, 10, 1)]
        [TestCase(50, 2, 50, 100, 3, 1, 0)]
        [TestCase(3, 5, 10, 7, 3, 9, 22, 8)]
        public void Does_TryDeleteEntryAsync_ActuallyDeleteEntry_IfEntryExists(int requestedKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            this.service.TryDeleteEntryAsync(requestedKey).GetAwaiter().GetResult();

            var stillExists = this.service.TryGetValueAsync(requestedKey, out string value).GetAwaiter().GetResult();

            Assert.True(!stillExists && value == null);
        }

        [Test]
        [TestCase(1, 6, 3, 4, 2, 10, 1)]
        [TestCase(50, 2, 50, 100, 3, 1, 0)]
        [TestCase(3, 5, 10, 7, 3, 9, 22, 8)]
        public void Does_DoesEntryExistAsync_ReturnTrue_IfEntryExists(int entryKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var exists = this.service.DoesEntryExistAsync(entryKey).GetAwaiter().GetResult();

            Assert.True(exists);
        }

        [Test]
        [TestCase(5, 6, 3, 4, 2, 10, 1)]
        [TestCase(55, 2, 50, 100, 3, 1, 0)]
        [TestCase(334, 5, 10, 7, 3, 9, 22, 8)]
        public void Does_DoesEntryExistAsync_ReturnFalse_IfEntryDoesNotExists(int entryKey, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var exists = this.service.DoesEntryExistAsync(entryKey).GetAwaiter().GetResult();

            Assert.False(exists);
        }

        [Test]
        [TestCase("Item number 3", 6, 3, 4, 2, 10, 1)]
        [TestCase("Item number 1", 55, 2, 50, 100, 3, 1, 0)]
        [TestCase("Item number 6", 5, 10, 7, 3, 9, 22, 8)]
        public void Does_GetAssociatedKeyAsync_ReturnKey_If_ValueExists(string requestedValue, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            var key = this.service.GetAssociatedKeyAsync(requestedValue).GetAwaiter().GetResult();

            var expectedItemForKey = this.service.GetValueAsync(key).GetAwaiter().GetResult();

            Assert.That(expectedItemForKey == requestedValue && stored.Contains(key));
        }


        [Test]
        [TestCase("Item number 10", 6, 3, 4, 2, 10, 1)]
        [TestCase("Item number 20", 55, 2, 50, 100, 3, 1, 0)]
        [TestCase("Item number 50", 5, 10, 7, 3, 9, 22, 8)]
        public void Does_GetAssociatedKeyAsync_Throw_Argument_Exception_If_ValueNotFound(string requestedValue, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], $"Item number {i + 1}").GetAwaiter().GetResult();
            }

            Assert.Throws<ArgumentException>(() =>
            {
                this.service.GetAssociatedKeyAsync(requestedValue).GetAwaiter().GetResult();
            });
        }

        [Test]
        [TestCase(5, 10, 2, 1, 22, 33, 50)]
        [TestCase(5, 2)]
        [TestCase(3)]
        [TestCase()]
        [TestCase(1000, 1, 3)]
        public void Does_GetAllValuesAsync_Return_All_Stored_Values(params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], stored[i].ToString()).GetAwaiter().GetResult();
            }

            var values = this.service.GetAllValuesAsync().GetAwaiter().GetResult();

            foreach (var item in values)
            {
                if (!stored.Contains(Convert.ToInt32(item)))
                    Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        [TestCase(2, 5, 10, 2, 1, 22, 33, 50)]
        [TestCase(5, 5, 2)]
        [TestCase(3, 3)]
        [TestCase(5)]
        [TestCase(3, 1000, 1, 3)]
        public void Does_GetAllValuesExceptAsync_Return_All_Stored_Values_Except_The_Specified(int except, params int[] stored)
        {
            for (int i = 0; i < stored.Length; i++)
            {
                this.service.StoreEntryAsync(stored[i], stored[i].ToString()).GetAwaiter().GetResult();
            }

            var values = this.service.GetAllValuesExceptAsync(except).GetAwaiter().GetResult();
            var control = stored.Where(p => p != except);

            if (values.Count() != control.Count())
                Assert.Fail();

            foreach (var item in values)
            {
                if (!control.Contains(Convert.ToInt32(item)))
                    Assert.Fail();
            }
        }
    }
}
