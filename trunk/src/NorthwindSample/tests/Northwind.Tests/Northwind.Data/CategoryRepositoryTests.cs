﻿using System;
using System.Collections.Generic;
using FluentNHibernate;
using FluentNHibernate.AutoMap;
using NUnit.Framework;
using SharpArch.Core.PersistenceSupport;
using Northwind.Core;
using Northwind.Data;
using System.Diagnostics;
using SharpArch.Core.DomainModel;
using NUnit.Framework.SyntaxHelpers;
using SharpArch.Data.NHibernate;
using SharpArch.Testing.NUnit.NHibernate;

namespace Tests.Northwind.Data
{
    [TestFixture]
    [Category("DB Tests")]
    public class CategoryRepositoryTests : RepositoryTestsBase
    {

        protected override void LoadTestData()
        {
            CreatePersistedCategory("Beverages");
            CreatePersistedCategory("Snacks");

        }

        [Test]
        public void CanGetAllCategories()
        {
            IList<Category> categories = categoryRepository.GetAll();

            Assert.That(categories, Is.Not.Null);
            Assert.That(categories.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanGetCategoryById()
        {
            Category category = categoryRepository.Get(1);

            Assert.That(category.CategoryName, Is.EqualTo("Beverages"));
        }

        private void CreatePersistedCategory(string categoryName)
        {
            Category category = new Category(categoryName);
            categoryRepository.SaveOrUpdate(category);
            FlushSessionAndEvict(category);
        }

        private bool GetAutoMappingFilter(Type t)
        {
            return t.Namespace == "Northwind.Core" && (t.Name.Contains("OrdersExtensions") == false);
        }

        private void GetConventions(Conventions c)
        {
            c.GetPrimaryKeyNameFromType = type => type.Name + "ID";
            c.FindIdentity = type => type.Name == "ID";
            c.GetTableName = type => Inflector.Net.Inflector.Pluralize(type.Name);
            c.IsBaseType = isBaseTypeConvention;
            c.GetForeignKeyNameOfParent = type => type.Name + "ID";
        }

        private bool isBaseTypeConvention(Type arg)
        {
            var derivesFromEntity = arg == typeof(Entity);
            var derivesFromEntityWithTypedId = arg.IsGenericType && (arg.GetGenericTypeDefinition() == typeof(EntityWithTypedId<>));
            return derivesFromEntity || derivesFromEntityWithTypedId;
        }

        private IRepository<Category> categoryRepository = new Repository<Category>();
    }
}