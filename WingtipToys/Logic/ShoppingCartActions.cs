using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartId { get; set; }

        private ProductContext _db = new ProductContext();

        public const string CartSessionKey = "CartID";

        public void AddToCart(int id)
        {
            //retrieve the product from the database
            ShoppingCartID = GetCartID();

            var cartItem = _db.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartID && c.ProductId == id);

            if(cartItem == null)
            {
                //create a new cart item if no cart item exists
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = ShoppingCartID,
                    Product = _db.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                _db.ShoppingCartItems.Add(cartItem);
            } 
            else
            {
                //if the item does exist in the cart
                //then add one to the quantity
                cartItem.Quantity++;
            }
            _db.SaveChanges();
        }

        public void Dispose()
        {
            if(_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        public string GetCartID()
        {
            if(HttpContext.Current.Session[CartSessionKey] == null)
            {
                if(!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    //generate a new random GUID using System.Guid class
                    Guid tempCardId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCardId.ToString();
                }
            }

            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartID();

            return _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
        }
    }
}