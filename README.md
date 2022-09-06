# E-Commerce-Advanced-Console-Application
Improving upon E-Commmerce Basic Console Application.

Creates to two derived classed called ProductByWeight and ProductByQuantity that override your default pricing behavior using the model class Product.


ProductByWeight : calculate the price using a real valued property called "Weight" that measures weight to the tenth

ProductByQuantity : integer valued property called "Quantity" that keeps track of how many of a given item exists in either the inventory or the cart.
- allows the item to be BOGO (buy one, get one) 

Your application should store both types of products into a single, heterogeneous (i.e., polymorphic) list for both the inventory and the cart. In addition, when displaying lists of products (including search results), users should now be able to:
Sort Types for Displaying list of products:
- Sort by name
- Sort by total price (cart)
- Sort by unit price (inventory)

Each search only allows 5 products to be displayed at a time, allowing navigation through list. 

Checkout:
- requests payment infomation
- requests shipping address 
