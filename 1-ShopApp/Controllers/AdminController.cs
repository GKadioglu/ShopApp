using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1_ShopApp.Extensions;
using _1_ShopApp.Identity;
using _1_ShopApp.Models;
using _1_ShopApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.entity;

namespace _1_ShopApp.Controllers
{
    // bir alttaki satır, admin sayfasına gitmek için login gerektirdiğini belirtiyor. Login olunmazsa, aşağıdaki hiçbir metodu çalıştıramaz ve göremeyiz.
    [Authorize (Roles = "admin")]
    public class AdminController: Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public AdminController(IProductService productService, ICategoryService categoryService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;

        }

        
        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i=>i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModels(){

                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                });
            }
            return Redirect("~/admin/user/list");
        }


        [HttpPost]
          public async Task<IActionResult> UserEdit(UserDetailsModels model, string[] selectedRoles)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!= null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        selectedRoles = selectedRoles?? new string[] {};
                        await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles).ToArray<string>());

                        return Redirect("/admin/user/list");

                    }
                    return Redirect("/admin/user/list");
                }
            }
            return View(model);
        }



        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }


        [HttpGet]
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var item in _userManager.Users.ToList())
            {
                var list = await _userManager.IsInRoleAsync(item, role.Name) ? members : nonmembers;
                list.Add(item);
            }

            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }

            [HttpPost]
            public async Task<IActionResult> RoleEdit(RoleEditModel model)
            {

                if(!ModelState.IsValid)
                {
                    foreach (var userId in model.IdsToAdd ?? new string[]{})
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if(user!=null)
                        {
                            var result = await _userManager.AddToRoleAsync(user,model.RoleName);
                            if(!result.Succeeded)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                            }
                        }
                    }
                
                    foreach (var userId in model.IdsToDelete ?? new string[] {})
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if(user!=null)
                        {
                            var result = await _userManager.RemoveFromRolesAsync(user, new List<string>{model.RoleName});
                            if(!result.Succeeded)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                            }
                        }
                    }
                }
                return Redirect("/admin/role/"+model.RoleId);
            }

        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("",item.Description);
                    }
                }
            }
            return View(model);

        }

        public async Task <IActionResult> ProductList()
        {
            var products = await _productService.GetAll(); 
            return View(new ProductListModel()
            {
                Products =  products
                
            });
        }

        public async Task<IActionResult> CategoryList()
        {
            var categories = await _categoryService.GetAll();
            return View(new CategoryListViewModel()
            {
                Categories = categories
            });
        }


        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductModel model)
        {
            if(ModelState.IsValid)
            {
                    var entity = new Product()
            {
                Name = model.Name,
                Url = model.Url,
                Price = model.Price,
                Description = model.Description,
                ImageUrl = model.ImageUrl

            };

             if(_productService.Create(entity))
                {
                    TempData.Put("message", new AlertMessage() {
                        Title = "Kayıt Eklendi!",
                        Message = "kayıt Eklendi!",
                        AlertType = "success"
                    }); 
                  return RedirectToAction("ProductList");
                }
                    TempData.Put("message", new AlertMessage() {
                        Title = _productService.ErrorMesage,
                        Message = _productService.ErrorMesage,
                        AlertType = "danger"
                    });
                return View(model);
             }

            return View(model);
            
        }

        [HttpGet]
         public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
            var entity = new Category()
            {
                Name = model.Name,
                Url = model.Url
            };
            _categoryService.Create(entity);
             var msg =  new AlertMessage()
            {
               Message= $"{entity.Name} isimli kategori eklendi",
               AlertType ="success"
            };

              TempData["message"] = JsonConvert.SerializeObject(msg);

            return RedirectToAction("CategoryList");
         }
            return View(model);
        }

         [HttpGet]
        public async Task<IActionResult> ProductEdit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var entity = _productService.GetByIdWithCategories((int)id);
            if(entity == null )
            {
                return NotFound();
            }
            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Url = entity.Url,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                Description = entity.Description,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,
                SelectedCategories = entity.ProductCategories.Select(i=>i.Category).ToList()
            };

            ViewBag.Categories = await _categoryService.GetAll();
            return View(model);;
        }
[HttpPost]
public async Task<IActionResult> ProductEdit(ProductModel model, int[] categoryIds, IFormFile file)
{
    if (ModelState.IsValid)
    {
        // Veritabanından güncellenecek ürünü getir
        var entity = await _productService.GetById(model.ProductId);
        if (entity == null)
        {
            return NotFound();
        }

        // Entity'yi güncelle
        entity.Name = model.Name;
        entity.Price = model.Price;
        entity.Description = model.Description;
        entity.ImageUrl = model.ImageUrl;
        entity.Url = model.Url;
        entity.IsApproved = model.IsApproved;
        entity.IsHome = model.IsHome;

        // Dosya işlemleri
        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName);
            var randomName = $"{DateTime.Now.Ticks}{extension}";
            entity.ImageUrl = randomName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", randomName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        // Güncelleme işlemi
        _productService.Update(entity, categoryIds);

        // Güncelleme başarılı olduğunda mesaj oluştur ve yönlendir
        TempData.Put("message", new AlertMessage() {
                        Title = "Kayıt Güncellendi!",
                        Message = "Kayıt Güncellendi!",
                        AlertType = "success"
                    });
        return RedirectToAction("ProductList");
    }

    // Model doğrulaması başarısız olduğunda `SelectedCategories`'i doldurun
    model.SelectedCategories = _productService.GetByIdWithCategories(model.ProductId)
        .ProductCategories.Select(i => i.Category).ToList();

    ViewBag.Categories = await _categoryService.GetAll();
    return View(model);
}

        [HttpGet]
        public IActionResult CategoryEdit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var entity = _categoryService.GetByIdWithProducts((int)id);
            if(entity == null )
            {
                return NotFound();
            }
            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p=>p.Product).ToList()
            };
            return View(model);;
        }

        [HttpPost]
        public async Task<IActionResult> CategoryEdit(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
            var entity = await _categoryService.GetById(model.CategoryId);
            if(entity == null)
            {
                return NotFound();
            }
            entity.Name = model.Name;
            entity.Url = model.Url;
            _categoryService.Update(entity);
            
            var msg =  new AlertMessage()
            {
               Message= $"{entity.Name} isimli kategori güncellendi",
               AlertType ="success"
            };

              TempData["message"] = JsonConvert.SerializeObject(msg);


            return RedirectToAction("CategoryList");
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var entity = await _productService.GetById(productId);
            if(entity != null) 
            {
                _productService.Delete(entity);
            }

           var msg =  new AlertMessage()
            {
               Message= $"{entity.Name} isimli ürün silindi",
               AlertType ="danger"
            };

              TempData["message"] = JsonConvert.SerializeObject(msg);

            return RedirectToAction("ProductList");
        }

        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var entity = await _categoryService.GetById(categoryId);
            if(entity != null) 
            {
                _categoryService.Delete(entity);
            }

           var msg =  new AlertMessage()
            {
               Message= $"{entity.Name} isimli kategori silindi",
               AlertType ="danger"
            };

              TempData["message"] = JsonConvert.SerializeObject(msg);

            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteFromCategory(int productId, int categoryId)
        {
            _categoryService.DeleteFromCategory(productId,categoryId);
            return Redirect("admin/categories/"+categoryId);
        }

    }
}