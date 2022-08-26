using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Application.Activities;
using Domain;

namespace Application.SubcutaneousTests.Activities;
public class ListTest
{
    [Test]
    public async Task ShouldReturnlistOfActivities(){

        var activityCommand = new Create.Command
            {
                Activity = new Activity
                {
                    Title = "Test Activity",
                    Description = "Description",
                    Category = "Food",
                    Date = DateTime.Today,
                    City = "London",
                    Venue = "Test Venue"
                }
            };
    }
}