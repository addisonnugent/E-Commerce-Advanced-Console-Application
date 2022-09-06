using Newtonsoft.Json;
using System;
using Library.ECommerceApp.Models;
using System.Text.Json;
using Library.ECommerceApp.Utility;

namespace Library.ECommerceApp.Services
{

	public class ProductServices
	{
		private ListNavigator<Product> listNavigator;
		private List<Product> ProductList;
		private int pagecount;


		// Returns list of products in inventory
		public List<Product> Products
		{
			get { return ProductList; }
		}


		public int NextId
		{
			get
			{
				if (!Products.Any()){
					return 1;
				}
				return Products.Select(p => p.Id).Max() + 1;
			}
		}


		private static ProductServices? current;


		public static ProductServices Current   // singletons
		{
			get
			{
				if (current == null) {
					current = new ProductServices();
				}

				return current;
			}
		}


		// Creates list of products
		private ProductServices()
		{
			ProductList = new List<Product>();
			listNavigator = new ListNavigator<Product>(ProductList);
		}


		// Creates new Product & adds to ProductList
		public void Create(Product? product)
		{
			if (product != null)
			{
				product.Id = NextId;
				ProductList.Add(product);
			}
		}

		// Updates Product if exisits 
		public void Update(Product? product)
		{
			if (product == null) { return; }
			var prod = ProductList.FirstOrDefault(p => p.Id == product.Id);

			if (prod != null)   //if item DNE then add if quantity > 0
			{
				ProductList.Remove(prod);
			}


			if (product is ProductByQuantity)   // Quantity
			{
				var productbyquantity = product as ProductByQuantity;
				if (productbyquantity != null)
				{
					if (productbyquantity.Quantity <= 0)
						return;

					ProductList.Add(product);

				}
			}
			else if (product is ProductByWeight) // Weight
			{
				var productbyweight = product as ProductByWeight;
				if (productbyweight != null)
				{
					if (productbyweight.Weight <= 0)
						return;

					ProductList.Add(product);
				}
			}
		}


		// Deletes product with matching id #
		public void Delete(int id)
		{
			var productToDelete = ProductList.FirstOrDefault(p => p.Id == id);
			if (productToDelete == null) {
				return;
			}
			ProductList.Remove(productToDelete);
		}


		public void Load(string fileName)
		{
			var productsJson = File.ReadAllText(fileName);
			JsonSerializerSettings settings = new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Objects
			};

			ProductList = JsonConvert.DeserializeObject<List<Product>>(productsJson, settings) ?? new List<Product>();
		}

		public void Save(string fileName)
		{
			File.WriteAllText(fileName, String.Empty);
			JsonSerializerSettings settings = new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.All
			};

