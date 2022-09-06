using System;
namespace Library.ECommerceApp.Models
{
	public partial class ProductByWeight : Product
	{
		
		public double Weight { get; set; }
		public double Price { get; set; }
		public double TotalPrice { get; set; }

		public ProductByWeight()
		{
			priceinp = Price;
		}
		public override string ToString()
		{	
        return $"{Id}\t{Name}\t\t$ {Price} per lb\t\t{Weight} lbs avalible\t{Description}\t\t";
		}

		public double getTotalPrice()
    {
        TotalPrice = Weight * Price;
        return TotalPrice;
    }
  }
}
