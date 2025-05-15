using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1_ShopApp.Identity;
using _1_ShopApp.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.entity;

namespace _1_ShopApp.Controllers
{
    [Authorize]
    public class CartController: Controller
    {
        private ICartService _cartService;
        private UserManager<User> _userManager;
        private IOrderService _orderService;
        public CartController(ICartService cartService, UserManager<User> userManager, IOrderService orderService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _orderService = orderService; 
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            return View(new CartModel() {

                CartId = cart.Id,
                CartItems = cart.CardItems.Select(i=> new CartItemModel ()
                {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity
                }).ToList()
            });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);

            _cartService.AddtoCart(userId,productId,quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId,productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            var ordermodel = new OrderModel();
            ordermodel.CartModel = new CartModel(){
                CartId = cart.Id,
                CartItems = cart.CardItems.Select(i=> new CartItemModel ()
                {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity
                }).ToList()
            };
            
            return View(ordermodel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderModel model)
        {
            if(!ModelState.IsValid)
            {

            var userId = _userManager.GetUserId(User);
            var cart = _cartService.GetCartByUserId(userId);

            model.CartModel = new CartModel(){
                CartId = cart.Id,
                CartItems = cart.CardItems.Select(i=> new CartItemModel ()
                {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity
                }).ToList()
            };

                var payment = await PaymentProcess(model);

                if(payment.Status == "success")
                {
                        SaveOrder(model,payment,userId);
                        ClearCart(model.CartModel.CartId);
                        return View("Success");
                }
                else
                {
                    var msg = new AlertMessage()
                    {
                        Message = $"{payment.ErrorMessage}",
                        AlertType = "danger"
                    };
                    TempData["message"] = JsonConvert.SerializeObject(msg);
                }
            }
            return View(model);
        }

        public IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);
           var orders = _orderService.GetOrders(userId);

            var orderlistModel = new List<OrderListModel>();
            OrderListModel orderModel; 

            foreach(var item in orders)
            {
                orderModel = new OrderListModel();

                orderModel.OrderId = item.Id;
                orderModel.OrderNumber = item.OrderNumber;
                orderModel.OrderDate = item.OrderDate;
                orderModel.Phone = item.Phone;
                orderModel.FirstName = item.FirstName;
                orderModel.LastName = item.LastName;
                orderModel.Email = item.Email;
                orderModel.Adress = item.Adress;
                orderModel.City = item.City;
                orderModel.OrderState = item.OrderState;
                orderModel.PaymentType = item.PaymentType;

                orderModel.OrderItems = item.OrderItems.Select(i=> new OrderItemModel() {

                    OrderItemId = i.Id,
                    Name = i.Product.Name,
                    Price = (double)i.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ImageUrl


                }).ToList();

                orderlistModel.Add(orderModel);
            }

            return View("Orders", orderlistModel);
        }

        private void ClearCart(int CartId)
        {
            _cartService.ClearCart(CartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string? userId)
        {
            var order = new Order();
            order.OrderNumber = new Random().Next(111111,999999).ToString();
            order.OrderState = EnumOrderState.completed;
            order.PaymentType = EnumPaymentType.CreditCard;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
            order.OrderDate = new DateTime();
            order.FirstName = model.FirstName;
            order.LastName = model.LastName;
            order.UserId = userId;
            order.Adress = model.Address;
            order.Phone = model.Phone;
            order.Email = model.Email;
            order.City = model.City;
            order.Note = model.Note;
            order.OrderItems = new List<shopapp.entity.OrderItem>();

            foreach(var item in model.CartModel.CartItems)
            {
                var orderItem = new shopapp.entity.OrderItem()
                {
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems = new List<shopapp.entity.OrderItem>();
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);

        }

       private async Task<Payment> PaymentProcess(OrderModel model)
        {
              Options options = new Options();
                options.ApiKey = "sandbox-byv7vQA4R5fvRgvWCRVP4nETXQyGydM8";
                options.SecretKey = "sandbox-5UNdIVrzY8dainYgSCESYATRBPQUqDtO";
                options.BaseUrl = "https://sandbox-api.iyzipay.com";
                        
                CreatePaymentRequest request = new CreatePaymentRequest();
                request.Locale = Locale.TR.ToString();
                request.ConversationId = new Random().Next(111111111,999999999).ToString();
                request.Price = model.CartModel.TotalPrice().ToString();
                request.PaidPrice = model.CartModel.TotalPrice().ToString();
                request.Currency = Currency.TRY.ToString();
                request.Installment = 1;
                request.BasketId = "B67832";
                request.PaymentChannel = PaymentChannel.WEB.ToString();
                request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

                PaymentCard paymentCard = new PaymentCard();
                paymentCard.CardHolderName = model.CardName;
                paymentCard.CardNumber = model.CardNumber;
                paymentCard.ExpireMonth = model.ExpirationMonth;
                paymentCard.ExpireYear = model.ExpirationYear;
                paymentCard.Cvc = model.Cvc;
                paymentCard.RegisterCard = 0;
                request.PaymentCard = paymentCard;

                // paymentCard.CardNumber = "5528790000000008";
                // paymentCard.ExpireMonth = "12";
                // paymentCard.ExpireYear = "2030"; 
                // paymentCard.Cvc = "123";

                Buyer buyer = new Buyer();
                buyer.Id = "BY789";
                buyer.Name = model.FirstName;
                buyer.Surname = model.LastName;
                buyer.GsmNumber = model.Phone;
                buyer.Email = model.Email;
                buyer.IdentityNumber = "74300864791";
                buyer.LastLoginDate = "2015-10-05 12:43:35";
                buyer.RegistrationDate = "2013-04-21 15:12:09";
                buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
                buyer.Ip = "85.34.78.112";
                buyer.City = model.City;
                buyer.Country = "Turkey";
                buyer.ZipCode = "34732";
                request.Buyer = buyer;

                Address shippingAddress = new Address();
                shippingAddress.ContactName = "Jane Doe";
                shippingAddress.City = "Istanbul";
                shippingAddress.Country = "Turkey";
                shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
                shippingAddress.ZipCode = "34742";
                request.ShippingAddress = shippingAddress;

                Address billingAddress = new Address();
                billingAddress.ContactName = "Jane Doe";
                billingAddress.City = "Istanbul";
                billingAddress.Country = "Turkey";
                billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
                billingAddress.ZipCode = "34742";
                request.BillingAddress = billingAddress;

                List<BasketItem> basketItems = new List<BasketItem>();
                BasketItem basketItem;
                foreach(var item in model.CartModel.CartItems)
                {
                    basketItem = new BasketItem();
                    basketItem.Id = item.ProductId.ToString();
                    basketItem.Name = item.Name;
                    basketItem.Category1 = "Elektronik";
                    basketItem.Price = (item.Price * item.Quantity).ToString();
                    basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                    basketItems.Add(basketItem);
                }
                    request.BasketItems = basketItems;
                return await Payment.Create(request, options);

        }
    }
}