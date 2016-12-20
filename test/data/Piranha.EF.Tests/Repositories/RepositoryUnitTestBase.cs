/*
 * Copyright (c) 2016 Billy Wolfington
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using AutoMapper;
using System.Collections.Generic;

namespace Piranha.EF.Tests.Repositories
 {
     public abstract class RepositoryUnitTestBase<TRepository>
        where TRepository : class
    {
        #region Properties
        /// <summary>
        /// The databse api to inject into <see cref="repository" />
        /// </summary>
        protected readonly Mock<IDb> mockDb;

        /// <summary>
        /// The repository being tested
        /// </summary>
        protected readonly TRepository repository;
        #endregion

        #region Test initialization
        /// <summary>
        /// Default constructor/test initialization
        /// </summary>
        public RepositoryUnitTestBase() {
            mockDb = new Mock<IDb>();
            repository = SetupRepository();
            Module module = new Module();
            module.Init();
            SetupMockDbData();
        }

        /// <summary>
        /// Constructs the repository to be assigned to <see cref="repository" />
        /// </summary>
        /// <returns>
        /// The repository
        /// </returns>
        protected abstract TRepository SetupRepository();

        /// <summary>
        /// Setup <see cref="mockDb" /> data for use in all tests
        /// </summary>
        protected abstract void SetupMockDbData();
        #endregion

        /// <summary>
        /// Converts an integer to a Guid
        /// </summary>
        /// <param name="value">The integer to convert</param>
        /// <returns>
        /// The Guid
        /// </returns>
        protected Guid ConvertIntToGuid(int value) {
            byte[] valueAsBytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(valueAsBytes, 0);
            return new Guid(valueAsBytes);
        }

        /// <summary>
        /// Binds a queryable as the data behind the DbSet
        /// </summary>
        /// <param name="mockDbSet">The mocked DbSet</param>
        /// <param name="source">The data behind</param>
        protected void SetupMockDbSet<T>(Mock<DbSet<T>> mockDbSet, IQueryable<T> source)
            where T : class {
            mockDbSet.As<IQueryable<T>>().Setup(s => s.Provider).Returns(source.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(s => s.Expression).Returns(source.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(s => s.ElementType).Returns(source.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(s => s.GetEnumerator()).Returns(source.GetEnumerator());
        }

        /// <summary>
        /// Randomizes the ordering of the given list in place
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToShuffle">The list to shuffle</param>
        /// <remarks>
        /// Solution found StackOverflow http://stackoverflow.com/a/1262619/5884242
        /// </remarks>
        protected void Shuffle<T>(IList<T> listToShuffle) {
            Random rng = new Random();
            int n = listToShuffle.Count;

            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[n];
                listToShuffle[n] = value;
            }
        }
    }
 }