			string strJson = JsonConvert.SerializeObject(ProductList, settings);
			File.WriteAllText(fileName, strJson);
		}


		// ----------------- Shopping Cart ----------------------

    // Adds a unit by id #
		public void AddUnit(int id)
		{
			// quantity of product + 1
			var addProduct = ProductList.FirstOrDefault(p => p.Id == id);
			if (addProduct == null) {
				return;
			}

			if (addProduct is ProductByQuantity)  // Quantity
      {
				var productby = addProduct as ProductByQuantity;
				if (productby != null)
					productby.Quantity = productby.Quantity + 1;
			}
			else if (addProduct is ProductByWeight)  // Weight
			{
				var productby = addProduct as ProductByWeight;
				if (productby != null)
					productby.Weight = productby.Weight + 1;
			}
		}


    // Removes a unit from by id #
		public void RemoveUnit(int id)
		{
			var productToDelete = ProductList.FirstOrDefault(p => p.Id == id);
			if (productToDelete == null) {
				return;
			}

			if (productToDelete is ProductByQuantity)  // Quantity
			{
				var productby = productToDelete as ProductByQuantity;
				if (productby != null)
					productby.Quantity = productby.Quantity - 1;
			}
			else if (productToDelete is ProductByWeight)   // WEight
			{
				var productby = productToDelete as ProductByWeight;
				if (productby != null)
					productby.Weight = productby.Weight - 1;
			}
		}


		// ----------------  NAVIGATING LIST ----------------

		public void Navigator()
		{
			Console.Write("How would you like your Inventory Sorted By:\n1) Name\n2) Price Per Unit\nEnter choice: ");
			
			var sortType = int.Parse(Console.ReadLine() ?? "0");
			var plist = ProductList.OrderBy(p => p.Name).ToList(); 
			if (sortType == 1)   // Name Sort
			{
				plist = ProductList.OrderBy(p => p.Name).ToList();                                                 
			}
			else if (sortType == 2)  // Price Sort
			{
				plist = ProductList.OrderBy(q => q.priceinp).ToList();
			}
			else
			{
				Console.WriteLine("Invalid Sort Type Printing as is. ");
			}

			Console.WriteLine("Printing Inventory... \n");
			bool cont = true;

			listNavigator = new ListNavigator<Product>(plist);

			if (Products.Count <= 0) { Console.WriteLine("\n***** INVENTORY EMPTY *****\n"); }
			else{
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"-----------------------------------------------  Current Inventory  -----------------------------------------------");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------\n\n");
				Console.WriteLine($"--------------------------------------------------    Page 1    ---------------------------------------------------");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"ID#:\tName:\t\tPrice per unit:\t\tUnits:\t\t\tDescription:\tDEALS:");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				
        pagecount = 1;
				CurrentPage();
        
				while (cont)
				{
					// ---- choices of what to do next ----
					Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

					Console.WriteLine("(1) Next   (2) Previous   (3) First   (4) Last   (5) Enter Page Number   (6) EXIT "); // Create
					Console.Write("\nEnter choice: ");             // Exit
					var input = int.Parse(Console.ReadLine() ?? "0");
					Console.Clear();
					
					// -- switch for whats next
					switch (input)
					{
						case 1:         // Next
							Next();
							break;
						case 2:         // Previous
							Previous();
							break;
						case 3:         // First
							First();
							break;
						case 4:         // Last
							Last();
							break;
						case 5:         // Enter Page Number
							Console.Write("\nEnter Page number: ");            
							var pagenum = int.Parse(Console.ReadLine() ?? "0");
							Find(pagenum);
							break;
						case 6:         // Exit
							cont = false;
							break;

						default:
							Console.WriteLine("Invalid Choice, Try Again.\n");
							break;
					}
				}	
			}
		}


		public void CurrentPage()
		{
			Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GetCurrentPage());
			dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
		}

		public void Next()
		{
      if (listNavigator.HasNextPage)
      {
        pagecount++;
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\tUnits:\t\t\tDescription:\tDEALS:");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

				Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoForward());
				dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
			}
      else {
				Console.WriteLine("No Next Page.");
			}
		}
    

		public void Previous()
		{
			if (listNavigator.HasPreviousPage)
			{
				pagecount--;
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"  ID#:\tName:\t\tPrice per unit:\t\tUnits:\t\t\tDescription:\tDEALS:");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

				Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoBackward());
				dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
			}
			else {
				Console.WriteLine("No Previous Page.");
			}
		}

		public void Find(int pagenum)
		{
			if(pagenum <= ProductList.Count/5 || pagenum < 0)
      {
				pagecount = pagenum;
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
				Console.WriteLine($"ID#:\tName:\t\tPrice per unit:\t\tUnits:\t\t\tDescription:\tDEALS:");
				Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

				Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoToPage(pagecount));
				dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
			}
      else {
				Console.WriteLine("No Such Page Number.");
			}
		}

		public void First()
		{
			pagecount = 1;
			Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
			Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
			Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
			Console.WriteLine($"ID#:\tName:\t\tPrice per unit:\t\tUnits:\t\t\tDescription:\tDEALS:");
			Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

			Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoToFirstPage());
			dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
		}


		public void Last()
		{
			pagecount = ProductList.Count/5;
			Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
			Console.WriteLine($"--------------------------------------------------    Page {pagecount}     ---------------------------------------------------");
			Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");
			Console.WriteLine($"ID#:\tName:\t\tPrice per unit:\t\tUnits:\t\t\tDescription:\tDEALS:");
			Console.WriteLine($"-------------------------------------------------------------------------------------------------------------------");

			Dictionary<int, Product> dictionary = new Dictionary<int, Product>(listNavigator.GoToLastPage());
			dictionary.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
		}
    
	} // END PRODUCT SERVICE 
}
