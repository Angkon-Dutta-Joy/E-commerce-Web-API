using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();   //for swagger
builder.Services.AddSwaggerGen();            //for swagger


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();     //Use prefix means middleware
    app.UseSwaggerUI();
}

//Now if we go to http://localhost:<port_number>/swagger/index.html we can see all the endpoints in swagger 

app.UseHttpsRedirection();


app.MapGet("/hello", () =>
 {
    return "Hello World!";
  });

app.MapPost("/post", () =>
 {
    return "Hello Post!";
  });

List<Category> categories = new List<Category>();


// //Read a Category => GET : /api/categories
// app.MapGet("/api/categories", () =>
// {
//   return Results.Ok(categories);
// });

//Read a Category => GET : /api/categories
app.MapGet("/api/categories", ([FromQuery] string searchValue = "") =>
{
  
  //search categories using this value
  if (!String.IsNullOrEmpty(searchValue))
  {
    //Console.WriteLine(searchValue);
    var searchCategories = categories.Where(c => c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
    return Results.Ok(searchCategories);
  }

  return Results.Ok(categories);
});

// //Create a Category => POST : /api/categories
// app.MapPost("/api/categories", () =>
// {
//   var newCategory = new Category
//   {
//     CategoryId = Guid.NewGuid(),
//     Name = "Electronics",
//     Description = "Devices and gadgets including Phones, laptops, and other elctronics equipment",
//     CreatedAt = DateTime.UtcNow,
//   };
//   categories.Add(newCategory);
//   return Results.Created($"/api/categories/{newCategory.CategoryId}", newCategory);
// });

//Create a Category => POST : /api/categories
app.MapPost("/api/categories", ([FromBody] Category categoryData) =>
{

  //Console.WriteLine($"{categoryData}");

  if (string.IsNullOrEmpty(categoryData.Name))
  {
    return Results.BadRequest("Category name is required and can't be empty.");
  }
  if (categoryData.Name.Length >= 2)
  {
    return Results.BadRequest("Category name must be at least 2 characters long.");
  }
  
  var newCategory = new Category
  {
    CategoryId = Guid.NewGuid(),
    Name = categoryData.Name,
    Description = categoryData.Description,
    CreatedAt = DateTime.UtcNow,
  };
  categories.Add(newCategory);
  return Results.Created($"/api/categories/{newCategory.CategoryId}", newCategory);
});


// //Delete a Category => Delete : /api/categories
// app.MapDelete("/api/categories", () =>
// {
//   var foundCategory = categories.FirstOrDefault(category => category.CategoryId == Guid.Parse("f6158254-99bb-4d1c-81cb-91d9720e759a"));
//   if (foundCategory == null)
//   {
//     return Results.NotFound("Category with this id does not exist");
//   }
//   categories.Remove(foundCategory);
//   return Results.NoContent();
// });

//Delete a Category => Delete : /api/categories/{categoryId}
app.MapDelete("/api/categories/{categoryId:guid}", (Guid categoryId) =>
{
  var foundCategory = categories.FirstOrDefault(category => category.CategoryId == categoryId);
  if (foundCategory == null)
  {
    return Results.NotFound("Category with this id does not exist");
  }
  categories.Remove(foundCategory);
  return Results.NoContent();
});



// //Update a Category => PUT : /api/categories
// app.MapPut("/api/categories", () =>
// {
//   var foundCategory = categories.FirstOrDefault(category => category.CategoryId == Guid.Parse("f6158254-99bb-4d1c-81cb-91d9720e759a"));
//   if (foundCategory == null)
//   {
//     return Results.NotFound("Category with this id does not exist");
//   }

//   foundCategory.Name = "Smart Phone";
//   foundCategory.Description = "Smart phone is a nice Category";
//   return Results.NoContent();
// });

//Update a Category => PUT : /api/categories
app.MapPut("/api/categories/{categoryId:guid}", (Guid categoryId, [FromBody] Category categoryData ) =>
{
  var foundCategory = categories.FirstOrDefault(category => category.CategoryId == categoryId);
  if (foundCategory == null)
  {
    return Results.NotFound("Category with this id does not exist");
  }

  if (categoryData == null)
  {
    return Results.BadRequest("Category data is missing.");
  }

  if (!string.IsNullOrEmpty(categoryData.Name))
  {
    if (categoryData.Name.Length >= 2)
    {
      foundCategory.Name = categoryData.Name;
    }
    else
    {
      return Results.BadRequest("Category name must be at least 2 characters long.");
    }
  }

  if (!string.IsNullOrWhiteSpace(categoryData.Description))
  {
    foundCategory.Description = categoryData.Description ;
  }
  
  return Results.NoContent();
});

app.Run();


public record Category
{
  public Guid CategoryId { get; set; }
  public string Name { get; set; }
  public string Description { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }

};

//CRUD
// Create => Create a Category => POST : /api/categories
// Read => Read a Category => GET : /api/categories
// Update => Get a Category by Id => PUT : /api/categories
// Delete => Delete a Category => DELETE : /api/categories