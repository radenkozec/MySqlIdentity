﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MySql.AspNet.Identity;
using MySqlIdentity.Example.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MySqlIdentity.Example.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        public RolesController()
        {
        }

        public RolesController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Roles/
        public ActionResult Index()
        {
            var test = RoleManager.Roles.ToList();
            return View(test);
        }

        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id != null)
            {
                var role = await RoleManager.FindByIdAsync(id);
                // Get the list of Users in this Role
                var users = new List<ApplicationUser>();

                // Get the list of Users in this Role
                foreach (var user in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                    {
                        users.Add(user);
                    }
                }

                ViewBag.Users = users;
                ViewBag.UserCount = users.Count();
                return View(role);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(IdentityRole roleViewModel)
        {
            if (ModelState.IsValid)
            {
                // Use ApplicationRole, not IdentityRole:
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id != null)
            {
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityRole roleModel = new IdentityRole { Id = role.Id, Name = role.Name };
                return View(roleModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] IdentityRole roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id != null)
            {
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    var role = await RoleManager.FindByIdAsync(id);
                    if (role == null)
                    {
                        return HttpNotFound();
                    }
                    IdentityResult result;
                    if (deleteUser != null)
                    {
                        result = await RoleManager.DeleteAsync(role);
                    }
                    else
                    {
                        result = await RoleManager.DeleteAsync(role);
                    }
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return View();
        }
    }
}