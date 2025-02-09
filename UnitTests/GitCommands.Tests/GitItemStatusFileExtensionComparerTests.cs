﻿using System.Collections;
using GitCommands;
using GitCommands.Git;

namespace GitCommandsTests
{
    [TestFixture]
    public class GitItemStatusFileExtensionComparerTests
    {
        private GitItemStatusFileExtensionComparer _comparerUnderTest;

        [SetUp]
        public void Setup()
        {
            _comparerUnderTest = new GitItemStatusFileExtensionComparer();
        }

        [Test]
        public void Compares_file_extension_then_normal_item_comparison()
        {
            GitItemStatus[] unsortedItems = new[]
            {
                new GitItemStatus("src/newFile.txt") { OldName = null },
                new GitItemStatus("src/newFile.cs") { OldName = null },
                new GitItemStatus("src/Alice.cs") { OldName = null },
                new GitItemStatus("src/newName.cs") { OldName = "src/oldName.cs" },
                new GitItemStatus("newFile.txt") { OldName = null },
                new GitItemStatus("newName.cs") { OldName = "oldName.cs" },
                new GitItemStatus("changeExtension.cs") { OldName = "changeExtension.txt" },
            };

            List<GitItemStatus> copy = unsortedItems.ToList();
            copy.Sort(_comparerUnderTest);
            List<GitItemStatus> sorted = copy;

            CollectionAssert.AreEqual(
                new GitItemStatus[]
                {
                    new("changeExtension.cs") { OldName = "changeExtension.txt" },
                    new("newName.cs") { OldName = "oldName.cs" },
                    new("src/Alice.cs") { OldName = null },
                    new("src/newFile.cs") { OldName = null },
                    new("src/newName.cs") { OldName = "src/oldName.cs" },
                    new("newFile.txt") { OldName = null },
                    new("src/newFile.txt") { OldName = null },
                },
                sorted,
                new GitItemStatusCollectionEqualityComparer());
        }

        [Test]
        public void Nulls_are_handled()
        {
            Assert.AreEqual(1, _comparerUnderTest.Compare(new GitItemStatus("name"), null));
            Assert.AreEqual(0, _comparerUnderTest.Compare(null, null));
            Assert.AreEqual(-1, _comparerUnderTest.Compare(null, new GitItemStatus("name")));
        }

        /// <summary>
        /// This is a pure service for the NUnit CollectionAssert function call above.
        /// Not sure why it takes <see cref="IComparer"/> over <see cref="IEqualityComparer"/>.
        /// This is not a typical comparer that can be used for sorting. We return 0 for equal
        /// and 1 for not equal.
        /// </summary>
        private class GitItemStatusCollectionEqualityComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (x is null || y is null)
                {
                    return 1;
                }

                GitItemStatus lhs = x as GitItemStatus;
                GitItemStatus rhs = y as GitItemStatus;

                return lhs.Name == rhs.Name && lhs.OldName == rhs.OldName ? 0 : 1;
            }
        }
    }
}
