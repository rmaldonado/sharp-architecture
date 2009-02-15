﻿using NUnit.Framework;
using MvcContrib.TestHelper;
using Northwind.Web.Controllers;
using SharpArch.Core.PersistenceSupport;
using Northwind.Core;
using Rhino.Mocks;
using NUnit.Framework.SyntaxHelpers;
using System.Web.Mvc;
using System.Collections.Generic;
using SharpArch.Testing;

namespace Tests.Northwind.Web.Controllers
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        [Test]
        public void CanListCategories() {
            CategoriesController controller = 
                new CategoriesController(CreateMockCategoryRepository());

            ViewResult result = 
                controller.Index().AssertViewRendered().ForView("");

            Assert.That(result.ViewData, Is.Not.Null);
            Assert.That(result.ViewData.Model as List<Category>, Is.Not.Null);
            Assert.That((result.ViewData.Model as List<Category>).Count, Is.EqualTo(3));
        }

        [Test]
        public void CanCreateCategory() {
            CategoriesController controller =
                new CategoriesController(CreateMockCategoryRepository());

            ViewResult result =
                controller.Create("Hawaiian").AssertViewRendered().ForView("");

            Assert.That(result.ViewData, Is.Not.Null);
            Assert.That(result.ViewData.Model as Category, Is.Not.Null);
            Assert.That((result.ViewData.Model as Category).Id, Is.GreaterThan(0));
        }

        [Test]
        public void CanDetailCategory() {
            CategoriesController controller = 
                new CategoriesController(CreateMockCategoryRepository());

            ViewResult result = 
                controller.Show(1).AssertViewRendered().ForView("");

            // The builder object acts as a wrapper around the controller, 
            // so be sure to interrogate it instead of the controller
            Assert.That(result.ViewData, Is.Not.Null);
            Assert.That(result.ViewData.Model as Category, Is.Not.Null);
            Assert.That((result.ViewData.Model as Category).Id, Is.EqualTo(1));
        }

        private IRepository<Category> CreateMockCategoryRepository() {
            MockRepository mocks = new MockRepository();

            IRepository<Category> mockedRepository = mocks.StrictMock<IRepository<Category>>();
            Expect.Call(mockedRepository.GetAll())
                .Return(CreateCategories());
            Expect.Call(mockedRepository.Get(1)).IgnoreArguments()
                .Return(CreateCategory());
            Expect.Call(mockedRepository.SaveOrUpdate(null)).IgnoreArguments()
                .Return(CreateCategory());
            mocks.Replay(mockedRepository);

            return mockedRepository;
        }

        private Category CreateCategory() {
            Category category = new Category("Hawaiian");
            EntityIdSetter<int>.SetIdOf(category, 1);
            return category;
        }

        private List<Category> CreateCategories() {
            List<Category> categories = new List<Category>();

            categories.Add(new Category("Blue"));
            categories.Add(new Category("Green"));
            categories.Add(new Category("Yellow"));

            return categories;
        }
    }
}
