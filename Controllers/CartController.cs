﻿using Bakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Controllers
{
    public class CartController : Controller
    {
        BakeryContext _context;

        public CartController(BakeryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = Tools.GetUserId(_context, User);
            
            return View
            (
                _context.CartItems.Include(i => i.Product)
                    .Where(i => i.UserId == userId)
                    .ToList()
            );
        }

        public IActionResult Add(int? productId)
        {
            if (productId is null)
            {
                return new NoContentResult();
            }

            var userId = Tools.GetUserId(_context, User);

            var item = _context.CartItems.FirstOrDefault(i => i.ProductId == productId && i.UserId == userId);
            if (item is null)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                _context.CartItems.Add(new CartItem() { Count = 1, Product = product, UserId = userId });
            }
            else
            {
                item.Count++;
            }

            _context.SaveChanges();
            return new NoContentResult();
        }

        public IActionResult IncreaseCountByOne(int? cartItemId)
        {
            if (cartItemId == null)
            {
                return new NoContentResult();
            }

            var item = _context.CartItems.Where(i => i.Id == cartItemId).FirstOrDefault();
            if (item == null)
            {
                return new NoContentResult();
            }

            item.Count++;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult DecreaseCountByOne(int? cartItemId)
        {
            if (cartItemId == null)
            {
                return new NoContentResult();
            }

            var item = _context.CartItems.Where(i => i.Id == cartItemId).FirstOrDefault();
            if (item == null)
            {
                return new NoContentResult();
            }

            if (item.Count > 1)
            {
                item.Count--;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return Delete(item.Id);
            }
        }

        public IActionResult Delete(int? cartItemId)
        {
            if (cartItemId == null)
            {
                return new NoContentResult();
            }

            var item = _context.CartItems.Where(i => i.Id == cartItemId).FirstOrDefault();
            if (item == null)
            {
                return new NoContentResult();
            }
            _context.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var userId = Tools.GetUserId(_context, User);

            if (!_context.CartItems.Any(i => i.UserId == userId))
            {
                return Content($"Невозможно оформить заказ: в Корзине нет товаров.");
            }

            var totalSum = 0;
            var totalCount = 0;
            foreach(var cartItem in _context.CartItems.Include(i => i.Product).Where(i => i.UserId == userId))
            {
                var newOrder = new Order() 
                { 
                    Count = cartItem.Count, 
                    Date = DateTime.Now, 
                    Product = cartItem.Product 
                };

                totalSum += cartItem.Count * cartItem.Product.Price;
                totalCount += cartItem.Count;

                _context.Orders.Add(newOrder);
                _context.Remove(cartItem);
            }

            _context.SaveChanges();
            return Content($"Куплено {totalCount} товаров на сумму {totalSum}.");
        }
    }
}
